//****************************************//
// mTouch-PDFReader library
// Note view controller
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
using mTouchPDFReader.Library.Data.Objects;
using mTouchPDFReader.Library.Data.Managers;
using mTouchPDFReader.Library.XViews;

namespace mTouchPDFReader.Library.Views.Management
{
	public class NoteViewController : UIViewControllerWithPopover
	{				
		#region Fields
		
		/// <summary>
		/// Editing note
		/// </summary>
		private DocumentNote mNote;
		
		/// <summary>
		/// Note text view
		/// </summary>
		private UITextView mNoteTxt;
		
		#endregion

		#region Constructors
		
		public NoteViewController(DocumentNote note, Action<object> callbackAction) : base(null, null, callbackAction)
		{
			mNote = note;
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
			toolBarTitle.Text = "Note".t();
			var btnNavigate = new UIButton(new RectangleF(5, 5, 30, 30));
			btnNavigate.SetImage(UIImage.FromFile("Images/Toolbar/Save32.png"), UIControlState.Normal);
			btnNavigate.TouchUpInside += delegate {
				mNote.Note = mNoteTxt.Text;
				DocumentNoteManager.Instance.SaveNote(mNote);
				CallbackAction(mNoteTxt);
				mPopoverController.Dismiss(true);
			};
			toolBar.AddSubview(toolBarTitle);
			toolBar.AddSubview(btnNavigate);
			View.AddSubview(toolBar);
			
			// Create text note
			mNoteTxt = new UITextView(new RectangleF(0, 44, View.Bounds.Width, View.Bounds.Height));
			mNoteTxt.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;	
			mNoteTxt.Font = UIFont.SystemFontOfSize(17.0f);
			mNoteTxt.Text = mNote.Note;
			View.AddSubview(mNoteTxt);
		}
		
		/// <summary>
		/// Returns popover size, must be overrided in child classes
		/// </summary>
		/// <returns>Popover size</returns>
		protected override SizeF GetPopoverSize()
		{
			return new SizeF(400, 400);
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

