//
// mTouch-PDFReader library
// ThumbsViewController.cs (Thumbs view controller)
//
//  Author:
//       Alexander Matsibarov (macasun) <amatsibarov@gmail.com>
//
//  Copyright (c) 2014 Alexander Matsibarov
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.XViews;
using mTouchPDFReader.Library.Managers;

namespace mTouchPDFReader.Library.Views.Core
{
	public class ThumbsVC : UIViewControllerWithPopover
	{		
		#if DEBUG
		private static void WriteMessageToDebugLog(string method, string message)
		{
			Console.WriteLine(string.Format("{0} {1}", method, message));
		}		
		#endif
		
		#region Constants			
		private const int ThumbPadding = 20;		
		#endregion
		
		#region Fields			
		private bool _Initializing;
		private readonly float _ParentViewWidth;
		private readonly Action<object> _OpenPageCallback;
		private UIScrollView _ScrollView;
		private readonly List<ThumbWithPageNumberView> _ThumbViews;
		private ThumbWithPageNumberView _CurrentThumbView;
		private int _MaxVisibleThumbsCount;
		private int _FirstVisibleThumbNumber;
		private float _CurrentContentOffsetX;
		private int _ScrollDirection;		
		#endregion
			
		#region Constructors		
		public ThumbsVC(float parentViewWidth, Action<object> callbackAction) 
			: base(null, null, callbackAction)
		{
			_Initializing = false;
			_ParentViewWidth = parentViewWidth;
			_OpenPageCallback = callbackAction; 
			_ThumbViews = new List<ThumbWithPageNumberView>();
			_ThumbViews.Capacity = MgrAccessor.OptionsMgr.Options.ThumbsBufferSize;					
		}				
		#endregion
		
		#region UIViewController methods		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			View.Frame = new RectangleF(View.Frame.Left, View.Frame.Top, _ParentViewWidth, View.Frame.Height);
			View.BackgroundColor = UIColor.Clear;
			
			_ScrollView = new UIScrollView(View.Bounds)
			              {
				              ScrollsToTop = false, 
							  DelaysContentTouches = false, 
							  ShowsVerticalScrollIndicator = false, 
							  ShowsHorizontalScrollIndicator = true, 
							  AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight, 
							  ContentMode = UIViewContentMode.Redraw, 
							  BackgroundColor = UIColor.Clear, 
							  UserInteractionEnabled = true, 
							  AutosizesSubviews = false
			              };
			_ScrollView.Scrolled += ScrollViewScrolled;
			
			View.AddSubview(_ScrollView);
		}
		
		protected override SizeF GetPopoverSize()
		{
			return new SizeF(_ParentViewWidth - 20, ThumbPadding * 2 + MgrAccessor.OptionsMgr.Options.ThumbSize);
		}			
		#endregion
		
		#region UIScrollViewDelegate methods		
		private void ScrollViewScrolled(object sender, EventArgs args)
		{
			if (_Initializing) {
				return;
			}
			
			// If content offset isn't changed or it's out from content size - exit
			#if DEBUG
			WriteMessageToDebugLog("ScrollViewScrolled", 
				string.Format("mScrollView [ContentOffset={0}, Bounds={1}, ContentSize={2}]", _ScrollView.ContentOffset.X, _ScrollView.Bounds, _ScrollView.ContentSize));
			#endif
			if (_ScrollView.ContentOffset.X == _CurrentContentOffsetX) {
				#if DEBUG
				WriteMessageToDebugLog("ScrollViewScrolled", "1");
				#endif
				return;
			}
			if ((_ScrollView.ContentOffset.X < 0) || (_ScrollView.ContentOffset.X > _ScrollView.ContentSize.Width - _ScrollView.Bounds.Width + 2)) {
				#if DEBUG
				WriteMessageToDebugLog("ScrollViewScrolled", "2");
				#endif
				return;
			}
			// If the first visible thumb number isn't changed - exit
			int newThumbNumber = GetThumbNumberByX(_ScrollView.ContentOffset.X);
			#if DEBUG
			WriteMessageToDebugLog("ScrollViewScrolled", string.Format("mFirstVisibleThumbNumber={0}, newThumbNumber={1}", _FirstVisibleThumbNumber, newThumbNumber));
			#endif
			if (newThumbNumber == -1) {
				return;
			}
			if (_FirstVisibleThumbNumber == newThumbNumber) {
				return;
			}			
			
			// Store scroll direction, current offset and first visible thumb number
			_ScrollDirection = (newThumbNumber < _FirstVisibleThumbNumber) ? -1 : 1;
			_CurrentContentOffsetX = _ScrollView.ContentOffset.X;
			_FirstVisibleThumbNumber = newThumbNumber;
			
			// Create thumbs for displayed rect
			CreateThumbs();
		}				
		#endregion
				
		#region Logic		
		private int GetThumbNumberByX(float x)
		{
			if ((x < 0) || (x > _ScrollView.ContentSize.Width - ThumbPadding)) {
				return -1;
			}
			int retValue = (int)(x / (float)(MgrAccessor.OptionsMgr.Options.ThumbSize + ThumbPadding)) + 1;
			#if DEBUG
			WriteMessageToDebugLog("GetThumbNumberByX", string.Format("x = {0}, pageNumber={1}", x, retValue));
			#endif
			return retValue;
		}
		
		private RectangleF GetThumbFrameByPage(int pageNumber)
		{
			float left = (pageNumber - 1) * (ThumbPadding + MgrAccessor.OptionsMgr.Options.ThumbSize) + ThumbPadding;
			var retValue = new RectangleF(left, ThumbPadding, MgrAccessor.OptionsMgr.Options.ThumbSize, MgrAccessor.OptionsMgr.Options.ThumbSize);
			#if DEBUG
			WriteMessageToDebugLog("GetThumbFrameByPage", string.Format("frame = {0}", retValue));
			#endif
			return retValue;
		}
		
		private void CreateThumbs()
		{
			// Check existing thumbs in visible rectangle, create thumbs if needed
			for (int i = _FirstVisibleThumbNumber; i <= _FirstVisibleThumbNumber + _MaxVisibleThumbsCount; i++) {
				CreateThumbForPageIfNotExists(i);
			}
		}
		
		private ThumbWithPageNumberView CreateThumbForPageIfNotExists(int pageNumber)
		{
			if ((pageNumber <= 0) || (pageNumber > PDFDocument.PageCount)) {
				return null;
			}
			
			// Checks thumb existing 
			foreach (var thumbView in _ThumbViews) {
				if (thumbView.PageNumber == pageNumber) {
					return thumbView;
				}
			}
			
			// Create new thumb
			RectangleF viewRect = GetThumbFrameByPage(pageNumber);
			var newThumbView = new ThumbWithPageNumberView(viewRect, pageNumber, ThumbSelected);
			if (pageNumber == PDFDocument.CurrentPageNumber) {
				_CurrentThumbView = newThumbView;
				newThumbView.SetAsSelected();
			}
			// Insert view to first or last position of the list (according direction)
			_ScrollView.AddSubview(newThumbView);
			if (_ScrollDirection > 0) {
				_ThumbViews.Add(newThumbView);
			} else {
				_ThumbViews.Insert(0, newThumbView);
			}
			#if DEBUG
			WriteMessageToDebugLog("CreateThumb", string.Format("page={0}, viewRect={1}", pageNumber, viewRect));
			#endif
			
			// Pack thumbs size to ThumbsBufferSize (only one extra item can be created - remove it)
			if (_ThumbViews.Count > MgrAccessor.OptionsMgr.Options.ThumbsBufferSize) {
				// Remove view from first or last position of the list (according direction)
				if (_ScrollDirection > 0) {
					_ThumbViews [0].RemoveFromSuperview();
					_ThumbViews.RemoveAt(0);
				} else {
					_ThumbViews [_ThumbViews.Count - 1].RemoveFromSuperview();
					_ThumbViews.RemoveAt(_ThumbViews.Count - 1);
				}
			}
			#if DEBUG
			WriteMessageToDebugLog("CreateThumb", string.Format("currentBufferSize={0}", _ThumbViews.Count));
			#endif
			return newThumbView;
		}
		
		private void UpdateScrollViewContentSize()
		{
			float contentWidth = (ThumbPadding + MgrAccessor.OptionsMgr.Options.ThumbSize) * PDFDocument.PageCount + ThumbPadding;
			float contentHeight = _ScrollView.Frame.Size.Height;
			_ScrollView.ContentSize = new SizeF(contentWidth, contentHeight);
		}

		public virtual void InitThumbs()
		{
			UpdateScrollViewContentSize();
			CalcParameters();
		}
		
		private void CalcParameters()
		{
			_Initializing = true;
			
			// Calc maximum visible thumbs count 
			_MaxVisibleThumbsCount = (int)Math.Ceiling(View.Bounds.Width / (float)MgrAccessor.OptionsMgr.Options.ThumbSize);
			if (_MaxVisibleThumbsCount > PDFDocument.PageCount) {
				_MaxVisibleThumbsCount = PDFDocument.PageCount;
			}
			
			// Calc first visible thumb
			if (_MaxVisibleThumbsCount == PDFDocument.PageCount) {
				_FirstVisibleThumbNumber = 1;
			} else {
				int maxThumb = PDFDocument.CurrentPageNumber + _MaxVisibleThumbsCount - 1;
				if (maxThumb <= PDFDocument.PageCount) {
					_FirstVisibleThumbNumber = PDFDocument.CurrentPageNumber;
				} else {
					_FirstVisibleThumbNumber = PDFDocument.PageCount - _MaxVisibleThumbsCount + 2;
				}
			}
			#if DEBUG
			WriteMessageToDebugLog("CalcParameters", string.Format("mVisibleThumbsMaxCount={0}, mFirstVisibleThumbNumber={1}", _MaxVisibleThumbsCount, _FirstVisibleThumbNumber));
			#endif
			
			// Set default scroll direction equal as 1
			_ScrollDirection = 1;
			
			// Set scrollview content offset on the first displayed thumb
			RectangleF viewRect = GetThumbFrameByPage(_FirstVisibleThumbNumber);
			_ScrollView.ContentOffset = new PointF(viewRect.X - ThumbPadding, _ScrollView.ContentOffset.Y);
			_CurrentContentOffsetX = _ScrollView.ContentOffset.X;
			#if DEBUG
			WriteMessageToDebugLog("CalcParameters", string.Format("mScrollView.ContentOffset.X={0}", _ScrollView.ContentOffset.X));
			#endif
			
			// Create thumbs for displayed rect
			CreateThumbs();
			
			_Initializing = false;
		}
				
		private void ThumbSelected(ThumbWithPageNumberView thumbView)
		{
			if (thumbView != _CurrentThumbView) {
				// Unselect old thumb
				if (_CurrentThumbView != null) {
					_CurrentThumbView.SetAsUnselected();
				}
				// Select and store current thumb
				_CurrentThumbView = thumbView;
				_CurrentThumbView.SetAsSelected();
				// Open selected page
				_OpenPageCallback(_CurrentThumbView.PageNumber);
			}
		}			
		#endregion
	}
}