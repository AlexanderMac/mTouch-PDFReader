//
// mTouch-PDFReader library
// PageContentTile.cs (Page content view extended layer )
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
		/// Gets or sets the draw layer action.
		/// </summary>
		public Action<CGContext> OnDraw {
			get { return _OnDraw; }
			set { _OnDraw = value; }
		}
		private Action<CGContext> _OnDraw;
		
		/// <summary>
		/// Gets the fade duration.
		/// </summary>
		[Export("fadeDuration")]
		public static new double FadeDuration {
			get { return 0.001; }
		}
		
		/// <summary>
		/// Default.
		/// </summary>
		public PageContentTile() : base()
		{
			Initialize();
        }
		
		/// <summary>
		/// Working.
		/// </summary>
        public PageContentTile(IntPtr handle) : base(handle)
        {
			Initialize();
        }
		
		/// <summary>
		/// Initializes the layer.
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
		/// Draws the layer.
		/// </summary>
		public override void DrawInContext(CGContext ctx)
		{
			_OnDraw(ctx);
		}
	}
}