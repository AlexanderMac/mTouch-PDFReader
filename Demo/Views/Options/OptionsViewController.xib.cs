//
// mTouch-PDFReader demo
// OptionsViewController.cs 
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
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			var optionsView = new OptionsTableViewController();
			optionsView.View.Frame = View.Bounds;
			View.AddSubview(optionsView.View);
		}

		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
}

