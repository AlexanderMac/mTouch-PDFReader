//
// mTouch-PDFReader library
// Options.cs (DOptions)
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
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Data.Enums;

namespace mTouchPDFReader.Library.Data.Objects
{
	public class Options
	{
		#region Default constants	
		private const UIPageViewControllerTransitionStyle DefaultPageTransitionStyle = UIPageViewControllerTransitionStyle.PageCurl;
		private const UIPageViewControllerNavigationOrientation DefaultPageNavigationOrientation = UIPageViewControllerNavigationOrientation.Horizontal;

		private const bool DefaultToolbarVisible = true;
		private const bool DefaultStatusbarVisible = true;
		private const bool DefaultPageNumberVisible = true;

		private const AutoScaleModes DefaultAutoScaleMode = AutoScaleModes.AutoWidth;
		private const int DefaultZoomScaleLevels = 10;
		private const bool DefaultAllowZoomByDoubleTouch = true;
		private const int DefaultThumbsBufferSize = 20;
		private const int DefaultThumbSize = 125;		
		#endregion
		
		#region Constants allowable minimum/maximum values		
		public const int MinZoomScaleLevels = 2;
		public const int MaxZoomScaleLevels = 10;
		public const int MinThumbsBufferSize = 15;
		public const int MaxThumbsBufferSize = 100;
		public const int MinThumbSize = 100;
		public const int MaxThumbSize = 200;	
		#endregion
		
		#region Fields	
		public UIPageViewControllerTransitionStyle PageTransitionStyle { get; set; }
		public UIPageViewControllerNavigationOrientation PageNavigationOrientation { get; set; }
		public bool ToolbarVisible { get; set; }
		public bool SliderVisible { get; set; }
		public bool PageNumberVisible { get; set; }
		public AutoScaleModes AutoScaleMode { get; set; }

		public int ZoomScaleLevels {
			get {
				return _ZoomScaleLevels;
			}
			set {
				if (_ZoomScaleLevels != value) {
					_ZoomScaleLevels = value;
					if (_ZoomScaleLevels < MinZoomScaleLevels) {
						_ZoomScaleLevels = MinZoomScaleLevels;
					}
					if (_ZoomScaleLevels > MaxZoomScaleLevels) {
						_ZoomScaleLevels = MaxZoomScaleLevels;
					}
				}
			}
		}
		private int _ZoomScaleLevels;
		
		public bool AllowZoomByDoubleTouch { get; set; }

		public int ThumbsBufferSize {
			get {
				return _ThumbsBufferSize;
			}
			set {
				if (_ThumbsBufferSize != value) {
					_ThumbsBufferSize = value;
					if (_ThumbsBufferSize < MinThumbsBufferSize) {
						_ThumbsBufferSize = MinThumbsBufferSize;
					}
					if (_ThumbsBufferSize > MaxThumbsBufferSize) {
						_ThumbsBufferSize = MaxThumbsBufferSize;
					}
				}
			}
		}
		private int _ThumbsBufferSize;

		public int ThumbSize {
			get {
				return _ThumbSize;
			}
			set {
				if (_ThumbSize != value) {
					_ThumbSize = value;
					if (_ThumbSize < MinThumbSize) {
						_ThumbSize = MinThumbSize;
					}
					if (_ThumbSize > MaxThumbSize) {
						_ThumbSize = MaxThumbSize;
					}
				}
			}
		}
		private int _ThumbSize;

		public DateTime LibraryReleaseDate {
			get {
				return SystemInfo.ReleaseDate;
			}
		}

		public string LibraryVersion {
			get {
				return SystemInfo.Version;
			}
		}		
		#endregion	
		
		#region Logic		
		public Options()
		{
			PageTransitionStyle = DefaultPageTransitionStyle;
			PageNavigationOrientation = DefaultPageNavigationOrientation;
			ToolbarVisible = DefaultToolbarVisible;
			SliderVisible = DefaultStatusbarVisible;
			PageNumberVisible = DefaultPageNumberVisible;
			AutoScaleMode = DefaultAutoScaleMode;
			ZoomScaleLevels = DefaultZoomScaleLevels;
			AllowZoomByDoubleTouch = DefaultAllowZoomByDoubleTouch;
			_ThumbsBufferSize = DefaultThumbsBufferSize;
			_ThumbSize = DefaultThumbSize;
		}
		#endregion
	}
}

