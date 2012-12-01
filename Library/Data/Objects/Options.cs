//
// mTouch-PDFReader library
// Options.cs (DOptions)
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
using System.Xml;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Data.Enums;

namespace mTouchPDFReader.Library.Data.Objects
{
	public class Options
	{
		#region Default constants		
		private const PageTurningTypes DefaultPageTurningType = PageTurningTypes.Horizontal;
		private const bool DefaultToolbarVisible = true;
		private const bool DefaultStatusbarVisible = true;
		private const bool DefaultPageNumberVisible = true;
		private const bool DefaultNoteBtnVisible = true;
		private const bool DefaultBookmarksBtnVisible = true;
		private const bool DefaultThumbsBtnVisible = true;
		private readonly UIColor DefaultBackgroundColor = UIColor.ScrollViewTexturedBackgroundColor;
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
		/// <summary>
		/// Gets or sets the type of the page turning.
		/// </summary>
		public PageTurningTypes PageTurningType {
			get {
				return _PageTurningType;
			}
			set {
				if (_PageTurningType != value) {
					_PageTurningType = value;
				}
			}
		}
		private PageTurningTypes _PageTurningType;
		
		/// <summary>
		/// Gets or sets a value indicating whether the toolbar is visible.
		/// </summary>
		public bool ToolbarVisible {
			get {
				return _ToolbarVisible;
			}
			set {
				if (_ToolbarVisible != value) {
					_ToolbarVisible = value;
				}
			}
		}
		private bool _ToolbarVisible;

		/// <summary>
		/// Gets or sets a value indicating whether the bottombar is visible.
		/// </summary>
		public bool BottombarVisible {
			get {
				return _BottombarVisible;
			}
			set {
				if (_BottombarVisible != value) {
					_BottombarVisible = value;
				}
			}
		}
		private bool _BottombarVisible;

		/// <summary>
		/// Gets or sets a value indicating whether the pageNumber label is visible.
		/// number visible.
		/// </summary>
		public bool PageNumberVisible {
			get {
				return _PageNumberVisible;
			}
			set {
				if (_PageNumberVisible != value) {
					_PageNumberVisible = value;
				}
			}
		}
		private bool _PageNumberVisible;

		/// <summary>
		/// Gets or sets a value indicating whether the Note button is visible.
		/// </summary>
		/// <value>
		public bool NoteBtnVisible {
			get {
				return _NoteBtnVisible;
			}
			set {
				if (_NoteBtnVisible != value) {
					_NoteBtnVisible = value;
				}
			}
		}
		private bool _NoteBtnVisible;

		/// <summary>
		/// Gets or sets a value indicating whether the Bookmarks button is visible.
		/// </summary>
		/// <value>
		public bool BookmarksBtnVisible {
			get {
				return _BookmarksBtnVisible;
			}
			set {
				if (_BookmarksBtnVisible != value) {
					_BookmarksBtnVisible = value;
				}
			}
		}
		private bool _BookmarksBtnVisible;

		/// <summary>
		/// Gets or sets a value indicating whether the Thumbs button is visible.
		/// </summary>
		/// <value>
		public bool ThumbsBtnVisible {
			get {
				return _ThumbsBtnVisible;
			}
			set {
				if (_ThumbsBtnVisible != value) {
					_ThumbsBtnVisible = value;
				}
			}
		}
		private bool _ThumbsBtnVisible;

		/// <summary>
		/// Gets or sets the color of the background.
		/// </summary>
		/// <value>
		public UIColor BackgroundColor {
			get {
				return _BackgroundColor;
			}
			set {
				if (_BackgroundColor != value) {
					_BackgroundColor = value;
				}
			}
		}
		private UIColor _BackgroundColor;

		/// <summary>
		/// Gets or sets the zoom scale levels.
		/// </summary>
		/// <value>
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
		
		/// <summary>
		/// Gets or sets a value indicating whether the double touch is allowed.
		/// by double touch.
		/// </summary>
		public bool AllowZoomByDoubleTouch {
			get {
				return _AllowZoomByDoubleTouch;
			}
			set {
				if (_AllowZoomByDoubleTouch != value) {
					_AllowZoomByDoubleTouch = value;
				}
			}
		}
		private bool _AllowZoomByDoubleTouch;

		/// <summary>
		/// Gets or sets the size of the thumbs buffer.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the size of the thumb.
		/// </summary>
		/// <value>
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

		/// <summary>
		/// Gets the library release date.
		/// </summary>
		/// <value>
		public DateTime LibraryReleaseDate {
			get {
				return SystemInfo.ReleaseDate;
			}
		}

		/// <summary>
		/// Gets the library version.
		/// </summary>
		/// <value>
		public string LibraryVersion {
			get {
				return SystemInfo.Version;
			}
		}		
		#endregion	
		
		#region Logic		
		/// <summary>
		/// Working.
		/// </summary>
		public Options()
		{
			_PageTurningType = DefaultPageTurningType;
			_ToolbarVisible = DefaultToolbarVisible;
			_BottombarVisible = DefaultStatusbarVisible;
			_PageNumberVisible = DefaultPageNumberVisible;
			_NoteBtnVisible = DefaultNoteBtnVisible;
			_BookmarksBtnVisible = DefaultBookmarksBtnVisible;
			_ThumbsBtnVisible = DefaultThumbsBtnVisible;
			_BackgroundColor = DefaultBackgroundColor;
			_ZoomScaleLevels = DefaultZoomScaleLevels;
			_AllowZoomByDoubleTouch = DefaultAllowZoomByDoubleTouch;
			_ThumbsBufferSize = DefaultThumbsBufferSize;
			_ThumbSize = DefaultThumbSize;
		}
		#endregion
	}
}

