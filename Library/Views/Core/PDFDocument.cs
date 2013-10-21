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
		public static bool DocumentHasLoaded {
			get {
				return _DocumentHasLoaded;
			}
		}
		private static bool _DocumentHasLoaded;
		
		private static CGPDFDocument _Document;

		public static string DocName {
			get {
				return _DocName;
			}
		}
		private static string _DocName;

		public static string DocFilePath {
			get {
				return _DocFilePath;
			}
		}
		private static string _DocFilePath;
		
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
		
		public static int PageCount {
			get {
				if (_Document != null) {
					return _Document.Pages;
				}
				return 0;
			}
		}		
		#endregion		
		
		static PDFDocument()
		{
			_DocumentHasLoaded = false;
			_CurrentPageNumber = -1;
		}
		
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

		public static void OpenDocument(string docName, byte[] buffer)
		{
			// Close previous opened document
			CloseDocument();
			// Open new document
			_CurrentPageNumber = -1;
			_DocName = docName;
			_DocFilePath = String.Empty;

			try {
				_Document = new CGPDFDocument(new CGDataProvider(buffer, 0, buffer.Length));
				_DocumentHasLoaded = true;
			} catch (Exception) {
				_DocumentHasLoaded = false;
				using (var alert = new UIAlertView("Error", "Open PDF document error", null, "Ok")) {
					alert.Show();
				}
			}			
		}
		
		public static void CloseDocument()
		{
			if (_Document != null) {
				_DocumentHasLoaded = false;
				_Document.Dispose();
			}
		}
		
		public static CGPDFPage GetPage(int pageNumber)
		{
			if ((_Document != null) && (pageNumber > 0) && (pageNumber <= PageCount)) {
				return _Document.GetPage(pageNumber);
			}
			return null;
		}
	}
}