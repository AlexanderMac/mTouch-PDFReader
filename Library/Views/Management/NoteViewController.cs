//
// mTouch-PDFReader library
// NoteViewController.cs (Note view controller)
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
using mTouchPDFReader.Library.Utils;
using mTouchPDFReader.Library.Interfaces;
using mTouchPDFReader.Library.Data.Objects;
using mTouchPDFReader.Library.Managers;
using mTouchPDFReader.Library.XViews;

namespace mTouchPDFReader.Library.Views.Management
{
	public class NoteViewController : UIViewControllerWithPopover
	{				
		#region Fields		
		/// <summary>
		/// The editing note.
		/// </summary>
		private DocumentNote _Note;
		
		/// <summary>
		/// The note text view.
		/// </summary>
		private UITextView _TxtNote;		
		#endregion

		#region Constructors
		/// <summary>
		/// Working.
		/// </summary>
		public NoteViewController(DocumentNote note, Action<object> callbackAction) : base(null, null, callbackAction)
		{
			_Note = note;
		}
		#endregion

		#region UI logic
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
				_Note.Note = _TxtNote.Text;
				RC.Get<IDocumentNoteManager>().Save(_Note);
				_PopoverController.Dismiss(true);
			};
			toolBar.AddSubview(toolBarTitle);
			toolBar.AddSubview(btnNavigate);
			View.AddSubview(toolBar);
			
			// Create text note
			_TxtNote = new UITextView(new RectangleF(0, 44, View.Bounds.Width, View.Bounds.Height));
			_TxtNote.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;	
			_TxtNote.Font = UIFont.SystemFontOfSize(17.0f);
			_TxtNote.Text = _Note.Note;
			View.AddSubview(_TxtNote);
		}
		
		protected override SizeF GetPopoverSize()
		{
			return new SizeF(400, 400);
		}

		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		#endregion
	}
}

