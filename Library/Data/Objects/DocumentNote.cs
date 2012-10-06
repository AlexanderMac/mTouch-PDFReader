//
// mTouch-PDFReader library
// DocumentNote.cs (DDocument note)
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

namespace mTouchPDFReader.Library.Data.Objects
{
	public class DocumentNote
	{
		/// <summary>
		/// Gets the document Id.
		/// </summary>
		public int DocId {
			get {
				return _DocId;
			}
		}
		private int _DocId;
		
		/// <summary>
		/// Get the note Id.
		/// </summary>
		public int Id {
			get {
				return _Id;
			}
			set {
				_Id = value;
			}
		}
		private int _Id;
		
		/// <summary>
		/// Get the note.
		/// </summary>
		public string Note {
			get {
				return _Note;
			}
			set {
				_Note = value;
			}
		}
		private string _Note;
			
		/// <summary>
		/// Working.
		/// </summary>
		/// <param name="docId">The PDF document id.</param>
		/// <param name="id">The note id.</param>
		/// <param name="note">The note.</param>
		public DocumentNote(int docId, int id, string note)
		{
			_DocId = docId;
			_Id = id;
			_Note = note;
		}
	}
}

