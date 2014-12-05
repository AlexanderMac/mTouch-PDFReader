//
// mTouch-PDFReader library
// ThumbView.cs
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

using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace mTouchPDFReader.Library.Views.Core
{
	public class ThumbView : UIView
	{							
		#region Data		
		public int PageNumber {
			get { 
				return _pageNumber; 
			}
			set {
				if (value != _pageNumber) {
					_pageNumber = value;
					_imageView.Image = GetThumbImage(_thumbContentSize, _pageNumber);
				}
			}
		}
		private int _pageNumber;
		
		private readonly float _thumbContentSize;
		private readonly UIImageView _imageView;		
		#endregion
		
		#region UIView members		
		public ThumbView(RectangleF frame, float thumbContentSize, int pageNumber) : base(frame)
		{
			_pageNumber = pageNumber;			
			_thumbContentSize = thumbContentSize;
			
			AutosizesSubviews = false;
			UserInteractionEnabled = false;
			ContentMode = UIViewContentMode.Redraw;
			AutoresizingMask = UIViewAutoresizing.None;
			BackgroundColor = UIColor.Clear;
			
			_imageView = new UIImageView(Bounds);
			_imageView.AutosizesSubviews = false;
			_imageView.UserInteractionEnabled = false;
			_imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			_imageView.AutoresizingMask = UIViewAutoresizing.None;
			_imageView.BackgroundColor = UIColor.Clear;
			_imageView.ClipsToBounds = true;
			_imageView.Image = GetThumbImage(_thumbContentSize, _pageNumber);

			AddSubview(_imageView);
		}				
		#endregion
		
		#region Logic		
		private static UIImage GetThumbImage(float thumbContentSize, int pageNumber)
		{
			if ((pageNumber <= 0) || (pageNumber > PDFDocument.PageCount)) {
				return null;
			}
			
			var pageSize = PageContentView.GetPageViewSize(pageNumber);
			if (pageSize.Width % 2 > 0) {
				pageSize.Width--;
			}
			if (pageSize.Height % 2 > 0) {
				pageSize.Height--;
			}
			
			var targetSize = new Size((int)pageSize.Width, (int)pageSize.Height);
			
			CGImage pageImage;
			using (CGColorSpace rgb = CGColorSpace.CreateDeviceRGB()) {
				using (var context = new CGBitmapContext(null, targetSize.Width, targetSize.Height, 8, 0, rgb, CGBitmapFlags.ByteOrder32Little | CGBitmapFlags.NoneSkipFirst)) {
					using (var pdfPage = PDFDocument.GetPage(pageNumber)) {
						var thumbRect = new RectangleF(0.0f, 0.0f, targetSize.Width, targetSize.Height);
						context.SetFillColor(1.0f, 1.0f, 1.0f, 1.0f);
						context.FillRect(thumbRect);
						context.ConcatCTM(pdfPage.GetDrawingTransform(CGPDFBox.Crop, thumbRect, 0, true));
						context.SetRenderingIntent(CGColorRenderingIntent.Default);
						context.InterpolationQuality = CGInterpolationQuality.Default;
						context.DrawPDFPage(pdfPage);

						pageImage = context.ToImage();
					}
				}
			}			
			return UIImage.FromImage(pageImage);			
		}		
		#endregion
	}
}
