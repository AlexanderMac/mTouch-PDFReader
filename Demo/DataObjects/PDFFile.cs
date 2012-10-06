//
// mTouch-PDFReader demo
// PDFFile.cs (PDF file)
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
using System.Collections.Generic;
using System.IO;

namespace mTouchPDFReader.Demo.DataObjects
{
	public class PDFFile
	{
		/// <summary>
		/// Gets the PDF document id.
		/// </summary>
		public int Id {
			get {
				return _Id;
			}
		}
		private int _Id;
		
		/// <summary>
		/// Gets the PDF file name.
		/// </summary>
		public string Name {
			get {
				return _Name;
			}
		}
		private string _Name;
		
		/// <summary>
		/// Gets the PDF file path.
		/// </summary>
		public string FilePath {
			get {
				return _FilePath;
			}
		}
		private string _FilePath;
		
		/// <summary>
		/// Gets the PDF file size in bytes.
		/// </summary>
		public int FileSize {
			get {
				return _FileSize;
			}
		}
		private int _FileSize;

		/// <summary>
		/// Gets the PDF file size in kbytes.
		/// </summary>
		public float FileSizeKb {
			get {
				return (float)_FileSize / 1024f;
			}
		}

		/// <summary>
		/// Gets the PDF file size in mbytes.
		/// </summary>
		public float FileSizeMb {
			get {
				return (float)_FileSize / (1024f * 1024f);
			}
		}
		
		/// <summary>
		/// Gets the loaded pdf files info list.
		/// </summary>
		public static List<PDFFile> PDFFiles {
			get {
				return _PDFFilesInfo;
			}
		}
		private static List<PDFFile> _PDFFilesInfo;
		
		/// <summary>
		/// Static.
		/// </summary>
		static PDFFile()
		{
			_PDFFilesInfo = new List<PDFFile>();
		}
		
		/// <summary>
		/// Working.
		/// </summary>
		public PDFFile(string name, int id, string filePath, int fileSize)
		{
			_Name = name;
			_Id = id;
			_FilePath = filePath;
			_FileSize = fileSize;
		}
		
		/// <summary>
		/// Fills DocumentInfo list from passed directories
		/// </summary>
		/// <param name="dirs">Array of directories pathes</param>
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
					var docInfo = new PDFFile(fi.Name, i, fi.FullName, (int)fi.Length);
					_PDFFilesInfo.Add(docInfo);
					i++;
				}
			}
		}
	}
}

