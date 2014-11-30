//
// mTouch-PDFReader library
// ThumbsVC.cs
//
//  Author:
//       Alexander Matsibarov <amatsibarov@gmail.com>
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
		
		#region Data			
		private const int ThumbPadding = 20;		

		private bool _isInitializing;
		private readonly float _parentViewWidth;
		private readonly Action<object> _openPageCallback;
		private UIScrollView _scrollView;
		private readonly List<ThumbWithPageNumberView> _thumbViews;
		private ThumbWithPageNumberView _currentThumbView;
		private int _maxVisibleThumbsCount;
		private int _firstVisibleThumbNumber;
		private float _currentContentOffsetX;
		private int _scrollDirection;		
		#endregion
			
		#region UIViewController members		
		public ThumbsVC(float parentViewWidth, Action<object> callbackAction) 
			: base(null, null, callbackAction)
		{
			_isInitializing = false;
			_parentViewWidth = parentViewWidth;
			_openPageCallback = callbackAction; 
			_thumbViews = new List<ThumbWithPageNumberView>();
			_thumbViews.Capacity = MgrAccessor.SettingsMgr.Settings.ThumbsBufferSize;					
		}				

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			View.Frame = new RectangleF(View.Frame.Left, View.Frame.Top, _parentViewWidth, View.Frame.Height);
			View.BackgroundColor = UIColor.Clear;
			
			_scrollView = new UIScrollView(View.Bounds) 
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
			_scrollView.Scrolled += ScrollViewScrolled;
			
			View.AddSubview(_scrollView);
		}
		
		protected override SizeF getPopoverSize()
		{
			return new SizeF(_parentViewWidth - 20, ThumbPadding * 2 + MgrAccessor.SettingsMgr.Settings.ThumbSize);
		}			
		#endregion
		
		#region UIScrollViewDelegate members		
		private void ScrollViewScrolled(object sender, EventArgs args)
		{
			if (_isInitializing) {
				return;
			}
			
			#if DEBUG
			WriteMessageToDebugLog("ScrollViewScrolled", 
				string.Format("mScrollView [ContentOffset={0}, Bounds={1}, ContentSize={2}]", _scrollView.ContentOffset.X, _scrollView.Bounds, _scrollView.ContentSize));
			#endif
			if (_scrollView.ContentOffset.X == _currentContentOffsetX) {
				#if DEBUG
				WriteMessageToDebugLog("ScrollViewScrolled", "1");
				#endif
				return;
			}
			if ((_scrollView.ContentOffset.X < 0) || (_scrollView.ContentOffset.X > _scrollView.ContentSize.Width - _scrollView.Bounds.Width + 2)) {
				#if DEBUG
				WriteMessageToDebugLog("ScrollViewScrolled", "2");
				#endif
				return;
			}

			int newThumbNumber = getThumbNumberByX(_scrollView.ContentOffset.X);
			#if DEBUG
			WriteMessageToDebugLog("ScrollViewScrolled", string.Format("mFirstVisibleThumbNumber={0}, newThumbNumber={1}", _firstVisibleThumbNumber, newThumbNumber));
			#endif
			if (newThumbNumber == -1) {
				return;
			}
			if (_firstVisibleThumbNumber == newThumbNumber) {
				return;
			}			
			
			_scrollDirection = (newThumbNumber < _firstVisibleThumbNumber) ? -1 : 1;
			_currentContentOffsetX = _scrollView.ContentOffset.X;
			_firstVisibleThumbNumber = newThumbNumber;
			
			createThumbs();
		}				
		#endregion
				
		#region Logic		
		private int getThumbNumberByX(float x)
		{
			if ((x < 0) || (x > _scrollView.ContentSize.Width - ThumbPadding)) {
				return -1;
			}
			int retValue = (int)(x / (float)(MgrAccessor.SettingsMgr.Settings.ThumbSize + ThumbPadding)) + 1;
			#if DEBUG
			WriteMessageToDebugLog("GetThumbNumberByX", string.Format("x = {0}, pageNumber={1}", x, retValue));
			#endif

			return retValue;
		}
		
		private RectangleF getThumbFrameByPage(int pageNumber)
		{
			float left = (pageNumber - 1) * (ThumbPadding + MgrAccessor.SettingsMgr.Settings.ThumbSize) + ThumbPadding;
			var retValue = new RectangleF(left, ThumbPadding, MgrAccessor.SettingsMgr.Settings.ThumbSize, MgrAccessor.SettingsMgr.Settings.ThumbSize);
			#if DEBUG
			WriteMessageToDebugLog("GetThumbFrameByPage", string.Format("frame = {0}", retValue));
			#endif
			return retValue;
		}
		
		private void createThumbs()
		{
			for (int i = _firstVisibleThumbNumber; i <= _firstVisibleThumbNumber + _maxVisibleThumbsCount; i++) {
				createThumbForPageIfNotExists(i);
			}
		}
		
		private ThumbWithPageNumberView createThumbForPageIfNotExists(int pageNumber)
		{
			if ((pageNumber <= 0) || (pageNumber > PDFDocument.PageCount)) {
				return null;
			}
			
			foreach (var thumbView in _thumbViews) {
				if (thumbView.PageNumber == pageNumber) {
					return thumbView;
				}
			}
			
			RectangleF viewRect = getThumbFrameByPage(pageNumber);
			var newThumbView = new ThumbWithPageNumberView(viewRect, pageNumber, thumbSelected);
			if (pageNumber == PDFDocument.CurrentPageNumber) {
				_currentThumbView = newThumbView;
				newThumbView.SetAsSelected();
			}

			_scrollView.AddSubview(newThumbView);
			if (_scrollDirection > 0) {
				_thumbViews.Add(newThumbView);
			} else {
				_thumbViews.Insert(0, newThumbView);
			}
			#if DEBUG
			WriteMessageToDebugLog("CreateThumb", string.Format("page={0}, viewRect={1}", pageNumber, viewRect));
			#endif
			
			if (_thumbViews.Count > MgrAccessor.SettingsMgr.Settings.ThumbsBufferSize) {
				if (_scrollDirection > 0) {
					_thumbViews [0].RemoveFromSuperview();
					_thumbViews.RemoveAt(0);
				} else {
					_thumbViews [_thumbViews.Count - 1].RemoveFromSuperview();
					_thumbViews.RemoveAt(_thumbViews.Count - 1);
				}
			}
			#if DEBUG
			WriteMessageToDebugLog("CreateThumb", string.Format("currentBufferSize={0}", _thumbViews.Count));
			#endif

			return newThumbView;
		}
		
		private void updateScrollViewContentSize()
		{
			float contentWidth = (ThumbPadding + MgrAccessor.SettingsMgr.Settings.ThumbSize) * PDFDocument.PageCount + ThumbPadding;
			float contentHeight = _scrollView.Frame.Size.Height;
			_scrollView.ContentSize = new SizeF(contentWidth, contentHeight);
		}

		public virtual void InitThumbs()
		{
			updateScrollViewContentSize();
			calcParameters();
		}
		
		private void calcParameters()
		{
			_isInitializing = true;
			
			_maxVisibleThumbsCount = (int)Math.Ceiling(View.Bounds.Width / (float)MgrAccessor.SettingsMgr.Settings.ThumbSize);
			if (_maxVisibleThumbsCount > PDFDocument.PageCount) {
				_maxVisibleThumbsCount = PDFDocument.PageCount;
			}
			
			if (_maxVisibleThumbsCount == PDFDocument.PageCount) {
				_firstVisibleThumbNumber = 1;
			} else {
				int maxThumb = PDFDocument.CurrentPageNumber + _maxVisibleThumbsCount - 1;
				if (maxThumb <= PDFDocument.PageCount) {
					_firstVisibleThumbNumber = PDFDocument.CurrentPageNumber;
				} else {
					_firstVisibleThumbNumber = PDFDocument.PageCount - _maxVisibleThumbsCount + 2;
				}
			}
			#if DEBUG
			WriteMessageToDebugLog("CalcParameters", string.Format("mVisibleThumbsMaxCount={0}, mFirstVisibleThumbNumber={1}", _maxVisibleThumbsCount, _firstVisibleThumbNumber));
			#endif
			
			_scrollDirection = 1;
			
			RectangleF viewRect = getThumbFrameByPage(_firstVisibleThumbNumber);
			_scrollView.ContentOffset = new PointF(viewRect.X - ThumbPadding, _scrollView.ContentOffset.Y);
			_currentContentOffsetX = _scrollView.ContentOffset.X;
			#if DEBUG
			WriteMessageToDebugLog("CalcParameters", string.Format("mScrollView.ContentOffset.X={0}", _scrollView.ContentOffset.X));
			#endif
			
			createThumbs();
			
			_isInitializing = false;
		}
				
		private void thumbSelected(ThumbWithPageNumberView thumbView)
		{
			if (thumbView != _currentThumbView) {
				if (_currentThumbView != null) {
					_currentThumbView.SetAsUnselected();
				}

				_currentThumbView = thumbView;
				_currentThumbView.SetAsSelected();

				_openPageCallback(_currentThumbView.PageNumber);
			}
		}			
		#endregion
	}
}