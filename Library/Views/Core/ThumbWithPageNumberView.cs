//****************************************//
// mTouch-PDFReader library
// Page content thumb view with page number label
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace mTouchPDFReader.Library.Views.Core
{
	public class ThumbWithPageNumberView : UIView
	{
		#region Constants
		
		/// <summary>
		/// Padding beetwen views
		/// </summary>
		private const int Padding = 5;
		
		/// <summary>
		/// Page number view label height
		/// </summary>
		private const int PageNumberLabelHeight = 20;
		
		#endregion
			
		#region Fields
		
		/// <summary>
		/// Page number displayed in thumb
		/// </summary>
		public int PageNumber {
			get { 
				return mThumbView.PageNumber; 
			}
			set {
				mThumbView.PageNumber = value;
				mPageNumberLabel.Text = value.ToString();
			}
		}
		
		/// <summary>
		/// Page thumb view
		/// </summary>
		private ThumbView mThumbView;	
		
		/// <summary>
		/// Page number label
		/// </summary>
		private UILabel mPageNumberLabel;
		
		/// <summary>
		/// Callback called when user clicked by thumb
		/// </summary>
		private Action<ThumbWithPageNumberView> mThumbSelectedCallback;		
		
		#endregion
		
		#region UIView methods
		
		/// <summary>
		/// Work constructor
		/// </summary>
		/// <param name="frame">Thumb frame</param>
		/// <param name="pageNumber">Page number</param>
		/// <param name="thumbSelectedCallback"></param>
		public ThumbWithPageNumberView(RectangleF frame, int pageNumber, Action<ThumbWithPageNumberView> thumbSelectedCallback) : base(frame)
		{
			mThumbSelectedCallback = thumbSelectedCallback;
			SetAsUnselected();
			
			// Create and init page thumb view
			int thumbContentSize = (int)Bounds.Size.Width;
			RectangleF thumbViewFrame = new RectangleF(0, Padding, Bounds.Size.Width, Bounds.Size.Height - 2 * Padding - PageNumberLabelHeight);
			mThumbView = new ThumbView(thumbViewFrame, thumbContentSize, pageNumber);
			
			// Create and init page number label		
			var pageNumberLabelFrame = new RectangleF(0, 2 * Padding + thumbViewFrame.Height, Bounds.Size.Width, PageNumberLabelHeight);
			mPageNumberLabel = new UILabel(pageNumberLabelFrame);
			mPageNumberLabel.TextAlignment = UITextAlignment.Center;
			mPageNumberLabel.BackgroundColor = UIColor.Clear;
			mPageNumberLabel.TextColor = UIColor.White;
			mPageNumberLabel.Font = UIFont.SystemFontOfSize(16.0f);
			mPageNumberLabel.ShadowOffset = new SizeF(0.0f, 1.0f);
			mPageNumberLabel.ShadowColor = UIColor.Black;
			mPageNumberLabel.AdjustsFontSizeToFitWidth = true;
			mPageNumberLabel.MinimumFontSize = 12.0f;
			mPageNumberLabel.Text = pageNumber.ToString();
			
			AddSubview(mThumbView);
			AddSubview(mPageNumberLabel);
		}
			
		/// <summary>
		/// Calls when user clicks by thumb
		/// </summary>
		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			mThumbSelectedCallback(this);
		} 
		
		#endregion
		
		#region Logic
		
		/// <summary>
		/// Marks thumb as selected 
		/// </summary>
		public void SetAsSelected()
		{
			BackgroundColor = UIColor.Blue; // ToDo: Use gradient color or view
		}
		
		/// <summary>
		/// Marks thumb as unselected 
		/// </summary>
		public void SetAsUnselected()
		{
			BackgroundColor = UIColor.LightGray;
		}
		
		#endregion
	}
}
