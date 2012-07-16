//****************************************//
// mTouch-PDFReader library
// Document view controller
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
using mTouchPDFReader.Library.Data.Objects;
using mTouchPDFReader.Library.Data.Enums;
using mTouchPDFReader.Library.Data.Managers;
using mTouchPDFReader.Library.XViews;
using mTouchPDFReader.Library.Views.Management;

namespace mTouchPDFReader.Library.Views.Core
{
	public class DocumentViewController : UIViewController
	{			
		#region Constants
		
		private const int MaxPageViewsCount = 3;
		
		private const float BarPaddingH = 5.0f;
		
		private const float BarPaddingV = 5.0f;
		
		private const float ToolbarHeight = 50.0f;
		
		private const float BottombarHeight = 45.0f;
		
		private const float FirstToolButtonLeft = 20.0f;
		
		private const float FirstToolButtonTop = 7.0f;
		
		private const float ToolButtonSize = 36.0f;	
		
		private readonly SizeF PageNumberLabelSize = new SizeF(75.0f, 35.0f);
		
		#endregion
		
		#region Fields	
		
		/// <summary>
		/// Loaded document id 
		/// </summary>
		private int mDocumentId;
			
		/// <summary>
		/// Main scroll view
		/// </summary>
		private UIScrollView mScrollView;
		
		/// <summary>
		/// Toolbar view
		/// </summary>
		private UIView mToolbar;
		
		/// <summary>
		/// Open navigate to page button
		/// </summary>
		private UIButton mBtnNavigateToPage;
			
		/// <summary>
		/// Open Note view button 
		/// </summary>
		private UIButton mBtnNote;
		
		/// <summary>
		/// Open bookmarsks list view button
		/// </summary>
		private UIButton mBtnBookmarksList;
		
		/// <summary>
		/// Open Thumbs view button
		/// </summary>
		private UIButton mBtnThumbs;
		
		/// <summary>
		/// Bottom bar view
		/// </summary>
		private UIView mBottomBar;
		
		/// <summary>
		/// Slider view
		/// </summary>
		private UISlider mSlider;
		
		/// <summary>
		/// Page number label
		/// </summary>
		private UILabel mPageNumberLabel;
		
		/// <summary>
		/// List of page views
		/// </summary>
		private List<PageView> mPageViews;
		
		/// <summary>
		/// Current page view
		/// </summary>
		private PageView mCurrentPageView;
		
		/// <summary>
		/// Notification observers
		/// </summary>
		private List<NSObject> mNotificationObservers; 
		
		/// <summary>
		/// Old device orientation
		/// </summary>
		private UIInterfaceOrientation mOldDeviceOrientation;
		
		#endregion
			
		#region Constructors

		public DocumentViewController(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		[Export("initWithCoder:")]
		public DocumentViewController(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		public DocumentViewController() : base(null, null)
		{
			Initialize(); 
		}

		void Initialize()
		{
			mNotificationObservers = new List<NSObject>();
			mPageViews = new List<PageView>();
		}
		
		#endregion
		
		#region UIViewController methods
		
		/// <summary>
		/// Calls when view has loaded
		/// </summary>
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			mOldDeviceOrientation = GetDeviceOrientation();
			
			// Init View			
			View.BackgroundColor = UIColor.ScrollViewTexturedBackgroundColor;
	
			// Create toolbar
			if (OptionsManager.Instance.Options.ToolbarVisible) {
				mToolbar = CreateToolbar();
				if (mToolbar != null) {
					View.AddSubview(mToolbar);
				}
			}
			
			// Create bottom bar (with slider)
			if (OptionsManager.Instance.Options.BottombarVisible) {
				mBottomBar = CreateBottomBar();
				if (mBottomBar != null) {
					View.AddSubview(mBottomBar);
				} else {
					if (mSlider != null) {
						mSlider.RemoveFromSuperview();
						mSlider.Dispose();
					}
					if (mPageNumberLabel != null) {
						mPageNumberLabel.RemoveFromSuperview();
						mPageNumberLabel.Dispose();
					}
				}
			}
			
			// Create pages view - Scrollview
			mScrollView = new UIScrollView(GetScrollViewFrame());
			mScrollView.ScrollsToTop = false;
			mScrollView.PagingEnabled = true;
			mScrollView.DelaysContentTouches = false;
			mScrollView.ShowsVerticalScrollIndicator = false;
			mScrollView.ShowsHorizontalScrollIndicator = false;
			mScrollView.ContentMode = UIViewContentMode.Redraw;
			mScrollView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			mScrollView.BackgroundColor = UIColor.Clear;
			mScrollView.UserInteractionEnabled = true;
			mScrollView.AutosizesSubviews = false;
			mScrollView.DecelerationEnded += ScrollViewDecelerationEnded;
			
			// Add root scroll view to parent
			View.AddSubview(mScrollView);
			
			// Subscribe notifications
			NSString notificationObserverDeviceRotated = new NSString("UIDeviceOrientationDidChangeNotification");
			var deviceRotated = NSNotificationCenter.DefaultCenter.AddObserver(notificationObserverDeviceRotated, DeviceRotated);
			mNotificationObservers.Add(deviceRotated);
		}
		
		/// <summary>
		/// Called when permission is shought to rotate
		/// </summary>
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		/// <summary>
		/// Calls when device rotated
		/// </summary>
		/// <param name="notification">NSNotification parameter</param>	
		private void DeviceRotated(NSNotification notification)
		{
			UIInterfaceOrientation newOrientation = GetDeviceOrientation();
			if (newOrientation != mOldDeviceOrientation) {
				UpdateScrollViewContentSize();
				UpdateScrollViewContentOffset();
				UpdatePageViewsFrame();
				ResetPageViewsZoomScale();
			}
			mOldDeviceOrientation = newOrientation; 
		}
		
		/// <summary>
		/// Calls when object is disposing
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			PDFDocument.CloseDocument();
			NSNotificationCenter.DefaultCenter.RemoveObservers(mNotificationObservers);
			base.Dispose(disposing);
		}
		
		#endregion
		
		#region UIScrollViewDelegate methods
		
		/// <summary>
		/// Calls when scrollView deceleration has ended
		/// </summary>
		private void ScrollViewDecelerationEnded(object sender, EventArgs args)
		{
			int pageNumber = 0;
			float contentOffset = (OptionsManager.Instance.Options.pPageTurningType == PageTurningType.Horizontal) ? mScrollView.ContentOffset.X : mScrollView.ContentOffset.Y;
			foreach (var view in mPageViews) {
				bool found = (OptionsManager.Instance.Options.pPageTurningType == PageTurningType.Horizontal) ? (contentOffset == view.Frame.X) : (contentOffset == view.Frame.Y);
				if (found) {
					pageNumber = view.PageNumber;
					break;
				}
			}
			if (pageNumber != 0) {
				OpenDocumentPage(pageNumber);
			}
		}
		
		#endregion
				
		#region Logic
		
		/// <summary>
		/// Presents popover
		/// </summary>
		private void PresentPopover(UIViewControllerWithPopover viewCtrl, RectangleF frame)
		{
			var popoverController = new UIPopoverController(viewCtrl);
			viewCtrl.PopoverController = popoverController;
			popoverController.PresentFromRect(frame, View, UIPopoverArrowDirection.Any, true);
		}	
		
		/// <summary>
		/// Gets current device orientation
		/// </summary>
		/// <returns>Current device orientation</returns>
		private UIInterfaceOrientation GetDeviceOrientation()
		{
			switch (UIDevice.CurrentDevice.Orientation) {
				case UIDeviceOrientation.LandscapeLeft:
					return UIInterfaceOrientation.LandscapeLeft;
				case UIDeviceOrientation.LandscapeRight:
					return UIInterfaceOrientation.LandscapeRight;
				case UIDeviceOrientation.Portrait:
					return UIInterfaceOrientation.Portrait;
				case UIDeviceOrientation.PortraitUpsideDown:
					return UIInterfaceOrientation.PortraitUpsideDown;
			}
			return UIInterfaceOrientation.Portrait;
		}
		
		/// <summary>
		/// Calculates and returns root scroll view frame 
		/// </summary>
		/// <returns>Root scroll view frame size</returns>
		private RectangleF GetScrollViewFrame()
		{
			var retValue = View.Bounds;
			if (mToolbar != null) {
				retValue.Y = mToolbar.Frame.Bottom;
				retValue.Height -= mToolbar.Bounds.Height + BarPaddingV;
			}
			if (mBottomBar != null) {
				retValue.Height -= mBottomBar.Bounds.Height + BarPaddingV;
			}
			return retValue;
		}
		
		/// <summary>
		/// Calculates and returns root srcoll view subivew frame
		/// </summary>
		/// <returns></returns>
		private RectangleF GetScrollViewSubViewFrame()
		{
			var retValue = RectangleF.Empty;
			retValue.Size = mScrollView.Bounds.Size;
			return retValue;
		}
		
		/// <summary>
		/// Updates main scroll view content size
		/// </summary>
		private void UpdateScrollViewContentSize()
		{
			int viewPagesCount = PDFDocument.PageCount;
			if (viewPagesCount > MaxPageViewsCount) {
				viewPagesCount = MaxPageViewsCount;
			}
			// Calc the content width and height
			float contentWidth;
			float contentHeight;
			if (OptionsManager.Instance.Options.pPageTurningType == PageTurningType.Horizontal) {
				contentWidth = mScrollView.Frame.Size.Width * viewPagesCount;
				contentHeight = mScrollView.Frame.Size.Height;
			} else {
				contentWidth = mScrollView.Frame.Size.Width;
				contentHeight = mScrollView.Frame.Size.Height * viewPagesCount;
			}
			mScrollView.ContentSize = new SizeF(contentWidth, contentHeight);
		}

		/// <summary>
		/// Updates main scroll view content offset
		/// </summary>
		private void UpdateScrollViewContentOffset()
		{
			// Calc new contentOffset position
			PointF contentOffset = PointF.Empty;
			RectangleF viewRect = RectangleF.Empty;
			viewRect.Size = mScrollView.Bounds.Size;
			if (OptionsManager.Instance.Options.pPageTurningType == PageTurningType.Horizontal) {
				float viewWidthX1 = viewRect.Width;
				float viewWidthX2 = viewWidthX1 * 2.0f;				
				if (PDFDocument.PageCount >= MaxPageViewsCount) {
					if (PDFDocument.CurrentPageNumber == PDFDocument.PageCount) {
						contentOffset.X = viewWidthX2;
					} else if (PDFDocument.CurrentPageNumber != 1) {
						contentOffset.X = viewWidthX1;
					}
				} else if (PDFDocument.CurrentPageNumber == (MaxPageViewsCount - 1)) {
					contentOffset.X = viewWidthX1;
				}
			} else {
				float viewHeightY1 = viewRect.Height;
				float viewHeightY2 = viewHeightY1 * 2.0f;
				if (PDFDocument.PageCount >= MaxPageViewsCount) {
					if (PDFDocument.CurrentPageNumber == PDFDocument.PageCount) {
						contentOffset.Y = viewHeightY2;
					} else if (PDFDocument.CurrentPageNumber != 1) {
						contentOffset.Y = viewHeightY1;
					}
				} else if (PDFDocument.CurrentPageNumber == (MaxPageViewsCount - 1)) {
					contentOffset.Y = viewHeightY1;
				}
			}
			// Move scrollView contentOffset to show left, middle or right view
			mScrollView.ContentOffset = contentOffset;
		}

		/// <summary>
		/// Updates page views frame
		/// </summary>
		private void UpdatePageViewsFrame()
		{
			RectangleF viewRect = GetScrollViewSubViewFrame();
			foreach (var view in mPageViews) {
				view.Frame = viewRect;
				viewRect = CalcFrameForNextPage(viewRect);
			}
		}

		/// <summary>
		/// Calculates the default page view content offset
		/// </summary>
		private PointF CalcDefaultPageViewContentOffset()
		{
			if (mCurrentPageView == null)
			{
				return new PointF(0, 0);
			}
			return OptionsManager.Instance.Options.pPageTurningType == PageTurningType.Horizontal
				? new PointF(0.0f, mCurrentPageView.ContentOffset.Y)
				: new PointF(mCurrentPageView.ContentOffset.X, 0.0f);
		}

		/// <summary>
		/// Sets equal zoomScale for all pages
		/// </summary>
		private void SetPagesEqualZoomScale(PageView pageView, float zoomScale)
		{
			foreach (var view in mPageViews) {
				if (view != pageView) {
					// Set equal zoomScale from all pageViews 
					view.ZoomScale = zoomScale;
					// Offset pageView, to it don't overlapped
					view.ContentOffset = CalcDefaultPageViewContentOffset();
				}
			}
		}

		/// <summary>
		/// Resets page views zoomScale 
		/// </summary>
		private void ResetPageViewsZoomScale()
		{
			foreach (var view in mPageViews) {
				view.UpdateMinimumMaximumZoom();
				view.ZoomReset();
			}
		}
		
		/// <summary>
		/// Returns rect for next page 
		/// </summary>
		/// <param name="viewRect">Frame rect for current page</param>
		/// <returns>Frame rect for next page</returns>
		private RectangleF CalcFrameForNextPage(RectangleF viewRect)
		{
			if (OptionsManager.Instance.Options.pPageTurningType == PageTurningType.Horizontal) {
				viewRect.X += viewRect.Size.Width;
			} else {
				viewRect.Y += viewRect.Size.Height;
			}
			return viewRect;
		}

		/// <summary>
		/// Update slider max value
		/// </summary>
		private void UpdateSliderMaxValue()
		{
			if (mSlider != null) {
				mSlider.MaxValue = PDFDocument.PageCount;
			}
		}		
		
		/// <summary>
		/// Creates toolbar
		/// </summary>
		/// <returns>Toolbar view</returns>
		protected virtual UIView CreateToolbar()
		{
			// Create toolbar
			var toolBarFrame = View.Bounds;
			toolBarFrame.X += BarPaddingH;
			toolBarFrame.Y += BarPaddingV;
			toolBarFrame.Width -= BarPaddingH * 2;
			toolBarFrame.Height = ToolbarHeight;
			var toolBar = new UIXToolbarView(toolBarFrame, 0.92f, 0.32f, 0.8f);
			toolBar.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleWidth;
			
			// Create toolbar buttons
			var btnFrame = new RectangleF(FirstToolButtonLeft, FirstToolButtonTop, ToolButtonSize, ToolButtonSize);
			CreateToolbarButton(toolBar, ref btnFrame, "Images/Toolbar/NavigateToFirst48.png", delegate {
				OpenFirstPage();
			});
			CreateToolbarButton(toolBar, ref btnFrame, "Images/Toolbar/NavigateToPrior48.png", delegate {
				OpenPriorPage();
			});
			mBtnNavigateToPage = CreateToolbarButton(toolBar, ref btnFrame, "Images/Toolbar/NavigateToPage48.png", delegate {
				var view = new GotoPageViewController(p => { OpenDocumentPage((int)p); });
				PresentPopover(view, mBtnNavigateToPage.Frame);
			});
			CreateToolbarButton(toolBar, ref btnFrame, "Images/Toolbar/NavigateToNext48.png", delegate {
				OpenNextPage();
			});
			CreateToolbarButton(toolBar, ref btnFrame, "Images/Toolbar/NavigateToLast48.png", delegate {
				OpenLastPage();
			});
			CreateToolbarButton(toolBar, ref btnFrame, "Images/Toolbar/ZoomOut48.png", delegate {
				ZoomOut();
			});
			CreateToolbarButton(toolBar, ref btnFrame, "Images/Toolbar/ZoomIn48.png", delegate {
				ZoomIn(); 
			});
			if (OptionsManager.Instance.Options.NoteBtnVisible) {
				mBtnNote = CreateToolbarButton(toolBar, ref btnFrame, "Images/Toolbar/Note48.png", delegate {
					var note = DocumentNoteManager.Instance.LoadNote(mDocumentId);
					var view = new NoteViewController(note, p => { /* Noting */ });
					PresentPopover(view, mBtnNote.Frame);
				});
			}
			if (OptionsManager.Instance.Options.BookmarksBtnVisible) {
				mBtnBookmarksList = CreateToolbarButton(toolBar, ref btnFrame, "Images/Toolbar/BookmarksList48.png", delegate {
					var bookmarks = DocumentBookmarkManager.Instance.LoadBookmarks(mDocumentId);
					var view = new BookmarksViewController(mDocumentId, bookmarks, PDFDocument.CurrentPageNumber, p => { OpenDocumentPage((int)p); });
					PresentPopover(view, mBtnBookmarksList.Frame);
				});
			}
			if (OptionsManager.Instance.Options.ThumbsBtnVisible) {
				mBtnThumbs = CreateToolbarButton(toolBar, ref btnFrame, "Images/Toolbar/Thumbs32.png", delegate {
					var view = new ThumbsViewController(View.Bounds.Width, p => { OpenDocumentPage((int)p); });
					PresentPopover(view, mBtnThumbs.Frame);
					view.InitThumbs();
				});
			}
			return toolBar;
		}
		
		/// <summary>
		/// Creates toolbar button
		/// </summary>
		/// <param name="toolbar">Toolbar view</param>
		/// <param name="frame">Button frame</param>
		/// <param name="imageUrl">Image button url</param>
		/// <param name="action">Assoutiated button action</param>
		/// <returns>Button view</returns>
		protected virtual UIButton CreateToolbarButton(UIView toolbar, ref RectangleF frame, string imageUrl, Action action)
		{
			var btn = new UIButton(frame);
			btn.SetImage(UIImage.FromFile(imageUrl), UIControlState.Normal);
			btn.TouchUpInside += delegate { 
				action();
			};
			toolbar.AddSubview(btn);
			frame.Offset(44, 0);
			return btn;
		}
		
		/// <summary>
		/// Creates bottom bar
		/// </summary>
		/// <returns>Bottom bar view</returns>
		protected virtual UIView CreateBottomBar()
		{
			// Create bottom bar	
			var bottomBarFrame = View.Bounds;
			bottomBarFrame.X += BarPaddingH;
			bottomBarFrame.Y = bottomBarFrame.Size.Height - BarPaddingV - BottombarHeight;
			bottomBarFrame.Width -= BarPaddingH * 2;
			bottomBarFrame.Height = BottombarHeight;
			var bottomBar = new UIXToolbarView(bottomBarFrame, 0.92f, 0.32f, 0.8f);
			bottomBar.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleWidth;
			
			// Create slider
			float sliderWidth = bottomBarFrame.Width - 15;
			if (OptionsManager.Instance.Options.PageNumberVisible) {
				sliderWidth -= PageNumberLabelSize.Width;
			}
			var pageSliderFrame = new RectangleF(5, 10, sliderWidth, 20);
			mSlider = new UISlider(pageSliderFrame);
			mSlider.MinValue = 1;
			mSlider.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			mSlider.ValueChanged += delegate { 
				OpenDocumentPage((int)mSlider.Value);
			};
			bottomBar.AddSubview(mSlider);
			
			// Create page number view		
			if (OptionsManager.Instance.Options.PageNumberVisible) {
				var pageNumberViewFrame = new RectangleF(pageSliderFrame.Width + 10, 5, PageNumberLabelSize.Width, PageNumberLabelSize.Height);
				var pageNumberView = new UIView(pageNumberViewFrame);
				pageNumberView.AutosizesSubviews = false;
				pageNumberView.UserInteractionEnabled = false;
				pageNumberView.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin;
				pageNumberView.BackgroundColor = UIColor.FromWhiteAlpha(0.4f, 0.5f);
				pageNumberView.Layer.CornerRadius = 5.0f;
				pageNumberView.Layer.ShadowOffset = new SizeF(0.0f, 0.0f);
				pageNumberView.Layer.ShadowPath = UIBezierPath.FromRect(pageNumberView.Bounds).CGPath;
				pageNumberView.Layer.ShadowRadius = 2.0f;
				pageNumberView.Layer.ShadowOpacity = 1.0f;
				// Create page number label
				var pageNumberLabelFrame = RectangleFExtensions.Inset(pageNumberView.Bounds, 4.0f, 2.0f);
				mPageNumberLabel = new UILabel(pageNumberLabelFrame);
				mPageNumberLabel.AutosizesSubviews = false;
				mPageNumberLabel.AutoresizingMask = UIViewAutoresizing.None;
				mPageNumberLabel.TextAlignment = UITextAlignment.Center;
				mPageNumberLabel.BackgroundColor = UIColor.Clear;
				mPageNumberLabel.TextColor = UIColor.White;
				mPageNumberLabel.Font = UIFont.SystemFontOfSize(16.0f);
				mPageNumberLabel.ShadowOffset = new SizeF(0.0f, 1.0f);
				mPageNumberLabel.ShadowColor = UIColor.Black;
				mPageNumberLabel.AdjustsFontSizeToFitWidth = true;
				mPageNumberLabel.MinimumFontSize = 12.0f;
				pageNumberView.AddSubview(mPageNumberLabel);
				bottomBar.AddSubview(pageNumberView);
			}			
			return bottomBar;
		}

		/// <summary>
		/// Opens document
		/// </summary>
		/// <param name="docId">Document id</param>
		/// <param name="docName">Document name</param>
		/// <param name="docPath">Document file path</param>
		public virtual void OpenDocument(int docId, string docName, string docPath)
		{
			mDocumentId = docId;
			Title = docName;
			PDFDocument.OpenDocument(docName, docPath);
			UpdateScrollViewContentSize();
			UpdatePageViewsFrame();
			UpdateSliderMaxValue();
			OpenDocumentPage(1);
		}
		
		/// <summary>
		/// Opens document page
		/// </summary>
		/// <param name="pageNumber">Document page number</param>
		public virtual void OpenDocumentPage(int pageNumber)
		{
			if (PDFDocument.DocumentHasLoaded && (pageNumber != PDFDocument.CurrentPageNumber)) {
				if ((pageNumber < 1) || (pageNumber > PDFDocument.PageCount)) {
					return;
				}
				
				// Set current page
				PDFDocument.CurrentPageNumber = pageNumber;
				
				// Calc min, max page	
				int minValue;
				int maxValue;
				if (PDFDocument.PageCount <= MaxPageViewsCount) {
					minValue = 1;
					maxValue = PDFDocument.PageCount;
				} else {
					minValue = PDFDocument.CurrentPageNumber - 1;
					maxValue = PDFDocument.CurrentPageNumber + 1;
					if (minValue < 1) {
						minValue++;
						maxValue++;
					} else if (maxValue > PDFDocument.PageCount) {
						minValue--;
						maxValue--;
					}
				}
				
				// Create/update page views for displayed pages				
				var unusedPageViews = new List<PageView>(mPageViews);
				RectangleF viewRect = GetScrollViewSubViewFrame();
				for (int i = minValue, j = 0; i <= maxValue; i++,j++) {
					PageView pageView = mPageViews.FirstOrDefault(v => v.PageNumber == i);
					if (pageView == null) {
						pageView = new PageView(viewRect, i);
						pageView.ZoomScale = mCurrentPageView != null ? mCurrentPageView.ZoomScale : pageView.MinimumZoomScale;
						pageView.ContentOffset = CalcDefaultPageViewContentOffset();
						pageView.ZoomingEnded += delegate(object sender, ZoomingEndedEventArgs e) {
							SetPagesEqualZoomScale((PageView)sender, e.AtScale);
						};
						mScrollView.AddSubview(pageView);
						mPageViews.Add(pageView);
					} else {
						pageView.Frame = viewRect;
						pageView.PageNumber = i;
						unusedPageViews.Remove(pageView);
					}
					viewRect = CalcFrameForNextPage(viewRect);
					if (i == PDFDocument.CurrentPageNumber) {
						mCurrentPageView = pageView;
					}
				}
				// Clear unused page views
				foreach (var view in unusedPageViews) {
					view.RemoveFromSuperview();
					mPageViews.Remove(view);					
				}
		
				// Update scroll view content offset
				UpdateScrollViewContentOffset();
				
				// Update PageNumber label
				if (mPageNumberLabel != null) {
					mPageNumberLabel.Text = string.Format(@"{0}/{1}", PDFDocument.CurrentPageNumber, PDFDocument.PageCount);
				}
				
				// Update slider position
				if (mSlider != null) {
					mSlider.Value = PDFDocument.CurrentPageNumber;
				}
			}
		}
		
		#endregion
		
		#region Events
			
		/// <summary>
		/// Opens first page of the document
		/// </summary>
		protected virtual void OpenFirstPage()
		{
			OpenDocumentPage(1);
		}
		
		/// <summary>
		/// Opens prior page of the document
		/// </summary>
		protected virtual void OpenPriorPage()
		{
			OpenDocumentPage(PDFDocument.CurrentPageNumber - 1);
		}
		
		/// <summary>
		/// Opens next page of the document
		/// </summary>
		protected virtual void OpenNextPage()
		{
			OpenDocumentPage(PDFDocument.CurrentPageNumber + 1);
		}
		
		/// <summary>
		/// Opens last page of the document
		/// </summary>
		protected virtual void OpenLastPage()
		{
			OpenDocumentPage(PDFDocument.PageCount);
		}
		
		/// <summary>
		/// Zooming out
		/// </summary>
		protected virtual void ZoomOut()
		{
			if (PDFDocument.DocumentHasLoaded) {
				mCurrentPageView.ZoomDecrement();
			}
		}
		
		/// <summary>
		/// Zooming in
		/// </summary>
		protected virtual void ZoomIn()
		{
			if (PDFDocument.DocumentHasLoaded) {
				mCurrentPageView.ZoomIncrement();
			}
		}
		
		#endregion
	}
}