//
// mTouch-PDFReader library
// UIViewControllerWithPopover.cs
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

// TODO: Required!? 
namespace mTouchPDFReader.Library.XViews
{
	public abstract class UIViewControllerWithPopover : UIViewController
	{
		#region Data
		protected Action<object> CallbackAction {
			get {
				return _callbackAction;
			}
		}
		private Action<object> _callbackAction;
		
		public UIPopoverController PopoverController {
			set {
				_popoverController = value;
			}
		}
		protected UIPopoverController _popoverController;
		#endregion

		#region Logic		
		public UIViewControllerWithPopover(IntPtr handle) : base(handle)
		{
		}

		[Export("initWithCoder:")]
		public UIViewControllerWithPopover(NSCoder coder) : base(coder)
		{
		}
		
		public UIViewControllerWithPopover(string nibName, NSBundle bundle, Action<object> callbackAction) 
			: base(nibName, bundle)
		{
			_callbackAction = callbackAction;	
		}
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			ContentSizeForViewInPopover = getPopoverSize();		
		}

		protected abstract SizeF getPopoverSize();
		#endregion
	}
}
