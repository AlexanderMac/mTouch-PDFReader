//****************************************//
// mTouch-PDFReader library
// Bookmarks view controller
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
	public class BookmarksViewController : UIViewControllerWithPopover
	{		
		#region Fields
		
		/// <summary>
		/// Loaded document id 
		/// </summary>
		private int mDocumentId;
		
		/// <summary>
		/// Current document page
		/// </summary>
		private List<DocumentBookmark> mBookmarks;

		/// <summary>
		/// Current document page
		/// </summary>
		private int mCurrentPageNumber;
		
		/// <summary>
		/// Bookmarks table view
		/// </summary>
		private UITableView mBookmarksTable;
		
		/// <summary>
		/// New bookmarks cell view
		/// </summary>
		private UITableViewCell mNewBookmarkCell;
		
		/// <summary>
		/// New bookmark name text view
		/// </summary>
		private UITextField mNewBookmarkNameTxt;
		
		/// <summary>
		/// TableView edit mode
		/// </summary>
		private UITableViewCellEditingStyle mEditMode;
		
		#endregion

		#region Constructors
		
		public BookmarksViewController(int docId, List<DocumentBookmark> bookmarks, int currentPageNumber, Action<object> callbackAction) : base(null, null, callbackAction)
		{
			mDocumentId = docId;
			mBookmarks = bookmarks;
			mCurrentPageNumber = currentPageNumber;
			mEditMode = UITableViewCellEditingStyle.None;
		}

		#endregion

		/// <summary>
		/// Calls when view are loaded 
		/// </summary>
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
			mBookmarksTable = new UITableView(new RectangleF(0, 44, View.Bounds.Width, View.Bounds.Height), UITableViewStyle.Plain);
			mBookmarksTable.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			mBookmarksTable.RowHeight = 55;
			mBookmarksTable.Source = new DataSource(this);
			View.AddSubview(mBookmarksTable);
			
			// Create bookmark cell and text field
			mNewBookmarkCell = new UITableViewCell(UITableViewCellStyle.Default, null);
			mNewBookmarkCell.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin;
			mNewBookmarkCell.Frame = new RectangleF(0, 0, View.Bounds.Width, 55);
			mNewBookmarkNameTxt = new UITextField(new RectangleF(40, 12, View.Bounds.Width - 45, 31));
			mNewBookmarkNameTxt.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			mNewBookmarkNameTxt.BorderStyle = UITextBorderStyle.RoundedRect;
			mNewBookmarkNameTxt.Font = UIFont.SystemFontOfSize(16.0f);
			mNewBookmarkCell.AddSubview(mNewBookmarkNameTxt);
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

		/// <summary>
		/// Sets editing mode
		/// </summary>
		private void SetEditingMode(UITableViewCellEditingStyle mode)
		{
			bool editing;
			switch (mode) {
				case UITableViewCellEditingStyle.Insert:
					mEditMode = UITableViewCellEditingStyle.Insert;
					editing = true;
					break;
				case UITableViewCellEditingStyle.Delete:
					mEditMode = UITableViewCellEditingStyle.Delete;
					editing = true;
					break;
				default:
					mEditMode = UITableViewCellEditingStyle.None;
					editing = false;
					break;
			}
			mBookmarksTable.SetEditing(editing, true);
			mBookmarksTable.ReloadData();
		}

		/// <summary>
		/// TableView datasource
		/// </summary> 
		class DataSource : UITableViewSource
		{
			const string CellIdentifier = "Cell";

			private BookmarksViewController mController;

			/// <summary>
			/// Work constructor
			/// </summary>
			public DataSource(BookmarksViewController controller)
			{
				mController = controller;
			}

			/// <summary>
			/// Gets correct row index for insert and delete modes
			/// </summary>
			private int GetCorrectRowIndex(int rowIndex)
			{
				return (mController.mEditMode == UITableViewCellEditingStyle.Insert) ? rowIndex - 1 : rowIndex;
			}

			/// <summary>
			/// Adds new row
			/// </summary>
			private void AddNewRow(UITableView tableView, NSIndexPath indexPath)
			{
				string bookmakrName = string.IsNullOrEmpty(mController.mNewBookmarkNameTxt.Text) 
					? string.Format("Bookmark {0}".t(), mController.mBookmarks.Count + 1)
					: mController.mNewBookmarkNameTxt.Text;
				tableView.BeginUpdates();
				var newIndexPath = NSIndexPath.FromRowSection(indexPath.Row + mController.mBookmarks.Count + 1, 0);
				var newBookmark = new DocumentBookmark(mController.mDocumentId, -1, bookmakrName, mController.mCurrentPageNumber);
				DocumentBookmarkManager.Instance.SaveBookmark(newBookmark);
				mController.mBookmarks.Add(newBookmark);
				mController.mBookmarksTable.InsertRows(new NSIndexPath[] { newIndexPath }, UITableViewRowAnimation.Fade);
				tableView.EndUpdates();
				mController.SetEditingMode(UITableViewCellEditingStyle.None);
			}

			/// <summary>
			/// Deletes row
			/// </summary>
			private void DeleteRow(UITableView tableView, NSIndexPath indexPath)
			{
				DocumentBookmarkManager.Instance.DeleteBookmark(mController.mBookmarks[GetCorrectRowIndex(indexPath.Row)].Id);
				mController.mBookmarks.RemoveAt(indexPath.Row);
				tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
				mController.SetEditingMode(UITableViewCellEditingStyle.None);
			}

			/// <summary>
			/// Returns rows count
			/// </summary>
			public override int RowsInSection(UITableView tableview, int section)
			{
				return (mController.mEditMode == UITableViewCellEditingStyle.Insert) ? mController.mBookmarks.Count + 1 : mController.mBookmarks.Count;
			}

			/// <summary>
			/// Sets edit mode: delete or insert
			/// </summary>
			public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
			{
				// Insert button alwasy should be at the top 
				if ((mController.mEditMode == UITableViewCellEditingStyle.Insert) && (indexPath.Row == 0)) {
					return UITableViewCellEditingStyle.Insert;
				} else if (mController.mEditMode == UITableViewCellEditingStyle.Delete) {
					return UITableViewCellEditingStyle.Delete;
				} else {
					return UITableViewCellEditingStyle.None;
				}
			}

			/// <summary>
			/// Execute edit operation
			/// </summary>
			public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
			{
				if (editingStyle == UITableViewCellEditingStyle.Insert) {
					mController.mNewBookmarkNameTxt.ResignFirstResponder();
					AddNewRow(tableView, indexPath);
				} else if (editingStyle == UITableViewCellEditingStyle.Delete) {
					DeleteRow(tableView, indexPath);
				}
			}

			/// <summary>
			/// Returns title for deleted row
			/// </summary>
			public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
			{
				return "Delete bookmark?".t();
			}

			/// <summary>
			/// Returns row by id
			/// </summary>
			public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				UITableViewCell cell;
				if ((mController.mEditMode == UITableViewCellEditingStyle.Insert) && (indexPath.Row == 0)) {
					cell = mController.mNewBookmarkCell;
					mController.mNewBookmarkNameTxt.Text = string.Format("Bookmark {0}".t(), mController.mBookmarks.Count + 1);
				} else {
					cell = tableView.DequeueReusableCell(CellIdentifier);
					if (cell == null) {
						cell = new UITableViewCell(UITableViewCellStyle.Subtitle, CellIdentifier);
					}
					cell.TextLabel.Text = mController.mBookmarks[GetCorrectRowIndex(indexPath.Row)].Name;
					cell.DetailTextLabel.Text = string.Format("Page {0}".t(), mController.mBookmarks[GetCorrectRowIndex(indexPath.Row)].PageNumber);
				}
				return cell;
			}

			/// <summary>
			/// Opens view assotiated with row 
			/// </summary>
			public override void RowSelected(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				if ((mController.mEditMode == UITableViewCellEditingStyle.Insert) && (indexPath.Row == 0)) {
					// Insert button always at the top
					AddNewRow(tableView, indexPath);
				} else {
					int pageNumber = mController.mBookmarks[GetCorrectRowIndex(indexPath.Row)].PageNumber;
					mController.CallbackAction(pageNumber);
					mController.mPopoverController.Dismiss(true);
				}
			}
		}
	}
}

