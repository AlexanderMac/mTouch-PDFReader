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
	public class DocumentViewController : UIViewController
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
		private readonly int _DocumentId;
		private readonly string _DocumentName;
		private readonly string _DocumentPath;
		private UIPageViewController _BookPageViewController;
		private UIView _Toolbar;
		private UIButton _BtnNavigateToPage;
		private UIButton _BtnNote;
		private UIButton _BtnBookmarksList;
		private UIButton _BtnThumbs;
		private UIButton _BtnAutoWidth;
		private UIButton _BtnAutoHeight;
		private UIView _BottomBar;
		private UISlider _Slider;
		private UILabel _PageNumberLabel;
		private AutoScaleModes _AutoScaleMode;
		#endregion
			
		#region Initialization
		public DocumentViewController(int docId, string docName, string docPath) : base(null, null)
		{
			_DocumentId = docId;
			_DocumentName = docName;
			_DocumentPath = docPath;
		}
		#endregion
		
		#region UIViewController	
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Load document
			PDFDocument.OpenDocument(_DocumentName, _DocumentPath);
			
			// Init View	
			Title = _DocumentName;
			View.BackgroundColor = UIColor.ScrollViewTexturedBackgroundColor;
			_AutoScaleMode = MgrAccessor.OptionsMgr.Options.AutoScaleMode;
	
			// Create toolbar
			if (MgrAccessor.OptionsMgr.Options.ToolbarVisible) {
				_Toolbar = _CreateToolbar();
				View.AddSubview(_Toolbar);
			}
			
			// Create bottom bar
			if (MgrAccessor.OptionsMgr.Options.SliderVisible || MgrAccessor.OptionsMgr.Options.PageNumberVisible) {
				_BottomBar = _CreateBottomBar();
				View.AddSubview(_BottomBar);
				_UpdateSliderMaxValue();
			}
			
			// Create the book PageView controller
			_BookPageViewController = new UIPageViewController(
				MgrAccessor.OptionsMgr.Options.PageTransitionStyle,
				MgrAccessor.OptionsMgr.Options.PageNavigationOrientation, 
				UIPageViewControllerSpineLocation.Min);		
			_BookPageViewController.View.Frame = _GetBookViewFrameRect();
			_BookPageViewController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			_BookPageViewController.View.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
			_BookPageViewController.GetNextViewController = GetNextPageViewController;
			_BookPageViewController.GetPreviousViewController = GetPreviousPageViewController;
			_BookPageViewController.GetSpineLocation = GetSpineLocation;
			_BookPageViewController.DidFinishAnimating += delegate(object sender, UIPageViewFinishedAnimationEventArgs e) {
				PageFinishedAnimation(e.Completed, e.Finished, e.PreviousViewControllers);
			};
			_BookPageViewController.SetViewControllers(
				new UIViewController[] { GetPageViewController(1) }, 
				UIPageViewControllerNavigationDirection.Forward, 
				false,
				s => ExecAfterOpenPageActions());	

			AddChildViewController(_BookPageViewController);
			_BookPageViewController.DidMoveToParentViewController(this);
			View.AddSubview(_BookPageViewController.View);
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			foreach (var pageVC in _BookPageViewController.ChildViewControllers.Cast<PageViewController>()) {
				pageVC.PageView.NeedUpdateZoomAndOffset = true;
			}
		}

		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		protected override void Dispose(bool disposing)
		{
			PDFDocument.CloseDocument();
			base.Dispose(disposing);
		}		
		#endregion
		
		#region UIPageViewController	
		private UIViewController GetPreviousPageViewController(UIPageViewController pageController, UIViewController referenceViewController)
		{			
			var curPageCntr = referenceViewController as PageViewController;
			if (curPageCntr.PageNumber == 0) {
				return null;				
			} 
			int pageNumber = curPageCntr.PageNumber - 1;
			return GetPageViewController(pageNumber);
		}

		private UIViewController GetNextPageViewController(UIPageViewController pageController, UIViewController referenceViewController)
		{			
			var curPageCntr = referenceViewController as PageViewController;
			if (curPageCntr.PageNumber == (PDFDocument.PageCount)) {				
				return _BookPageViewController.SpineLocation == UIPageViewControllerSpineLocation.Mid
					? _GetEmptyPageContentVC()
					: null;	
			} else if (curPageCntr.PageNumber == -1) { 
				return null;
			}
			int pageNumber = curPageCntr.PageNumber + 1;
			return GetPageViewController(pageNumber);
		}	

		private UIPageViewControllerSpineLocation GetSpineLocation(UIPageViewController pageViewController, UIInterfaceOrientation orientation)
		{
			var currentPageVC = _GetCurrentPageContentVC();
			pageViewController.DoubleSided = false;
			pageViewController.SetViewControllers(new UIViewController[] { currentPageVC }, UIPageViewControllerNavigationDirection.Forward, true, s => { });
			return UIPageViewControllerSpineLocation.Min;
		}

		private void PageFinishedAnimation(bool completed, bool finished, UIViewController[] previousViewControllers)
		{
			if (completed) {
				ExecAfterOpenPageActions();
			}
		}
		#endregion
				
		#region UI Logic		
		private void _PresentPopover(UIViewControllerWithPopover viewCtrl, RectangleF frame)
		{
			var popoverController = new UIPopoverController(viewCtrl);
			viewCtrl.PopoverController = popoverController;
			popoverController.PresentFromRect(frame, View, UIPopoverArrowDirection.Any, true);
		}	
		
		private UIInterfaceOrientation _GetDeviceOrientation()
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
		
		private void _UpdateSliderMaxValue()
		{
			if (_Slider != null) {
				_Slider.MaxValue = PDFDocument.PageCount;
			}
		}		

		protected virtual UIView _CreateToolbar()
		{
			// Create toolbar
			var toolBarFrame = View.Bounds;
			toolBarFrame.X += BarPaddingH;
			toolBarFrame.Y += BarPaddingV;
			toolBarFrame.Width -= BarPaddingH * 2;
			toolBarFrame.Height = ToolbarHeight;
			var toolBar = new UIXToolbarView(toolBarFrame, 0.92f, 0.32f, 0.8f);
			toolBar.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleWidth;

			var btnFrame = new RectangleF(FirstToolButtonLeft, FirstToolButtonTop, ToolButtonSize, ToolButtonSize);

			// Create toolbar buttons
			_CreateToolbarButton(toolBar, DocumentActionTypes.NavigateToFirstPage, ref btnFrame, "Images/Toolbar/NavigateToFirst48.png", _OpenFirstPage);
			_CreateToolbarButton(toolBar, DocumentActionTypes.NavigateToPriorPage, ref btnFrame, "Images/Toolbar/NavigateToPrior48.png", _OpenPriorPage);
			_BtnNavigateToPage = _CreateToolbarButton(toolBar, DocumentActionTypes.NavigateToPage, ref btnFrame, "Images/Toolbar/NavigateToPage48.png", () => {
				var view = new GotoPageViewController(p => OpenDocumentPage((int)p));
				_PresentPopover(view, _BtnNavigateToPage.Frame);
			});
			_CreateToolbarButton(toolBar, DocumentActionTypes.NavigateToNextPage, ref btnFrame, "Images/Toolbar/NavigateToNext48.png", _OpenNextPage);
			_CreateToolbarButton(toolBar, DocumentActionTypes.NavigateToLastPage, ref btnFrame, "Images/Toolbar/NavigateToLast48.png", _OpenLastPage);
			_CreateToolbarSeparator(ref btnFrame);

			_CreateToolbarButton(toolBar, DocumentActionTypes.ZoomOut, ref btnFrame, "Images/Toolbar/ZoomOut48.png", ZoomOut);
			_CreateToolbarButton(toolBar, DocumentActionTypes.ZoomIn, ref btnFrame, "Images/Toolbar/ZoomIn48.png", _ZoomIn);
			_CreateToolbarSeparator(ref btnFrame);

			_BtnNote = _CreateToolbarButton(toolBar, DocumentActionTypes.Note, ref btnFrame, "Images/Toolbar/Note48.png", () => {
				var note = MgrAccessor.DocumentNoteMgr.Load(_DocumentId);
				var view = new NoteViewController(note, null);
				_PresentPopover(view, _BtnNote.Frame);
			});
			_BtnBookmarksList = _CreateToolbarButton(toolBar, DocumentActionTypes.Bookmarks, ref btnFrame, "Images/Toolbar/BookmarksList48.png", () => {
				var bookmarks = MgrAccessor.DocumentBookmarkMgr.LoadList(_DocumentId);
				var view = new BookmarksViewController(_DocumentId, bookmarks, PDFDocument.CurrentPageNumber, p => OpenDocumentPage((int)p));
				_PresentPopover(view, _BtnBookmarksList.Frame);
			});
			_BtnThumbs = _CreateToolbarButton(toolBar, DocumentActionTypes.Thumbs, ref btnFrame, "Images/Toolbar/Thumbs48.png", () => {
				var view = new ThumbsViewController(View.Bounds.Width, p => OpenDocumentPage((int)p));
				_PresentPopover(view, _BtnThumbs.Frame);
				view.InitThumbs();
			});
			_CreateToolbarSeparator(ref btnFrame);

			_BtnAutoWidth = _CreateToolbarButton(toolBar, DocumentActionTypes.AutoWidth, ref btnFrame, _GetImagePathForButton(DocumentActionTypes.AutoWidth), () => SetAutoWidth());
			_BtnAutoHeight = _CreateToolbarButton(toolBar, DocumentActionTypes.AutoHeight, ref btnFrame, _GetImagePathForButton(DocumentActionTypes.AutoHeight), () => SetAutoHeight());
			_CreateToolbarSeparator(ref btnFrame);

			// Create user defined toolbar items
			_CreateUserDefinedToolbarItems(toolBar, ref btnFrame);

			return toolBar;
		}

		private void _CreateToolbarSeparator(ref RectangleF frame)
		{
			frame.Offset(22, 0);
		}

		protected virtual UIButton _CreateToolbarButton(UIView toolbar, DocumentActionTypes actionType, ref RectangleF frame, string imagePath, Action action)
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

		protected virtual void _CreateUserDefinedToolbarItems(UIView toolbar, ref RectangleF frame)
		{
			// Nothing. Should be overrided in child classes.
		}

		protected virtual UIView _CreateBottomBar()
		{
			var bottomBarFrame = View.Bounds;
			bottomBarFrame.X += BarPaddingH;
			bottomBarFrame.Y = bottomBarFrame.Size.Height - BarPaddingV - BottombarHeight;
			bottomBarFrame.Width -= BarPaddingH * 2;
			bottomBarFrame.Height = BottombarHeight;
			var bottomBar = new UIXToolbarView(bottomBarFrame, 0.92f, 0.32f, 0.8f);
			bottomBar.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleWidth;

			if (MgrAccessor.OptionsMgr.Options.SliderVisible) {
				float sliderWidth = bottomBarFrame.Width - 15;
				if (MgrAccessor.OptionsMgr.Options.PageNumberVisible) {
					sliderWidth -= PageNumberLabelSize.Width;
				}
				var pageSliderFrame = new RectangleF(5, 10, sliderWidth, 20);
				_Slider = new UISlider(pageSliderFrame);
				_Slider.MinValue = 1;
				_Slider.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
				_Slider.ValueChanged += delegate {
					if (_PageNumberLabel != null) {
						_PageNumberLabel.Text = string.Format(@"{0}/{1}", (int)_Slider.Value, PDFDocument.PageCount);
					}
				};
				_Slider.TouchUpInside += delegate(object sender, EventArgs e) {
					OpenDocumentPage((int)_Slider.Value);
				};
				bottomBar.AddSubview(_Slider);
			}

			if (MgrAccessor.OptionsMgr.Options.PageNumberVisible) {
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
		#endregion

		#region PDFDocument logic
		private RectangleF _GetBookViewFrameRect()
		{
			var rect = View.Bounds;
			if (_Toolbar != null) {
				rect.Y = _Toolbar.Frame.Bottom;
				rect.Height -= _Toolbar.Bounds.Height + BarPaddingV;
			}
			if (_BottomBar != null) {
				rect.Height -= _BottomBar.Bounds.Height + BarPaddingV;
			}
			return rect;
		}
		
		private RectangleF _GetPageViewFrameRect()
		{
			return _BookPageViewController.View.Bounds; 
		}

		private void _UpdatePageViewAutoScaleMode()
		{
			var pageVCs = _BookPageViewController.ViewControllers
				.Cast<PageViewController>()
				.Where(x => x != null && x.IsNotEmptyPage);

			foreach (var pageVC in pageVCs) {
				pageVC.PageView.NeedUpdateZoomAndOffset = true;
				pageVC.PageView.AutoScaleMode = _AutoScaleMode;
				pageVC.PageView.SetNeedsLayout();
			}
		}

		private PageViewController _GetCurrentPageContentVC()
		{
			return (PageViewController)_BookPageViewController.ViewControllers[0];
		}

		private PageViewController _GetEmptyPageContentVC()
		{
			return new PageViewController(_GetPageViewFrameRect(), _AutoScaleMode, -1);
		}

		private PageViewController GetPageViewController(int pageNumber)
		{
			if (!PDFDocument.DocumentHasLoaded || pageNumber < 1 || pageNumber > PDFDocument.PageCount || pageNumber == PDFDocument.CurrentPageNumber) {
				return null;
			}
			return new PageViewController(_GetPageViewFrameRect(), _AutoScaleMode, pageNumber);
		}

		private int _GetPageIncValue()
		{
			return _BookPageViewController.SpineLocation == UIPageViewControllerSpineLocation.Mid ? 2 : 1;
		}

		private string _GetImagePathForButton(DocumentActionTypes actionType)
		{
			var ret = "";
			if (actionType == DocumentActionTypes.AutoWidth) {
				ret = _AutoScaleMode == AutoScaleModes.AutoWidth 
					? "Images/Toolbar/AutoWidth_Selected48.png" 
					: "Images/Toolbar/AutoWidth48.png";
			} else if (actionType == DocumentActionTypes.AutoHeight) {
				ret = _AutoScaleMode == AutoScaleModes.AutoHeight 
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

			// Calc navigation direction
			var navDirection = pageNumber < PDFDocument.CurrentPageNumber  
				? UIPageViewControllerNavigationDirection.Reverse
				: UIPageViewControllerNavigationDirection.Forward;

			// Create single PageView
			var pageVC = GetPageViewController(pageNumber);

			// Open page
			_BookPageViewController.SetViewControllers(
				new UIViewController[] { pageVC }, 
				navDirection, 
				true, 
				s => { ExecAfterOpenPageActions(); });
		}		

		private void ExecAfterOpenPageActions()
		{
			// Set current page
			PDFDocument.CurrentPageNumber = _GetCurrentPageContentVC().PageNumber;	
			
			// Update PageNumber label
			if (_PageNumberLabel != null) {
				_PageNumberLabel.Text = string.Format(@"{0}/{1}", PDFDocument.CurrentPageNumber, PDFDocument.PageCount);
			}
			
			// Update slider position
			if (_Slider != null) {
				_Slider.Value = PDFDocument.CurrentPageNumber;
			}
		}
		#endregion
		
		#region Events			
		protected virtual void _OpenFirstPage()
		{
			OpenDocumentPage(1);
		}
		
		protected virtual void _OpenPriorPage()
		{
			OpenDocumentPage(PDFDocument.CurrentPageNumber - _GetPageIncValue());
		}
		
		protected virtual void _OpenNextPage()
		{
			OpenDocumentPage(PDFDocument.CurrentPageNumber + _GetPageIncValue());
		}
		
		protected virtual void _OpenLastPage()
		{
			OpenDocumentPage(PDFDocument.PageCount);
		}
		
		protected virtual void ZoomOut()
		{
			var pageVC = _GetCurrentPageContentVC();
			if (PDFDocument.DocumentHasLoaded && pageVC.IsNotEmptyPage) {
				pageVC.PageView.ZoomDecrement();
			}
		}
		
		protected virtual void _ZoomIn()
		{
			var pageVC = _GetCurrentPageContentVC();
			if (PDFDocument.DocumentHasLoaded && pageVC.IsNotEmptyPage) {
				pageVC.PageView.ZoomIncrement();
			}
		}

		public void SetAutoScaleMode(AutoScaleModes autoScaleMode)
		{
			_AutoScaleMode = autoScaleMode;
			_BtnAutoWidth.SetImage(UIImage.FromFile(_GetImagePathForButton(DocumentActionTypes.AutoWidth)), UIControlState.Normal);
			_BtnAutoHeight.SetImage(UIImage.FromFile(_GetImagePathForButton(DocumentActionTypes.AutoHeight)), UIControlState.Normal);
			_UpdatePageViewAutoScaleMode();
		}

		public virtual void SetAutoWidth()
		{
			SetAutoScaleMode(AutoScaleModes.AutoWidth);
		}

		public virtual void SetAutoHeight()
		{
			SetAutoScaleMode(AutoScaleModes.AutoHeight);
		}
		#endregion
	}
}