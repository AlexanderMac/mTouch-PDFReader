//
// mTouch-PDFReader library
// Settings.cs
//
// Copyright (c) 2012-2014 Alexander Matsibarov (amatsibarov@gmail.com)
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
			_thumbSize = DefaultThumbSize;
		}
		#endregion
	}
}

