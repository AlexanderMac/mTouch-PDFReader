//****************************************//
// mTouch-PDFReader library
// Options
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using System.Xml;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Data.Enums;

namespace mTouchPDFReader.Library.Data.Objects
{
	public class Options
	{
		#region Constants Default
		
		private const PageTurningType DefaultPageTurningType = PageTurningType.Vertical;
		
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

		private PageTurningType mPageTurningType;
		public PageTurningType pPageTurningType {
			get { return mPageTurningType; }
			set {
				if (mPageTurningType != value) {
					mPageTurningType = value;
					SetModified();
				}
			}
		}
		
		private bool mToolbarVisible;
		public bool ToolbarVisible {
			get { return mToolbarVisible; }
			set {
				if (mToolbarVisible != value) {
					mToolbarVisible = value;
					SetModified();
				}
			}
		}

		private bool mBottombarVisible;
		public bool BottombarVisible {
			get { return mBottombarVisible; }
			set {
				if (mBottombarVisible != value) {
					mBottombarVisible = value;
					SetModified();
				}
			}
		}

		private bool mPageNumberVisible;
		public bool PageNumberVisible {
			get { return mPageNumberVisible; }
			set {
				if (mPageNumberVisible != value) {
					mPageNumberVisible = value;
					SetModified();
				}
			}
		}

		private bool mNoteBtnVisible;
		public bool NoteBtnVisible {
			get { return mNoteBtnVisible; }
			set {
				if (mNoteBtnVisible != value) {
					mNoteBtnVisible = value;
					SetModified();
				}
			}
		}

		private bool mBookmarksBtnVisible;
		public bool BookmarksBtnVisible {
			get { return mBookmarksBtnVisible; }
			set {
				if (mBookmarksBtnVisible != value) {
					mBookmarksBtnVisible = value;
					SetModified();
				}
			}
		}

		private bool mThumbsBtnVisible;
		public bool ThumbsBtnVisible {
			get { return mThumbsBtnVisible; }
			set {
				if (mThumbsBtnVisible != value) {
					mThumbsBtnVisible = value;
					SetModified();
				}
			}
		}

		private UIColor mBackgroundColor;
		public UIColor BackgroundColor {
			get { return mBackgroundColor; }
			set {
				if (mBackgroundColor != value) {
					mBackgroundColor = value;
					SetModified();
				}
			}
		}

		private int mZoomScaleLevels;
		public int ZoomScaleLevels {
			get { return mZoomScaleLevels; }
			set {
				if (mZoomScaleLevels != value) {
					mZoomScaleLevels = value;
					if (mZoomScaleLevels < MinZoomScaleLevels) {
						mZoomScaleLevels = MinZoomScaleLevels;
					}
					if (mZoomScaleLevels > MaxZoomScaleLevels) {
						mZoomScaleLevels = MaxZoomScaleLevels;
					}
					SetModified();
				}
			}
		}
		
		private bool mAllowZoomByDoubleTouch;
		public bool AllowZoomByDoubleTouch {
			get { return mAllowZoomByDoubleTouch; }
			set {
				if (mAllowZoomByDoubleTouch != value) {
					mAllowZoomByDoubleTouch = value;
					SetModified();
				}
			}
		}

		private int mThumbsBufferSize;
		public int ThumbsBufferSize {
			get { return mThumbsBufferSize; }
			set {
				if (mThumbsBufferSize != value) {
					mThumbsBufferSize = value;
					if (mThumbsBufferSize < MinThumbsBufferSize) {
						mThumbsBufferSize = MinThumbsBufferSize;
					}
					if (mThumbsBufferSize > MaxThumbsBufferSize) {
						mThumbsBufferSize = MaxThumbsBufferSize;
					}
					SetModified();
				}
			}
		}

		private int mThumbSize;
		public int ThumbSize {
			get { return mThumbSize; }
			set {
				if (mThumbSize != value) {
					mThumbSize = value;
					if (mThumbSize < MinThumbSize) {
						mThumbSize = MinThumbSize;
					}
					if (mThumbSize > MaxThumbSize) {
						mThumbSize = MaxThumbSize;
					}
					SetModified();
				}
			}
		}

		public DateTime LibraryReleaseDate {
			get { return SystemInfo.ReleaseDate; }
		}

		public string LibraryVersion {
			get { return SystemInfo.Version; }
		}
		
		#endregion	
		
		#region Logic
		
		/// <summary>
		/// default constructor
		/// </summary>
		public Options()
		{
			mPageTurningType = DefaultPageTurningType;
			mToolbarVisible = DefaultToolbarVisible;
			mBottombarVisible = DefaultStatusbarVisible;
			mPageNumberVisible = DefaultPageNumberVisible;
			mNoteBtnVisible = DefaultNoteBtnVisible;
			mBookmarksBtnVisible = DefaultBookmarksBtnVisible;
			mThumbsBtnVisible = DefaultThumbsBtnVisible;
			mBackgroundColor = DefaultBackgroundColor;
			mZoomScaleLevels = DefaultZoomScaleLevels;
			mAllowZoomByDoubleTouch = DefaultAllowZoomByDoubleTouch;
			mThumbsBufferSize = DefaultThumbsBufferSize;
			mThumbSize = DefaultThumbSize;
		}
		
		public void SetModified()
		{
			//??
		}
		
		#endregion
	}
}

