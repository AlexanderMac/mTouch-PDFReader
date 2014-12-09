//
// mTouch-PDFReader library
// SettingsManager.cs
//
// Copyright (c) 2012-2014 AlexanderMac(amatsibarov@gmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// 'Software'), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions:

// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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
		private const string SettingsFileName = "mTouchPDFReader.Settings.v30.xml";		
					
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
