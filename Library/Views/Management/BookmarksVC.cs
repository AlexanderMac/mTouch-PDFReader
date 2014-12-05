//
// mTouch-PDFReader library
// BookmarksVC.cs
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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Data.Objects;
using mTouchPDFReader.Library.Managers;

namespace mTouchPDFReader.Library.Views.Management
{
	public class BookmarksVC : UIViewController
	{		
		#region Data		
		private int _documentId;	
		private List<DocumentBookmark> _bookmarks;
		private int _currentPageNumber;
		private Action<object> _callbackAction;
		private UITableView _bookmarksTable;
		private UITableViewCell _newBookmarkCell;
		private UITextField _newBookmarkNameTxt;
		private UITableViewCellEditingStyle _editMode;		
		#endregion

		#region UIViewController members
		public BookmarksVC(int docId, List<DocumentBookmark> bookmarks, int currentPageNumber, Action<object> callbackAction) : base(null, null)
		{
			_documentId = docId;
			_bookmarks = bookmarks;
			_currentPageNumber = currentPageNumber;
			_callbackAction = callbackAction;
			_editMode = UITableViewCellEditingStyle.None;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var btnAddBookmark = new UIBarButtonItem();
			btnAddBookmark.Image = UIImage.FromFile("add.png");
			btnAddBookmark.Clicked += delegate {
				setEditMode(UITableViewCellEditingStyle.Insert);
			};
			var btnDeleteBookmark = new UIBarButtonItem();
			btnDeleteBookmark.Image = UIImage.FromFile("delete.png");
			btnDeleteBookmark.Clicked += delegate {
				setEditMode(UITableViewCellEditingStyle.Delete); 
			};
			var btnClose = new UIBarButtonItem();
			btnClose.Image = UIImage.FromFile("close.png");
			btnClose.Clicked += delegate { 
				DismissViewController(true, null);
			};
			var space = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);

			var toolBarTitle = new UILabel(new RectangleF(0, 0, View.Bounds.Width, 44));
			toolBarTitle.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			toolBarTitle.BackgroundColor = UIColor.Clear;
			toolBarTitle.TextColor = UIColor.White;
			toolBarTitle.TextAlignment = UITextAlignment.Center;
			toolBarTitle.Text = "Bookmarks".t();

			var toolBar = new UIToolbar(new RectangleF(0, 0, View.Bounds.Width, 44));
			toolBar.BarStyle = UIBarStyle.Black;
			toolBar.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin |UIViewAutoresizing.FlexibleWidth;
			toolBar.SetItems(new [] { btnAddBookmark, btnDeleteBookmark, space, btnClose }, false);
			toolBar.AddSubview(toolBarTitle);
			View.AddSubview(toolBar);
			
			_bookmarksTable = new UITableView(new RectangleF(0, 44, View.Bounds.Width, View.Bounds.Height), UITableViewStyle.Plain);
			_bookmarksTable.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
			_bookmarksTable.Source = new DataSource(this);
			View.AddSubview(_bookmarksTable);
			
			_newBookmarkCell = new UITableViewCell(UITableViewCellStyle.Default, null);
			_newBookmarkCell.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin;
			_newBookmarkCell.Frame = new RectangleF(0, 0, View.Bounds.Width, 55);
			_newBookmarkNameTxt = new UITextField(new RectangleF(40, 12, View.Bounds.Width - 45, 31));
			_newBookmarkNameTxt.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			_newBookmarkNameTxt.BorderStyle = UITextBorderStyle.RoundedRect;
			_newBookmarkNameTxt.Font = UIFont.SystemFontOfSize(16.0f);
			_newBookmarkCell.AddSubview(_newBookmarkNameTxt);
		}

		private void setEditMode(UITableViewCellEditingStyle mode)
		{
			bool editing;
			switch (mode) {
				case UITableViewCellEditingStyle.Insert:
					_editMode = UITableViewCellEditingStyle.Insert;
					editing = true;
					break;
				case UITableViewCellEditingStyle.Delete:
					_editMode = UITableViewCellEditingStyle.Delete;
					editing = true;
					break;
				default:
					_editMode = UITableViewCellEditingStyle.None;
					editing = false;
					break;
			}
			_bookmarksTable.SetEditing(editing, true);
			_bookmarksTable.ReloadData();
		}
		#endregion

		#region Table DataSource
		private class DataSource : UITableViewSource
		{
			const string CellIdentifier = "Cell";
			private BookmarksVC _vc;

			public DataSource(BookmarksVC vc)
			{
				_vc = vc;
			}

			private int getCorrectRowIndex(int rowIndex)
			{
				return (_vc._editMode == UITableViewCellEditingStyle.Insert) ? rowIndex - 1 : rowIndex;
			}

			private void addNewRow(UITableView tableView, NSIndexPath indexPath)
			{
				string bookmakrName = string.IsNullOrEmpty(_vc._newBookmarkNameTxt.Text) 
					? string.Format("Bookmark {0}".t(), _vc._bookmarks.Count + 1)
					: _vc._newBookmarkNameTxt.Text;
				tableView.BeginUpdates();
				var newIndexPath = NSIndexPath.FromRowSection(indexPath.Row + _vc._bookmarks.Count + 1, 0);
				var newBookmark = MgrAccessor.DocumentBookmarkMgr.GetNew(_vc._documentId, bookmakrName, _vc._currentPageNumber);
				MgrAccessor.DocumentBookmarkMgr.Save(newBookmark);
				_vc._bookmarks.Add(newBookmark);
				_vc._bookmarksTable.InsertRows(new NSIndexPath[] { newIndexPath }, UITableViewRowAnimation.Fade);
				tableView.EndUpdates();
				_vc.setEditMode(UITableViewCellEditingStyle.None);
			}

			private void deleteRow(UITableView tableView, NSIndexPath indexPath)
			{
				MgrAccessor.DocumentBookmarkMgr.Delete(_vc._bookmarks [getCorrectRowIndex(indexPath.Row)].Id);
				_vc._bookmarks.RemoveAt(indexPath.Row);
				tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
				_vc.setEditMode(UITableViewCellEditingStyle.None);
			}

			public override int RowsInSection(UITableView tableview, int section)
			{
				return (_vc._editMode == UITableViewCellEditingStyle.Insert) ? _vc._bookmarks.Count + 1 : _vc._bookmarks.Count;
			}

			public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
			{
				if ((_vc._editMode == UITableViewCellEditingStyle.Insert) && (indexPath.Row == 0)) {
					return UITableViewCellEditingStyle.Insert;
				}
				if (_vc._editMode == UITableViewCellEditingStyle.Delete) {
					return UITableViewCellEditingStyle.Delete;
				}
				return UITableViewCellEditingStyle.None;
			}

			public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
			{
				if (editingStyle == UITableViewCellEditingStyle.Insert) {
					_vc._newBookmarkNameTxt.ResignFirstResponder();
					addNewRow(tableView, indexPath);
				} else if (editingStyle == UITableViewCellEditingStyle.Delete) {
					deleteRow(tableView, indexPath);
				}
			}

			public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
			{
				return "Delete bookmark?".t();
			}

			public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				UITableViewCell cell;
				if ((_vc._editMode == UITableViewCellEditingStyle.Insert) && (indexPath.Row == 0)) {
					cell = _vc._newBookmarkCell;
					_vc._newBookmarkNameTxt.Text = string.Format("Bookmark {0}".t(), _vc._bookmarks.Count + 1);
				} else {
					cell = tableView.DequeueReusableCell(CellIdentifier);
					if (cell == null) {
						cell = new UITableViewCell(UITableViewCellStyle.Subtitle, CellIdentifier);
					}
					cell.TextLabel.Text = _vc._bookmarks [getCorrectRowIndex(indexPath.Row)].Name;
					cell.DetailTextLabel.Text = string.Format("Page {0}".t(), _vc._bookmarks [getCorrectRowIndex(indexPath.Row)].PageNumber);
				}

				return cell;
			}

			public override void RowSelected(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				if ((_vc._editMode == UITableViewCellEditingStyle.Insert) && (indexPath.Row == 0)) {
					addNewRow(tableView, indexPath);
				} else {
					int pageNumber = _vc._bookmarks [getCorrectRowIndex(indexPath.Row)].PageNumber;
					_vc._callbackAction(pageNumber);
					_vc.DismissViewController(true, null);
				}
			}
		}
		#endregion
	}
}
