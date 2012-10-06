//
// mTouch-PDFReader library
// DocumentBookmarkManager.cs (Document bookmarks manager)
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
using mTouchPDFReader.Library.Data.Objects;

namespace mTouchPDFReader.Library.Data.Managers
{
	public class DocumentBookmarkManager
	{
		#region Fields
		/// <summary>
		/// Single manager instance 
		/// </summary>
		public static DocumentBookmarkManager Instance {
			get {
				return _Instance;
			}
			internal set {
				_Instance = value;
			}
		}
		private static DocumentBookmarkManager _Instance;		
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