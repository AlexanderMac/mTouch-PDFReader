//
// mTouch-PDFReader library
// PageContentTile.cs
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
		#region Data
		public Action<CGContext> OnDraw {
			get { return _onDraw; }
			set { _onDraw = value; }
		}
		private Action<CGContext> _onDraw;
		
		[Export("fadeDuration")]
		public static new double FadeDuration {
			get { return 0.001; }
		}
		#endregion

		#region Logic
		public PageContentTile()
		{
			Initialize();
        }
		
        public PageContentTile(IntPtr handle) : base(handle)
        {
			Initialize();
        }
		
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
		
		public override void DrawInContext(CGContext ctx)
		{
			_onDraw(ctx);
		}
		#endregion
	}
}