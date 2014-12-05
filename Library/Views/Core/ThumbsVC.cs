//
// mTouch-PDFReader library
// ThumbsVC.cs
//
// Copyright (c) 2012-2014 Alexander Matsibarov (amatsibarov@gmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// 'Software'), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions:

// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Managers;

namespace mTouchPDFReader.Library.Views.Core
{
	public class ThumbsVC : UIViewController
	{		
		#region Data			
		private const int ThumbMargin = 5;
		private const int ThumbCols = 3;

		private readonly Action<object> _openPageCallback;
		private UIView _thumbsViewContainer;
		private UIPageControl _thumbsPageControl;

		private class ThumbsPageInfo
		{
			public int ThumbSize { get; set; }
			public int ThumbRows { get; set; }
			public int ThumbsCountPerPage {
				get {
					return ThumbCols * ThumbRows;
				}
			}
		}
		#endregion
			
		#region UIViewController members		
		public ThumbsVC(Action<object> callbackAction) : base(null, null)
		{
			_openPageCallback = callbackAction; 
		}				

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var space = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
			var btnClose = new UIBarButtonItem();
			btnClose.Image = UIImage.FromFile("close.png");
			btnClose.Clicked += delegate { 
				DismissViewController(true, null);
			};

			var toolBarTitle = new UILabel(new RectangleF(0, 0, View.Bounds.Width, 44));
			toolBarTitle.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			toolBarTitle.BackgroundColor = UIColor.Clear;
			toolBarTitle.TextColor = UIColor.White;
			toolBarTitle.TextAlignment = UITextAlignment.Center;
			toolBarTitle.Text = "Thumbs".t();

			var toolBar = new UIToolbar(new RectangleF(0, 0, View.Bounds.Width, 44));
			toolBar.BarStyle = UIBarStyle.Black;
			toolBar.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			toolBar.SetItems(new [] { space, btnClose }, false);
			toolBar.AddSubview(toolBarTitle);
			View.AddSubview(toolBar);

			_thumbsViewContainer = new UIView(new RectangleF(0, 44, View.Bounds.Width, View.Bounds.Height - 85)); 
			_thumbsViewContainer.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight; 
			_thumbsViewContainer.BackgroundColor = UIColor.Gray;
			View.AddSubview(_thumbsViewContainer);

			_thumbsPageControl = new UIPageControl(new RectangleF(0, View.Bounds.Height - 30, View.Bounds.Width, 20));
			_thumbsPageControl.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
			_thumbsPageControl.ValueChanged += delegate {
				createThumbsPage(_thumbsPageControl.CurrentPage);
			};
			View.AddSubview(_thumbsPageControl);
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			createThumbsPage(0);
			var thumbsPageInfo = getThumbsPageInfo();
			_thumbsPageControl.Pages = (int)Math.Ceiling(PDFDocument.PageCount / (float)thumbsPageInfo.ThumbsCountPerPage);
		}
		#endregion
				
		#region Logic
		private void createThumbsPage(int thumbsPageNumber)
		{
			var thumbsPageInfo = getThumbsPageInfo();

			var thumbViews = new List<UIView>();
			var pageNumber = ThumbCols * thumbsPageInfo.ThumbRows * thumbsPageNumber + 1;
			var top = ThumbMargin;
			for (var r = 0; r < thumbsPageInfo.ThumbRows; r++) {
				var left = ThumbMargin;
				for (var c = 0; c < ThumbCols; c++) {
					var thumbRect = new RectangleF(left, top, thumbsPageInfo.ThumbSize, thumbsPageInfo.ThumbSize);
					var thumbView = new ThumbWithPageNumberView(thumbRect, pageNumber, thumbSelected);
					if (pageNumber == PDFDocument.CurrentPageNumber) {
						thumbView.SetAsSelected();
					}
					thumbViews.Add(thumbView);

					left += thumbsPageInfo.ThumbSize + ThumbMargin;
					pageNumber++;
					if (pageNumber > PDFDocument.PageCount) {
						break;
					}
				}
				top += thumbsPageInfo.ThumbSize + ThumbMargin;
				if (pageNumber > PDFDocument.PageCount) {
					break;
				}
			}
				
			foreach (var view in _thumbsViewContainer.Subviews) {
				view.RemoveFromSuperview();
			}
			_thumbsViewContainer.AddSubviews(thumbViews.ToArray());
		}

		private ThumbsPageInfo getThumbsPageInfo()
		{
			var ret = new ThumbsPageInfo();
			ret.ThumbSize = (int)((_thumbsViewContainer.Bounds.Width - ThumbMargin) / ThumbCols) - ThumbMargin;
			ret.ThumbRows = (int)((_thumbsViewContainer.Bounds.Height - ThumbMargin) / ret.ThumbSize);

			return ret;
		}

		private void thumbSelected(ThumbWithPageNumberView thumbView)
		{
			thumbView.SetAsSelected();
			_openPageCallback(thumbView.PageNumber);

			DismissViewController(true, null);
		}		
		#endregion
	}
}
