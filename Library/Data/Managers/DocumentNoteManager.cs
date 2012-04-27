//****************************************//
// mTouch-PDFReader library
// Document note manager
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using mTouchPDFReader.Library.Data.Objects;

namespace mTouchPDFReader.Library.Data.Managers
{
	public class DocumentNoteManager
	{
		#region Fields

		/// <summary>
		/// Single manager instance 
		/// </summary>
		private static DocumentNoteManager mInstance;
		public static DocumentNoteManager Instance {
			get {
				return mInstance;
			}
			internal set { mInstance = value; }
		}
		
		#endregion
		
		#region Logic
		
		/// <summary>
		/// Returns a new id for a new note, it is may be a database id record, for example 
		/// </summary>
		/// <returns>New note id</returns>
		/// <remarks>No logic implemented, should be inherited in childs</remarks>
		protected virtual int GetNewId()
		{
			return 0;
		}

		/// <summary>
		/// Loads a document note by a document id
		/// </summary>
		/// <param name="docId">PDF document Id</param>
		/// <returns>Note</returns>
		/// <remarks>No logic implemented, should be inherited in childs</remarks>
		public virtual DocumentNote LoadNote(int docId)
		{
			return new DocumentNote(docId, -1, string.Empty);
		}

		/// <summary>
		/// Saves a document note
		/// </summary>
		/// <param name="note">Note</param>
		/// <remarks>No logic implemented, should be inherited in childs</remarks>
		public virtual void SaveNote(DocumentNote note)
		{
			// Noting
		}
		
		#endregion
	}
}
