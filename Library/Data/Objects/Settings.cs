//
// mTouch-PDFReader library
// Settings.cs
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
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Data.Enums;

namespace mTouchPDFReader.Library.Data.Objects
{
	public class Settings
	{
		#region Constants	
		private const UIPageViewControllerTransitionStyle DefaultPageTransitionStyle = UIPageViewControllerTransitionStyle.PageCurl;
		private const UIPageViewControllerNavigationOrientation DefaultPageNavigationOrientation = UIPageViewControllerNavigationOrientation.Horizontal;

		private const bool DefaultTopToolbarVisible = true;
		private const bool DefaultBottomToolbarVisible = true;

		private const AutoScaleModes DefaultAutoScaleMode = AutoScaleModes.AutoHeight;
		private const int DefaultZoomScaleLevels = 10;
		private const bool DefaultAllowZoomByDoubleTouch = true;
		private const int DefaultThumbsBufferSize = 20;
		private const int DefaultThumbSize = 125;		
	
		public const int MinZoomScaleLevels = 2;
		public const int MaxZoomScaleLevels = 10;
		public const int MinThumbsBufferSize = 15;
		public const int MaxThumbsBufferSize = 100;
		public const int MinThumbSize = 100;
		public const int MaxThumbSize = 200;	
		#endregion
		
		#region Data
		public UIPageViewControllerTransitionStyle PageTransitionStyle { get; set; }
		public UIPageViewControllerNavigationOrientation PageNavigationOrientation { get; set; }
		public bool TopToolbarVisible { get; set; }
		public bool BottomToolbarVisible { get; set; }
		public AutoScaleModes AutoScaleMode { get; set; }

		public int ZoomScaleLevels {
			get {
				return _zoomScaleLevels;
			}
			set {
				if (_zoomScaleLevels != value) {
					_zoomScaleLevels = value;
					if (_zoomScaleLevels < MinZoomScaleLevels) {
						_zoomScaleLevels = MinZoomScaleLevels;
					}
					if (_zoomScaleLevels > MaxZoomScaleLevels) {
						_zoomScaleLevels = MaxZoomScaleLevels;
					}
				}
			}
		}
		private int _zoomScaleLevels;
		
		public bool AllowZoomByDoubleTouch { get; set; }

		public int ThumbsBufferSize {
			get {
				return _thumbsBufferSize;
			}
			set {
				if (_thumbsBufferSize != value) {
					_thumbsBufferSize = value;
					if (_thumbsBufferSize < MinThumbsBufferSize) {
						_thumbsBufferSize = MinThumbsBufferSize;
					}
					if (_thumbsBufferSize > MaxThumbsBufferSize) {
						_thumbsBufferSize = MaxThumbsBufferSize;
					}
				}
			}
		}
		private int _thumbsBufferSize;

		public int ThumbSize {
			get {
				return _thumbSize;
			}
			set {
				if (_thumbSize != value) {
					_thumbSize = value;
					if (_thumbSize < MinThumbSize) {
						_thumbSize = MinThumbSize;
					}
					if (_thumbSize > MaxThumbSize) {
						_thumbSize = MaxThumbSize;
					}
				}
			}
		}
		private int _thumbSize;

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
		public Settings()
		{
			PageTransitionStyle = DefaultPageTransitionStyle;
			PageNavigationOrientation = DefaultPageNavigationOrientation;
			TopToolbarVisible = DefaultTopToolbarVisible;
			BottomToolbarVisible = DefaultBottomToolbarVisible;
			AutoScaleMode = DefaultAutoScaleMode;
			ZoomScaleLevels = DefaultZoomScaleLevels;
			AllowZoomByDoubleTouch = DefaultAllowZoomByDoubleTouch;
			_thumbsBufferSize = DefaultThumbsBufferSize;
			_thumbSize = DefaultThumbSize;
		}
		#endregion
	}
}

