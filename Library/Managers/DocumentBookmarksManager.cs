//
// mTouch-PDFReader library
// DocumentBookmarksManager.cs
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
using mTouchPDFReader.Library.Interfaces;
using mTouchPDFReader.Library.Data.Objects;

namespace mTouchPDFReader.Library.Managers
{
	public class DocumentBookmarksManager : IDocumentBookmarksManager
	{
		public virtual DocumentBookmark GetNew(int docId, string name, int pageNumber)
		{
			return new DocumentBookmark {
				Id = -1,
				DocId = docId, 
				Name = name,
				PageNumber = pageNumber
			};
		}

		public virtual List<DocumentBookmark> GetAllForDocument(int docId)
		{
			return new List<DocumentBookmark>();
		}
		
		public virtual void Save(DocumentBookmark bookmark)
		{
			// Noting
		}	
		
		public virtual void Delete(int bookmarkId)
		{
			// Nothing
		}
	}
}