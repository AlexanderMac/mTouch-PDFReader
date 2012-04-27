//****************************************//
// mTouch-PDFReader library
// Page content View 
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using System.Text;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.ObjCRuntime;

namespace mTouchPDFReader.Library.Views.Core
{
	public class PageContentView : UIView
	{			
		#region Fields
		
		/// <summary>
		/// Instruct that needed a CAGradientLayer (not the default CALayer) for the Layer property 
		/// </summary>
		[Export("layerClass")]
		public static Class LayerClass()
		{
			return new Class(typeof(PageContentTile));
		}
		
		/// <summary>
		/// Page number displayed in view
		/// </summary>
		private int mPageNumber;
		public int PageNumber {
			get { 
				return mPageNumber; 
			}
			set {
				mPageNumber = value;
			}
		}
		
		#endregion
		
		#region UIView methods
		
		/// <summary>
		/// Work constructor
		/// </summary>
		public PageContentView(RectangleF frame, int pageNumber) : base(frame)
		{
			mPageNumber = pageNumber;
			AutosizesSubviews = false;
			UserInteractionEnabled = false;
			ClearsContextBeforeDrawing = false;
			ContentMode = UIViewContentMode.Redraw;
			AutoresizingMask = UIViewAutoresizing.None;
			BackgroundColor = UIColor.Clear;
			(Layer as PageContentTile).OnDraw = Draw;
		}		
		
		/// <summary>
		/// Gets view size to contain page 
		/// </summary>
		/// <param name="pageNumber">Page number</param>
		/// <returns>Page rect/returns>
		public static RectangleF GetPageViewSize(int pageNumber)
		{
			RectangleF pageRect = RectangleF.Empty;
			if (PDFDocument.DocumentHasLoaded) {
				// Check the lower page bounds
				if (pageNumber < 1) {
					pageNumber = 1;
				}
				// Check the upper page bounds
				if (pageNumber > PDFDocument.PageCount) {
					pageNumber = PDFDocument.PageCount;
				}
				using (CGPDFPage pdfPage = PDFDocument.GetPage(pageNumber)) {
					if (pdfPage != null) {
						// Calc effective rect
						RectangleF cropBoxRect = pdfPage.GetBoxRect(CGPDFBox.Crop);
						RectangleF mediaBoxRect = pdfPage.GetBoxRect(CGPDFBox.Media);
						RectangleF effectiveRect = RectangleF.Intersect(cropBoxRect, mediaBoxRect);
			
						// Calc page width and height, accoring by rotation
						switch (pdfPage.RotationAngle) {
							default:
							case 0:
							case 180:
								pageRect.Width = effectiveRect.Size.Width;
								pageRect.Height = effectiveRect.Size.Height;
								break;
							case 90:
							case 270:
								pageRect.Height = effectiveRect.Size.Width;
								pageRect.Width = effectiveRect.Size.Height;
								break;
						}
						if (pageRect.Width % 2 > 0) {
							pageRect.Width--;
						}
						if (pageRect.Height % 2 > 0) {
							pageRect.Height--;
						}
					} 
				}
			}
			return pageRect;
		}
		
		/// <summary>
		/// Draws the document page
		/// </summary>		
		private void Draw(CGContext context)
		{
			if (!PDFDocument.DocumentHasLoaded) {
				return;
			}
			
			// Draw page
			context.SetFillColor(1.0f, 1.0f, 1.0f, 1.0f);
			using (CGPDFPage pdfPage = PDFDocument.GetPage(mPageNumber)) {
				context.TranslateCTM(0, Bounds.Height);
				context.ScaleCTM(1.0f, -1.0f);
				context.ConcatCTM(pdfPage.GetDrawingTransform(CGPDFBox.Crop, Bounds, 0, true));
				context.SetRenderingIntent(CGColorRenderingIntent.Default);
				context.InterpolationQuality = CGInterpolationQuality.Default;
				context.DrawPDFPage(pdfPage);
			}
		}		
		
		#endregion
	}
}
