//****************************************//
// mTouch-PDFReader demo
// Simple document bookmarks manager
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using System.Collections.Generic;
using System.Linq;
using mTouchPDFReader.Library.Data.Managers;
using mTouchPDFReader.Library.Data.Objects;

namespace mTouchPDFReader.Demo.DataObjects
{
	public class MyDocumentBookmarkManager : DocumentBookmarkManager
	{
		#region Fields
		
		/// <summary>
		/// List of the all document bookmarks
		/// </summary>
		private static List<DocumentBookmark> mAllBookmarks;
		
		#endregion
		
		#region Logic
		
		/// <summary>
		/// Static constructor
		/// </summary>
		static MyDocumentBookmarkManager()
		{
			mAllBookmarks = new List<DocumentBookmark>();
		}
		
		/// <summary>
		/// Returns new id for new bookmark, it may be database id record, for example 
		/// </summary>
		/// <returns>New bookmark id</returns>
		protected override int GetNewId()
		{
			return mAllBookmarks.Count + 1;
		}

		/// <summary>
		/// Loads document bookmarks list by document id
		/// </summary>
		/// <param name="docId">PDF document Id</param>
		/// <returns>Bookmarks list</returns>
		public override List<DocumentBookmark> LoadBookmarks(int docId)
		{
			var retValue = mAllBookmarks.Where(d => d.DocId == docId).ToList();
			return retValue;
		}
		
		/// <summary>
		/// Saves bookmark 
		/// </summary>
		/// <param name="bookmark">Bookmark</param>
		public override void SaveBookmark(DocumentBookmark bookmark)
		{
			if (!mAllBookmarks.Contains(bookmark)) {
				bookmark.Id = GetNewId();
				mAllBookmarks.Add(bookmark);
			}
		}	
		
		/// <summary>
		/// Deletes bookmark by bookmark id 
		/// </summary>
		/// <param name="bookmarkId">Bookmark id</param>
		public override void DeleteBookmark(int bookmarkId)
		{
			var bookmark = mAllBookmarks.First(d => d.Id == bookmarkId);
			if (bookmark != null) {
				mAllBookmarks.Remove(bookmark);
			}
		}
		
		#endregion
	}
}

