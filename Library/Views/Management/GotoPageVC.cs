//
// mTouch-PDFReader library
// GotoPageVC.cs
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
using System.Drawing;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Views.Core;
using mTouchPDFReader.Library.Managers;

namespace mTouchPDFReader.Library.Views.Management
{
	public class GotoPageVC : UIViewController
	{
		#region Data
		private UITextField _txtPageNumber;
		private Action<object> _callbackAction;
		#endregion

		#region GotoPageVC members
		public GotoPageVC(Action<object> callbackAction) : base(null, null)	
		{
			_callbackAction = callbackAction;
		}
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var btnApply = new UIBarButtonItem();
			btnApply.Image = UIImage.FromFile("apply.png");
			btnApply.Clicked += delegate { 
				int pageNumber;
				int.TryParse(_txtPageNumber.Text, out pageNumber);
				if ((pageNumber <= 0) || (pageNumber > PDFDocument.PageCount)) {
					using (var alert = new UIAlertView("Error".t(), "Invalid page number".t(), null, "Ok")) {
						alert.Show();
					}
				} else {
					_callbackAction(pageNumber);
				}
				DismissViewController(true, null);
			};
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
			toolBarTitle.Text = "Go to page".t();

			var toolBar = new UIToolbar(new RectangleF(0, 0, View.Bounds.Width, 44));
			toolBar.BarStyle = UIBarStyle.Black;
			toolBar.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			toolBar.SetItems(new [] { btnApply, space, btnClose }, false);
			toolBar.AddSubview(toolBarTitle);
			View.AddSubview(toolBar);
			
			_txtPageNumber = new UITextField(new RectangleF(20, 58, View.Bounds.Width - 40, 31));
			_txtPageNumber.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			_txtPageNumber.BorderStyle = UITextBorderStyle.RoundedRect;
			_txtPageNumber.KeyboardType = UIKeyboardType.NumberPad;
			_txtPageNumber.Font = UIFont.SystemFontOfSize(17.0f);
			_txtPageNumber.Placeholder = "Enter page number".t();
			View.AddSubview(_txtPageNumber);
		}
		#endregion
	}
}

