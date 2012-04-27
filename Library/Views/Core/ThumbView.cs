//****************************************//
// mTouch-PDFReader library
// Page content thumb View 
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace mTouchPDFReader.Library.Views.Core
{
	public class ThumbView : UIView
	{							
		#region Fields
		
		/// <summary>
		/// Page number displayed in thumb
		/// </summary>
		private int mPageNumber;
		public int PageNumber {
			get { 
				return mPageNumber; 
			}
			set {
				if (value != mPageNumber) {
					mPageNumber = value;
					mImageView.Image = GetThumbImage(mThumbContentSize, mPageNumber);
				}
			}
		}
		
		/// <summary>
		/// Thumb content size
		/// </summary>
		private float mThumbContentSize;
		
		/// <summary>
		/// Thumb image view
		/// </summary>
		private UIImageView mImageView;
		
		#endregion
		
		#region UIView methods
		
		/// <summary>
		/// Work constructor
		/// </summary>
		public ThumbView(RectangleF frame, float thumbContentSize, int pageNumber) : base(frame)
		{
			mPageNumber = pageNumber;			
			mThumbContentSize = thumbContentSize;
			
			AutosizesSubviews = false;
			UserInteractionEnabled = false;
			ContentMode = UIViewContentMode.Redraw;
			AutoresizingMask = UIViewAutoresizing.None;
			BackgroundColor = UIColor.Clear;
			
			// Create and init thumb image view
			mImageView = new UIImageView(Bounds);
			mImageView.AutosizesSubviews = false;
			mImageView.UserInteractionEnabled = false;
			mImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			mImageView.AutoresizingMask = UIViewAutoresizing.None;
			mImageView.BackgroundColor = UIColor.Clear;
			mImageView.ClipsToBounds = true;
			mImageView.Image = GetThumbImage(mThumbContentSize, mPageNumber);

			AddSubview(mImageView);
		}		
		
		#endregion
		
		#region Logic
		
		/// <summary>
		/// Returns thumb image object for page 
		/// </summary>
		/// <param name="thumbContentSize">Thumb content size</param>
		/// <param name="pageNumber">Page number for what will created image object</param>
		/// <returns>Page image object</returns>
		private static UIImage GetThumbImage(float thumbContentSize, int pageNumber)
		{
			if ((pageNumber <= 0) || (pageNumber > PDFDocument.PageCount)) {
				return null;
			}
			
			// Calc page view size
			var pageSize = PageContentView.GetPageViewSize(pageNumber);
			
			// Calc scale factor to fit page on thumb image
			float scaleByWidth = (thumbContentSize / pageSize.Width);
			float scaleByHeight = (thumbContentSize / pageSize.Height);
			float scale = 0.0f;
			if (pageSize.Height > pageSize.Width) {
				// For Portrait
				scale = scaleByHeight;
			} else {
				// For Landscape
				scale = scaleByWidth;
			}
			
			// Calc target size	
			var targetSize = new Size((int)(pageSize.Width * scale), (int)(pageSize.Height * scale));
			if (targetSize.Width % 2 > 0) {
				targetSize.Width--;
			}
			if (targetSize.Height % 2 > 0) {
				targetSize.Height--;
			}
			targetSize.Width *= (int)UIScreen.MainScreen.Scale;
			targetSize.Height *= (int)UIScreen.MainScreen.Scale;
			
			// Draw page on CGImage
			CGImage pageImage;
			using (CGColorSpace rgb = CGColorSpace.CreateDeviceRGB()) {
				using (CGBitmapContext context = new CGBitmapContext(null, targetSize.Width, targetSize.Height, 8, 0, rgb, CGBitmapFlags.ByteOrder32Little | CGBitmapFlags.NoneSkipFirst)) {
					using (CGPDFPage pdfPage = PDFDocument.GetPage(pageNumber)) {
						// Draw page on custom CGBitmap context
						RectangleF thumbRect = new RectangleF(0.0f, 0.0f, targetSize.Width, targetSize.Height);
						context.SetFillColor(1.0f, 1.0f, 1.0f, 1.0f);
						context.FillRect(thumbRect);
						context.ConcatCTM(pdfPage.GetDrawingTransform(CGPDFBox.Crop, thumbRect, 0, true));
						context.SetRenderingIntent(CGColorRenderingIntent.Default);
						context.InterpolationQuality = CGInterpolationQuality.Default;
						context.DrawPDFPage(pdfPage);
						// Create CGImage from custom CGBitmap context
						pageImage = context.ToImage();
					}
				}
			}			
			return UIImage.FromImage(pageImage);			
		}
		
		#endregion
	}
}
