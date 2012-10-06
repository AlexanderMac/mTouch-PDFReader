//
// mTouch-PDFReader library
// DocumentViewController.cs (Document view controller implementation)
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
		/// The document id.
		/// </summary>
		private int _DocumentId;
			
		/// <summary>
		/// The main scroll view.
		/// </summary>
		private UIScrollView _ScrollView;
		
		/// <summary>
		/// The toolbar view.
		/// </summary>
		private UIView _Toolbar;
		
		/// <summary>
		/// The open page button.
		/// </summary>
		private UIButton _BtnNavigateToPage;
			
		/// <summary>
		/// The open note view button. 
		/// </summary>
		private UIButton _BtnNote;
		
		/// <summary>
		/// The open bookmarsks list view button.
		/// </summary>
		private UIButton _BtnBookmarksList;
		
		/// <summary>
		/// The open thumbs view button.
		/// </summary>
		private UIButton _BtnThumbs;
		
		/// <summary>
		/// The bottom bar view.
		/// </summary>
		private UIView _BottomBar;
		
		/// <summary>
		/// The slider view.
		/// </summary>
		private UISlider _Slider;
		
		/// <summary>
		/// The page number label.
		/// </summary>
		private UILabel _PageNumberLabel;
		
		/// <summary>
		/// The list of the page views.
		/// </summary>
		private List<PageView> _PageViews;
		
		/// <summary>
		/// The current page view.
		/// </summary>
		private PageView _CurrentPageView;
		
		/// <summary>
		/// The notification observers.
		/// </summary>
		private List<NSObject> _NotificationObservers; 
		
		/// <summary>
		/// The old device orientation.
		/// </summary>
		private UIInterfaceOrientation _OldDeviceOrientation;		
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
			_NotificationObservers = new List<NSObject>();
			_PageViews = new List<PageView>();
		}		
		#endregion
		
		#region UIViewController methods	
		/// <summary>
		/// Calls when view has loaded
		/// </summary>
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			_OldDeviceOrientation = GetDeviceOrientation();
			
			// Init View			
			View.BackgroundColor = UIColor.ScrollViewTexturedBackgroundColor;
	
			// Create toolbar
			if (RC.Get<IOptionsManager>().Options.ToolbarVisible) {
				_Toolbar = CreateToolbar();
				if (_Toolbar != null) {
					View.AddSubview(_Toolbar);
				}
			}
			
			// Create bottom bar (with slider)
			if (RC.Get<IOptionsManager>().Options.BottombarVisible) {
				_BottomBar = CreateBottomBar();
				if (_BottomBar != null) {
					View.AddSubview(_BottomBar);
				} else {
					if (_Slider != null) {
						_Slider.RemoveFromSuperview();
						_Slider.Dispose();
					}
					if (_PageNumberLabel != null) {
						_PageNumberLabel.RemoveFromSuperview();
						_PageNumberLabel.Dispose();
					}
				}
			}
			
			// Create pages view - Scrollview
			_ScrollView = new UIScrollView(GetScrollViewFrame());
			_ScrollView.ScrollsToTop = false;
			_ScrollView.PagingEnabled = true;
			_ScrollView.DelaysContentTouches = false;
			_ScrollView.ShowsVerticalScrollIndicator = false;
			_ScrollView.ShowsHorizontalScrollIndicator = false;
			_ScrollView.ContentMode = UIViewContentMode.Redraw;
			_ScrollView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			_ScrollView.BackgroundColor = UIColor.Clear;
			_ScrollView.UserInteractionEnabled = true;
			_ScrollView.AutosizesSubviews = false;
			_ScrollView.DecelerationEnded += ScrollViewDecelerationEnded;
			
			// Add root scroll view to parent
			View.AddSubview(_ScrollView);
			
			// Subscribe notifications
			NSString notificationObserverDeviceRotated = new NSString("UIDeviceOrientationDidChangeNotification");
			var deviceRotated = NSNotificationCenter.DefaultCenter.AddObserver(notificationObserverDeviceRotated, DeviceRotated);
			_NotificationObservers.Add(deviceRotated);
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
			if (newOrientation != _OldDeviceOrientation) {
				UpdateScrollViewContentSize();
				UpdateScrollViewContentOffset();
				UpdatePageViewsFrame();
				ResetPageViewsZoomScale();
			}
			_OldDeviceOrientation = newOrientation; 
		}
		
		/// <summary>
		/// Calls when object is disposing
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			PDFDocument.CloseDocument();
			NSNotificationCenter.DefaultCenter.RemoveObservers(_NotificationObservers);
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
			float contentOffset = (RC.Get<IOptionsManager>().Options.PageTurningType == PageTurningTypes.Horizontal) ? _ScrollView.ContentOffset.X : _ScrollView.ContentOffset.Y;
			foreach (var view in _PageViews) {
				bool found = (RC.Get<IOptionsManager>().Options.PageTurningType == PageTurningTypes.Horizontal) ? (contentOffset == view.Frame.X) : (contentOffset == view.Frame.Y);
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
			if (_Toolbar != null) {
				retValue.Y = _Toolbar.Frame.Bottom;
				retValue.Height -= _Toolbar.Bounds.Height + BarPaddingV;
			}
			if (_BottomBar != null) {
				retValue.Height -= _BottomBar.Bounds.Height + BarPaddingV;
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
			retValue.Size = _ScrollView.Bounds.Size;
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
			if (RC.Get<IOptionsManager>().Options.PageTurningType == PageTurningTypes.Horizontal) {
				contentWidth = _ScrollView.Frame.Size.Width * viewPagesCount;
				contentHeight = _ScrollView.Frame.Size.Height;
			} else {
				contentWidth = _ScrollView.Frame.Size.Width;
				contentHeight = _ScrollView.Frame.Size.Height * viewPagesCount;
			}
			_ScrollView.ContentSize = new SizeF(contentWidth, contentHeight);
		}

		/// <summary>
		/// Updates main scroll view content offset
		/// </summary>
		private void UpdateScrollViewContentOffset()
		{
			// Calc new contentOffset position
			PointF contentOffset = PointF.Empty;
			RectangleF viewRect = RectangleF.Empty;
			viewRect.Size = _ScrollView.Bounds.Size;
			if (RC.Get<IOptionsManager>().Options.PageTurningType == PageTurningTypes.Horizontal) {
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
			_ScrollView.ContentOffset = contentOffset;
		}

		/// <summary>
		/// Updates page views frame
		/// </summary>
		private void UpdatePageViewsFrame()
		{
			RectangleF viewRect = GetScrollViewSubViewFrame();
			foreach (var view in _PageViews) {
				view.Frame = viewRect;
				viewRect = CalcFrameForNextPage(viewRect);
			}
		}

		/// <summary>
		/// Calculates the default page view content offset
		/// </summary>
		private PointF CalcDefaultPageViewContentOffset(int pageNumber)
		{
			if (_CurrentPageView == null) {
				return new PointF(0, 0);
			}
			bool pageIsAfterCurrent = pageNumber > _CurrentPageView.PageNumber;
			if (RC.Get<IOptionsManager>().Options.PageTurningType == PageTurningTypes.Horizontal) {
				var left = pageIsAfterCurrent ? 0.0f : _CurrentPageView.Bounds.Width;
				return new PointF(left, _CurrentPageView.ContentOffset.Y);
			} else {
				var top = pageIsAfterCurrent ? 0.0f : _CurrentPageView.Bounds.Height;
				return new PointF(_CurrentPageView.ContentOffset.X, top);
			}
		}

		/// <summary>
		/// Sets equal zoomScale for all pages
		/// </summary>
		private void SetPagesEqualZoomScale(PageView currentPageView, float zoomScale)
		{
			for (int i = 0; i < _PageViews.Count; i++) {
				var view = _PageViews [i];
				if (view != currentPageView) {
					// Set equal zoomScale from all pageViews 
					view.ZoomScale = zoomScale;
					// Offset pageView, to it don't overlapped
					view.ContentOffset = CalcDefaultPageViewContentOffset(view.PageNumber);
				}
			}
		}

		/// <summary>
		/// Sets equal offset for all pages
		/// </summary>
		private void SetPagesEqualOffset(PageView currentPageView)
		{
			for (int i = 0; i < _PageViews.Count; i++) {
				var view = _PageViews [i];
				if (view != currentPageView) {
					view.ContentOffset = CalcDefaultPageViewContentOffset(view.PageNumber);
				}
			}
		}

		/// <summary>
		/// Resets page views zoomScale 
		/// </summary>
		private void ResetPageViewsZoomScale()
		{
			foreach (var view in _PageViews) {
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
			if (RC.Get<IOptionsManager>().Options.PageTurningType == PageTurningTypes.Horizontal) {
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
			if (_Slider != null) {
				_Slider.MaxValue = PDFDocument.PageCount;
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
			_BtnNavigateToPage = CreateToolbarButton(toolBar, ref btnFrame, "Images/Toolbar/NavigateToPage48.png", delegate {
				var view = new GotoPageViewController(p => {
					OpenDocumentPage((int)p); });
				PresentPopover(view, _BtnNavigateToPage.Frame);
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
			if (RC.Get<IOptionsManager>().Options.NoteBtnVisible) {
				_BtnNote = CreateToolbarButton(toolBar, ref btnFrame, "Images/Toolbar/Note48.png", delegate {
					var note = RC.Get<IDocumentNoteManager>().Load(_DocumentId);
					var view = new NoteViewController(note, p => { /* Noting */ });
					PresentPopover(view, _BtnNote.Frame);
				});
			}
			if (RC.Get<IOptionsManager>().Options.BookmarksBtnVisible) {
				_BtnBookmarksList = CreateToolbarButton(toolBar, ref btnFrame, "Images/Toolbar/BookmarksList48.png", delegate {
					var bookmarks = RC.Get<IDocumentBookmarkManager>().LoadList(_DocumentId);
					var view = new BookmarksViewController(_DocumentId, bookmarks, PDFDocument.CurrentPageNumber, p => {
						OpenDocumentPage((int)p); });
					PresentPopover(view, _BtnBookmarksList.Frame);
				});
			}
			if (RC.Get<IOptionsManager>().Options.ThumbsBtnVisible) {
				_BtnThumbs = CreateToolbarButton(toolBar, ref btnFrame, "Images/Toolbar/Thumbs32.png", delegate {
					var view = new ThumbsViewController(View.Bounds.Width, p => {
						OpenDocumentPage((int)p); });
					PresentPopover(view, _BtnThumbs.Frame);
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
			if (RC.Get<IOptionsManager>().Options.PageNumberVisible) {
				sliderWidth -= PageNumberLabelSize.Width;
			}
			var pageSliderFrame = new RectangleF(5, 10, sliderWidth, 20);
			_Slider = new UISlider(pageSliderFrame);
			_Slider.MinValue = 1;
			_Slider.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			_Slider.ValueChanged += delegate { 
				OpenDocumentPage((int)_Slider.Value);
			};
			bottomBar.AddSubview(_Slider);

			// Create page number view		
			if (RC.Get<IOptionsManager>().Options.PageNumberVisible) {
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
				_PageNumberLabel = new UILabel(pageNumberLabelFrame);
				_PageNumberLabel.AutosizesSubviews = false;
				_PageNumberLabel.AutoresizingMask = UIViewAutoresizing.None;
				_PageNumberLabel.TextAlignment = UITextAlignment.Center;
				_PageNumberLabel.BackgroundColor = UIColor.Clear;
				_PageNumberLabel.TextColor = UIColor.White;
				_PageNumberLabel.Font = UIFont.SystemFontOfSize(16.0f);
				_PageNumberLabel.ShadowOffset = new SizeF(0.0f, 1.0f);
				_PageNumberLabel.ShadowColor = UIColor.Black;
				_PageNumberLabel.AdjustsFontSizeToFitWidth = true;
				_PageNumberLabel.MinimumFontSize = 12.0f;
				pageNumberView.AddSubview(_PageNumberLabel);
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
			_DocumentId = docId;
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
				var unusedPageViews = new List<PageView>(_PageViews);
				RectangleF viewRect = GetScrollViewSubViewFrame();
				for (int i = minValue, j = 0; i <= maxValue; i++,j++) {
					PageView pageView = _PageViews.FirstOrDefault(v => v.PageNumber == i);
					if (pageView == null) {
						pageView = new PageView(viewRect, i);
						pageView.ZoomScale = _CurrentPageView != null ? _CurrentPageView.ZoomScale : pageView.MinimumZoomScale;
						pageView.ContentOffset = CalcDefaultPageViewContentOffset(i);
						pageView.ZoomingEnded += delegate(object sender, ZoomingEndedEventArgs e) {
							var view = (PageView)sender;
							SetPagesEqualZoomScale(view, view.ZoomScale);
						};/*
						pageView.DecelerationEnded += delegate(object sender, EventArgs e) {
							var view = (PageView)sender;
							SetPagesEqualOffset(view);
						};*/
						_ScrollView.AddSubview(pageView);
						_PageViews.Add(pageView);
					} else {
						pageView.Frame = viewRect;
						pageView.PageNumber = i;
						unusedPageViews.Remove(pageView);
					}
					viewRect = CalcFrameForNextPage(viewRect);
					if (i == PDFDocument.CurrentPageNumber) {
						_CurrentPageView = pageView;
					}
				}
				// Clear unused page views
				foreach (var view in unusedPageViews) {
					view.RemoveFromSuperview();
					_PageViews.Remove(view);					
				}
		
				// Update scroll view content offset
				UpdateScrollViewContentOffset();
				
				// Update PageNumber label
				if (_PageNumberLabel != null) {
					_PageNumberLabel.Text = string.Format(@"{0}/{1}", PDFDocument.CurrentPageNumber, PDFDocument.PageCount);
				}
				
				// Update slider position
				if (_Slider != null) {
					_Slider.Value = PDFDocument.CurrentPageNumber;
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
				_CurrentPageView.ZoomDecrement();
			}
		}
		
		/// <summary>
		/// Zooming in
		/// </summary>
		protected virtual void ZoomIn()
		{
			if (PDFDocument.DocumentHasLoaded) {
				_CurrentPageView.ZoomIncrement();
			}
		}		
		#endregion
	}
}