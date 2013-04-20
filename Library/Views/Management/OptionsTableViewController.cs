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
using mTouchPDFReader.Library.Utils;
using mTouchPDFReader.Library.Interfaces;
using mTouchPDFReader.Library.Managers;
using mTouchPDFReader.Library.Data.Objects;
using mTouchPDFReader.Library.Data.Enums;

namespace mTouchPDFReader.Library.Views.Management
{
	public class OptionsTableViewController : UITableViewController
	{
		#region Constants		
		private const int DefaultCellWidth = 600;
		private const int DefaultCellHeight = 50;		
		#endregion
		
		#region Fields		
		// Page transition style and navigation orientation
		private UITableViewCell _PageTransitionStyleCell;
		private UITableViewCell _PageNavigationOrientationCell;
		private UITableViewCell _AutoScaleMode;
		// Visibility
		private UITableViewCell _ToolbarVisibilityCell;
		private UITableViewCell _BottombarVisibilityCell;
		private UITableViewCell _PageNumberVisibilityCell;
		// Zoom
		private UITableViewCell _ZoomScaleLevelsCell;
		private UITableViewCell _ZoomByDoubleTouchCell;
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
			View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
   
			// Page transition style and navigation orientation
			_PageTransitionStyleCell = _CreatePageTransitionStyleCell();
			_PageNavigationOrientationCell = _CreatePageNavigationOrientationCell();
			_AutoScaleMode = _CreateAutoScaleModeCell();
			// Visibility
			_ToolbarVisibilityCell = _CreateToolbarVisibilityCell();
			_BottombarVisibilityCell = _CreateBottombarVisibilityCell();
			_PageNumberVisibilityCell = _CreatePageNumberVisibilityCell();
			// Zoom
			_ZoomScaleLevelsCell = _CreateZoomScaleLevelsCell();
			_ZoomByDoubleTouchCell = _CreatemZoomByDoubleTouchCell();
			// Library info
			_LibraryReleaseDateCell = _CreateLibraryReleaseDateCell();
			_LibraryVersionCell = _CreateLibraryVersionCell();
			
			TableView = new UITableView(View.Bounds, UITableViewStyle.Grouped);
			TableView.BackgroundView = null;
			TableView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			TableView.Source = new DataSource(this);        	
		}

		/// <summary>
		/// Called when permission is shought to rotate
		/// </summary>
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		#region Helpers		
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
			var ctrl = new UISwitch(new RectangleF(DefaultCellWidth - width - 45, 10, width, 30));
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
			int width = 200;
			var slider = new UISlider(new RectangleF(DefaultCellWidth - width - 55, 10, width, 30));
			slider.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin;
			slider.MinValue = minValue;
			slider.MaxValue = maxValue;
			return slider;
		}
		#endregion

		#region Creating cells
		/// <summary>
		/// Creates the page transition style cell. 
		/// </summary>
		/// <returns>The table cell.</returns>
		private UITableViewCell _CreatePageTransitionStyleCell()
		{
			var cell = CreateCell("PageTransitionStyleCell");
			var label = CreateTitleLabelControl("Transition style".t());
			var seg = CreateSegmentControl(new string[] { "Curl".t(), "Scroll".t() }, 150);
			seg.SelectedSegment = (int)MgrAccessor.OptionsMgr.Options.PageTransitionStyle;
			seg.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Options.PageTransitionStyle = (UIPageViewControllerTransitionStyle)seg.SelectedSegment;
				MgrAccessor.OptionsMgr.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(seg);
			return cell;
		}

		/// <summary>
		/// Creates the page navigation orientation cell. 
		/// </summary>
		/// <returns>The table cell.</returns>
		private UITableViewCell _CreatePageNavigationOrientationCell()
		{
			var cell = CreateCell("PageNavigationOrientationCell");
			var label = CreateTitleLabelControl("Navigation orientation".t());
			var seg = CreateSegmentControl(new string[] { "Horizontal".t(), "Vertical".t() }, 250);
			seg.SelectedSegment = (int)MgrAccessor.OptionsMgr.Options.PageTransitionStyle;
			seg.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Options.PageNavigationOrientation = (UIPageViewControllerNavigationOrientation)seg.SelectedSegment;
				MgrAccessor.OptionsMgr.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(seg);
			return cell;
		}
		
		/// <summary>
		/// Creates the toolbar visibility cell.
		/// </summary>
		/// <returns>The table cell.</returns>
		private UITableViewCell _CreateToolbarVisibilityCell()
		{
			var cell = CreateCell("ToolbarVisibilityCell");
			var label = CreateTitleLabelControl("Toolbar".t());
			var switchCtrl = CreateSwitchControl(new string[] { "Yes".t(), "No".t() });
			switchCtrl.SetState(MgrAccessor.OptionsMgr.Options.ToolbarVisible, false);
			switchCtrl.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Options.ToolbarVisible = switchCtrl.On;
				MgrAccessor.OptionsMgr.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(switchCtrl);
			return cell;
		}
		
		/// <summary>
		/// Creates the bottombar visibility cell.
		/// </summary>
		/// <returns>The table cell.</returns>
		private UITableViewCell _CreateBottombarVisibilityCell()
		{
			var cell = CreateCell("BottombarVisibilityCell");
			var label = CreateTitleLabelControl("Bottombar".t());
			var switchCtrl = CreateSwitchControl(new string[] { "Yes".t(), "No".t() });
			switchCtrl.SetState(MgrAccessor.OptionsMgr.Options.BottombarVisible, false);
			switchCtrl.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Options.BottombarVisible = switchCtrl.On;
				MgrAccessor.OptionsMgr.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(switchCtrl);
			return cell;
		}
		
		/// <summary>
		/// Creates the page number visibility cell.
		/// </summary>
		/// <returns>The table cell.</returns>
		private UITableViewCell _CreatePageNumberVisibilityCell()
		{
			var cell = CreateCell("PageNumberVisibilityCell");
			var label = CreateTitleLabelControl("Page number".t());
			var switchCtrl = CreateSwitchControl(new string[] { "Yes".t(), "No".t() });
			switchCtrl.SetState(MgrAccessor.OptionsMgr.Options.PageNumberVisible, false);
			switchCtrl.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Options.PageNumberVisible = switchCtrl.On;
				MgrAccessor.OptionsMgr.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(switchCtrl);
			return cell;
		}

		/// <summary>
		/// Creates the page auto scale mode cell. 
		/// </summary>
		/// <returns>The table cell.</returns>
		private UITableViewCell _CreateAutoScaleModeCell()
		{
			var cell = CreateCell("AutoScaleModelCell");
			var label = CreateTitleLabelControl("Auto scale mode".t());
			var seg = CreateSegmentControl(new string[] { "Auto width".t(), "Auto height".t() }, 250);
			seg.SelectedSegment = (int)MgrAccessor.OptionsMgr.Options.AutoScaleMode;
			seg.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Options.AutoScaleMode = (AutoScaleModes)seg.SelectedSegment;
				MgrAccessor.OptionsMgr.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(seg);
			return cell;
		}

		/// <summary>
		/// Creates the zoom scale levels cell.
		/// </summary>
		/// <returns>The table cell.</returns>
		private UITableViewCell _CreateZoomScaleLevelsCell()
		{
			var cell = CreateCell("ZoomScaleLevelsCell");
			var label = CreateTitleLabelControl("Zoom scale levels".t());
			var slider = CreateSliderControl(Options.MinZoomScaleLevels, Options.MaxZoomScaleLevels);
			slider.SetValue(MgrAccessor.OptionsMgr.Options.ZoomScaleLevels, false);
			slider.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Options.ZoomScaleLevels = (int)slider.Value;
				MgrAccessor.OptionsMgr.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(slider);
			return cell;
		}
		
		/// <summary>
		/// Creates the zoom by double touch cell.
		/// </summary>
		/// <returns>The table cell.</returns>
		private UITableViewCell _CreatemZoomByDoubleTouchCell()
		{
			var cell = CreateCell("ZoomByDoubleTouchCell");
			var label = CreateTitleLabelControl("Scale by double click".t());
			var switchCtrl = CreateSwitchControl(new string[] { "Yes".t(), "No".t() });
			switchCtrl.SetState(MgrAccessor.OptionsMgr.Options.AllowZoomByDoubleTouch, false);
			switchCtrl.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Options.AllowZoomByDoubleTouch = switchCtrl.On;
				MgrAccessor.OptionsMgr.Save();
			};
			cell.AddSubview(label);
			cell.AddSubview(switchCtrl);
			return cell;
		}
		
		/// <summary>
		/// Creates the library release date cell. 
		/// </summary>
		/// <returns>The table cell.</returns>
		private UITableViewCell _CreateLibraryReleaseDateCell()
		{
			var cell = CreateCell("LibraryReleaseDateCell");
			var label = CreateTitleLabelControl("Release date".t());
			var labelInfo = CreateValueLabelControl(MgrAccessor.OptionsMgr.Options.LibraryReleaseDate.ToShortDateString());			
			cell.AddSubview(label);
			cell.AddSubview(labelInfo);
			return cell;
		}
		
		/// <summary>
		/// Creates the library version cell. 
		/// </summary>
		/// <returns>The table cell.</returns>
		private UITableViewCell _CreateLibraryVersionCell()
		{
			var cell = CreateCell("LibraryVersionCell");
			var label = CreateTitleLabelControl("Version".t());
			var labelInfo = CreateValueLabelControl(MgrAccessor.OptionsMgr.Options.LibraryVersion);
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
			private const int SectionsCount = 4;
			private readonly int[] RowsInSections = new int[] { 2, 3, 3, 2 };
			private readonly string[] SectionTitles = new string[] { "Transition style".t(), "Visibility".t(), "Scale".t(), "Library information".t() };
			
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
						switch (indexPath.Row) {
							case 0:
								return _Controller._PageTransitionStyleCell;
							case 1:
								return _Controller._PageNavigationOrientationCell;
						}
						break;
					case 1:
						switch (indexPath.Row) {
							case 0:
								return _Controller._ToolbarVisibilityCell;
							case 1:
								return _Controller._BottombarVisibilityCell;
							case 2:
								return _Controller._PageNumberVisibilityCell;
						}
						break;
					case 2:
						switch (indexPath.Row) {
							case 0:
								return _Controller._AutoScaleMode;
							case 1:
								return _Controller._ZoomScaleLevelsCell;
							case 2:
								return _Controller._ZoomByDoubleTouchCell;
						}
						break;
					case 3:
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

