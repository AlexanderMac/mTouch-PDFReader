//
// mTouch-PDFReader library
// ThumbWithPageNumberView.cs (Page content thumb view with page number label
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

using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace mTouchPDFReader.Library.Views.Core
{
	public class ThumbWithPageNumberView : UIView
	{
		#region Constants		
		private const int Padding = 5;
		private const int PageNumberLabelHeight = 20;		
		#endregion
			
		#region Fields
		public int PageNumber {
			get { 
				return _ThumbView.PageNumber; 
			}
			set {
				_ThumbView.PageNumber = value;
				_PageNumberLabel.Text = value.ToString();
			}
		}
		
		private readonly ThumbView _ThumbView;	
		private readonly UILabel _PageNumberLabel;
		private readonly Action<ThumbWithPageNumberView> _ThumbSelectedCallback;				
		#endregion
		
		#region UIView methods		
		public ThumbWithPageNumberView(RectangleF frame, int pageNumber, Action<ThumbWithPageNumberView> thumbSelectedCallback) : base(frame)
		{
			_ThumbSelectedCallback = thumbSelectedCallback;
			SetAsUnselected();
			
			// Create and init page thumb view
			int thumbContentSize = (int)Bounds.Size.Width;
			var thumbViewFrame = new RectangleF(0, Padding, Bounds.Size.Width, Bounds.Size.Height - 2 * Padding - PageNumberLabelHeight);
			_ThumbView = new ThumbView(thumbViewFrame, thumbContentSize, pageNumber);
			
			// Create and init page number label		
			var pageNumberLabelFrame = new RectangleF(0, 2 * Padding + thumbViewFrame.Height, Bounds.Size.Width, PageNumberLabelHeight);
			_PageNumberLabel = new UILabel(pageNumberLabelFrame);
			_PageNumberLabel.TextAlignment = UITextAlignment.Center;
			_PageNumberLabel.BackgroundColor = UIColor.Clear;
			_PageNumberLabel.TextColor = UIColor.White;
			_PageNumberLabel.Font = UIFont.SystemFontOfSize(16.0f);
			_PageNumberLabel.ShadowOffset = new SizeF(0.0f, 1.0f);
			_PageNumberLabel.ShadowColor = UIColor.Black;
			_PageNumberLabel.AdjustsFontSizeToFitWidth = true;
			_PageNumberLabel.MinimumFontSize = 12.0f;
			_PageNumberLabel.Text = pageNumber.ToString();
			
			AddSubview(_ThumbView);
			AddSubview(_PageNumberLabel);
		}
			
		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			_ThumbSelectedCallback(this);
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
