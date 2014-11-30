//
// mTouch-PDFReader library
// NoteVC.cs
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
using mTouchPDFReader.Library.Data.Objects;
using mTouchPDFReader.Library.Managers;

namespace mTouchPDFReader.Library.Views.Management
{
	public class NoteVC : UIViewController
	{				
		#region Data		
		private DocumentNote _note;
		private UITextView _txtNote;		
		#endregion

		#region logic
		public NoteVC(DocumentNote note) : base(null, null) 
		{
			_note = note;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var btnApply = new UIBarButtonItem();
			btnApply.Image = UIImage.FromFile("apply.png");
			btnApply.Clicked += delegate {
				_note.Note = _txtNote.Text;
				MgrAccessor.DocumentNoteMgr.Save(_note);
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
			toolBarTitle.Text = "Note".t();

			var toolBar = new UIToolbar(new RectangleF(0, 0, View.Bounds.Width, 44));
			toolBar.BarStyle = UIBarStyle.Black;
			toolBar.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			toolBar.SetItems(new [] { btnApply, space, btnClose }, false);
			toolBar.AddSubview(toolBarTitle);
			View.AddSubview(toolBar);
			
			_txtNote = new UITextView(new RectangleF(0, 44, View.Bounds.Width, View.Bounds.Height));
			_txtNote.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;	
			_txtNote.Font = UIFont.SystemFontOfSize(17.0f);
			_txtNote.Text = _note.Note;
			View.AddSubview(_txtNote);
		}
		#endregion
	}
}

