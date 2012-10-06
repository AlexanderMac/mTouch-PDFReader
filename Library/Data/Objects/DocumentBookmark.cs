//
// mTouch-PDFReader library
// DocumentBookmark.cs (Document bookmark)
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
using mTouchPDFReader.Library.Data.Enums;

namespace mTouchPDFReader.Library.Data.Objects
{
	public class DocumentBookmark
	{
		/// <summary>
		/// Gets the document Id.
		/// </summary>
		public int DocId {
			get { return _DocId; }
		}
		private int _DocId;

		/// <summary>
		/// Gets the bookmark Id.
		/// </summary>
		public int Id {
			get { return _Id; }
			set { _Id = value; }
		}
		private int _Id;
		
		/// <summary>
		/// Gets the bookmark name.
		/// </summary>
		public string Name {
			get { return _Name; }
		}
		private string _Name;
		
		/// <summary>
		/// Gets the bookmark page number.
		/// </summary>
		public int PageNumber {
			get { return _PageNumber; }
		}
		private int _PageNumber;
		
		/// <summary>
		/// Working.
		/// </summary>
		/// <param name="docId">The PDF document id.</param>
		/// <param name="id">The bookmark id.</param>
		/// <param name="name">The bookmark name.</param>
		/// <param name="pageNumber">The bookmark page number.</param>
		public DocumentBookmark(int docId, int id, string name, int pageNumber)
		{
			_DocId = docId;
			_Id = id;
			_Name = name;
			_PageNumber = pageNumber;
		}
	}
}

