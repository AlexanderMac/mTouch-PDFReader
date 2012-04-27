//****************************************//
// mTouch-PDFReader library
// Translate manager
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using MonoTouch.Foundation;

namespace mTouchPDFReader.Library.Data.Managers
{
	public static class TranslateManager
	{
		/// <summary>
		/// Returns translate for key
		/// </summary>
		/// <param name='key'>Translated word</param>
		public static string t(this string key)
		{
			return NSBundle.MainBundle.LocalizedString(key, "", "");
		}		
	}
}

