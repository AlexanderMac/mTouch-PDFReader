//
// mTouch-PDFReader library
// NoteVC.cs
//
// Copyright (c) 2012-2014 AlexanderMac(amatsibarov@gmail.com)
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

