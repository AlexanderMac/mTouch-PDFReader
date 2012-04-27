//****************************************//
// mTouch-PDFReader demo
// Objects activator
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using mTouchPDFReader.Library.Data;
using mTouchPDFReader.Library.Data.Managers;

namespace mTouchPDFReader.Demo.DataObjects
{
	public class MyObjectsActivator : ObjectsActivator
	{
		#region Logic	
		
		/// <summary>
		/// Returns DocumentNoteManager instance
		/// </summary>
		/// <returns></returns>
		protected override DocumentNoteManager CreateDocumentNoteManager()
		{
			return new MyDocumentNoteManager();
		}
		
		/// <summary>
		/// Returns DocumentBookmarkManager instance
		/// </summary>
		/// <returns></returns>
		protected override DocumentBookmarkManager CreateDocumentBookmarkManager()
		{
			return new MyDocumentBookmarkManager();
		}
		
		#endregion
	}
}

