//
// mTouch-PDFReader library
// ThumbView.cs (Page content thumb View)
//
//  Author:
//       Alexander Matsibarov (macasun) <amatsibarov@gmail.com>
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
		#region Fields		
		public int PageNumber {
			get { 
				return _PageNumber; 
			}
			set {
				if (value != _PageNumber) {
					_PageNumber = value;
					_ImageView.Image = GetThumbImage(_ThumbContentSize, _PageNumber);
				}
			}
		}
		private int _PageNumber;
		
		private readonly float _ThumbContentSize;
		private readonly UIImageView _ImageView;		
		#endregion
		
		#region UIView methods		
		public ThumbView(RectangleF frame, float thumbContentSize, int pageNumber) : base(frame)
		{
			_PageNumber = pageNumber;			
			_ThumbContentSize = thumbContentSize;
			
			AutosizesSubviews = false;
			UserInteractionEnabled = false;
			ContentMode = UIViewContentMode.Redraw;
			AutoresizingMask = UIViewAutoresizing.None;
			BackgroundColor = UIColor.Clear;
			
			// Create and init thumb image view
			_ImageView = new UIImageView(Bounds);
			_ImageView.AutosizesSubviews = false;
			_ImageView.UserInteractionEnabled = false;
			_ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			_ImageView.AutoresizingMask = UIViewAutoresizing.None;
			_ImageView.BackgroundColor = UIColor.Clear;
			_ImageView.ClipsToBounds = true;
			_ImageView.Image = GetThumbImage(_ThumbContentSize, _PageNumber);

			AddSubview(_ImageView);
		}				
		#endregion
		
		#region Logic		
		private static UIImage GetThumbImage(float thumbContentSize, int pageNumber)
		{
			if ((pageNumber <= 0) || (pageNumber > PDFDocument.PageCount)) {
				return null;
			}
			
			// Calc page view size
			var pageSize = PageContentView.GetPageViewSize(pageNumber);
			if (pageSize.Width % 2 > 0) {
				pageSize.Width--;
			}
			if (pageSize.Height % 2 > 0) {
				pageSize.Height--;
			}
			
			// Calc target size	
			var targetSize = new Size((int)pageSize.Width, (int)pageSize.Height);
			
			// Draw page on CGImage
			CGImage pageImage;
			using (CGColorSpace rgb = CGColorSpace.CreateDeviceRGB()) {
				using (var context = new CGBitmapContext(null, targetSize.Width, targetSize.Height, 8, 0, rgb, CGBitmapFlags.ByteOrder32Little | CGBitmapFlags.NoneSkipFirst)) {
					using (var pdfPage = PDFDocument.GetPage(pageNumber)) {
						// Draw page on custom CGBitmap context
						var thumbRect = new RectangleF(0.0f, 0.0f, targetSize.Width, targetSize.Height);
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
