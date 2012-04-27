//****************************************//
// mTouch-PDFReader demo
// PDF file
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using System.Collections.Generic;
using System.IO;

namespace mTouchPDFReader.Demo.DataObjects
{
	public class PDFFile
	{
		/// <summary>
		/// PDF document id 
		/// </summary>
		private int mId;
		public int Id {
			get { return mId; }
		}
		
		/// <summary>
		/// PDF file name
		/// </summary>
		private string mName;
		public string Name {
			get { return mName; }
		}
		
		/// <summary>
		/// PDF file path
		/// </summary>
		private string mFilePath;
		public string FilePath {
			get { return mFilePath; }
		}
		
		/// <summary>
		/// PDF file size in bytes, kbytes, mbytes
		/// </summary>
		private int mFileSize;
		public int FileSize {
			get { return mFileSize; }
		}
		public float FileSizeKb {
			get { return (float)mFileSize / 1024f; }
		}
		public float FileSizeMb {
			get { return (float)mFileSize / (1024f * 1024f); }
		}
		
		/// <summary>
		/// All loaded pdf files info
		/// </summary>
		private static List<PDFFile> mPDFFilesInfo;
		public static List<PDFFile> PDFFiles {
			get { return mPDFFilesInfo; }
		}
		
		/// <summary>
		/// Static constructor
		/// </summary>
		static PDFFile()
		{
			mPDFFilesInfo = new List<PDFFile>();
		}
		
		/// <summary>
		/// Default constructor
		/// </summary>
		public PDFFile(string name, int id, string filePath, int fileSize)
		{
			mName = name;
			mId = id;
			mFilePath = filePath;
			mFileSize = fileSize;
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
					mPDFFilesInfo.Add(docInfo);
					i++;
				}
			}
		}
	}
}

