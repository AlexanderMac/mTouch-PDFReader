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
using System.Collections.Generic;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Views.Management;
using mTouchPDFReader.Library.Managers;
using mTouchPDFReader.Library.Data.Enums;

namespace mTouchPDFReader.Library.Views.Core
{
	public class DocumentVC : UIViewController
	{			
		#region Data
		private const float DefaultToolbarHeight = 44.0f;
		private const float DefaultMargin = 5.0f;
		private const float SliderLeft = 170.0f;
		private const float SliderHeight = 30.0f;
		private const float PageNumberWidth = 60.0f;
		private const float PageNumberHeight = 50.0f;
		private const int MaxPageViewsCount = 3;

		private readonly int _documentId;
		private readonly string _documentName;
		private readonly string _documentPath;
		private UIPageViewController _bookPageViewController;
		private UIView _topToolbar;
		private UIBarButtonItem _btnAutoWidth;
		private UIBarButtonItem _btnAutoHeight;
		private UIView _bottomToolbar;
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
			_autoScaleMode = MgrAccessor.SettingsMgr.Settings.AutoScaleMode;
	
			if (MgrAccessor.SettingsMgr.Settings.TopToolbarVisible) {
				_topToolbar = createTopToolbar();
				View.AddSubview(_topToolbar);
			}
			
			if (MgrAccessor.SettingsMgr.Settings.BottomToolbarVisible) {
				_bottomToolbar = createBottomToolbar();
				View.AddSubview(_bottomToolbar);
				updateSliderMaxValue();
			}

			_bookPageViewController = new UIPageViewController(
				MgrAccessor.SettingsMgr.Settings.PageTransitionStyle,
				MgrAccessor.SettingsMgr.Settings.PageNavigationOrientation, 
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

			var actualFrameRect = getRootViewFrameRect();
			if (_topToolbar != null) {
				_topToolbar.Frame = new RectangleF(actualFrameRect.Left, actualFrameRect.Top, actualFrameRect.Width, DefaultToolbarHeight);
			}
			if (_bottomToolbar != null) {
				_bottomToolbar.Frame = new RectangleF(actualFrameRect.Left, 
					actualFrameRect.Bottom - DefaultToolbarHeight, 
					actualFrameRect.Width, 
					DefaultToolbarHeight);
			}
			_bookPageViewController.View.Frame = getBookViewFrameRect();

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
		private RectangleF getRootViewFrameRect()
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

		protected virtual UIView createTopToolbar()
		{
			var toolBarFrame = getRootViewFrameRect();
			toolBarFrame.Height = DefaultToolbarHeight;

			var toolBar = new UIToolbar(toolBarFrame);
			toolBar.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleWidth;

			createToolbarButton(toolBar, DocumentActionTypes.ZoomOut, "zoomOut.png", zoomOut);
			createToolbarButton(toolBar, DocumentActionTypes.ZoomIn, "zoomIn.png", zoomIn);
			createToolbarSeparator(toolBar);

			createToolbarButton(toolBar, DocumentActionTypes.Note, "note.png", () => {
				var note = MgrAccessor.DocumentNoteMgr.Load(_documentId);
				var vc = new NoteVC(note);
				vc.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				PresentViewController(vc, true, null);
			});
			createToolbarButton(toolBar, DocumentActionTypes.Bookmarks, "bookmarksList.png", () => {
				var bookmarks = MgrAccessor.DocumentBookmarkMgr.GetAllForDocument(_documentId);
				var vc = new BookmarksVC(_documentId, bookmarks, PDFDocument.CurrentPageNumber, p => OpenDocumentPage((int)p));
				vc.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				PresentViewController(vc, true, null);
			});
			createToolbarButton(toolBar, DocumentActionTypes.Thumbs, "thumbs.png", () => {
				var vc = new ThumbsVC(p => OpenDocumentPage((int)p));
				vc.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				PresentViewController(vc, true, null);
			});
			createToolbarSeparator(toolBar);

			_btnAutoWidth = createToolbarButton(toolBar, DocumentActionTypes.AutoWidth, getImagePathForButton(DocumentActionTypes.AutoWidth), setAutoWidth);
			_btnAutoHeight = createToolbarButton(toolBar, DocumentActionTypes.AutoHeight, getImagePathForButton(DocumentActionTypes.AutoHeight), setAutoHeight);
			createToolbarSeparator(toolBar);

			return toolBar;
		}

		protected virtual UIView createBottomToolbar()
		{
			var bottomBarFrame = getRootViewFrameRect();
			bottomBarFrame.Y += bottomBarFrame.Height - DefaultToolbarHeight;
			bottomBarFrame.Height = DefaultToolbarHeight;

			var bottomBar = new UIToolbar(bottomBarFrame);
			bottomBar.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleWidth;

			createToolbarButton(bottomBar, DocumentActionTypes.NavigateToFirstPage, "navigateToFirst.png", openFirstPage);
			createToolbarButton(bottomBar, DocumentActionTypes.NavigateToPriorPage, "navigateToPrior.png", openPriorPage);
			createToolbarButton(bottomBar, DocumentActionTypes.NavigateToPage, "navigateToPage.png", () => {
				var vc = new GotoPageVC(p => OpenDocumentPage((int)p));
				vc.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
				PresentViewController(vc, true, null);
			});
			createToolbarButton(bottomBar, DocumentActionTypes.NavigateToNextPage, "navigateToNext.png", openNextPage);
			createToolbarButton(bottomBar, DocumentActionTypes.NavigateToLastPage, "navigateToLast.png", openLastPage);
			createToolbarSeparator(bottomBar);

			var pageNumberLeft = bottomBarFrame.Width - PageNumberWidth - DefaultMargin;
			var pageNumberLabelFrame = new RectangleF(pageNumberLeft, (bottomBarFrame.Height - PageNumberHeight) / 2, PageNumberWidth, PageNumberHeight);
			_pageNumberLabel = new UILabel(pageNumberLabelFrame);
			_pageNumberLabel.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin;
			_pageNumberLabel.Font = UIFont.SystemFontOfSize(12.0f);
			_pageNumberLabel.TextAlignment = UITextAlignment.Center;
			_pageNumberLabel.TextColor = UIColor.Gray;
			bottomBar.AddSubview(_pageNumberLabel);

			var sliderWidth = pageNumberLeft - SliderLeft - DefaultMargin;
			var sliderFrame = new RectangleF(SliderLeft, (bottomBarFrame.Height - SliderHeight) / 2, sliderWidth, SliderHeight);
			_slider = new UISlider(sliderFrame);
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
			
			return bottomBar;
		}

		private void addButtonToToolbarBar(UIToolbar toolbar, UIBarButtonItem item)
		{
			var items = toolbar.Items ?? new UIBarButtonItem[0];
			var itemsList = items.ToList();
			itemsList.Add(item);
			toolbar.SetItems(itemsList.ToArray(), false);
		}

		private void createToolbarSeparator(UIToolbar toolbar)
		{
			var item = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace);
			addButtonToToolbarBar(toolbar, item);
		}

		private UIBarButtonItem createToolbarButton(UIToolbar toolbar, DocumentActionTypes actionType, string imagePath, Action action)
		{
			var btn = new UIBarButtonItem();
			btn.Image = UIImage.FromFile(imagePath);
			btn.Clicked += delegate { 
				action();
			};

			addButtonToToolbarBar(toolbar, btn);

			return btn;
		}
		#endregion

		#region PDFDocument logic
		private RectangleF getBookViewFrameRect()
		{
			var rect = getRootViewFrameRect();

			if (_topToolbar != null) {
				rect.Y = _topToolbar.Frame.Bottom;
				rect.Height -= _topToolbar.Frame.Height;
			}
			if (_bottomToolbar != null) {
				rect.Height -= (rect.Bottom - _bottomToolbar.Frame.Top);
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
					? "autoWidth_Selected.png" 
					: "autoWidth.png";
			} else if (actionType == DocumentActionTypes.AutoHeight) {
				ret = _autoScaleMode == AutoScaleModes.AutoHeight 
					? "autoHeight_Selected.png" 
					: "autoHeight.png";
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
			_btnAutoWidth.Image = UIImage.FromFile(getImagePathForButton(DocumentActionTypes.AutoWidth));
			_btnAutoHeight.Image = UIImage.FromFile(getImagePathForButton(DocumentActionTypes.AutoHeight));

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