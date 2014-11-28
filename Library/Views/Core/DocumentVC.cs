//
// mTouch-PDFReader library
// DocumentVC.cs
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
using System.Drawing;
using System.Linq;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.XViews;
using mTouchPDFReader.Library.Views.Management;
using mTouchPDFReader.Library.Managers;
using mTouchPDFReader.Library.Data.Enums;

namespace mTouchPDFReader.Library.Views.Core
{
	public class DocumentVC : UIViewController
	{			
		#region Constants		
		private const int MaxPageViewsCount = 3;
		private const int MaxToolbarButtonsCount = 15;
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
		private readonly int _documentId;
		private readonly string _documentName;
		private readonly string _documentPath;
		private UIPageViewController _bookPageViewController;
		private UIView _toolbar;
		private UIButton _btnNavigateToPage;
		private UIButton _btnNote;
		private UIButton _btnBookmarksList;
		private UIButton _btnThumbs;
		private UIButton _btnAutoWidth;
		private UIButton _btnAutoHeight;
		private UIView _bottomBar;
		private UISlider _slider;
		private UILabel _pageNumberLabel;
		private AutoScaleModes _autoScaleMode;
		#endregion
			
		#region UIViewController members
		public DocumentVC(int docId, string docName, string docPath) : base(null, null)
		{
			_documentId = docId;
			_documentName = docName;
			_documentPath = docPath;
		}
	
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			PDFDocument.OpenDocument(_documentName, _documentPath);
			
			Title = _documentName;
			View.BackgroundColor = UIColor.Gray;
			_autoScaleMode = MgrAccessor.OptionsMgr.Settings.AutoScaleMode;
	
			if (MgrAccessor.OptionsMgr.Settings.ToolbarVisible) {
				_toolbar = createToolbar();
				View.AddSubview(_toolbar);
			}
			
			if (MgrAccessor.OptionsMgr.Settings.SliderVisible || MgrAccessor.OptionsMgr.Settings.PageNumberVisible) {
				_bottomBar = createBottomBar();
				View.AddSubview(_bottomBar);
				updateSliderMaxValue();
			}

			_bookPageViewController = new UIPageViewController(
				MgrAccessor.OptionsMgr.Settings.PageTransitionStyle,
				MgrAccessor.OptionsMgr.Settings.PageNavigationOrientation, 
				UIPageViewControllerSpineLocation.Min);		
			_bookPageViewController.View.Frame = getBookViewFrameRect();
			_bookPageViewController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			_bookPageViewController.View.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
			_bookPageViewController.GetNextViewController = getNextPageViewController;
			_bookPageViewController.GetPreviousViewController = getPreviousPageViewController;
			_bookPageViewController.GetSpineLocation = getSpineLocation;
			_bookPageViewController.DidFinishAnimating += delegate(object sender, UIPageViewFinishedAnimationEventArgs e) {
				pageFinishedAnimation(e.Completed, e.Finished, e.PreviousViewControllers);
			};
			_bookPageViewController.SetViewControllers(
				new UIViewController[] { getPageViewController(1) }, 
				UIPageViewControllerNavigationDirection.Forward, 
				false,
				s => execActionsAfterOpenPage());	

			AddChildViewController(_bookPageViewController);
			_bookPageViewController.DidMoveToParentViewController(this);
			View.AddSubview(_bookPageViewController.View);
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			foreach (var pageVC in _bookPageViewController.ChildViewControllers.Cast<PageVC>()) {
				pageVC.PageView.NeedUpdateZoomAndOffset = true;
			}
		}

		protected override void Dispose(bool disposing)
		{
			PDFDocument.CloseDocument();
			base.Dispose(disposing);
		}		
		#endregion
		
		#region UIPageViewController	
		private UIViewController getPreviousPageViewController(UIPageViewController pageController, UIViewController referenceViewController)
		{			
			var curPageCntr = referenceViewController as PageVC;
			if (curPageCntr.PageNumber == 0) {
				return null;				
			} 
			int pageNumber = curPageCntr.PageNumber - 1;

			return getPageViewController(pageNumber);
		}

		private UIViewController getNextPageViewController(UIPageViewController pageController, UIViewController referenceViewController)
		{			
			var curPageCntr = referenceViewController as PageVC;
			if (curPageCntr.PageNumber == (PDFDocument.PageCount)) {				
				return _bookPageViewController.SpineLocation == UIPageViewControllerSpineLocation.Mid
					? getEmptyPageContentVC()
					: null;	
			} else if (curPageCntr.PageNumber == -1) { 
				return null;
			}
			int pageNumber = curPageCntr.PageNumber + 1;

			return getPageViewController(pageNumber);
		}	

		private UIPageViewControllerSpineLocation getSpineLocation(UIPageViewController pageViewController, UIInterfaceOrientation orientation)
		{
			var currentPageVC = getCurrentPageContentVC();
			pageViewController.DoubleSided = false;
			pageViewController.SetViewControllers(new UIViewController[] { currentPageVC }, UIPageViewControllerNavigationDirection.Forward, true, s => { });

			return UIPageViewControllerSpineLocation.Min;
		}

		private void pageFinishedAnimation(bool completed, bool finished, UIViewController[] previousViewControllers)
		{
			if (completed) {
				execActionsAfterOpenPage();
			}
		}
		#endregion
				
		#region UI Logic
		private RectangleF getActualViewFrameRect()
		{
			var rect = View.Bounds;

			if (NavigationController != null) {
				rect.Y += NavigationController.NavigationBar.Frame.Bottom;
				rect.Height -= NavigationController.NavigationBar.Frame.Bottom;
			}
			if (TabBarController != null) {
				rect.Height -= (View.Bounds.Bottom - TabBarController.TabBar.Frame.Top);
			}

			return rect;
		}

		private void presentPopover(UIViewControllerWithPopover viewCtrl, RectangleF frame)
		{
			var popoverController = new UIPopoverController(viewCtrl);
			viewCtrl.PopoverController = popoverController;
			popoverController.PresentFromRect(frame, View, UIPopoverArrowDirection.Any, true);
		}	
		
		private UIInterfaceOrientation getDeviceOrientation()
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
		
		private void updateSliderMaxValue()
		{
			if (_slider != null) {
				_slider.MaxValue = PDFDocument.PageCount;
			}
		}		

		protected virtual UIView createToolbar()
		{
			var toolBarFrame = getActualViewFrameRect();
			toolBarFrame.X += BarPaddingH;
			toolBarFrame.Y += BarPaddingV;
			toolBarFrame.Width -= BarPaddingH * 2;
			toolBarFrame.Height = ToolbarHeight;

			var toolBar = new UIXToolbarView(toolBarFrame, 0.92f, 0.32f, 0.8f);
			toolBar.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleWidth;

			var btnFrame = new RectangleF(FirstToolButtonLeft, FirstToolButtonTop, ToolButtonSize, ToolButtonSize);

			createToolbarButton(toolBar, DocumentActionTypes.NavigateToFirstPage, ref btnFrame, "Images/Toolbar/NavigateToFirst48.png", openFirstPage);
			createToolbarButton(toolBar, DocumentActionTypes.NavigateToPriorPage, ref btnFrame, "Images/Toolbar/NavigateToPrior48.png", openPriorPage);
			_btnNavigateToPage = createToolbarButton(toolBar, DocumentActionTypes.NavigateToPage, ref btnFrame, "Images/Toolbar/NavigateToPage48.png", () => {
				var view = new GotoPageVC(p => OpenDocumentPage((int)p));
				presentPopover(view, _btnNavigateToPage.Frame);
			});
			createToolbarButton(toolBar, DocumentActionTypes.NavigateToNextPage, ref btnFrame, "Images/Toolbar/NavigateToNext48.png", openNextPage);
			createToolbarButton(toolBar, DocumentActionTypes.NavigateToLastPage, ref btnFrame, "Images/Toolbar/NavigateToLast48.png", openLastPage);
			createToolbarSeparator(ref btnFrame);

			createToolbarButton(toolBar, DocumentActionTypes.ZoomOut, ref btnFrame, "Images/Toolbar/ZoomOut48.png", zoomOut);
			createToolbarButton(toolBar, DocumentActionTypes.ZoomIn, ref btnFrame, "Images/Toolbar/ZoomIn48.png", zoomIn);
			createToolbarSeparator(ref btnFrame);

			_btnNote = createToolbarButton(toolBar, DocumentActionTypes.Note, ref btnFrame, "Images/Toolbar/Note48.png", () => {
				var note = MgrAccessor.DocumentNoteMgr.Load(_documentId);
				var view = new NoteVC(note, null);
				presentPopover(view, _btnNote.Frame);
			});
			_btnBookmarksList = createToolbarButton(toolBar, DocumentActionTypes.Bookmarks, ref btnFrame, "Images/Toolbar/BookmarksList48.png", () => {
				var bookmarks = MgrAccessor.DocumentBookmarkMgr.LoadList(_documentId);
				var view = new BookmarksVC(_documentId, bookmarks, PDFDocument.CurrentPageNumber, p => OpenDocumentPage((int)p));
				presentPopover(view, _btnBookmarksList.Frame);
			});
			_btnThumbs = createToolbarButton(toolBar, DocumentActionTypes.Thumbs, ref btnFrame, "Images/Toolbar/Thumbs48.png", () => {
				var view = new ThumbsVC(View.Bounds.Width, p => OpenDocumentPage((int)p));
				presentPopover(view, _btnThumbs.Frame);
				view.InitThumbs();
			});
			createToolbarSeparator(ref btnFrame);

			_btnAutoWidth = createToolbarButton(toolBar, DocumentActionTypes.AutoWidth, ref btnFrame, getImagePathForButton(DocumentActionTypes.AutoWidth), () => setAutoWidth());
			_btnAutoHeight = createToolbarButton(toolBar, DocumentActionTypes.AutoHeight, ref btnFrame, getImagePathForButton(DocumentActionTypes.AutoHeight), () => setAutoHeight());
			createToolbarSeparator(ref btnFrame);

			createUserDefinedToolbarItems(toolBar, ref btnFrame);

			return toolBar;
		}

		private void createToolbarSeparator(ref RectangleF frame)
		{
			frame.Offset(22, 0);
		}

		protected virtual UIButton createToolbarButton(UIView toolbar, DocumentActionTypes actionType, ref RectangleF frame, string imagePath, Action action)
		{
			var btn = new UIButton(frame);
			btn.SetImage(UIImage.FromFile(imagePath), UIControlState.Normal);
			btn.TouchUpInside += delegate { 
				action();
			};
			toolbar.AddSubview(btn);
			frame.Offset(44, 0);

			return btn;
		}

		protected virtual void createUserDefinedToolbarItems(UIView toolbar, ref RectangleF frame)
		{
			// Nothing. Should be overrided in child class.
		}

		protected virtual UIView createBottomBar()
		{
			var bottomBarFrame = getActualViewFrameRect();
			bottomBarFrame.X += BarPaddingH;
			bottomBarFrame.Y += bottomBarFrame.Height - (BottombarHeight + BarPaddingV);
			bottomBarFrame.Width -= BarPaddingH * 2;
			bottomBarFrame.Height = BottombarHeight;

			var bottomBar = new UIXToolbarView(bottomBarFrame, 0.92f, 0.32f, 0.8f);
			bottomBar.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleWidth;

			if (MgrAccessor.OptionsMgr.Settings.SliderVisible) {
				float sliderWidth = bottomBarFrame.Width - 15;
				if (MgrAccessor.OptionsMgr.Settings.PageNumberVisible) {
					sliderWidth -= PageNumberLabelSize.Width;
				}
				var pageSliderFrame = new RectangleF(5, 10, sliderWidth, 20);
				_slider = new UISlider(pageSliderFrame);
				_slider.MinValue = 1;
				_slider.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
				_slider.ValueChanged += delegate {
					if (_pageNumberLabel != null) {
						_pageNumberLabel.Text = string.Format(@"{0}/{1}", (int)_slider.Value, PDFDocument.PageCount);
					}
				};
				_slider.TouchUpInside += delegate(object sender, EventArgs e) {
					OpenDocumentPage((int)_slider.Value);
				};
				bottomBar.AddSubview(_slider);
			}

			if (MgrAccessor.OptionsMgr.Settings.PageNumberVisible) {
				var pageNumberViewFrame = new RectangleF(bottomBarFrame.Width - PageNumberLabelSize.Width - 5, 5, PageNumberLabelSize.Width, PageNumberLabelSize.Height);
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

				var pageNumberLabelFrame = RectangleFExtensions.Inset(pageNumberView.Bounds, 4.0f, 2.0f);
				_pageNumberLabel = new UILabel(pageNumberLabelFrame);
				_pageNumberLabel.AutosizesSubviews = false;
				_pageNumberLabel.AutoresizingMask = UIViewAutoresizing.None;
				_pageNumberLabel.TextAlignment = UITextAlignment.Center;
				_pageNumberLabel.BackgroundColor = UIColor.Clear;
				_pageNumberLabel.TextColor = UIColor.White;
				_pageNumberLabel.Font = UIFont.SystemFontOfSize(16.0f);
				_pageNumberLabel.ShadowOffset = new SizeF(0.0f, 1.0f);
				_pageNumberLabel.ShadowColor = UIColor.Black;
				_pageNumberLabel.AdjustsFontSizeToFitWidth = true;
				_pageNumberLabel.MinimumFontSize = 12.0f;
				
				pageNumberView.AddSubview(_pageNumberLabel);
				bottomBar.AddSubview(pageNumberView);
			}
			
			return bottomBar;
		}
		#endregion

		#region PDFDocument logic
		private RectangleF getBookViewFrameRect()
		{
			var rect = getActualViewFrameRect();

			if (_toolbar != null) {
				rect.Y = _toolbar.Frame.Bottom;
				rect.Height -= _toolbar.Frame.Height + BarPaddingV;
			}
			if (_bottomBar != null) {
				rect.Height -= (rect.Bottom - _bottomBar.Frame.Top) + BarPaddingV;
			}

			return rect;
		}
		
		private RectangleF getPageViewFrameRect()
		{
			return _bookPageViewController.View.Bounds; 
		}

		private void updatePageViewAutoScaleMode()
		{
			var pageVCs = _bookPageViewController.ViewControllers
				.Cast<PageVC>()
				.Where(x => x != null && x.IsNotEmptyPage);

			foreach (var pageVC in pageVCs) {
				pageVC.PageView.NeedUpdateZoomAndOffset = true;
				pageVC.PageView.AutoScaleMode = _autoScaleMode;
				pageVC.PageView.SetNeedsLayout();
			}
		}

		private PageVC getCurrentPageContentVC()
		{
			return (PageVC)_bookPageViewController.ViewControllers[0];
		}

		private PageVC getEmptyPageContentVC()
		{
			return new PageVC(getPageViewFrameRect(), _autoScaleMode, -1);
		}

		private PageVC getPageViewController(int pageNumber)
		{
			if (!PDFDocument.DocumentHasLoaded || pageNumber < 1 || pageNumber > PDFDocument.PageCount || pageNumber == PDFDocument.CurrentPageNumber) {
				return null;
			}
			return new PageVC(getPageViewFrameRect(), _autoScaleMode, pageNumber);
		}

		private int getPageIncValue()
		{
			return _bookPageViewController.SpineLocation == UIPageViewControllerSpineLocation.Mid ? 2 : 1;
		}

		private string getImagePathForButton(DocumentActionTypes actionType)
		{
			var ret = "";
			if (actionType == DocumentActionTypes.AutoWidth) {
				ret = _autoScaleMode == AutoScaleModes.AutoWidth 
					? "Images/Toolbar/AutoWidth_Selected48.png" 
					: "Images/Toolbar/AutoWidth48.png";
			} else if (actionType == DocumentActionTypes.AutoHeight) {
				ret = _autoScaleMode == AutoScaleModes.AutoHeight 
					? "Images/Toolbar/AutoHeight_Selected48.png" 
					: "Images/Toolbar/AutoHeight48.png";
			}
		
			return ret;
		}

		public virtual void OpenDocumentPage(int pageNumber)
		{
			if (pageNumber < 1 || pageNumber > PDFDocument.PageCount) {
				return;
			}

			var navDirection = pageNumber < PDFDocument.CurrentPageNumber  
				? UIPageViewControllerNavigationDirection.Reverse
				: UIPageViewControllerNavigationDirection.Forward;

			var pageVC = getPageViewController(pageNumber);
			if (pageVC == null) {
				return;
			}

			_bookPageViewController.SetViewControllers(
				new UIViewController[] { pageVC }, 
				navDirection, 
				true, 
				s => { execActionsAfterOpenPage(); });
		}		

		private void execActionsAfterOpenPage()
		{
			PDFDocument.CurrentPageNumber = getCurrentPageContentVC().PageNumber;	
			
			if (_pageNumberLabel != null) {
				_pageNumberLabel.Text = string.Format(@"{0}/{1}", PDFDocument.CurrentPageNumber, PDFDocument.PageCount);
			}
			
			if (_slider != null) {
				_slider.Value = PDFDocument.CurrentPageNumber;
			}
		}
		#endregion
		
		#region Events			
		protected virtual void openFirstPage()
		{
			OpenDocumentPage(1);
		}
		
		protected virtual void openPriorPage()
		{
			OpenDocumentPage(PDFDocument.CurrentPageNumber - getPageIncValue());
		}
		
		protected virtual void openNextPage()
		{
			OpenDocumentPage(PDFDocument.CurrentPageNumber + getPageIncValue());
		}
		
		protected virtual void openLastPage()
		{
			OpenDocumentPage(PDFDocument.PageCount);
		}
		
		protected virtual void zoomOut()
		{
			var pageVC = getCurrentPageContentVC();
			if (PDFDocument.DocumentHasLoaded && pageVC.IsNotEmptyPage) {
				pageVC.PageView.ZoomDecrement();
			}
		}
		
		protected virtual void zoomIn()
		{
			var pageVC = getCurrentPageContentVC();
			if (PDFDocument.DocumentHasLoaded && pageVC.IsNotEmptyPage) {
				pageVC.PageView.ZoomIncrement();
			}
		}

		protected virtual void setAutoScaleMode(AutoScaleModes autoScaleMode)
		{
			_autoScaleMode = autoScaleMode;
			_btnAutoWidth.SetImage(UIImage.FromFile(getImagePathForButton(DocumentActionTypes.AutoWidth)), UIControlState.Normal);
			_btnAutoHeight.SetImage(UIImage.FromFile(getImagePathForButton(DocumentActionTypes.AutoHeight)), UIControlState.Normal);
			updatePageViewAutoScaleMode();
		}

		protected virtual void setAutoWidth()
		{
			setAutoScaleMode(AutoScaleModes.AutoWidth);
		}

		protected virtual void setAutoHeight()
		{
			setAutoScaleMode(AutoScaleModes.AutoHeight);
		}
		#endregion
	}
}