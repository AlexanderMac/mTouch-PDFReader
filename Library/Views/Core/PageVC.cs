//
// mTouch-PDFReader library
// PageVC.cs
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
using mTouchPDFReader.Library.Data.Enums;

namespace mTouchPDFReader.Library.Views.Core
{
	public class PageVC : UIViewController
	{
		#region Data
		private readonly RectangleF _viewFrame;
		private readonly AutoScaleModes _autoScaleMode;
		public int PageNumber { get; private set; }

		public PageView PageView {
			get {
				return View as PageView;
			}
		}

		public bool IsNotEmptyPage {
			get {
				return PageView != null;
			}
		}
		#endregion

		#region Logic
		public PageVC(IntPtr handle) : base(handle)
		{
		}
		
		[Export("initWithCoder:")]
		public PageVC(NSCoder coder) : base(coder)
		{
		}
		
		public PageVC(RectangleF viewFrame, AutoScaleModes autoScaleMode, int pageNumber) : base(null, null)
		{
			_viewFrame = viewFrame;
			_autoScaleMode = autoScaleMode;
			PageNumber = pageNumber;
		}	

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			if (PageNumber == -1) {
				View.BackgroundColor = UIColor.Clear;
			} else {
				View = new PageView(_viewFrame, _autoScaleMode, PageNumber);
			}
		}
		#endregion
	}
}
