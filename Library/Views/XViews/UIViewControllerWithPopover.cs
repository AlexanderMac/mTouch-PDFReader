//****************************************//
// mTouch-PDFReader library
// Extended View with popover showing support
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace mTouchPDFReader.Library.XViews
{
	public abstract class UIViewControllerWithPopover : UIViewController
	{
		/// <summary>
		/// Callback action 
		/// </summary>
		private Action<object> mCallbackAction;
		protected Action<object> CallbackAction {
			get {
				return mCallbackAction;
			}
		}
		
		/// <summary>
		/// PopoverController 
		/// </summary>
		protected UIPopoverController mPopoverController;
		public UIPopoverController PopoverController {
			set { mPopoverController = value; }
		}
		
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
			mCallbackAction = callbackAction;	
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

