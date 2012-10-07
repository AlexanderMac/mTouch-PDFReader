//
// mTouch-PDFReader library
// PDFDocument.cs (Store and manage opened PDF document)
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
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace mTouchPDFReader.Library.Views.Core
{
	public static class PDFDocument
	{	
		#region Fields		
		/// <summary>
		/// Indicates that the document has loaded.
		/// </summary>
		public static bool DocumentHasLoaded {
			get {
				return _DocumentHasLoaded;
			}
		}
		private static bool _DocumentHasLoaded;
		
		/// <summary>
		/// The opened document.
		/// </summary>
		private static CGPDFDocument _Document;
		
		/// <summary>
		/// The document name.
		/// </summary>
		public static string DocName {
			get {
				return _DocName;
			}
		}
		private static string _DocName;

		/// <summary>
		/// The document file path.
		/// </summary>
		public static string DocFilePath {
			get {
				return _DocFilePath;
			}
		}
		private static string _DocFilePath;
		
		/// <summary>
		/// The current document page number.
		/// </summary>
		public static int CurrentPageNumber {
			get { 
				return _CurrentPageNumber; 
			}
			set {
				if ((_Document != null) && (value >= 1) && (value <= _Document.Pages)) {
					_CurrentPageNumber = value;
				}
			}
		}
		private static int _CurrentPageNumber;
		
		/// <summary>
		/// The document page count.
		/// </summary>
		public static int PageCount {
			get {
				if (_Document != null) {
					return _Document.Pages;
				}
				return 0;
			}
		}		
		#endregion		
		
		/// <summary>
		/// Static.
		/// </summary>
		static PDFDocument()
		{
			_DocumentHasLoaded = false;
			_CurrentPageNumber = -1;
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
			_CurrentPageNumber = -1;
			_DocName = docName;
			_DocFilePath = docFilePath;
			try {
				_Document = CGPDFDocument.FromFile(_DocFilePath);
				_DocumentHasLoaded = true;
			} catch (Exception) {
				_DocumentHasLoaded = false;
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
			if (_Document != null) {
				_DocumentHasLoaded = false;
				_Document.Dispose();
			}
		}
		
		/// <summary>
		/// Gets CGPDFPage by naumber
		/// </summary>
		/// <param name="pageNumber">Page number</param>
		/// <returns>Reference to CGPDFPage or null</returns>
		public static CGPDFPage GetPage(int pageNumber)
		{
			if ((_Document != null) && (pageNumber > 0) && (pageNumber <= PageCount)) {
				return _Document.GetPage(pageNumber);
			}
			return null;
		}
	}
}