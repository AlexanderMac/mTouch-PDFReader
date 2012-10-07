//
// mTouch-PDFReader library
//   PageViewController.cs
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

using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace mTouchPDFReader.Library.Views.Core
{
	public class PageViewController : UIViewController
	{
		/// <summary>
		/// The PageView frame.
		/// </summary>
		private RectangleF _ViewFrame;

		/// <summary>
		/// The page number.
		/// </summary>
		private int _PageNumber;

		/// <summary>
		/// Gets the PageView.
		/// </summary>
		public PageView PageView {
			get {
				return (PageView)View;
			}
		}

		#region Constructors
		public PageViewController(IntPtr handle) : base(handle)
		{
		}
		
		[Export("initWithCoder:")]
		public PageViewController(NSCoder coder) : base(coder)
		{
		}
		
		public PageViewController(RectangleF viewFrame, int pageNumber) : base(null, null)
		{
			_ViewFrame = viewFrame;
			_PageNumber = pageNumber;
		}	
		#endregion

		#region UIViewController
		/// <summary>
		/// Called after the controller’s view is loaded into memory.
		/// </summary>
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			View = new PageView(_ViewFrame, _PageNumber);
		}
		#endregion
	}
}

