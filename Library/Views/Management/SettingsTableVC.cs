//
// mTouch-PDFReader library
// SettingsTableVC.cs
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
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Managers;
using mTouchPDFReader.Library.Data.Objects;
using mTouchPDFReader.Library.Data.Enums;

namespace mTouchPDFReader.Library.Views.Management
{
	public class SettingsTableVC : UITableViewController
	{
		#region Constants & Fields
		private const int DefaultLabelLeft = 15;
		private const int DefaultLabelWidth = 200;	
		private UITableViewCell _pageTransitionStyleCell;
		private UITableViewCell _pageNavigationOrientationCell;
		private UITableViewCell _autoScaleMode;
		private UITableViewCell _toolbarVisibilityCell;
		private UITableViewCell _sliderVisibility;
		private UITableViewCell _pageNumberVisibilityCell;
		private UITableViewCell _zoomScaleLevelsCell;
		private UITableViewCell _zoomByDoubleTouchCell;
		private UITableViewCell _libraryReleaseDateCell;
		private UITableViewCell _libraryVersionCell;		
		#endregion
		
		#region Constructors		
		public SettingsTableVC() : base(null, null) { }
		#endregion	

		#region UIViewController members
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
   
			_pageTransitionStyleCell = createPageTransitionStyleCell();
			_pageNavigationOrientationCell = createPageNavigationOrientationCell();
			_autoScaleMode = createAutoScaleModeCell();

			_toolbarVisibilityCell = createToolbarVisibilityCell();
			_sliderVisibility = createSliderVisibilityCell();
			_pageNumberVisibilityCell = createPageNumberVisibilityCell();

			_zoomScaleLevelsCell = createZoomScaleLevelsCell();
			_zoomByDoubleTouchCell = createmZoomByDoubleTouchCell();

			_libraryReleaseDateCell = createLibraryReleaseDateCell();
			_libraryVersionCell = createLibraryVersionCell();

			TableView = new UITableView(View.Bounds, UITableViewStyle.Grouped)
				{
					BackgroundView = null, 
					AutoresizingMask = UIViewAutoresizing.All, 
					Source = new DataSource(this)
				};
		}
		#endregion
		
		#region Create controls helpers		
		private UITableViewCell createCell(string id)
		{
			var cell = new UITableViewCell(UITableViewCellStyle.Default, id)
				{
					AutoresizingMask = UIViewAutoresizing.All,
					BackgroundColor = UIColor.White,
					SelectionStyle = UITableViewCellSelectionStyle.None
				};
			return cell;
		}
		
		private UILabel createTitleLabelControl(string title)
		{
			var label = new UILabel(new RectangleF(DefaultLabelLeft, 15, DefaultLabelWidth, 20))
				{
					AutoresizingMask = UIViewAutoresizing.All, 
					BackgroundColor = UIColor.Clear, 
					Text = title
				};
			return label;
		}
			
		private UILabel createValueLabelControl(RectangleF cellRect, string title)
		{
			const int width = 150;
			var label = new UILabel(new RectangleF(cellRect.Width - DefaultLabelLeft - width, 15, width, 20))
	            {
					AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin, 
					BackgroundColor = UIColor.Clear, 
					TextAlignment = UITextAlignment.Right, 
					Text = title
	            };
			return label;
		}
		
		private UISegmentedControl createSegmentControl(RectangleF cellRect, string[] values, int width)
		{
			var seg = new UISegmentedControl(new RectangleF(cellRect.Width - DefaultLabelLeft - width, 5, width, 30))
				{
					AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin
				};
			for (int i = 0; i < values.Length; i++) {
				seg.InsertSegment(values [i], i, false);
			}
			return seg;
		}
		
		private UISwitch createSwitchControl(RectangleF cellRect, string[] values)
		{
			const int width = 50;
			var ctrl = new UISwitch(new RectangleF(cellRect.Width - DefaultLabelLeft - width, 5, width, 30))
				{
					AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin
				};
			return ctrl;
		}
		
		private UISlider createSliderControl(RectangleF cellRect, int minValue, int maxValue)
		{
			const int width = 100;
			var slider = new UISlider(new RectangleF(cellRect.Width - DefaultLabelLeft - width, 5, width, 30))
				{
					AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin, 
					MinValue = minValue, 
					MaxValue = maxValue
				};
			return slider;
		}
		#endregion

		#region Create cells
		private UITableViewCell createPageTransitionStyleCell()
		{
			var cell = createCell("PageTransitionStyleCell");
			var label = createTitleLabelControl("Transition style".t());
			var seg = createSegmentControl(cell.Frame, new[] { "Curl".t(), "Scroll".t() }, 100);
			seg.SelectedSegment = (int)MgrAccessor.OptionsMgr.Settings.PageTransitionStyle;
			seg.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Settings.PageTransitionStyle = (UIPageViewControllerTransitionStyle)seg.SelectedSegment;
				MgrAccessor.OptionsMgr.Save();
			};

			cell.AddSubview(label);
			cell.AddSubview(seg);
			return cell;
		}

		private UITableViewCell createPageNavigationOrientationCell()
		{
			var cell = createCell("PageNavigationOrientationCell");
			var label = createTitleLabelControl("Navigation orientation".t());
			var seg = createSegmentControl(cell.Frame, new[] { "Horizontal".t(), "Vertical".t() }, 100);
			seg.SelectedSegment = (int)MgrAccessor.OptionsMgr.Settings.PageTransitionStyle;
			seg.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Settings.PageNavigationOrientation = (UIPageViewControllerNavigationOrientation)seg.SelectedSegment;
				MgrAccessor.OptionsMgr.Save();
			};

			cell.AddSubview(label);
			cell.AddSubview(seg);
			return cell;
		}
		
		private UITableViewCell createToolbarVisibilityCell()
		{
			var cell = createCell("ToolbarVisibilityCell");
			var label = createTitleLabelControl("Toolbar".t());
			var switchCtrl = createSwitchControl(cell.Frame, new[] { "Yes".t(), "No".t() });
			switchCtrl.SetState(MgrAccessor.OptionsMgr.Settings.ToolbarVisible, false);
			switchCtrl.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Settings.ToolbarVisible = switchCtrl.On;
				MgrAccessor.OptionsMgr.Save();
			};

			cell.AddSubview(label);
			cell.AddSubview(switchCtrl);
			return cell;
		}
		
		private UITableViewCell createSliderVisibilityCell()
		{
			var cell = createCell("SliderVisibilityCell");
			var label = createTitleLabelControl("Slider".t());
			var switchCtrl = createSwitchControl(cell.Frame, new[] { "Yes".t(), "No".t() });
			switchCtrl.SetState(MgrAccessor.OptionsMgr.Settings.SliderVisible, false);
			switchCtrl.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Settings.SliderVisible = switchCtrl.On;
				MgrAccessor.OptionsMgr.Save();
			};

			cell.AddSubview(label);
			cell.AddSubview(switchCtrl);
			return cell;
		}
		
		private UITableViewCell createPageNumberVisibilityCell()
		{
			var cell = createCell("PageNumberVisibilityCell");
			var label = createTitleLabelControl("Page number".t());
			var switchCtrl = createSwitchControl(cell.Frame, new[] { "Yes".t(), "No".t() });
			switchCtrl.SetState(MgrAccessor.OptionsMgr.Settings.PageNumberVisible, false);
			switchCtrl.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Settings.PageNumberVisible = switchCtrl.On;
				MgrAccessor.OptionsMgr.Save();
			};

			cell.AddSubview(label);
			cell.AddSubview(switchCtrl);
			return cell;
		}

		private UITableViewCell createAutoScaleModeCell()
		{
			var cell = createCell("AutoScaleModelCell");
			var label = createTitleLabelControl("Auto scale mode".t());
			var seg = createSegmentControl(cell.Frame, new[] { "Auto width".t(), "Auto height".t() }, 150);
			seg.SelectedSegment = (int)MgrAccessor.OptionsMgr.Settings.AutoScaleMode;
			seg.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Settings.AutoScaleMode = (AutoScaleModes)seg.SelectedSegment;
				MgrAccessor.OptionsMgr.Save();
			};

			cell.AddSubview(label);
			cell.AddSubview(seg);
			return cell;
		}

		private UITableViewCell createZoomScaleLevelsCell()
		{
			var cell = createCell("ZoomScaleLevelsCell");
			var label = createTitleLabelControl("Zoom scale levels".t());
			var slider = createSliderControl(cell.Frame, Settings.MinZoomScaleLevels, Settings.MaxZoomScaleLevels);
			slider.SetValue(MgrAccessor.OptionsMgr.Settings.ZoomScaleLevels, false);
			slider.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Settings.ZoomScaleLevels = (int)slider.Value;
				MgrAccessor.OptionsMgr.Save();
			};

			cell.AddSubview(label);
			cell.AddSubview(slider);
			return cell;
		}
		
		private UITableViewCell createmZoomByDoubleTouchCell()
		{
			var cell = createCell("ZoomByDoubleTouchCell");
			var label = createTitleLabelControl("Scale by double click".t());
			var switchCtrl = createSwitchControl(cell.Frame, new[] { "Yes".t(), "No".t() });
			switchCtrl.SetState(MgrAccessor.OptionsMgr.Settings.AllowZoomByDoubleTouch, false);
			switchCtrl.ValueChanged += delegate {
				MgrAccessor.OptionsMgr.Settings.AllowZoomByDoubleTouch = switchCtrl.On;
				MgrAccessor.OptionsMgr.Save();
			};

			cell.AddSubview(label);
			cell.AddSubview(switchCtrl);
			return cell;
		}
		
		private UITableViewCell createLibraryReleaseDateCell()
		{
			var cell = createCell("LibraryReleaseDateCell");
			var label = createTitleLabelControl("Release date".t());
			var labelInfo = createValueLabelControl(cell.Frame, MgrAccessor.OptionsMgr.Settings.LibraryReleaseDate.ToShortDateString());			

			cell.AddSubview(label);
			cell.AddSubview(labelInfo);
			return cell;
		}
		
		private UITableViewCell createLibraryVersionCell()
		{
			var cell = createCell("LibraryVersionCell");
			var label = createTitleLabelControl("Version".t());
			var labelInfo = createValueLabelControl(cell.Frame, MgrAccessor.OptionsMgr.Settings.LibraryVersion);

			cell.AddSubview(label);
			cell.AddSubview(labelInfo);
			return cell;
		}		
		#endregion

		#region Table DataSource
		protected class DataSource : UITableViewSource
		{
			private const int SectionsCount = 4;
			private readonly int[] RowsInSections = new[] { 2, 3, 3, 2 };
			private readonly string[] SectionTitles = new[] { "Transition style".t(), "Visibility".t(), "Scale".t(), "Library information".t() };		
			private readonly SettingsTableVC _vc;

			public DataSource(SettingsTableVC vc)
			{
				_vc = vc;
			}
		
			public override int NumberOfSections(UITableView tableView)
			{
				return SectionsCount;
			}
			
			public override int RowsInSection(UITableView tableview, int section)
			{
				return RowsInSections[section];
			}

			public override string TitleForHeader(UITableView tableView, int section)
			{
				return SectionTitles[section];
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				switch (indexPath.Section) {
					case 0:
						switch (indexPath.Row) {
							case 0:
								return _vc._pageTransitionStyleCell;
							case 1:
								return _vc._pageNavigationOrientationCell;
						}
						break;
					case 1:
						switch (indexPath.Row) {
							case 0:
								return _vc._toolbarVisibilityCell;
							case 1:
								return _vc._sliderVisibility;
							case 2:
								return _vc._pageNumberVisibilityCell;
						}
						break;
					case 2:
						switch (indexPath.Row) {
							case 0:
								return _vc._autoScaleMode;
							case 1:
								return _vc._zoomScaleLevelsCell;
							case 2:
								return _vc._zoomByDoubleTouchCell;
						}
						break;
					case 3:
						switch (indexPath.Row) {
							case 0:
								return _vc._libraryReleaseDateCell;
							case 1:
								return _vc._libraryVersionCell;
						}
						break;
				}
				return null;
			}
		}
		#endregion
	}
}