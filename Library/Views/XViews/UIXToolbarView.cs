//
// mTouch-PDFReader library
// UIXToolbarView.cs (Extended gradient toolbar with shadow)
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

using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;

namespace mTouchPDFReader.Library.XViews
{
	public class UIXToolbarView : UIView
	{	
		private int _ToolbarWidth;
		
		[Export("layerClass")]
		public static Class LayerClass()
		{			
			return new Class(typeof(CAGradientLayer));
		}
		
		public UIXToolbarView(RectangleF frame, float fromWhite, float toWhite, float alpha) : base(frame)
		{
			_ToolbarWidth = -1;
			AutosizesSubviews = true;
			UserInteractionEnabled = true;
			ContentMode = UIViewContentMode.Redraw;
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			BackgroundColor = UIColor.Clear;
			CAGradientLayer layer = (CAGradientLayer)this.Layer;
			layer.Colors = new CGColor[] {
        		UIColor.FromWhiteAlpha(fromWhite, alpha).CGColor,
        		UIColor.FromWhiteAlpha(toWhite, alpha).CGColor
        	};
			layer.CornerRadius = 5;
			layer.ShadowOffset = new SizeF(2.0f, 2.0f);
			layer.ShadowRadius = 4.0f;
			layer.ShadowOpacity = 1.0f;
			layer.ShadowPath = UIBezierPath.FromRect(Bounds).CGPath;
		}
		
		public override void Draw(RectangleF rect)
		{
			base.Draw(rect);
			// Recalc size of the layer shadow, if the toolbar width was changed
			if (_ToolbarWidth != rect.Width) {
				Layer.ShadowPath = UIBezierPath.FromRect(Bounds).CGPath;
			}
		}
	}
}
