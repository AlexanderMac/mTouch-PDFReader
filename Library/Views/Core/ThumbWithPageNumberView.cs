//
// mTouch-PDFReader library
// ThumbWithPageNumberView.cs
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
			_pageNumberLabel.MinimumFontSize = 12.0f;
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
			BackgroundColor = UIColor.Blue; // ToDo: Use gradient color or view
		}
		
		public void SetAsUnselected()
		{
			BackgroundColor = UIColor.LightGray;
		}		
		#endregion
	}
}
