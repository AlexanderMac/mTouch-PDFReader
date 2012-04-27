//****************************************//
// mTouch-PDFReader library
// Document bookmark manager
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using System.Collections.Generic;
using mTouchPDFReader.Library.Data.Objects;

namespace mTouchPDFReader.Library.Data.Managers
{
	public class DocumentBookmarkManager
	{
		#region Fields

		/// <summary>
		/// Single manager instance 
		/// </summary>
		private static DocumentBookmarkManager mInstance;
		public static DocumentBookmarkManager Instance {
			get {
				return mInstance;
			}
			internal set { mInstance = value; }
		}
		
		#endregion
		
		#region Logic
		
		/// <summary>
		/// Returns a new id for na ew bookmark, it is may be a database id record, for example 
		/// </summary>
		/// <returns>New bookmark id</returns>
		/// <remarks>No logic implemented, should be inherited in childs</remarks>
		protected virtual int GetNewId()
		{
			return 0;
		}
		
		/// <summary>
		/// Loads a document bookmarks list by a document id
		/// </summary>
		/// <param name="docId">PDF document Id</param>
		/// <returns>Bookmarks list</returns>
		/// <remarks>No logic implemented, should be inherited in childs</remarks>
		public virtual List<DocumentBookmark> LoadBookmarks(int docId)
		{
			return new List<DocumentBookmark>();
		}
		
		/// <summary>
		/// Saves a bookmark 
		/// </summary>
		/// <param name="bookmark">Bookmark</param>
		/// <remarks>No logic implemented, should be inherited in childs</remarks>
		public virtual void SaveBookmark(DocumentBookmark bookmark)
		{
			// Noting
		}	
		
		/// <summary>
		/// Deletes a bookmark by a bookmark id 
		/// </summary>
		/// <param name="bookmarkId">Bookmark id</param>
		/// <remarks>No logic implemented, should be inherited in childs</remarks>
		public virtual void DeleteBookmark(int bookmarkId)
		{
			// Nothing
		}
		
		#endregion
	}
}