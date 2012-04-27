//****************************************//
// mTouch-PDFReader library
// Goto page view controller
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Views.Core;
using mTouchPDFReader.Library.Data.Managers;
using mTouchPDFReader.Library.XViews;

namespace mTouchPDFReader.Library.Views.Management
{
	public class GotoPageViewController : UIViewControllerWithPopover
	{			
		#region Fields
		
		/// <summary>
		/// New page number text view
		/// </summary>
		private UITextField mPageNumberTxt;
		
		#endregion
		
		#region Constructors

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
				int.TryParse(mPageNumberTxt.Text, out pageNumber);
				if ((pageNumber <= 0) || (pageNumber > PDFDocument.PageCount)) {
					using (var alert = new UIAlertView("Error".t(), "Invalid page number".t(), null, "Ok")) {
						alert.Show();
					}
				} else {
					CallbackAction(pageNumber);
				}
				mPopoverController.Dismiss(true);
			};
			toolBar.AddSubview(toolBarTitle);
			toolBar.AddSubview(btnNavigate);
			View.AddSubview(toolBar);
			
			// Create PageNumber text field
			mPageNumberTxt = new UITextField(new RectangleF(20, 58, View.Bounds.Width - 40, 31));
			mPageNumberTxt.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			mPageNumberTxt.BorderStyle = UITextBorderStyle.RoundedRect;
			mPageNumberTxt.KeyboardType = UIKeyboardType.NumberPad;
			mPageNumberTxt.Font = UIFont.SystemFontOfSize(17.0f);
			mPageNumberTxt.Text = PDFDocument.CurrentPageNumber.ToString();
			View.AddSubview(mPageNumberTxt);
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

