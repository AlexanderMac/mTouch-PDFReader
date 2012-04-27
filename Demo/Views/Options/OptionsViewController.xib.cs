//****************************************//
// mTouch-PDFReader demo
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
using mTouchPDFReader.Library.Views.Management;

namespace mTouchPDFReader.Demo
{
	public partial class OptionsViewController : UIViewController
	{
		#region Constructors
		public OptionsViewController(IntPtr handle) : base(handle)
		{
		}

		[Export("initWithCoder:")]
		public OptionsViewController(NSCoder coder) : base(coder)
		{
		}
		
		public OptionsViewController() : base ("OptionsViewController", null)
		{
		}
		#endregion Constructors
		
		/// <summary>
		/// Calls when view are loaded
		/// </summary>
		public override void ViewDidLoad()
		{
			base.ViewDidLoad ();
			var optionsView = new OptionsTableViewController();
			optionsView.View.Frame = View.Bounds;
			optionsView.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			View.AddSubview(optionsView.View);
		}
		
		/// <summary>
		/// Called when permission is shought to rotate
		/// </summary>
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
}

