//
// mTouch-PDFReader demo
// MyDocumentBookmarksManager.cs
//
//  Author:
//       Alexander Matsibarov <amatsibarov@gmail.com>
//
//  Copyright (c) 2014 Alexander Matsibarov
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

using System.Collections.Generic;
using System.Linq;
using mTouchPDFReader.Library.Managers;
using mTouchPDFReader.Library.Data.Objects;

namespace mTouchPDFReader.Demo.Managers
{
	public class MyDocumentBookmarksManager : DocumentBookmarksManager
	{
		private static readonly List<DocumentBookmark> _allBookmarks = new List<DocumentBookmark>();		

		public override List<DocumentBookmark> LoadList(int docId)
		{
			var retValue = _allBookmarks.Where(d => d.DocId == docId).ToList();
			return retValue;
		}
		
		public override void Save(DocumentBookmark bookmark)
		{
			if (!_allBookmarks.Contains(bookmark)) {
				bookmark.Id = _allBookmarks.Count + 1;
				_allBookmarks.Add(bookmark);
			}
		}	
		
		public override void Delete(int bookmarkId)
		{
			var bookmark = _allBookmarks.First(d => d.Id == bookmarkId);
			if (bookmark != null) {
				_allBookmarks.Remove(bookmark);
			}
		}
	}
}
