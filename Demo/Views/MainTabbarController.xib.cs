//
// mTouch-PDFReader demo
// MainTabbarController.cs
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

			AddChildViewController(tabbarController);
			tabbarController.DidMoveToParentViewController(this);
			View.AddSubview(tabbarController.View);
		}

#pragma warning disable 672
		/// <summary>
		/// Called when permission is shought to rotate
		/// </summary>
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
#pragma warning restore 672
	}
}

