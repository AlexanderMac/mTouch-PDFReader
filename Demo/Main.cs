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

