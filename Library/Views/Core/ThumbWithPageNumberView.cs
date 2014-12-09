//
// mTouch-PDFReader library
// ThumbWithPageNumberView.cs
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

using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace mTouchPDFReader.Library.Views.Core
{
	public class ThumbWithPageNumberView : UIView
	{
		#region Data		
		private const int Padding = 5;
		private const int PageNumberLabelHeight = 20;		

		public int PageNumber {
			get { 
				return _thumbView.PageNumber; 
			}
			set {
				_thumbView.PageNumber = value;
				_pageNumberLabel.Text = value.ToString();
			}
		}
		
		private readonly ThumbView _thumbView;	
		private readonly UILabel _pageNumberLabel;
		private readonly Action<ThumbWithPageNumberView> _thumbSelectedCallback;				
		#endregion
		
		#region UIView members		
		public ThumbWithPageNumberView(RectangleF frame, int pageNumber, Action<ThumbWithPageNumberView> thumbSelectedCallback) : base(frame)
		{
			_thumbSelectedCallback = thumbSelectedCallback;
			SetAsUnselected();
			
			int thumbContentSize = (int)Bounds.Size.Width;
			var thumbViewFrame = new RectangleF(0, Padding, Bounds.Size.Width, Bounds.Size.Height - 2 * Padding - PageNumberLabelHeight);
			_thumbView = new ThumbView(thumbViewFrame, thumbContentSize, pageNumber);
			
			var pageNumberLabelFrame = new RectangleF(0, 2 * Padding + thumbViewFrame.Height, Bounds.Size.Width, PageNumberLabelHeight);
			_pageNumberLabel = new UILabel(pageNumberLabelFrame);
			_pageNumberLabel.TextAlignment = UITextAlignment.Center;
			_pageNumberLabel.BackgroundColor = UIColor.Clear;
			_pageNumberLabel.TextColor = UIColor.White;
			_pageNumberLabel.Font = UIFont.SystemFontOfSize(16.0f);
			_pageNumberLabel.ShadowOffset = new SizeF(0.0f, 1.0f);
			_pageNumberLabel.ShadowColor = UIColor.Black;
			_pageNumberLabel.AdjustsFontSizeToFitWidth = true;
			_pageNumberLabel.Text = pageNumber.ToString();
			
			AddSubview(_thumbView);
			AddSubview(_pageNumberLabel);
		}
			
		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			_thumbSelectedCallback(this);
		} 		
		#endregion
		
		#region Logic		
		public void SetAsSelected()
		{
			BackgroundColor = UIColor.Blue;
		}
		
		public void SetAsUnselected()
		{
			BackgroundColor = UIColor.LightGray;
		}		
		#endregion
	}
}
