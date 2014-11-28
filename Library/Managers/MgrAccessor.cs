//
// mTouch-PDFReader library
// MgrAccessor.cs
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

using mTouchPDFReader.Library.Interfaces;
using mTouchPDFReader.Library.Utils;

namespace mTouchPDFReader.Library.Managers
{
	public static class MgrAccessor
	{
		public static IDocumentNoteManager DocumentNoteMgr {
			get { return _DocumentNoteMgr ?? (_DocumentNoteMgr = RC.Get<IDocumentNoteManager>()); }
		}
		private static IDocumentNoteManager _DocumentNoteMgr; 

	
		public static IDocumentBookmarksManager DocumentBookmarkMgr {
			get { return _DocumentBookmarkMgr ?? (_DocumentBookmarkMgr = RC.Get<IDocumentBookmarksManager>()); }
		}
		private static IDocumentBookmarksManager _DocumentBookmarkMgr;

		public static ISettingsManager OptionsMgr {
			get { return _OptionsMgr ?? (_OptionsMgr = RC.Get<ISettingsManager>()); }
		}
		private static ISettingsManager _OptionsMgr;
	}
}
