//****************************************//
// mTouch-PDFReader demo
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace mTouchPDFReader.Demo
{
	public partial class MainTabbarController : UIViewController
	{
		#region Constructors
		public MainTabbarController(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		[Export("initWithCoder:")]
		public MainTabbarController(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		public MainTabbarController() : base("MainTabbarController", null)
		{
			Initialize();
		}

		void Initialize()
		{
		}		
		#endregion		
		
		/// <summary>
		/// Calls when view has loaded
		/// </summary>
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			tabbarController.View.Frame = View.Bounds;
			View.AddSubview(tabbarController.View);
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

