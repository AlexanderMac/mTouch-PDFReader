//
// mTouch-PDFReader library
// SettingsManager.cs
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

using System;
using System.Xml;
using System.IO;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Interfaces;
using mTouchPDFReader.Library.Data.Objects;
using mTouchPDFReader.Library.Data.Enums;

namespace mTouchPDFReader.Library.Managers
{
	public class SettingsManager : ISettingsManager
	{		
		#region Data		
		private const string SettingsFileName = "mTouchPDFReader.Settings.xml";		
					
		public Settings Settings {
			get {
				if (_settings == null) {
					_settings = new Settings();
					Load();
				}
				return _settings;
			}
		}
		private Settings _settings;
		
		private bool _initialized;		
		#endregion
		
		#region Logic
		protected SettingsManager()	{}

		public virtual void Load()
		{
			if (_initialized) {
				return;
			}
			_initialized = true;

			try {
				var settingsFullFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), SettingsFileName);
				var settingsXmlDoc = new XmlDocument();
				settingsXmlDoc.Load(settingsFullFileName);

				object val = getNodeValue(settingsXmlDoc, "/Settings/PageTransitionStyle", typeof(int));
				if (val != null) {
					_settings.PageTransitionStyle = (UIPageViewControllerTransitionStyle)Convert.ToInt32(val);
				}
				val = getNodeValue(settingsXmlDoc, "/Settings/PageNavigationOrientation", typeof(int));
				if (val != null) {
					_settings.PageNavigationOrientation = (UIPageViewControllerNavigationOrientation)Convert.ToInt32(val);
				}
				val = getNodeValue(settingsXmlDoc, "/Settings/TopToolbarVisible", typeof(bool));
				if (val != null) {
					_settings.TopToolbarVisible = Convert.ToBoolean(val);
				}
				val = getNodeValue(settingsXmlDoc, "/Settings/BottomToolbarVisible", typeof(bool));
				if (val != null) {
					_settings.BottomToolbarVisible = Convert.ToBoolean(val);
				}
				val = getNodeValue(settingsXmlDoc, "/Settings/AllowZoomByDoubleTouch", typeof(bool));
				if (val != null) {
					_settings.AllowZoomByDoubleTouch = Convert.ToBoolean(val);
				}
				val = getNodeValue(settingsXmlDoc, "/Settings/AutoScaleMode", typeof(int));
				if (val != null) {
					_settings.AutoScaleMode = (AutoScaleModes)Convert.ToInt32(val);
				}
				val = getNodeValue(settingsXmlDoc, "/Settings/ZoomScaleLevels", typeof(int));
				if (val != null) {
					_settings.ZoomScaleLevels = Convert.ToInt32(val);	
				}
				val = getNodeValue(settingsXmlDoc, "/Settings/ThumbsBufferSize", typeof(int));
				if (val != null) {
					_settings.ThumbsBufferSize = Convert.ToInt32(val);
				}
				val = getNodeValue(settingsXmlDoc, "/Settings/ThumbSize", typeof(int));
				if (val != null) {
					_settings.ThumbSize = Convert.ToInt32(val);
				}
			} catch (Exception ex) {
				Console.WriteLine("SettingsManager.Load exception: " + ex.ToString());
			}
		}
		
		public virtual void Save()
		{
			if (!_initialized) {
				return;
			}

			try {
				string xmlRow = 
					"<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + 
					"<Settings>" + 
					"	<PageTransitionStyle>" + (int)_settings.PageTransitionStyle + "</PageTransitionStyle>" +
					"	<PageNavigationOrientation>" + (int)_settings.PageNavigationOrientation + "</PageNavigationOrientation>" +
					"	<TopToolbarVisible>" + _settings.TopToolbarVisible + "</TopToolbarVisible>" +
					"	<BottomToolbarVisible>" + _settings.BottomToolbarVisible + "</BottomToolbarVisible>" +
					"	<AllowZoomByDoubleTouch>" + _settings.AllowZoomByDoubleTouch + "</AllowZoomByDoubleTouch>" +
					"	<ZoomScaleLevels>" + _settings.ZoomScaleLevels + "</ZoomScaleLevels>" +
					"	<AutoScaleMode>" + (int)_settings.AutoScaleMode + "</AutoScaleMode>" +
					"	<ThumbsBufferSize>" + _settings.ThumbsBufferSize + "</ThumbsBufferSize>" +
					"	<ThumbSize>" + _settings.ThumbSize + "</ThumbSize>" +
					"</Settings>";

				var settingsFullFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), SettingsFileName);
				var settingsXmlDoc = new XmlDocument();
				settingsXmlDoc.LoadXml(xmlRow);
				settingsXmlDoc.Save(settingsFullFileName);
			} catch (Exception ex) {
				Console.WriteLine("SettingsManager.Save exception: " + ex.ToString());
			}			
		}
		
		private string getNodeValue(XmlDocument optionsXmlDoc, string nodePath, Type valType)
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
			} catch (Exception ex) {
				Console.WriteLine("SettingsManager.getNodeValue exception: " + ex.ToString());
			}
			return null;
		}
		#endregion
	}
}
