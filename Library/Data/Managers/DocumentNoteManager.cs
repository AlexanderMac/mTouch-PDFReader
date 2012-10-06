//
// mTouch-PDFReader library
// DocumentNoteManager.cs (DDocument note manager)
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
using mTouchPDFReader.Library.Data.Objects;

namespace mTouchPDFReader.Library.Data.Managers
{
	public class DocumentNoteManager
	{
		#region Fields
		/// <summary>
		/// Single manager instance 
		/// </summary>
		public static DocumentNoteManager Instance {
			get {
				return _Instance;
			}
			internal set {
				_Instance = value;
			}
		}	
		private static DocumentNoteManager _Instance;
		#endregion
		
		#region Logic		
		/// <summary>
		/// Returns a new id for a new note, it is may be a database id record, for example 
		/// </summary>
		/// <returns>New note id</returns>
		/// <remarks>No logic implemented, should be inherited in childs</remarks>
		protected virtual int GetNewId()
		{
			return 0;
		}

		/// <summary>
		/// Loads a document note by a document id
		/// </summary>
		/// <param name="docId">PDF document Id</param>
		/// <returns>Note</returns>
		/// <remarks>No logic implemented, should be inherited in childs</remarks>
		public virtual DocumentNote LoadNote(int docId)
		{
			return new DocumentNote(docId, -1, string.Empty);
		}

		/// <summary>
		/// Saves a document note
		/// </summary>
		/// <param name="note">Note</param>
		/// <remarks>No logic implemented, should be inherited in childs</remarks>
		public virtual void SaveNote(DocumentNote note)
		{
			// Noting
		}		
		#endregion
	}
}
