//
// mTouch-PDFReader demo
// MyDocumentNoteManager.cs
//
// Copyright (c) 2012-2014 AlexanderMac(amatsibarov@gmail.com)
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
