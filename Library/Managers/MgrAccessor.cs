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

using Autofac;
using mTouchPDFReader.Library.Interfaces;

namespace mTouchPDFReader.Library.Managers
{
	public static class MgrAccessor
	{
		private static IContainer _container;

		public static IDocumentNoteManager DocumentNoteMgr {
			get { return _container.Resolve<IDocumentNoteManager>(); }
		}
	
		public static IDocumentBookmarksManager DocumentBookmarkMgr {
			get { return _container.Resolve<IDocumentBookmarksManager>(); }
		}

		public static ISettingsManager SettingsMgr {
			get { return _container.Resolve<ISettingsManager>(); }
		}

		public static void Initialize(ContainerBuilder builder)
		{
			_container = builder.Build();
		}
	}
}
