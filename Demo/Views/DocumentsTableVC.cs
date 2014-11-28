//
// mTouch-PDFReader demo
// DocumentsTableVC.cs
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
using System.IO;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using mTouchPDFReader.Demo.Data;
using mTouchPDFReader.Library.Views.Core;

namespace mTouchPDFReader.Demo
{
	partial class DocumentsTableVC : UITableViewController
	{
		public DocumentsTableVC (IntPtr handle) : base (handle)
		{
		}

		public DocumentsTableVC() : base("DocumentsTableVC", null)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			PDFDocumentFile.FillFromDirectories(new[] {
				Path.Combine(NSBundle.MainBundle.BundlePath, "Documents"),
				Environment.GetFolderPath(Environment.SpecialFolder.Personal)
			});

			Title = "Documents";
			TableView.Source = new DataSource(this);
		}

		private class DataSource : UITableViewSource
		{
			const string DefaultCellIdentifier = "DefaultCellIdentifier";
			private DocumentsTableVC _vc;

			public DataSource(DocumentsTableVC vc)
			{
				_vc = vc;
			}

			private void openDocument(int rowId)
			{				
				var docViewController = new DocumentVC(PDFDocumentFile.PDFFiles[rowId].Id, PDFDocumentFile.PDFFiles[rowId].Name, PDFDocumentFile.PDFFiles[rowId].FilePath);
				_vc.NavigationController.PushViewController(docViewController, true);
			}

			public override int RowsInSection(UITableView tableview, int section)
			{
				return PDFDocumentFile.PDFFiles.Count;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(DefaultCellIdentifier);
				if (cell == null) {
					cell = new UITableViewCell(UITableViewCellStyle.Subtitle, DefaultCellIdentifier);
				}

				cell.TextLabel.Text = PDFDocumentFile.PDFFiles [indexPath.Row].Name;
				cell.DetailTextLabel.Text = string.Format("Size: {0:f} Mb", PDFDocumentFile.PDFFiles [indexPath.Row].FileSizeMb);
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

				return cell;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				openDocument(indexPath.Row);
				tableView.DeselectRow(indexPath, true);
			}

			public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
			{
				openDocument(indexPath.Row);
				tableView.DeselectRow(indexPath, true);
			}
		}
	}
}
