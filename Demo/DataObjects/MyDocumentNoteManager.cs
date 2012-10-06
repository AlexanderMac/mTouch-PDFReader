//
// mTouch-PDFReader demo
// MyDocumentNoteManager.cs (Simple document note manager)
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
	public class MyDocumentNoteManager : DocumentNoteManager
	{
		#region Fields		
		/// <summary>
		/// The document notes list.
		/// </summary>
		private static List<DocumentNote> _AllNotes;		
		#endregion
		
		#region Logic		
		/// <summary>
		/// Static constructor
		/// </summary>
		static MyDocumentNoteManager()
		{
			_AllNotes = new List<DocumentNote>();
		}
		
		/// <summary>
		/// Returns new id for new note, it may be database id record, for example 
		/// </summary>
		/// <returns>New note id</returns>
		protected override int GetNewId()
		{
			return _AllNotes.Count + 1;
		}
		
		/// <summary>
		/// Loads document note by document id
		/// </summary>
		/// <param name="docId">PDF document Id</param>
		/// <returns>Note</returns>
		public override DocumentNote LoadNote(int docId)
		{
			var note = _AllNotes.FirstOrDefault(n => n.DocId == docId);
			if (note == null) {
				note = new DocumentNote(docId, -1, string.Empty);
			}			
			return note;
		}

		/// <summary>
		/// Saves document note for document id
		/// </summary>
		/// <param name="docId">PDF document Id</param>
		/// <param name="note">Note</param>
		public override void SaveNote(DocumentNote note)
		{
			if (!_AllNotes.Contains(note)) {
				note.Id = GetNewId();
				_AllNotes.Add(note);
			}
		}		
		#endregion
	}
}

