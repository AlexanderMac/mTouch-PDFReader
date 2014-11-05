//
// mTouch-PDFReader demo
// DocumentsTableController.cs
//
//  Author:
//       Alexander Matsibarov (macasun) <amatsibarov@gmail.com>
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
using mTouchPDFReader.Demo.Library;
using mTouchPDFReader.Library.Views.Core;

namespace mTouchPDFReader.Demo
{
	public partial class DocumentsTableController : UITableViewController
	{
		#region Constructors
		public DocumentsTableController(IntPtr handle) : base(handle)
		{
		}

		[Export("initWithCoder:")]
		public DocumentsTableController(NSCoder coder) : base(coder)
		{
		}

		public DocumentsTableController() : base("DocumentsTableController", null)
		{
		}	
		#endregion	
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			Title = "Documents";
			PDFFile.FillFromDirectories(new[] {
				Path.Combine(NSBundle.MainBundle.BundlePath, "Resource"),
				Environment.GetFolderPath(Environment.SpecialFolder.Personal)
			});
			TableView.Source = new DataSource(this);
		}

		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		class DataSource : UITableViewSource
		{
			const string DefaultCellIdentifier = "DefaultCellIdentifier";
			private readonly DocumentsTableController _Controller;

			public DataSource(DocumentsTableController controller)
			{
				_Controller = controller;
			}

			private void OpenDocument(int rowId)
			{				
				var docViewController = new DocumentViewController(PDFFile.PDFFiles[rowId].Id, PDFFile.PDFFiles[rowId].Name, PDFFile.PDFFiles[rowId].FilePath);
				_Controller.NavigationController.PushViewController(docViewController, true);
			}

			public override int RowsInSection(UITableView tableview, int section)
			{
				return PDFFile.PDFFiles.Count;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(DefaultCellIdentifier);
				if (cell == null) {
					cell = new UITableViewCell(UITableViewCellStyle.Subtitle, DefaultCellIdentifier);
				}
				cell.TextLabel.Text = PDFFile.PDFFiles [indexPath.Row].Name;
				cell.DetailTextLabel.Text = string.Format("Size: {0:f} Mb", PDFFile.PDFFiles [indexPath.Row].FileSizeMb);
				cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
				return cell;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				OpenDocument(indexPath.Row);
				tableView.DeselectRow(indexPath, true);
			}

			public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
			{
				OpenDocument(indexPath.Row);
				tableView.DeselectRow(indexPath, true);
			}
		}
	}
}

