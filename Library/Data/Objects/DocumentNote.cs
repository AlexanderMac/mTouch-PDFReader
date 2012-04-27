//****************************************//
// mTouch-PDFReader library
// Document note
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;

namespace mTouchPDFReader.Library.Data.Objects
{
	public class DocumentNote
	{
		/// <summary>
		/// Document Id
		/// </summary>
		private int mDocId;
		public int DocId {
			get { return mDocId; }
		}
		
		/// <summary>
		/// Note Id
		/// </summary>
		private int mId;
		public int Id {
			get { return mId; }
			set { mId = value; }
		}
		
		/// <summary>
		/// Note
		/// </summary>
		private string mNote;
		public string Note {
			get { return mNote; }
			set { mNote = value; }
		}
			
		/// <summary>
		/// Work constructor
		/// </summary>
		/// <param name="docId">PDF document id</param>
		/// <param name="id">Note id</param>
		/// <param name="note">Note</param>
		public DocumentNote(int docId, int id, string note)
		{
			mDocId = docId;
			mId = id;
			mNote = note;
		}
	}
}

