//
// mTouch-PDFReader library
// ThumbView.cs
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
