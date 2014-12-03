//
// mTouch-PDFReader library
// GotoPageVC.cs
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

