//
// mTouch-PDFReader demo
// Main.cs
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
using System.Linq;using MonoTouch.Foundation;
using MonoTouch.UIKit;
using mTouchPDFReader.Demo.DataObjects;
using mTouchPDFReader.Library.Views.Core;

namespace mTouchPDFReader.Demo
{
	public class Application
	{
		static void Main(string[] args)
		{
			UIApplication.Main(args);
		}
	}
	
	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			window.AddSubview(mainTabbarController.View);
			window.MakeKeyAndVisible();
			new MyObjectsActivator().CreateObjects(); 
			return true;
		}
		
		/// <summary>
		/// Calls when window is disposing
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			//??
			PDFDocument.CloseDocument();
			base.Dispose(disposing);			
		}
	}
}

