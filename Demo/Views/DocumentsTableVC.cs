//
// mTouch-PDFReader demo
// DocumentsTableVC.cs
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
