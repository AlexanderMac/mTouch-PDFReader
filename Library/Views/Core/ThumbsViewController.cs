//
// mTouch-PDFReader library
// ThumbsViewController.cs (Thumbs view controller)
//
//  Author:
//       Alexander Matsibarov (macasun) <amatsibarov@gmail.com>
//
//  Copyright (c) 2012 Alexander Matsibarov
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
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Utils;
using mTouchPDFReader.Library.Interfaces;
using mTouchPDFReader.Library.Data.Managers;
using mTouchPDFReader.Library.XViews;

namespace mTouchPDFReader.Library.Views.Core
{
	public class ThumbsViewController : UIViewControllerWithPopover
	{		
		#if DEBUG
		/// <summary>
		/// Writes the message to the log.
		/// </summary>
		private static void WriteMessageToDebugLog(string method, string message)
		{
			Console.WriteLine(string.Format("{0} {1}", method, message));
		}		
		#endif
		
		#region Constants			
		/// <summary>
		/// Thumb padding
		/// </summary>
		private const int ThumbPadding = 20;		
		#endregion
		
		#region Fields			
		/// <summary>
		/// Indicates, that the controller has initialized. 
		/// </summary>
		private bool _Initializing;
		
		/// <summary>
		/// The parent view width.
		/// </summary>
		private float _ParentViewWidth;
		
		/// <summary>
		/// The action called when the page opened.
		/// </summary>
		private Action<object> _OpenPageCallback;
		
		/// <summary>
		/// The main scroll view.
		/// </summary>
		private UIScrollView _ScrollView;
		
		/// <summary>
		/// The list of thumb views.
		/// </summary>
		private List<ThumbWithPageNumberView> _ThumbViews;
	
		/// <summary>
		/// The current thumb view (selected).
		/// </summary>
		private ThumbWithPageNumberView _CurrentThumbView;
		
		/// <summary>
		/// The maximum visible thumbs count.
		/// </summary>
		private int _MaxVisibleThumbsCount;
		
		/// <summary>
		/// First visible thumb number 
		/// </summary>
		private int _FirstVisibleThumbNumber;
		
		/// <summary>
		/// The current scrollview content offset by X.
		/// </summary>
		private float _CurrentContentOffsetX;
		
		/// <summary>
		/// The scroll direction. 
		/// </summary>
		private int _ScrollDirection;		
		#endregion
			
		#region Constructors		
		/// <summary>
		/// Working.
		/// </summary>
		public ThumbsViewController(float parentViewWidth, Action<object> callbackAction) 
			: base(null, null, callbackAction)
		{
			_Initializing = false;
			_ParentViewWidth = parentViewWidth;
			_OpenPageCallback = callbackAction; 
			_ThumbViews = new List<ThumbWithPageNumberView>();
			_ThumbViews.Capacity = RC.Get<IOptionsManager>().Options.ThumbsBufferSize;					
		}				
		#endregion
		
		#region UIViewController methods		
		/// <summary>
		/// Calls when view has loaded
		/// </summary>
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			// Init View	
			View.Frame = new RectangleF(View.Frame.Left, View.Frame.Top, _ParentViewWidth, View.Frame.Height);
			View.BackgroundColor = UIColor.Clear;
			
			// Create thumbs srollview
			_ScrollView = new UIScrollView(View.Bounds);
			_ScrollView.ScrollsToTop = false;
			_ScrollView.DelaysContentTouches = false;
			_ScrollView.ShowsVerticalScrollIndicator = false;
			_ScrollView.ShowsHorizontalScrollIndicator = true;
			_ScrollView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			_ScrollView.ContentMode = UIViewContentMode.Redraw;
			_ScrollView.BackgroundColor = UIColor.Clear;
			_ScrollView.UserInteractionEnabled = true;
			_ScrollView.AutosizesSubviews = false;
			_ScrollView.Scrolled += ScrollViewScrolled;
			
			// Add thumbs scroll view to parent
			View.AddSubview(_ScrollView);
		}
		
		/// <summary>
		/// Returns popover size, must be overrided in child classes
		/// </summary>
		/// <returns>Popover size</returns>
		protected override SizeF GetPopoverSize()
		{
			return new SizeF(_ParentViewWidth - 20, ThumbPadding * 2 + RC.Get<IOptionsManager>().Options.ThumbSize);
		}

		/// <summary>
		/// Called when permission is shought to rotate
		/// </summary>
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}			
		#endregion
		
		#region UIScrollViewDelegate methods		
		/// <summary>
		/// Calls when scrollView scrolled
		/// </summary>
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
		/// <summary>
		/// Returns thumb number by X 
		/// </summary>
		/// <param name="x"> X position</param>
		/// <returns>Thumb number</returns>
		private int GetThumbNumberByX(float x)
		{
			if ((x < 0) || (x > _ScrollView.ContentSize.Width - ThumbPadding)) {
				return -1;
			}
			int retValue = (int)(x / (float)(RC.Get<IOptionsManager>().Options.ThumbSize + ThumbPadding)) + 1;
			#if DEBUG
			WriteMessageToDebugLog("GetThumbNumberByX", string.Format("x = {0}, pageNumber={1}", x, retValue));
			#endif
			return retValue;
		}
		
		/// <summary>
		/// Returns thumb frame by page number
		/// </summary>
		/// <param name="pageNumber">Page number</param>
		/// <returns></returns>
		private RectangleF GetThumbFrameByPage(int pageNumber)
		{
			float left = (pageNumber - 1) * (ThumbPadding + RC.Get<IOptionsManager>().Options.ThumbSize) + ThumbPadding;
			var retValue = new RectangleF(left, ThumbPadding, RC.Get<IOptionsManager>().Options.ThumbSize, RC.Get<IOptionsManager>().Options.ThumbSize);
			#if DEBUG
			WriteMessageToDebugLog("GetThumbFrameByPage", string.Format("frame = {0}", retValue));
			#endif
			return retValue;
		}
		
		/// <summary>
		/// Create thumbs for displayed rect 
		/// </summary>
		private void CreateThumbs()
		{
			// Check existing thumbs in visible rectangle, create thumbs if needed
			for (int i = _FirstVisibleThumbNumber; i <= _FirstVisibleThumbNumber + _MaxVisibleThumbsCount; i++) {
				ThumbWithPageNumberView thumb = CreateThumbForPageIfNotExists(i);
				#if DEBUG
				if (thumb != null) {
					WriteMessageToDebugLog("CreateThumbs", string.Format("thumb.Frame.X={0}, thumb.PageNumber={1}", thumb.Frame, thumb.PageNumber));
				} else {
					WriteMessageToDebugLog("CreateThumbs", "thumb is null");
				}
				#endif
			}
		}
		
		/// <summary>
		/// Create thumb for page number, if it isn't exists  
		/// </summary>
		/// <param name="pageNumber">Page number</param>
		/// <returns></returns>
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
			if (_ThumbViews.Count > RC.Get<IOptionsManager>().Options.ThumbsBufferSize) {
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
		
		/// <summary>
		/// Updates thumbs scroll view content size
		/// </summary>
		private void UpdateScrollViewContentSize()
		{
			float contentWidth = (ThumbPadding + RC.Get<IOptionsManager>().Options.ThumbSize) * PDFDocument.PageCount + ThumbPadding;
			float contentHeight = _ScrollView.Frame.Size.Height;
			_ScrollView.ContentSize = new SizeF(contentWidth, contentHeight);
		}

		/// <summary>
		/// Init thumbs
		/// </summary>
		public virtual void InitThumbs()
		{
			UpdateScrollViewContentSize();
			CalcParameters();
		}
		
		/// <summary>
		/// Calcs parameters
		/// </summary>
		private void CalcParameters()
		{
			_Initializing = true;
			
			// Calc maximum visible thumbs count 
			_MaxVisibleThumbsCount = (int)Math.Ceiling(View.Bounds.Width / (float)RC.Get<IOptionsManager>().Options.ThumbSize);
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
				
		/// <summary>
		/// Callback called when user clicks by thumb
		/// </summary>
		/// <param name="thumbView">Clicked thumb</param>
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