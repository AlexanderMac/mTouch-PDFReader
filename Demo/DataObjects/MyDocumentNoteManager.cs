//****************************************//
// mTouch-PDFReader demo
// Simple document note manager
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

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
		/// List of the all document notes
		/// </summary>
		private static List<DocumentNote> mAllNotes;
		
		#endregion
		
		#region Logic
		
		/// <summary>
		/// Static constructor
		/// </summary>
		static MyDocumentNoteManager()
		{
			mAllNotes = new List<DocumentNote>();
		}
		
		/// <summary>
		/// Returns new id for new note, it may be database id record, for example 
		/// </summary>
		/// <returns>New note id</returns>
		protected override int GetNewId()
		{
			return mAllNotes.Count + 1;
		}
		
		/// <summary>
		/// Loads document note by document id
		/// </summary>
		/// <param name="docId">PDF document Id</param>
		/// <returns>Note</returns>
		public override DocumentNote LoadNote(int docId)
		{
			var note = mAllNotes.FirstOrDefault(n => n.DocId == docId);
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
			if (!mAllNotes.Contains(note)) {
				note.Id = GetNewId();
				mAllNotes.Add(note);
			}
		}
		
		#endregion
	}
}

