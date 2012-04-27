//****************************************//
// mTouch-PDFReader library
// System info
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;

namespace mTouchPDFReader.Library.Data.Objects
{
	public static class SystemInfo
	{		
		/// <summary>
		/// Release date
		/// </summary>
		public static DateTime ReleaseDate {
			get {
				return new DateTime(2012, 03, 09);
			}
		}
		
		/// <summary>
		/// Version
		/// </summary>
		public static string Version {
			get {
				return "1.0.2";
			}
		}
	}
}

