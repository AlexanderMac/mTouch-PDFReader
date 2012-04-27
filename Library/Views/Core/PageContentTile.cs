//****************************************//
// mTouch-PDFReader library
// Page content view extended layer 
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace mTouchPDFReader.Library.Views.Core
{
	[Register("PageContentTile")]
	public class PageContentTile : CATiledLayer
	{		
		/// <summary>
		/// Draw layer action
		/// </summary>
		private Action<CGContext> mOnDraw;
		public Action<CGContext> OnDraw {
			get { return mOnDraw; }
			set { mOnDraw = value; }
		}
		
		/// <summary>
		/// Fade duration value
		/// </summary>
		[Export("fadeDuration")]
		public static new double FadeDuration {
			get { return 0.001; }
		}
		
		/// <summary>
		/// Defaul constructor
		/// </summary>
		public PageContentTile() : base()
		{
			Initialize();
        }
		
		/// <summary>
		/// Work constructor
		/// </summary>
        public PageContentTile(IntPtr handle) : base(handle)
        {
			Initialize();
        }
		
		/// <summary>
		/// Initializes layer
		/// </summary>
		public void Initialize()
		{
			LevelsOfDetail = 4;
			LevelsOfDetailBias = 3;
			float wPixels = (UIScreen.MainScreen.Bounds.Width * UIScreen.MainScreen.Scale);
			float hPixels = (UIScreen.MainScreen.Bounds.Height * UIScreen.MainScreen.Scale);
			float max = (wPixels < hPixels) ? hPixels : wPixels;
			float sizeOfTiles = (max < 512.0f) ? 512.0f : 1024.0f;
			TileSize = new SizeF(sizeOfTiles, sizeOfTiles);
		}	
		
		/// <summary>
		/// Draws layers
		/// </summary>
		public override void DrawInContext(CGContext ctx)
		{
			mOnDraw(ctx);
		}
	}
}