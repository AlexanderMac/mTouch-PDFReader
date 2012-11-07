//
// mTouch-PDFReader library
// OptionsManager.cs (Options manager)
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
using System.Xml;
using System.IO;
using mTouchPDFReader.Library.Interfaces;
using mTouchPDFReader.Library.Data.Enums;
using mTouchPDFReader.Library.Data.Objects;

namespace mTouchPDFReader.Library.Managers
{
	public class OptionsManager : IOptionsManager
	{		
		#region Constants		
		/// <summary>
		/// The options file name. 
		/// </summary>
		private const string OptionsFileName = "mTouchPDFReader.Options.xml";		
		#endregion
		
		#region Fields					
		/// <summary>
		/// Gets the options. 
		/// </summary>
		public Options Options {
			get {
				if (_Options == null) {
					_Options = new Options();
					Load();
				}
				return _Options;
			}
		}
		private Options _Options;
		
		/// <summary>
		/// Initialized flag 
		/// </summary>
		private bool _Initialized;		
		#endregion
		
		#region Logic
		/// <summary>
		/// Hidden constructor to create instance only from RC.
		/// </summary>
		protected OptionsManager()	{}

		/// <summary>
		/// Loads options 
		/// </summary>
		public void Load()
		{
			if (_Initialized) {
				return;
			}
			_Initialized = true;
			
			var optionsFullFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), OptionsFileName);
			try {
				var optionsXmlDoc = new XmlDocument();
				optionsXmlDoc.Load(optionsFullFileName);
				object val = GetNodeValue(optionsXmlDoc, "/Options/PageTurningType", typeof(int));
				if (val != null) {
					_Options.PageTurningType = (PageTurningTypes)Convert.ToInt32(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/ToolbarVisible", typeof(bool));
				if (val != null) {
					_Options.ToolbarVisible = Convert.ToBoolean(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/BottombarVisible", typeof(bool));
				if (val != null) {
					_Options.BottombarVisible = Convert.ToBoolean(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/PageNumberVisible", typeof(bool));
				if (val != null) {
					_Options.PageNumberVisible = Convert.ToBoolean(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/NoteBtnVisible", typeof(bool));
				if (val != null) {
					_Options.NoteBtnVisible = Convert.ToBoolean(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/BookmarksBtnVisible", typeof(bool));
				if (val != null) {
					_Options.BookmarksBtnVisible = Convert.ToBoolean(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/ThumbsBtnVisible", typeof(bool));
				if (val != null) {
					_Options.ThumbsBtnVisible = Convert.ToBoolean(val);
				}
				/*
				object val = GetNodeValue(optionsXmlDoc, "/Options/BackgroundColor", typeof(byte));
				if (val != null) {
					_Options.BackgroundColor = DefaultBackgroundColor;
				}
				*/			
				val = GetNodeValue(optionsXmlDoc, "/Options/AllowZoomByDoubleTouch", typeof(bool));
				if (val != null) {
					_Options.AllowZoomByDoubleTouch = Convert.ToBoolean(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/ZoomScaleLevels", typeof(int));
				if (val != null) {
					_Options.ZoomScaleLevels = Convert.ToInt32(val);	
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/ThumbsBufferSize", typeof(int));
				if (val != null) {
					_Options.ThumbsBufferSize = Convert.ToInt32(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/ThumbSize", typeof(int));
				if (val != null) {
					_Options.ThumbSize = Convert.ToInt32(val);
				}
			} catch (Exception) {
				// Nothing
			}
		}
		
		/// <summary>
		/// Saves options 
		/// </summary>
		public void Save()
		{
			if (!_Initialized) {
				return;
			}
			
			var optionsFullFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), OptionsFileName);
			try {
				string xmlRow = 
"<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + 
					"<Options>" + 
					"	<PageTurningType>" + (int)_Options.PageTurningType + "</PageTurningType>" +
					"	<ToolbarVisible>" + _Options.ToolbarVisible + "</ToolbarVisible>" +
					"	<BottombarVisible>" + _Options.BottombarVisible + "</BottombarVisible>" + 
					"	<PageNumberVisible>" + _Options.PageNumberVisible + "</PageNumberVisible>" +
					"	<NoteBtnVisible>" + _Options.NoteBtnVisible + "</NoteBtnVisible>" +
					"	<BookmarksBtnVisible>" + _Options.BookmarksBtnVisible + "</BookmarksBtnVisible>" +
					"	<ThumbsBtnVisible>" + _Options.ThumbsBtnVisible + "</ThumbsBtnVisible>" +
					"	<AllowZoomByDoubleTouch>" + _Options.AllowZoomByDoubleTouch + "</AllowZoomByDoubleTouch>" +
					"	<ZoomScaleLevels>" + _Options.ZoomScaleLevels + "</ZoomScaleLevels>" +
					"	<ThumbsBufferSize>" + _Options.ThumbsBufferSize + "</ThumbsBufferSize>" +
					"	<ThumbSize>" + _Options.ThumbSize + "</ThumbSize>" +
					"</Options>";
				
				/*
				SetNodeValue(optionsXmlDoc, "/Options/PageTurningType", _Options.pPageTurningType);
				SetNodeValue(optionsXmlDoc, "/Options/ToolbarVisible", _Options.ToolbarVisible);
				SetNodeValue(optionsXmlDoc, "/Options/StatusbarVisible", _Options.StatusbarVisible);
				SetNodeValue(optionsXmlDoc, "/Options/PageNumberVisible", _Options.PageNumberVisible);
				SetNodeValue(optionsXmlDoc, "/Options/NoteBtnVisible", _Options.NoteBtnVisible);
				SetNodeValue(optionsXmlDoc, "/Options/BookmarksBtnVisible", _Options.BookmarksBtnVisible);
				SetNodeValue(optionsXmlDoc, "/Options/ThumbsBtnVisible", _Options.ThumbsBtnVisible);				
				//SetNodeValue(optionsXmlDoc, "/Options/BackgroundColor", _Options.BackgroundColor);
				SetNodeValue(optionsXmlDoc, "/Options/AllowZoomByDoubleTouch", _Options.AllowZoomByDoubleTouch);
				SetNodeValue(optionsXmlDoc, "/Options/ZoomScaleLevels", _Options.ZoomScaleLevels);
				SetNodeValue(optionsXmlDoc, "/Options/ThumbsBufferSize", _Options.ThumbsBufferSize);
				SetNodeValue(optionsXmlDoc, "/Options/ThumbSize", _Options.ThumbSize);
				*/
				var optionsXmlDoc = new XmlDocument();
				optionsXmlDoc.LoadXml(xmlRow);
				optionsXmlDoc.Save(optionsFullFileName);
			} catch (Exception) {
				// Nothing
			}			
		}
		
		/// <summary>
		/// Returns node value 
		/// </summary>
		/// <param name="optionsXmlDoc">Options XML document</param>
		/// <param name="nodePath">XPath to node</param>
		/// <param name="valType">Value type</param>
		/// <returns>Node value</returns>
		private string GetNodeValue(XmlDocument optionsXmlDoc, string nodePath, Type valType)
		{
			try {
				var xmlNode = optionsXmlDoc.SelectSingleNode(nodePath);
				if (xmlNode != null) {
					string val = xmlNode.InnerText;
					if (valType == typeof(int)) {
						int intVal;
						if (int.TryParse(val, out intVal)) {
							return val;
						}
					} else if (valType == typeof(bool)) {
						bool boolVal;
						if (bool.TryParse(val, out boolVal)) {
							return val;
						}
					}
					return xmlNode.InnerText;
				}
			} catch (Exception) {
				// Nothing	
			}
			return null;
		}
		
		/*
		/// <summary>
		/// Returns node value 
		/// </summary>
		/// <param name="optionsXmlDoc">Options XML document</param>
		/// <param name="nodePath">XPath to node</param>
		/// <param name="nodeValue">Node value</param>
		private void SetNodeValue(XmlDocument optionsXmlDoc, string nodePath, object nodeValue)
		{
			// ?? Check root node and node exists, if not, create
			try {
				var xmlNode = optionsXmlDoc.SelectSingleNode(nodePath);
				if (xmlNode != null) {
					xmlNode.InnerText = nodeValue.ToString();
				}
			} catch (Exception) {
				// Nothing	
			}
		}
		*/
		#endregion
	}
}
