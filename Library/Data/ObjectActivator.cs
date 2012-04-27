//****************************************//
// mTouch-PDFReader library
// Objects activator
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using mTouchPDFReader.Library.Data.Managers;

namespace mTouchPDFReader.Library.Data
{
	public class ObjectsActivator
	{
		#region Fields
		
		/// <summary>
		/// Initializing flag
		/// </summary>
		private bool mInitialized;
		
		#endregion
		
		#region Logic		
		
		/// <summary>
		/// Creates objects
		/// </summary>
		public void CreateObjects()
		{
			if (!mInitialized) {
				mInitialized = true;
				OptionsManager.Instance = CreateOptionsManager();
				DocumentNoteManager.Instance = CreateDocumentNoteManager();
				DocumentBookmarkManager.Instance = CreateDocumentBookmarkManager();
			}	
		}
		
		/// <summary>
		/// Returns OptionsManager instance
		/// </summary>
		/// <returns></returns>
		private OptionsManager CreateOptionsManager()
		{
			return new OptionsManager();
		}
		
		/// <summary>
		/// Returns DocumentNoteManager instance
		/// </summary>
		/// <returns></returns>
		protected virtual DocumentNoteManager CreateDocumentNoteManager()
		{
			return new DocumentNoteManager();
		}
		
		/// <summary>
		/// Returns DocumentBookmarkManager instance
		/// </summary>
		/// <returns></returns>
		protected virtual DocumentBookmarkManager CreateDocumentBookmarkManager()
		{
			return new DocumentBookmarkManager();
		}
		
		#endregion
	}
}

