//
// mTouch-PDFReader library
// UIViewControllerWithPopover.cs (Extended View with popover showing support)
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

namespace mTouchPDFReader.Library.XViews
{
	public abstract class UIViewControllerWithPopover : UIViewController
	{
		#region Fields
		/// <summary>
		/// The callback action. 
		/// </summary>
		protected Action<object> CallbackAction {
			get {
				return _CallbackAction;
			}
		}
		private Action<object> _CallbackAction;
		#endregion
		
		/// <summary>
		/// The Popover controller.
		/// </summary>
		public UIPopoverController PopoverController {
			set {
				_PopoverController = value;
			}
		}
		protected UIPopoverController _PopoverController;
		
		#region Constructors		
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
			_CallbackAction = callbackAction;	
		}		
		#endregion
		
		/// <summary>
		/// Calls when view are loaded 
		/// </summary>
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			ContentSizeForViewInPopover = GetPopoverSize();		
		}
		
		/// <summary>
		/// Returns popover size, must be override in child classes
		/// </summary>
		/// <returns>Popover size</returns>
		protected abstract SizeF GetPopoverSize();
	}
}

