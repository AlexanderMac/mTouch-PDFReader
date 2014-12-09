//
// mTouch-PDFReader library
// PageContentView.cs
//
// Copyright (c) 2012-2014 AlexanderMac(amatsibarov@gmail.com)
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

using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.ObjCRuntime;

namespace mTouchPDFReader.Library.Views.Core
{
	public class PageContentView : UIView
	{			
		#region Data		
		[Export("layerClass")]
		public static Class LayerClass()
		{
			return new Class(typeof(PageContentTile));
		}

		public int PageNumber {
			get { 
				return _pageNumber; 
			}
			set {
				_pageNumber = value;
			}
		}	
		private int _pageNumber;
		#endregion
		
		#region Logic		
		public PageContentView(RectangleF frame, int pageNumber) : base(frame)
		{
			_pageNumber = pageNumber;
			AutosizesSubviews = false;
			UserInteractionEnabled = false;
			ClearsContextBeforeDrawing = false;
			ContentMode = UIViewContentMode.Redraw;
			AutoresizingMask = UIViewAutoresizing.None;
			BackgroundColor = UIColor.Clear;
			(Layer as PageContentTile).OnDraw = draw;
		}		
		
		public static RectangleF GetPageViewSize(int pageNumber)
		{
			RectangleF pageRect = RectangleF.Empty;
			if (PDFDocument.DocumentHasLoaded) {
				if (pageNumber < 1) {
					pageNumber = 1;
				}

				if (pageNumber > PDFDocument.PageCount) {
					pageNumber = PDFDocument.PageCount;
				}

				using (CGPDFPage pdfPage = PDFDocument.GetPage(pageNumber)) {
					if (pdfPage != null) {
						RectangleF cropBoxRect = pdfPage.GetBoxRect(CGPDFBox.Crop);
						RectangleF mediaBoxRect = pdfPage.GetBoxRect(CGPDFBox.Media);
						RectangleF effectiveRect = RectangleF.Intersect(cropBoxRect, mediaBoxRect);
			
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
		
		private void draw(CGContext context)
		{
			if (!PDFDocument.DocumentHasLoaded) {
				return;
			}

			context.SetFillColor(1.0f, 1.0f, 1.0f, 1.0f);
			using (CGPDFPage pdfPage = PDFDocument.GetPage(_pageNumber)) {
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
