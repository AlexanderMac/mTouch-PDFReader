//
// mTouch-PDFReader library
// GotoPageViewController.cs (Goto page view controller)
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
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Views.Core;
using mTouchPDFReader.Library.Managers;
using mTouchPDFReader.Library.XViews;

namespace mTouchPDFReader.Library.Views.Management
{
	public class GotoPageViewController : UIViewControllerWithPopover
	{			
		/// <summary>
		/// The new page number text view.
		/// </summary>
		private UITextField _PageNumberTxt;		

		#region Constructors
		/// <summary>
		/// Working.
		/// </summary>
		public GotoPageViewController(Action<object> callbackAction) : base(null, null, callbackAction)
		{
		}
		#endregion

		/// <summary>
		/// Calls when view are loaded 
		/// </summary>
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			// Create toolbar, title label and button
			var toolBar = new UIToolbar(new RectangleF(0, 0, View.Bounds.Width, 44));
			toolBar.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleBottomMargin;
			toolBar.BarStyle = UIBarStyle.Black;
			var toolBarTitle = new UILabel(new RectangleF(0, 0, View.Bounds.Width, 44));
			toolBarTitle.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			toolBarTitle.BackgroundColor = UIColor.Clear;
			toolBarTitle.TextAlignment = UITextAlignment.Center;
			toolBarTitle.TextColor = UIColor.White;
			toolBarTitle.Font = UIFont.SystemFontOfSize(18.0f);
			toolBarTitle.Text = "Go...".t();
			var btnNavigate = new UIButton(new RectangleF(5, 5, 30, 30));
			btnNavigate.SetImage(UIImage.FromFile("Images/Toolbar/NavigateToPage32.png"), UIControlState.Normal);
			btnNavigate.TouchUpInside += delegate {
				int pageNumber;
				int.TryParse(_PageNumberTxt.Text, out pageNumber);
				if ((pageNumber <= 0) || (pageNumber > PDFDocument.PageCount)) {
					using (var alert = new UIAlertView("Error".t(), "Invalid page number".t(), null, "Ok")) {
						alert.Show();
					}
				} else {
					CallbackAction(pageNumber);
				}
				_PopoverController.Dismiss(true);
			};
			toolBar.AddSubview(toolBarTitle);
			toolBar.AddSubview(btnNavigate);
			View.AddSubview(toolBar);
			
			// Create PageNumber text field
			_PageNumberTxt = new UITextField(new RectangleF(20, 58, View.Bounds.Width - 40, 31));
			_PageNumberTxt.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			_PageNumberTxt.BorderStyle = UITextBorderStyle.RoundedRect;
			_PageNumberTxt.KeyboardType = UIKeyboardType.NumberPad;
			_PageNumberTxt.Font = UIFont.SystemFontOfSize(17.0f);
			_PageNumberTxt.Text = PDFDocument.CurrentPageNumber.ToString();
			View.AddSubview(_PageNumberTxt);
		}
		
		/// <summary>
		/// Returns popover size, must be overrided in child classes
		/// </summary>
		/// <returns>Popover size</returns>
		protected override SizeF GetPopoverSize()
		{
			return new SizeF(200, 100);
		}

		/// <summary>
		/// Called when permission is shought to rotate
		/// </summary>
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
}

