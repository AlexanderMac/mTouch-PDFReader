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
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace mTouchPDFReader.Demo
{
	public partial class DocumentsNavigationController : UINavigationController
	{
		#region Constructors
		public DocumentsNavigationController(IntPtr handle) : base(handle)
		{
		}

		[Export("initWithCoder:")]
		public DocumentsNavigationController(NSCoder coder) : base(coder)
		{
		}

		public DocumentsNavigationController() : base("DocumentsNavigationController", null)
		{
		}		
		#endregion	
		
		/// <summary>
		/// Calls when view has loaded
		/// </summary>
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			NavigationBar.BarStyle = UIBarStyle.Black;
			PushViewController(new DocumentsTableController(), false);
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

