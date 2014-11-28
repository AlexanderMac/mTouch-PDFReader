//
// mTouch-PDFReader demo
// PDFDocumentFile.cs
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
