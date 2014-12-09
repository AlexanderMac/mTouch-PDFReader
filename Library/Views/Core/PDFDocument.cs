//
// mTouch-PDFReader library
// PDFDocument.cs
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