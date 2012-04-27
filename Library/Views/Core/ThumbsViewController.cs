//****************************************//
// mTouch-PDFReader library
// Thumbs view controller
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Data.Managers;
using mTouchPDFReader.Library.XViews;

namespace mTouchPDFReader.Library.Views.Core
{
	public class ThumbsViewController : UIViewControllerWithPopover
	{		
		#if DEBUG
		/// <summary>
		/// Writes message to debug log
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
		/// Flag, indicates, that controller is initializing 
		/// </summary>
		private bool mInitializing;
		
		/// <summary>
		/// Parent view width
		/// </summary>
		private float mParentViewWidth;
		
		/// <summary>
		/// Callback to open page
		/// </summary>
		private Action<object> mOpenPageCallback;
		
		/// <summary>
		/// Main scroll view
		/// </summary>
		private UIScrollView mScrollView;
		
		/// <summary>
		/// List of thumb views
		/// </summary>
		private List<ThumbWithPageNumberView> mThumbViews;
	
		/// <summary>
		/// Current thumb view (selected)
		/// </summary>
		private ThumbWithPageNumberView mCurrentThumbView;
		
		/// <summary>
		/// Maximum visible thumbs count
		/// </summary>
		private int mMaxVisibleThumbsCount;
		
		/// <summary>
		/// First visible thumb number 
		/// </summary>
		private int mFirstVisibleThumbNumber;
		
		/// <summary>
		/// Current scrollview content offset by X
		/// </summary>
		private float mCurrentContentOffsetX;
		
		/// <summary>
		/// Scroll direction 
		/// </summary>
		private int mScrollDirection;
		
		#endregion
			
		#region Constructors
		
		/// <summary>
		/// Work constuctor
		/// </summary>
		public ThumbsViewController(float parentViewWidth, Action<object> callbackAction) 
			: base(null, null, callbackAction)
		{
			mInitializing = false;
			mParentViewWidth = parentViewWidth;
			mOpenPageCallback = callbackAction; 
			mThumbViews = new List<ThumbWithPageNumberView>();
			mThumbViews.Capacity = OptionsManager.Instance.Options.ThumbsBufferSize;					
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
			View.Frame = new RectangleF(View.Frame.Left, View.Frame.Top, mParentViewWidth, View.Frame.Height);
			View.BackgroundColor = UIColor.Clear;
			
			// Create thumbs srollview
			mScrollView = new UIScrollView(View.Bounds);
			mScrollView.ScrollsToTop = false;
			mScrollView.DelaysContentTouches = false;
			mScrollView.ShowsVerticalScrollIndicator = false;
			mScrollView.ShowsHorizontalScrollIndicator = true;
			mScrollView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			mScrollView.ContentMode = UIViewContentMode.Redraw;
			mScrollView.BackgroundColor = UIColor.Clear;
			mScrollView.UserInteractionEnabled = true;
			mScrollView.AutosizesSubviews = false;
			mScrollView.Scrolled += ScrollViewScrolled;
			
			// Add thumbs scroll view to parent
			View.AddSubview(mScrollView);
		}
		
		/// <summary>
		/// Returns popover size, must be overrided in child classes
		/// </summary>
		/// <returns>Popover size</returns>
		protected override SizeF GetPopoverSize()
		{
			return new SizeF(mParentViewWidth - 20, ThumbPadding * 2 + OptionsManager.Instance.Options.ThumbSize);
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
			if (mInitializing) {
				return;
			}
			
			// If content offset isn't changed or it's out from content size - exit
			#if DEBUG
			WriteMessageToDebugLog("ScrollViewScrolled", 
				string.Format("mScrollView [ContentOffset={0}, Bounds={1}, ContentSize={2}]", mScrollView.ContentOffset.X, mScrollView.Bounds, mScrollView.ContentSize));
			#endif
			if (mScrollView.ContentOffset.X == mCurrentContentOffsetX) {
				#if DEBUG
				WriteMessageToDebugLog("ScrollViewScrolled", "1");
				#endif
				return;
			}
			if ((mScrollView.ContentOffset.X < 0) || (mScrollView.ContentOffset.X > mScrollView.ContentSize.Width - mScrollView.Bounds.Width + 2)) {
				#if DEBUG
				WriteMessageToDebugLog("ScrollViewScrolled", "2");
				#endif
				return;
			}
			// If the first visible thumb number isn't changed - exit
			int newThumbNumber = GetThumbNumberByX(mScrollView.ContentOffset.X);
			#if DEBUG
			WriteMessageToDebugLog("ScrollViewScrolled", string.Format("mFirstVisibleThumbNumber={0}, newThumbNumber={1}", mFirstVisibleThumbNumber, newThumbNumber));
			#endif
			if (newThumbNumber == -1) {
				return;
			}
			if (mFirstVisibleThumbNumber == newThumbNumber) {
				return;
			}			
			
			// Store scroll direction, current offset and first visible thumb number
			mScrollDirection = (newThumbNumber < mFirstVisibleThumbNumber) ? -1 : 1;
			mCurrentContentOffsetX = mScrollView.ContentOffset.X;
			mFirstVisibleThumbNumber = newThumbNumber;
			
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
			if ((x < 0) || (x > mScrollView.ContentSize.Width - ThumbPadding)) {
				return -1;
			}
			int retValue = (int)(x / (float)(OptionsManager.Instance.Options.ThumbSize + ThumbPadding)) + 1;
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
			float left = (pageNumber - 1) * (ThumbPadding + OptionsManager.Instance.Options.ThumbSize) + ThumbPadding;
			var retValue = new RectangleF(left, ThumbPadding, OptionsManager.Instance.Options.ThumbSize, OptionsManager.Instance.Options.ThumbSize);
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
			for (int i = mFirstVisibleThumbNumber; i <= mFirstVisibleThumbNumber + mMaxVisibleThumbsCount; i++) {
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
			foreach (var thumbView in mThumbViews) {
				if (thumbView.PageNumber == pageNumber) {
					return thumbView;
				}
			}
			
			// Create new thumb
			RectangleF viewRect = GetThumbFrameByPage(pageNumber);
			var newThumbView = new ThumbWithPageNumberView(viewRect, pageNumber, ThumbSelected);
			if (pageNumber == PDFDocument.CurrentPageNumber) {
				mCurrentThumbView = newThumbView;
				newThumbView.SetAsSelected();
			}
			// Insert view to first or last position of the list (according direction)
			mScrollView.AddSubview(newThumbView);
			if (mScrollDirection > 0) {
				mThumbViews.Add(newThumbView);
			} else {
				mThumbViews.Insert(0, newThumbView);
			}
			#if DEBUG
			WriteMessageToDebugLog("CreateThumb", string.Format("page={0}, viewRect={1}", pageNumber, viewRect));
			#endif
			
			// Pack thumbs size to ThumbsBufferSize (only one extra item can be created - remove it)
			if (mThumbViews.Count > OptionsManager.Instance.Options.ThumbsBufferSize) {
				// Remove view from first or last position of the list (according direction)
				if (mScrollDirection > 0) {
					mThumbViews[0].RemoveFromSuperview();
					mThumbViews.RemoveAt(0);
				} else {
					mThumbViews[mThumbViews.Count - 1].RemoveFromSuperview();
					mThumbViews.RemoveAt(mThumbViews.Count - 1);
				}
			}
			#if DEBUG
			WriteMessageToDebugLog("CreateThumb", string.Format("currentBufferSize={0}", mThumbViews.Count));
			#endif
			return newThumbView;
		}
		
		/// <summary>
		/// Updates thumbs scroll view content size
		/// </summary>
		private void UpdateScrollViewContentSize()
		{
			float contentWidth = (ThumbPadding + OptionsManager.Instance.Options.ThumbSize) * PDFDocument.PageCount + ThumbPadding;
			float contentHeight = mScrollView.Frame.Size.Height;
			mScrollView.ContentSize = new SizeF(contentWidth, contentHeight);
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
			mInitializing = true;
			
			// Calc maximum visible thumbs count 
			mMaxVisibleThumbsCount = (int)Math.Ceiling(View.Bounds.Width / (float)OptionsManager.Instance.Options.ThumbSize);
			if (mMaxVisibleThumbsCount > PDFDocument.PageCount) {
				mMaxVisibleThumbsCount = PDFDocument.PageCount;
			}
			
			// Calc first visible thumb
			if (mMaxVisibleThumbsCount == PDFDocument.PageCount) {
				mFirstVisibleThumbNumber = 1;
			} else {
				int maxThumb = PDFDocument.CurrentPageNumber + mMaxVisibleThumbsCount - 1;
				if (maxThumb <= PDFDocument.PageCount) {
					mFirstVisibleThumbNumber = PDFDocument.CurrentPageNumber;
				} else {
					mFirstVisibleThumbNumber = PDFDocument.PageCount - mMaxVisibleThumbsCount + 2;
				}
			}
			#if DEBUG
			WriteMessageToDebugLog("CalcParameters", string.Format("mVisibleThumbsMaxCount={0}, mFirstVisibleThumbNumber={1}", mMaxVisibleThumbsCount, mFirstVisibleThumbNumber));
			#endif
			
			// Set default scroll direction equal as 1
			mScrollDirection = 1;
			
			// Set scrollview content offset on the first displayed thumb
			RectangleF viewRect = GetThumbFrameByPage(mFirstVisibleThumbNumber);
			mScrollView.ContentOffset = new PointF(viewRect.X - ThumbPadding , mScrollView.ContentOffset.Y);
			mCurrentContentOffsetX = mScrollView.ContentOffset.X;
			#if DEBUG
			WriteMessageToDebugLog("CalcParameters", string.Format("mScrollView.ContentOffset.X={0}", mScrollView.ContentOffset.X));
			#endif
			
			// Create thumbs for displayed rect
			CreateThumbs();
			
			mInitializing = false;
		}
				
		/// <summary>
		/// Callback called when user clicks by thumb
		/// </summary>
		/// <param name="thumbView">Clicked thumb</param>
		private void ThumbSelected(ThumbWithPageNumberView thumbView)
		{
			if (thumbView != mCurrentThumbView) {
				// Unselect old thumb
				if (mCurrentThumbView != null) {
					mCurrentThumbView.SetAsUnselected();
				}
				// Select and store current thumb
				mCurrentThumbView = thumbView;
				mCurrentThumbView.SetAsSelected();
				// Open selected page
				mOpenPageCallback(mCurrentThumbView.PageNumber);
			}
		}		
		
		#endregion
	}
}