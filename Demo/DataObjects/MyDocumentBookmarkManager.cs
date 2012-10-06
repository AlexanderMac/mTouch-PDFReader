//
// mTouch-PDFReader demo
// MyDocumentBookmarkManager.cs (Simple document bookmarks manager)
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
using System.Linq;
using mTouchPDFReader.Library.Data.Managers;
using mTouchPDFReader.Library.Data.Objects;

namespace mTouchPDFReader.Demo.DataObjects
{
	public class MyDocumentBookmarkManager : DocumentBookmarkManager
	{
		#region Fields		
		/// <summary>
		/// The document bookmarks list.
		/// </summary>
		private static List<DocumentBookmark> _AllBookmarks;		
		#endregion
		
		#region Logic	
		/// <summary>
		/// Hidden constructor to create instance only from RC.
		/// </summary>
		protected MyDocumentBookmarkManager() {}

		/// <summary>
		/// Static.
		/// </summary>
		static MyDocumentBookmarkManager()
		{
			_AllBookmarks = new List<DocumentBookmark>();
		}
		
		/// <summary>
		/// Gets the new bookmark identifier.
		/// </summary>
		/// <returns>The new bookmark identifier.</returns>
		protected override int GetNewId()
		{
			return _AllBookmarks.Count + 1;
		}

		/// <summary>
		/// Gets the <see cref="DocumentBookmark"/> bookmarks collection by the <see cref="docId"/>.
		/// </summary>
		/// <param name="docId">The PDF document Id.</param>
		/// <returns>The <see cref="DocumentBookmark"/> objects list.</returns>
		public override List<DocumentBookmark> LoadList(int docId)
		{
			var retValue = _AllBookmarks.Where(d => d.DocId == docId).ToList();
			return retValue;
		}
		
		/// <summary>
		/// Saves the <see cref="DocumentBookmark"/> object. 
		/// </summary>
		/// <param name="bookmark">The bookmark object.</param>
		public override void Save(DocumentBookmark bookmark)
		{
			if (!_AllBookmarks.Contains(bookmark)) {
				bookmark.Id = GetNewId();
				_AllBookmarks.Add(bookmark);
			}
		}	
		
		/// <summary>
		/// Deletes the <see cref="DocumentBookmark"/> object by <see cref="bookmarkId"/>.
		/// </summary>
		public override void Delete(int bookmarkId)
		{
			var bookmark = _AllBookmarks.First(d => d.Id == bookmarkId);
			if (bookmark != null) {
				_AllBookmarks.Remove(bookmark);
			}
		}		
		#endregion
	}
}

