//****************************************//
// mTouch-PDFReader library
// Document bookmark
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using mTouchPDFReader.Library.Data.Enums;

namespace mTouchPDFReader.Library.Data.Objects
{
	public class DocumentBookmark
	{
		/// <summary>
		/// Document Id
		/// </summary>
		private int mDocId;
		public int DocId {
			get { return mDocId; }
		}

		/// <summary>
		/// Bookmark Id
		/// </summary>
		private int mId;
		public int Id {
			get { return mId; }
			set { mId = value; }
		}
		
		/// <summary>
		/// Bookmark name
		/// </summary>
		private string mName;
		public string Name {
			get { return mName; }
		}
		
		/// <summary>
		/// Bookmark page number
		/// </summary>
		private int mPageNumber;
		public int PageNumber {
			get { return mPageNumber; }
		}
		
		/// <summary>
		/// Work constructor
		/// </summary>
		/// <param name="docId">PDF document id</param>
		/// <param name="id">Bookmark id</param>
		/// <param name="name">Bookmark name</param>
		/// <param name="pageNumber">Bookmark page number</param>
		public DocumentBookmark(int docId, int id, string name, int pageNumber)
		{
			mDocId = docId;
			mId = id;
			mName = name;
			mPageNumber = pageNumber;
		}
	}
}

