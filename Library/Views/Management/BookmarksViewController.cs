//
// mTouch-PDFReader library
// BookmarksViewController.cs (Bookmarks view controller)
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
using mTouchPDFReader.Library.Data.Objects;
using mTouchPDFReader.Library.Managers;
using mTouchPDFReader.Library.XViews;

namespace mTouchPDFReader.Library.Views.Management
{
	public class BookmarksViewController : UIViewControllerWithPopover
	{		
		#region Fields		
		private int _DocumentId;	
		private List<DocumentBookmark> _Bookmarks;
		private int _CurrentPageNumber;
		private UITableView _BookmarksTable;
		private UITableViewCell _NewBookmarkCell;
		private UITextField _NewBookmarkNameTxt;
		private UITableViewCellEditingStyle _EditMode;		
		#endregion

		#region Constructors	
		public BookmarksViewController(int docId, List<DocumentBookmark> bookmarks, int currentPageNumber, Action<object> callbackAction) : base(null, null, callbackAction)
		{
			_DocumentId = docId;
			_Bookmarks = bookmarks;
			_CurrentPageNumber = currentPageNumber;
			_EditMode = UITableViewCellEditingStyle.None;
		}
		#endregion

		#region UI Logic
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			// Create toolbar, title label and buttons
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
			btnAddBookmark.SetImage(UIImage.FromFile("Images/Toolbar/BookmarkAdd32.png"), UIControlState.Normal);
			btnAddBookmark.TouchUpInside += delegate {
				SetEditingMode(UITableViewCellEditingStyle.Insert);
			};
			var btnDeleteBookmark = new UIButton(new RectangleF(43, 5, 30, 30));
			btnDeleteBookmark.SetImage(UIImage.FromFile("Images/Toolbar/BookmarkDelete32.png"), UIControlState.Normal);
			btnDeleteBookmark.TouchUpInside += delegate { 
				SetEditingMode(UITableViewCellEditingStyle.Delete); 
			};
			toolBar.AddSubview(toolBarTitle);
			toolBar.AddSubview(btnAddBookmark);
			toolBar.AddSubview(btnDeleteBookmark);
			View.AddSubview(toolBar);
			
			// Create bookmarks table
			_BookmarksTable = new UITableView(new RectangleF(0, 44, View.Bounds.Width, View.Bounds.Height), UITableViewStyle.Plain);
			_BookmarksTable.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			_BookmarksTable.RowHeight = 55;
			_BookmarksTable.Source = new DataSource(this);
			View.AddSubview(_BookmarksTable);
			
			// Create bookmark cell and text field
			_NewBookmarkCell = new UITableViewCell(UITableViewCellStyle.Default, null);
			_NewBookmarkCell.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin;
			_NewBookmarkCell.Frame = new RectangleF(0, 0, View.Bounds.Width, 55);
			_NewBookmarkNameTxt = new UITextField(new RectangleF(40, 12, View.Bounds.Width - 45, 31));
			_NewBookmarkNameTxt.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			_NewBookmarkNameTxt.BorderStyle = UITextBorderStyle.RoundedRect;
			_NewBookmarkNameTxt.Font = UIFont.SystemFontOfSize(16.0f);
			_NewBookmarkCell.AddSubview(_NewBookmarkNameTxt);
		}
		
		protected override SizeF GetPopoverSize()
		{
			return new SizeF(400, 400);
		}

		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		private void SetEditingMode(UITableViewCellEditingStyle mode)
		{
			bool editing;
			switch (mode) {
				case UITableViewCellEditingStyle.Insert:
					_EditMode = UITableViewCellEditingStyle.Insert;
					editing = true;
					break;
				case UITableViewCellEditingStyle.Delete:
					_EditMode = UITableViewCellEditingStyle.Delete;
					editing = true;
					break;
				default:
					_EditMode = UITableViewCellEditingStyle.None;
					editing = false;
					break;
			}
			_BookmarksTable.SetEditing(editing, true);
			_BookmarksTable.ReloadData();
		}
		#endregion

		#region Table DataSource
		private class DataSource : UITableViewSource
		{
			const string CellIdentifier = "Cell";
			private BookmarksViewController _Controller;

			public DataSource(BookmarksViewController controller)
			{
				_Controller = controller;
			}

			private int GetCorrectRowIndex(int rowIndex)
			{
				return (_Controller._EditMode == UITableViewCellEditingStyle.Insert) ? rowIndex - 1 : rowIndex;
			}

			private void AddNewRow(UITableView tableView, NSIndexPath indexPath)
			{
				string bookmakrName = string.IsNullOrEmpty(_Controller._NewBookmarkNameTxt.Text) 
					? string.Format("Bookmark {0}".t(), _Controller._Bookmarks.Count + 1)
					: _Controller._NewBookmarkNameTxt.Text;
				tableView.BeginUpdates();
				var newIndexPath = NSIndexPath.FromRowSection(indexPath.Row + _Controller._Bookmarks.Count + 1, 0);
				var newBookmark = MgrAccessor.DocumentBookmarkMgr.GetNew(_Controller._DocumentId, bookmakrName, _Controller._CurrentPageNumber);
				MgrAccessor.DocumentBookmarkMgr.Save(newBookmark);
				_Controller._Bookmarks.Add(newBookmark);
				_Controller._BookmarksTable.InsertRows(new NSIndexPath[] { newIndexPath }, UITableViewRowAnimation.Fade);
				tableView.EndUpdates();
				_Controller.SetEditingMode(UITableViewCellEditingStyle.None);
			}

			private void DeleteRow(UITableView tableView, NSIndexPath indexPath)
			{
				MgrAccessor.DocumentBookmarkMgr.Delete(_Controller._Bookmarks [GetCorrectRowIndex(indexPath.Row)].Id);
				_Controller._Bookmarks.RemoveAt(indexPath.Row);
				tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
				_Controller.SetEditingMode(UITableViewCellEditingStyle.None);
			}

			public override int RowsInSection(UITableView tableview, int section)
			{
				return (_Controller._EditMode == UITableViewCellEditingStyle.Insert) ? _Controller._Bookmarks.Count + 1 : _Controller._Bookmarks.Count;
			}

			public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
			{
				// Insert button alwasy should be at the top 
				if ((_Controller._EditMode == UITableViewCellEditingStyle.Insert) && (indexPath.Row == 0)) {
					return UITableViewCellEditingStyle.Insert;
				}
				if (_Controller._EditMode == UITableViewCellEditingStyle.Delete) {
					return UITableViewCellEditingStyle.Delete;
				}
				return UITableViewCellEditingStyle.None;
			}

			public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
			{
				if (editingStyle == UITableViewCellEditingStyle.Insert) {
					_Controller._NewBookmarkNameTxt.ResignFirstResponder();
					AddNewRow(tableView, indexPath);
				} else if (editingStyle == UITableViewCellEditingStyle.Delete) {
					DeleteRow(tableView, indexPath);
				}
			}

			public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
			{
				return "Delete bookmark?".t();
			}

			public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				UITableViewCell cell;
				if ((_Controller._EditMode == UITableViewCellEditingStyle.Insert) && (indexPath.Row == 0)) {
					cell = _Controller._NewBookmarkCell;
					_Controller._NewBookmarkNameTxt.Text = string.Format("Bookmark {0}".t(), _Controller._Bookmarks.Count + 1);
				} else {
					cell = tableView.DequeueReusableCell(CellIdentifier);
					if (cell == null) {
						cell = new UITableViewCell(UITableViewCellStyle.Subtitle, CellIdentifier);
					}
					cell.TextLabel.Text = _Controller._Bookmarks [GetCorrectRowIndex(indexPath.Row)].Name;
					cell.DetailTextLabel.Text = string.Format("Page {0}".t(), _Controller._Bookmarks [GetCorrectRowIndex(indexPath.Row)].PageNumber);
				}
				return cell;
			}

			public override void RowSelected(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				if ((_Controller._EditMode == UITableViewCellEditingStyle.Insert) && (indexPath.Row == 0)) {
					AddNewRow(tableView, indexPath);
				} else {
					int pageNumber = _Controller._Bookmarks [GetCorrectRowIndex(indexPath.Row)].PageNumber;
					_Controller.CallbackAction(pageNumber);
					_Controller._PopoverController.Dismiss(true);
				}
			}
		}
		#endregion
	}
}

