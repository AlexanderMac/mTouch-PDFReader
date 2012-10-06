//
// mTouch-PDFReader library
// OptionsTableViewController.cs (Options table view controller)
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
using mTouchPDFReader.Library.Data.Managers;
using mTouchPDFReader.Library.Data.Enums;
using mTouchPDFReader.Library.Data.Objects;

namespace mTouchPDFReader.Library.Views.Management
{
	public class OptionsTableViewController : UITableViewController
	{
		#region Constants		
		private const int DefaultCellWidth = 600;
		private const int DefaultCellHeight = 50;		
		#endregion
		
		#region Fields		
		// Page turning
		private UITableViewCell _PageTurningTypeCell;
		// Visibility
		private UITableViewCell _ToolbarVisibilityCell;
		private UITableViewCell _BottombarVisibilityCell;
		private UITableViewCell _PageNumberVisibilityCell;
		private UITableViewCell _NoteBtnVisibilityCell;
		private UITableViewCell _BookmarksBtnVisibilityCell;
		private UITableViewCell _ThumbsBtnVisibilityCell; 
		// Colors
		private UITableViewCell _BackgroundColorCell; 
		// Zoom
		private UITableViewCell _ZoomScaleLevelsCell;
		private UITableViewCell _ZoomByDoubleTouchCell;
		// Thumbs
		private UITableViewCell _ThumbsBufferSizeCell;
		private UITableViewCell _ThumbSizeCell;
		// Library info
		private UITableViewCell _LibraryReleaseDateCell;
		private UITableViewCell _LibraryVersionCell;		
		#endregion
		
		#region Constructors		
		public OptionsTableViewController(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		[Export("initWithCoder:")]
		public OptionsTableViewController(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		public OptionsTableViewController() : base(null, null)
		{
			Initialize();
		}

		void Initialize()
		{
		}		
		#endregion	
		
		/// <summary>
		/// Calls when view has loaded
		/// </summary>
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
   
			// Page turning type
			_PageTurningTypeCell = CreateTurningTypeCell();
			// Visibility
			_ToolbarVisibilityCell = CreateToolbarVisibilityCell();
			_BottombarVisibilityCell = CreateBottombarVisibilityCell();
			_PageNumberVisibilityCell = CreatePageNumberVisibilityCell();
			_NoteBtnVisibilityCell = CreateNoteBtnVisibilityCell();
			_BookmarksBtnVisibilityCell = CreateBookmarksBtnVisibilityCell();
			_ThumbsBtnVisibilityCell = CreateThumbsBtnVisibilityCell();
			// Colors
			_BackgroundColorCell = CreateBackgroundColorCell();
			// Zoom
			_ZoomScaleLevelsCell = CreateZoomScaleLevelsCell();
			_ZoomByDoubleTouchCell = CreatemZoomByDoubleTouchCell();
			// Thumbs
			_ThumbsBufferSizeCell = CreateThumbsBufferSizeCell();
			_ThumbSizeCell = CreateThumbSizeCell();
			// Library info
			_LibraryReleaseDateCell = CreateLibraryReleaseDateCell();
			_LibraryVersionCell = CreateLibraryVersionCell();
			
			TableView = new UITableView(View.Bounds, UITableViewStyle.Grouped);
			TableView.Source = new DataSource(this);        	
		}
		
		/// <summary>
		/// Called when permission is shought to rotate
		/// </summary>
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		#region Logic		
		/// <summary>
		/// Creates table cell 
		/// </summary>
		/// <param name="id">Cell UID</param>
		/// <returns>Table cell</returns>
		private UITableViewCell CreateCell(string id)
		{
			var cell = new UITableViewCell(UITableViewCellStyle.Default, id);
			cell.Frame = new RectangleF(0, 0, DefaultCellWidth, DefaultCellHeight);
			cell.BackgroundColor = UIColor.White;
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			return cell;
		}
		
		/// <summary>
		/// Create title label control 
		/// </summary>
		/// <param name="title">Label title</param>
		/// <returns>Label control</returns>
		private UILabel CreateTitleLabelControl(string title)
		{
			var label = new UILabel(new RectangleF(60, 15, 400, 20));
			label.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin;
			label.BackgroundColor = UIColor.Clear;
			label.Text = title;
			label.Font = UIFont.BoldSystemFontOfSize(18.0f);
			return label;
		}
		
		/// <summary>
		/// Create value label control 
		/// </summary>
		/// <param name="title">Label title</param>
		/// <returns>Label control</returns>
		private UILabel CreateValueLabelControl(string title)
		{
			int width = 250;
			var label = new UILabel(new RectangleF(DefaultCellWidth - width - 60, 15, width, 20));
			label.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin;
			label.BackgroundColor = UIColor.Clear;
			label.TextAlignment = UITextAlignment.Right;
			label.Text = title;	
			return label;
		}
		
		/// <summary>
		/// Creates segment control 
		/// </summary>
		/// <param name="values">Segment control values</param>
		/// <param name="width">Control width</param>
		/// <returns>Segment control</returns>
		private UISegmentedControl CreateSegmentControl(string[] values, int width)
		{
			var seg = new UISegmentedControl(new RectangleF(DefaultCellWidth - width - 60, 10, width, 30));
			seg.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin;
			for (int i = 0; i < values.Length; i++) {
				seg.InsertSegment(values [i], i, false);
			}
			return seg;
		}
		
		/// <summary>
		/// Creates switch control
		/// <param name="values">Switch control values</param>
		/// <param name="width">Control width</param>
		/// </summary>
		/// <returns>Switch control</returns>
		private UISwitch CreateSwitchControl(string[] values)
		{
			int width = 90;
			var ctrl = new UISwitch(new RectangleF(DefaultCellWidth - width - 60, 10, width, 30));
			ctrl.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin;
			return ctrl;
		}
		
		/// <summary>
		/// Creates slider control
		/// </summary>
		/// <param name="minValue">Slider minimum value</param>
		/// <param name="maxValue">Slider maximum value</param>
		/// <returns>Slider control</returns>
		private UISlider CreateSliderControl(int minValue, int maxValue)
		{
			int width = 250;
			var slider = new UISlider(new RectangleF(DefaultCellWidth - width - 60, 10, width, 30));
			slider.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin;
			slider.MinValue = minValue;
			slider.MaxValue = maxValue;
			return slider;
		}
		
		/// <summary>
		/// Creates turning type cell 
		/// </summary>
		/// <returns>Table cell</returns>
		private UITableViewCell CreateTurningTypeCell()
		{
			var cell = CreateCell("PageTurningTypeCell");
			var label = CreateTitleLabelControl("Type".t());
			var seg = CreateSegmentControl(new string[] { "Horz.".t(),	"Vert.".t() }, 150);
			seg.SelectedSegment = (int)OptionsManager.Instance.Options.PageTurningType;
			seg.ValueChanged += delegate {
				OptionsManager.Instance.Options.PageTurningType = (PageTurningTypes)seg.SelectedSegment;
				OptionsManager.Instance.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(seg);
			return cell;
		}
		
		/// <summary>
		/// Creates toolbar visibility cell 
		/// </summary>
		/// <returns>Table cell</returns>
		private UITableViewCell CreateToolbarVisibilityCell()
		{
			var cell = CreateCell("ToolbarVisibilityCell");
			var label = CreateTitleLabelControl("Toolbar".t());
			var switchCtrl = CreateSwitchControl(new string[] { "Yes".t(), "No".t() });
			switchCtrl.SetState(OptionsManager.Instance.Options.ToolbarVisible, false);
			switchCtrl.ValueChanged += delegate {
				OptionsManager.Instance.Options.ToolbarVisible = switchCtrl.On;
				OptionsManager.Instance.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(switchCtrl);
			return cell;
		}
		
		/// <summary>
		/// Creates bottombar visibility cell 
		/// </summary>
		/// <returns>Table cell</returns>
		private UITableViewCell CreateBottombarVisibilityCell()
		{
			var cell = CreateCell("BottombarVisibilityCell");
			var label = CreateTitleLabelControl("Bottombar".t());
			var switchCtrl = CreateSwitchControl(new string[] { "Yes".t(), "No".t() });
			switchCtrl.SetState(OptionsManager.Instance.Options.BottombarVisible, false);
			switchCtrl.ValueChanged += delegate {
				OptionsManager.Instance.Options.BottombarVisible = switchCtrl.On;
				OptionsManager.Instance.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(switchCtrl);
			return cell;
		}
		
		/// <summary>
		/// Creates page number visibility cell 
		/// </summary>
		/// <returns>Table cell</returns>
		private UITableViewCell CreatePageNumberVisibilityCell()
		{
			var cell = CreateCell("PageNumberVisibilityCell");
			var label = CreateTitleLabelControl("Page number".t());
			var switchCtrl = CreateSwitchControl(new string[] { "Yes".t(), "No".t() });
			switchCtrl.SetState(OptionsManager.Instance.Options.PageNumberVisible, false);
			switchCtrl.ValueChanged += delegate {
				OptionsManager.Instance.Options.PageNumberVisible = switchCtrl.On;
				OptionsManager.Instance.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(switchCtrl);
			return cell;
		}
		
		/// <summary>
		/// Creates note button visibility cell 
		/// </summary>
		/// <returns>Table cell</returns>
		private UITableViewCell CreateNoteBtnVisibilityCell()
		{
			var cell = CreateCell("NoteBtnVisibilityCell");
			var label = CreateTitleLabelControl("Button 'Note'".t());
			var switchCtrl = CreateSwitchControl(new string[] { "Yes".t(), "No".t() });
			switchCtrl.SetState(OptionsManager.Instance.Options.NoteBtnVisible, false);
			switchCtrl.ValueChanged += delegate {
				OptionsManager.Instance.Options.NoteBtnVisible = switchCtrl.On;
				OptionsManager.Instance.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(switchCtrl);
			return cell;
		}
		
		/// <summary>
		/// Creates bookmarks button visibility cell 
		/// </summary>
		/// <returns>Table cell</returns>
		private UITableViewCell CreateBookmarksBtnVisibilityCell()
		{
			var cell = CreateCell("BookmarksBtnVisibilityCell");
			var label = CreateTitleLabelControl("Button 'Bookmarks'".t());
			var switchCtrl = CreateSwitchControl(new string[] { "Yes".t(), "No".t() });
			switchCtrl.SetState(OptionsManager.Instance.Options.BookmarksBtnVisible, false);
			switchCtrl.ValueChanged += delegate {
				OptionsManager.Instance.Options.BookmarksBtnVisible = switchCtrl.On;
				OptionsManager.Instance.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(switchCtrl);
			return cell;
		}
		
		/// <summary>
		/// Creates thumbs button visibility cell 
		/// </summary>
		/// <returns>Table cell</returns>
		private UITableViewCell CreateThumbsBtnVisibilityCell()
		{
			var cell = CreateCell("ThumbsBtnVisibilityCell");
			var label = CreateTitleLabelControl("Button 'Thumbs'".t());
			var switchCtrl = CreateSwitchControl(new string[] { "Yes".t(), "No".t() });
			switchCtrl.SetState(OptionsManager.Instance.Options.ThumbsBtnVisible, false);
			switchCtrl.ValueChanged += delegate {
				OptionsManager.Instance.Options.ThumbsBtnVisible = switchCtrl.On;
				OptionsManager.Instance.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(switchCtrl);
			return cell;
		}
		
		/// <summary>
		/// Creates background color cell 
		/// </summary>
		/// <returns>Table cell</returns>
		private UITableViewCell CreateBackgroundColorCell()
		{
			var cell = CreateCell("BackgroundColorCell");
			var label = CreateTitleLabelControl("Background".t());
			cell.AddSubview(label);
			return cell;
		}
		
		/// <summary>
		/// Creates zoom scale levels cell 
		/// </summary>
		/// <returns>Table cell</returns>
		private UITableViewCell CreateZoomScaleLevelsCell()
		{
			var cell = CreateCell("ZoomScaleLevelsCell");
			var label = CreateTitleLabelControl("Zoom scale levels".t());
			var slider = CreateSliderControl(Options.MinZoomScaleLevels, Options.MaxZoomScaleLevels);
			slider.SetValue(OptionsManager.Instance.Options.ZoomScaleLevels, false);
			slider.ValueChanged += delegate {
				OptionsManager.Instance.Options.ZoomScaleLevels = (int)slider.Value;
				OptionsManager.Instance.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(slider);
			return cell;
		}
		
		/// <summary>
		/// Creates zoom by double touch cell 
		/// </summary>
		/// <returns>Table cell</returns>
		private UITableViewCell CreatemZoomByDoubleTouchCell()
		{
			var cell = CreateCell("ZoomByDoubleTouchCell");
			var label = CreateTitleLabelControl("Scale by double click".t());
			var switchCtrl = CreateSwitchControl(new string[] { "Yes".t(), "No".t() });
			switchCtrl.SetState(OptionsManager.Instance.Options.AllowZoomByDoubleTouch, false);
			switchCtrl.ValueChanged += delegate {
				OptionsManager.Instance.Options.AllowZoomByDoubleTouch = switchCtrl.On;
				OptionsManager.Instance.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(switchCtrl);
			return cell;
		}
		
		/// <summary>
		/// Creates thumbs buffer size cell 
		/// </summary>
		/// <returns>Table cell</returns>
		private UITableViewCell CreateThumbsBufferSizeCell()
		{
			var cell = CreateCell("ThumbsBufferSizeCell");
			var label = CreateTitleLabelControl("Thumbs buffer size".t());
			var slider = CreateSliderControl(Options.MinThumbsBufferSize, Options.MaxThumbsBufferSize);
			slider.SetValue(OptionsManager.Instance.Options.ThumbsBufferSize, false);
			slider.ValueChanged += delegate {
				OptionsManager.Instance.Options.ThumbsBufferSize = (int)slider.Value;
				OptionsManager.Instance.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(slider);
			return cell;
		}
		
		/// <summary>
		/// Creates thumbs size cell 
		/// </summary>
		/// <returns>Table cell</returns>
		private UITableViewCell CreateThumbSizeCell()
		{
			var cell = CreateCell("ThumbSizeCell");
			var label = CreateTitleLabelControl("Thumbs size".t());
			var slider = CreateSliderControl(Options.MinThumbSize, Options.MaxThumbSize);
			slider.SetValue(OptionsManager.Instance.Options.ThumbSize, false);
			slider.ValueChanged += delegate {
				OptionsManager.Instance.Options.ThumbSize = (int)slider.Value;
				OptionsManager.Instance.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(slider);
			return cell;
		}
		
		/// <summary>
		/// Creates library release date cell 
		/// </summary>
		/// <returns>Table cell</returns>
		private UITableViewCell CreateLibraryReleaseDateCell()
		{
			var cell = CreateCell("LibraryReleaseDateCell");
			var label = CreateTitleLabelControl("Release date".t());
			var labelInfo = CreateValueLabelControl(OptionsManager.Instance.Options.LibraryReleaseDate.ToShortDateString());			
			cell.AddSubview(label);
			cell.AddSubview(labelInfo);
			return cell;
		}
		
		/// <summary>
		/// Creates library version cell 
		/// </summary>
		/// <returns>Table cell</returns>
		private UITableViewCell CreateLibraryVersionCell()
		{
			var cell = CreateCell("LibraryVersionCell");
			var label = CreateTitleLabelControl("Version".t());
			var labelInfo = CreateValueLabelControl(OptionsManager.Instance.Options.LibraryVersion);
			cell.AddSubview(label);
			cell.AddSubview(labelInfo);
			return cell;
		}		
		#endregion
		
		/// <summary>
		/// TableView datasource
		/// </summary> 
		class DataSource : UITableViewSource
		{
			private const int SectionsCount = 6;
			private readonly int[] RowsInSections = new int[] { 1, 6, 1, 2, 2, 2 };
			private readonly string[] SectionTitles = new string[] { "Turning".t(), "Visibility".t(), "Color".t(), "Scale".t(), "Thumbs".t(), "Library information".t() };
			
			/// <summary>
			/// Parent table controller
			/// </summary>
			private OptionsTableViewController _Controller;

			/// <summary>
			/// Work constructor
			/// </summary>
			public DataSource(OptionsTableViewController controller)
			{
				_Controller = controller;
			}
		
			/// <summary>
			/// Returns numbers of groups 
			/// </summary>
			public override int NumberOfSections(UITableView tableView)
			{
				return SectionsCount;
			}
			
			/// <summary>
			/// Returns rows count
			/// </summary>
			public override int RowsInSection(UITableView tableview, int section)
			{
				return RowsInSections [section];
			}
			
			/// <summary>
			/// Returns title four group
			/// </summary>
			public override string TitleForHeader(UITableView tableView, int section)
			{
				return SectionTitles [section];
			}

			/// <summary>
			/// Returns row by id
			/// </summary>
			public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				switch (indexPath.Section) {
					case 0:
						return _Controller._PageTurningTypeCell;
					case 1:
						switch (indexPath.Row) {
							case 0:
								return _Controller._ToolbarVisibilityCell;
							case 1:
								return _Controller._BottombarVisibilityCell;
							case 2:
								return _Controller._PageNumberVisibilityCell;
							case 3:
								return _Controller._NoteBtnVisibilityCell;
							case 4:
								return _Controller._BookmarksBtnVisibilityCell;
							case 5:
								return _Controller._ThumbsBtnVisibilityCell;
						}
						break;
					case 2:
						return _Controller._BackgroundColorCell;
					case 3:
						switch (indexPath.Row) {
							case 0:
								return _Controller._ZoomScaleLevelsCell;
							case 1:
								return _Controller._ZoomByDoubleTouchCell;
						}
						break;
					case 4:
						switch (indexPath.Row) {
							case 0:
								return _Controller._ThumbsBufferSizeCell;
							case 1:
								return _Controller._ThumbSizeCell;
						}
						break;
					case 5:
						switch (indexPath.Row) {
							case 0:
								return _Controller._LibraryReleaseDateCell;
							case 1:
								return _Controller._LibraryVersionCell;
						}
						break;
				}
				return null;
			}
		}
	}
}

