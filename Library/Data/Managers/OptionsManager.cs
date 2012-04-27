//****************************************//
// mTouch-PDFReader library
// Options manager
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using System.Xml;
using System.IO;
using mTouchPDFReader.Library.Data.Enums;
using mTouchPDFReader.Library.Data.Objects;

namespace mTouchPDFReader.Library.Data.Managers
{
	public class OptionsManager
	{		
		#region Constants
		
		/// <summary>
		/// Options file name 
		/// </summary>
		private const string OptionsFileName = "mTouchPDFReader.Options.xml";
		
		#endregion
		
		#region Fields
		
		/// <summary>
		/// Single manager instance 
		/// </summary>
		private static OptionsManager mInstance;
		public static OptionsManager Instance {
			get {
				return mInstance;
			}
			internal set {
				mInstance = value;
			}
		}
				
		/// <summary>
		/// Options 
		/// </summary>
		private Options mOptions;
		public Options Options {
			get {
				if (mOptions == null) {
					mOptions = new Options();
					Load();
				}
				return mOptions;
			}
		}
		
		/// <summary>
		/// Initialized flag 
		/// </summary>
		private bool mInitialized;
		
		#endregion
		
		#region Logic
		
		/// <summary>
		/// Loads options 
		/// </summary>
		public void Load()
		{
			if (mInitialized) {
				return;
			}
			mInitialized = true;
			
			var optionsFullFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), OptionsFileName);
			try {
				var optionsXmlDoc = new XmlDocument();
				optionsXmlDoc.Load(optionsFullFileName);
				object val = GetNodeValue(optionsXmlDoc, "/Options/PageTurningType", typeof(int));
				if (val != null) {
					mOptions.pPageTurningType = (PageTurningType)Convert.ToInt32(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/ToolbarVisible", typeof(bool));
				if (val != null) {
					mOptions.ToolbarVisible = Convert.ToBoolean(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/BottombarVisible", typeof(bool));
				if (val != null) {
					mOptions.BottombarVisible = Convert.ToBoolean(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/PageNumberVisible", typeof(bool));
				if (val != null) {
					mOptions.PageNumberVisible = Convert.ToBoolean(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/NoteBtnVisible", typeof(bool));
				if (val != null) {
					mOptions.NoteBtnVisible = Convert.ToBoolean(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/BookmarksBtnVisible", typeof(bool));
				if (val != null) {
					mOptions.BookmarksBtnVisible = Convert.ToBoolean(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/ThumbsBtnVisible", typeof(bool));
				if (val != null) {
					mOptions.ThumbsBtnVisible = Convert.ToBoolean(val);
				}
				/*
				object val = GetNodeValue(optionsXmlDoc, "/Options/BackgroundColor", typeof(byte));
				if (val != null) {
					mOptions.BackgroundColor = DefaultBackgroundColor;
				}
				*/			
				val = GetNodeValue(optionsXmlDoc, "/Options/AllowZoomByDoubleTouch", typeof(bool));
				if (val != null) {
					mOptions.AllowZoomByDoubleTouch = Convert.ToBoolean(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/ZoomScaleLevels", typeof(int));
				if (val != null) {
					mOptions.ZoomScaleLevels = Convert.ToInt32(val);	
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/ThumbsBufferSize", typeof(int));
				if (val != null) {
					mOptions.ThumbsBufferSize = Convert.ToInt32(val);
				}
				val = GetNodeValue(optionsXmlDoc, "/Options/ThumbSize", typeof(int));
				if (val != null) {
					mOptions.ThumbSize = Convert.ToInt32(val);
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
			if (!mInitialized) {
				return;
			}
			
			var optionsFullFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), OptionsFileName);
			try {
				string xmlRow = 
"<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +						
"<Options>" + 
"	<PageTurningType>" + (int)mOptions.pPageTurningType + "</PageTurningType>" +
"	<ToolbarVisible>" + mOptions.ToolbarVisible + "</ToolbarVisible>" +
"	<BottombarVisible>" + mOptions.BottombarVisible + "</BottombarVisible>" + 
"	<PageNumberVisible>" + mOptions.PageNumberVisible + "</PageNumberVisible>" +
"	<NoteBtnVisible>" + mOptions.NoteBtnVisible + "</NoteBtnVisible>" +
"	<BookmarksBtnVisible>" + mOptions.BookmarksBtnVisible + "</BookmarksBtnVisible>" +
"	<ThumbsBtnVisible>" + mOptions.ThumbsBtnVisible + "</ThumbsBtnVisible>" +
"	<AllowZoomByDoubleTouch>" + mOptions.AllowZoomByDoubleTouch + "</AllowZoomByDoubleTouch>" +
"	<ZoomScaleLevels>" + mOptions.ZoomScaleLevels + "</ZoomScaleLevels>" +
"	<ThumbsBufferSize>" + mOptions.ThumbsBufferSize + "</ThumbsBufferSize>" +
"	<ThumbSize>" + mOptions.ThumbSize + "</ThumbSize>" +
"</Options>";
				
				/*
				SetNodeValue(optionsXmlDoc, "/Options/PageTurningType", mOptions.pPageTurningType);
				SetNodeValue(optionsXmlDoc, "/Options/ToolbarVisible", mOptions.ToolbarVisible);
				SetNodeValue(optionsXmlDoc, "/Options/StatusbarVisible", mOptions.StatusbarVisible);
				SetNodeValue(optionsXmlDoc, "/Options/PageNumberVisible", mOptions.PageNumberVisible);
				SetNodeValue(optionsXmlDoc, "/Options/NoteBtnVisible", mOptions.NoteBtnVisible);
				SetNodeValue(optionsXmlDoc, "/Options/BookmarksBtnVisible", mOptions.BookmarksBtnVisible);
				SetNodeValue(optionsXmlDoc, "/Options/ThumbsBtnVisible", mOptions.ThumbsBtnVisible);				
				//SetNodeValue(optionsXmlDoc, "/Options/BackgroundColor", mOptions.BackgroundColor);
				SetNodeValue(optionsXmlDoc, "/Options/AllowZoomByDoubleTouch", mOptions.AllowZoomByDoubleTouch);
				SetNodeValue(optionsXmlDoc, "/Options/ZoomScaleLevels", mOptions.ZoomScaleLevels);
				SetNodeValue(optionsXmlDoc, "/Options/ThumbsBufferSize", mOptions.ThumbsBufferSize);
				SetNodeValue(optionsXmlDoc, "/Options/ThumbSize", mOptions.ThumbSize);
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
