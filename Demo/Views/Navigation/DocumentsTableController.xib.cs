//****************************************//
// mTouch-PDFReader demo
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using System.Collections.Generic;
using System.IO;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using mTouchPDFReader.Demo.DataObjects;
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
		
		/// <summary>
		/// Calls when view are loaded
		/// </summary>
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			Title = "Documents";
			PDFFile.FillFromDirectories(new string[] {
				Path.Combine(NSBundle.MainBundle.BundlePath, "Resource"),
				Environment.GetFolderPath(Environment.SpecialFolder.Personal)
			});
			TableView.Source = new DataSource(this);
		}
		
		/// <summary>
		/// Called when permission is shought to rotate
		/// </summary>
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		/// <summary>
		/// TableView datasource
		/// </summary> 
		class DataSource : UITableViewSource
		{
			/// <summary>
			/// Default cell id
			/// </summary>
			const string DefaultCellIdentifier = "DefaultCellIdentifier";
			
			/// <summary>
			/// Parent table controller
			/// </summary>
			private DocumentsTableController mController;

			/// <summary>
			/// Work constructor
			/// </summary>
			public DataSource(DocumentsTableController controller)
			{
				mController = controller;
			}

			/// <summary>
			/// Opens view assotiated with row 
			/// </summary>
			private void OpenDocument(int rowId)
			{				
				var docViewController = new DocumentViewController();
				mController.NavigationController.PushViewController(docViewController, true);
				docViewController.OpenDocument(PDFFile.PDFFiles[rowId].Id, PDFFile.PDFFiles[rowId].Name, PDFFile.PDFFiles[rowId].FilePath);
			}

			/// <summary>
			/// Returns rows count
			/// </summary>
			public override int RowsInSection(UITableView tableview, int section)
			{
				return PDFFile.PDFFiles.Count;
			}

			/// <summary>
			/// Returns row by id
			/// </summary>
			public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				UITableViewCell cell = null;
				cell = tableView.DequeueReusableCell(DefaultCellIdentifier);
				if (cell == null) {
					cell = new UITableViewCell(UITableViewCellStyle.Subtitle, DefaultCellIdentifier);
				}
				cell.TextLabel.Text = PDFFile.PDFFiles[indexPath.Row].Name;
				cell.DetailTextLabel.Text = string.Format("Size: {0:f} Mb", PDFFile.PDFFiles[indexPath.Row].FileSizeMb);
				cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
				return cell;
			}

			/// <summary>
			/// Selects theme when user clicked by row
			/// </summary>
			public override void RowSelected(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				OpenDocument(indexPath.Row);
				tableView.DeselectRow(indexPath, true);
			}

			/// <summary>
			/// Selectes theme when user clicked by accessory button
			/// </summary>
			public override void AccessoryButtonTapped(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				OpenDocument(indexPath.Row);
				tableView.DeselectRow(indexPath, true);
			}
		}
	}
}

