//
// mTouch-PDFReader demo
// PDFDocumentFile.cs
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
using System.IO;

namespace mTouchPDFReader.Demo.Data
{
	public class PDFDocumentFile 
	{
		public int Id {
			get {
				return _id;
			}
		}
		private int _id;
		
		public string Name {
			get {
				return _name;
			}
		}
		private string _name;
		
		public string FilePath {
			get {
				return _filePath;
			}
		}
		private string _filePath;
		
		public int FileSize {
			get {
				return _fileSize;
			}
		}
		private int _fileSize;

		public float FileSizeKb {
			get {
				return (float)_fileSize / 1024f;
			}
		}

		public float FileSizeMb {
			get {
				return (float)_fileSize / (1024f * 1024f);
			}
		}
		
		public static List<PDFDocumentFile> PDFFiles {
			get {
				return _pdfFilesInfo;
			}
		}
		private static List<PDFDocumentFile> _pdfFilesInfo;
		
		static PDFDocumentFile()
		{
			_pdfFilesInfo = new List<PDFDocumentFile>();
		}
		
		public PDFDocumentFile(string name, int id, string filePath, int fileSize)
		{
			_name = name;
			_id = id;
			_filePath = filePath;
			_fileSize = fileSize;
		}
		
		public static void FillFromDirectories(string[] dirs)
		{
			foreach (var dir in dirs) {
				if (!Directory.Exists(dir)) {
					continue;
				}

				int i = 1; // Very simple document ID
				var files = Directory.GetFiles(dir, "*.pdf");

				foreach (var file in files) {
					FileInfo fi = new FileInfo(file);
					var docInfo = new PDFDocumentFile(fi.Name, i, fi.FullName, (int)fi.Length);
					_pdfFilesInfo.Add(docInfo);
					i++;
				}
			}
		}
	}
}
