//****************************************//
// mTouch-PDFReader library
// Store and manage opened PDF document
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace mTouchPDFReader.Library.Views.Core
{
	public static class PDFDocument
	{	
		#region Fields
		
		/// <summary>
		/// Indicates document has loaded or not
		/// </summary>
		private static bool mDocumentHasLoaded;
		public static bool DocumentHasLoaded {
			get {
				return mDocumentHasLoaded;
			}
		}
		
		/// <summary>
		/// Opened document
		/// </summary>
		private static CGPDFDocument mDocument;
		
		/// <summary>
		/// Document name
		/// </summary>
		private static string mDocName;
		public static string DocName {
			get {
				return mDocName;
			}
		}

		/// <summary>
		/// Path to document file
		/// </summary>
		private static string mDocFilePath;
		public static string DocFilePath {
			get { return mDocFilePath; }
		}
		
		/// <summary>
		/// Current document page number
		/// </summary>
		private static int mCurrentPageNumber;
		public static int CurrentPageNumber {
			get { 
				return mCurrentPageNumber; 
			}
			set {
				if ((mDocument != null) && (value >= 1) && (value <= mDocument.Pages)) {
					mCurrentPageNumber = value;
				}
			}
		}
		
		/// <summary>
		/// Document page count
		/// </summary>
		public static int PageCount {
			get {
				if (mDocument != null) {
					return mDocument.Pages;
				}
				return 0;
			}
		}
		
		#endregion		
		
		/// <summary>
		/// Static constructor
		/// </summary>
		static PDFDocument()
		{
			mDocumentHasLoaded = false;
			mCurrentPageNumber = -1;
		}
		
		/// <summary>
		/// Opens document
		/// </summary>
		/// <param name="docName">Document name</param>
		/// <param name="docFilePath">Path to document file</param>
		public static void OpenDocument(string docName, string docFilePath)
		{
			// Close previous opened document
			CloseDocument();
			// Open new document
			mCurrentPageNumber = -1;
			mDocName = docName;
			mDocFilePath = docFilePath;
			try {
				mDocument = CGPDFDocument.FromFile(mDocFilePath);
				mDocumentHasLoaded = true;
			} catch (Exception) {
				mDocumentHasLoaded = false;
				using (var alert = new UIAlertView("Error", "Open PDF document error", null, "Ok")) {
					alert.Show();
				}
			}			
		}	
		
		/// <summary>
		/// Dispose opened PDFDocument data
		/// </summary>
		public static void CloseDocument()
		{
			if (mDocument != null) {
				mDocumentHasLoaded = false;
				mDocument.Dispose();
			}
		}
		
		/// <summary>
		/// Gets CGPDFPage by naumber
		/// </summary>
		/// <param name="pageNumber">Page number</param>
		/// <returns>Reference to CGPDFPage or null</returns>
		public static CGPDFPage GetPage(int pageNumber)
		{
			if ((mDocument != null) && (pageNumber > 0) && (pageNumber <= PageCount)) {
				return mDocument.GetPage(pageNumber);
			}
			return null;
		}
	}
}