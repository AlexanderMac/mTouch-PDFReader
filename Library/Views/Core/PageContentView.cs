//
// mTouch-PDFReader library
// PageContentView.cs (Page content View )
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
		/// Gets the layer class.
		/// </summary>
		[Export("layerClass")]
		public static Class LayerClass()
		{
			return new Class(typeof(PageContentTile));
		}
		
		/// <summary>
		/// Gets or sets the opened page number.
		/// </summary>
		public int PageNumber {
			get { 
				return _PageNumber; 
			}
			set {
				_PageNumber = value;
			}
		}	
		private int _PageNumber;
		#endregion
		
		#region UIView methods		
		/// <summary>
		/// Working.
		/// </summary>
		public PageContentView(RectangleF frame, int pageNumber) : base(frame)
		{
			_PageNumber = pageNumber;
			AutosizesSubviews = false;
			UserInteractionEnabled = false;
			ClearsContextBeforeDrawing = false;
			ContentMode = UIViewContentMode.Redraw;
			AutoresizingMask = UIViewAutoresizing.None;
			BackgroundColor = UIColor.Clear;
			(Layer as PageContentTile).OnDraw = Draw;
		}		
		
		/// <summary>
		/// Gets the view size for the opened PDF page.
		/// </summary>
		/// <param name="pageNumber">The page number,</param>
		/// <returns>The page view rect.</returns>
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
		/// Draws the page.
		/// </summary>		
		private void Draw(CGContext context)
		{
			if (!PDFDocument.DocumentHasLoaded) {
				return;
			}

			// Draw page
			context.SetFillColor(1.0f, 1.0f, 1.0f, 1.0f);
			using (CGPDFPage pdfPage = PDFDocument.GetPage(_PageNumber)) {
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
