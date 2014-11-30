//
// mTouch-PDFReader library
// BookmarksVC.cs
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
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Data.Objects;
using mTouchPDFReader.Library.Managers;
using mTouchPDFReader.Library.XViews;

namespace mTouchPDFReader.Library.Views.Management
{
	public class BookmarksVC : UIViewControllerWithPopover
	{		
		#region Data		
		private int _documentId;	
		private List<DocumentBookmark> _bookmarks;
		private int _currentPageNumber;
		private UITableView _bookmarksTable;
		private UITableViewCell _newBookmarkCell;
		private UITextField _newBookmarkNameTxt;
		private UITableViewCellEditingStyle _editMode;		
		#endregion

		#region UIViewController members
		public BookmarksVC(int docId, List<DocumentBookmark> bookmarks, int currentPageNumber, Action<object> callbackAction) : base(null, null, callbackAction)
		{
			_documentId = docId;
			_bookmarks = bookmarks;
			_currentPageNumber = currentPageNumber;
			_editMode = UITableViewCellEditingStyle.None;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			var toolBar = new UIToolbar(new RectangleF(0, 0, View.Bounds.Width, 44));
			toolBar.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleBottomMargin;
			toolBar.BarStyle = UIBarStyle.Black;

			var toolBarTitle = new UILabel(new RectangleF(0, 0, View.Bounds.Width, 44));
			toolBarTitle.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			toolBarTitle.BackgroundColor = UIColor.Clear;
			toolBarTitle.TextAlignment = UITextAlignment.Center;
			toolBarTitle.TextColor = UIColor.White;
			toolBarTitle.Font = UIFont.SystemFontOfSize(18.0f);
			toolBarTitle.Text = "Bookmarks".t();

			var btnAddBookmark = new UIButton(new RectangleF(5, 5, 30, 30));
			btnAddBookmark.SetImage(UIImage.FromFile("bookmarksList.png"), UIControlState.Normal);
			btnAddBookmark.TouchUpInside += delegate {
				setEditMode(UITableViewCellEditingStyle.Insert);
			};

			var btnDeleteBookmark = new UIButton(new RectangleF(43, 5, 30, 30));
			btnDeleteBookmark.SetImage(UIImage.FromFile("bookmarksList.png"), UIControlState.Normal);
			btnDeleteBookmark.TouchUpInside += delegate { 
				setEditMode(UITableViewCellEditingStyle.Delete); 
			};

			toolBar.AddSubview(toolBarTitle);
			toolBar.AddSubview(btnAddBookmark);
			toolBar.AddSubview(btnDeleteBookmark);
			View.AddSubview(toolBar);
			
			_bookmarksTable = new UITableView(new RectangleF(0, 44, View.Bounds.Width, View.Bounds.Height), UITableViewStyle.Plain);
			_bookmarksTable.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			_bookmarksTable.RowHeight = 55;
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
		
		protected override SizeF getPopoverSize()
		{
			return new SizeF(400, 400);
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
					_vc.CallbackAction(pageNumber);
					_vc._popoverController.Dismiss(true);
				}
			}
		}
		#endregion
	}
}
