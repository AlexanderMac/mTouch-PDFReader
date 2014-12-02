//
// mTouch-PDFReader demo
// MyDocumentNoteManager.cs
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
	public class MyDocumentNoteManager : DocumentNoteManager
	{
		private static readonly List<DocumentNote> _allNotes = new List<DocumentNote>();		

		public override DocumentNote Load(int docId)
		{
			var note = _allNotes.FirstOrDefault(n => n.DocId == docId);
			if (note == null) {
				note = new DocumentNote { Id = -1, DocId = docId, Note = string.Empty };
			}			
			return note;
		}

		public override void Save(DocumentNote note)
		{
			if (!_allNotes.Contains(note)) {
				note.Id = _allNotes.Count + 1;
				_allNotes.Add(note);
			}
		}
	}
}
