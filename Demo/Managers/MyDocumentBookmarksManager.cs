//
// mTouch-PDFReader demo
// MyDocumentBookmarksManager.cs
//
// Copyright (c) 2012-2014 Alexander Matsibarov (amatsibarov@gmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// 'Software'), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions:

// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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

		public override List<DocumentBookmark> GetAllForDocument(int docId)
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
