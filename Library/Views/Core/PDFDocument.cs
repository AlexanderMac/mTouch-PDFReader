//
// mTouch-PDFReader library
// PDFDocument.cs
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

using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace mTouchPDFReader.Library.Views.Core
{
	public static class PDFDocument
	{	
		#region Data		
		public static bool DocumentHasLoaded {
			get {
				return _documentHasLoaded;
			}
		}
		private static bool _documentHasLoaded;
		
		private static CGPDFDocument _document;

		public static string DocName {
			get {
				return _docName;
			}
		}
		private static string _docName;

		public static string DocFilePath {
			get {
				return _docFilePath;
			}
		}
		private static string _docFilePath;
		
		public static int CurrentPageNumber {
			get { 
				return _currentPageNumber; 
			}
			set {
				if ((_document != null) && (value >= 1) && (value <= _document.Pages)) {
					_currentPageNumber = value;
				}
			}
		}
		private static int _currentPageNumber;
		
		public static int PageCount {
			get {
				if (_document != null) {
					return _document.Pages;
				}
				return 0;
			}
		}		
		#endregion		

		#region Logic
		static PDFDocument()
		{
			_documentHasLoaded = false;
			_currentPageNumber = -1;
		}
		
		public static void OpenDocument(string docName, string docFilePath)
		{
			CloseDocument();

			_currentPageNumber = -1;
			_docName = docName;
			_docFilePath = docFilePath;
			try {
				_document = CGPDFDocument.FromFile(_docFilePath);
				_documentHasLoaded = true;
			} catch (Exception) {
				_documentHasLoaded = false;
				using (var alert = new UIAlertView("Error", "Open PDF document error", null, "Ok")) {
					alert.Show();
				}
			}			
		}	
		
		public static void CloseDocument()
		{
			if (_document != null) {
				_documentHasLoaded = false;
				_document.Dispose();
			}
		}
		
		public static CGPDFPage GetPage(int pageNumber)
		{
			if ((_document != null) && (pageNumber > 0) && (pageNumber <= PageCount)) {
				return _document.GetPage(pageNumber);
			}
			return null;
		}
		#endregion
	}
}