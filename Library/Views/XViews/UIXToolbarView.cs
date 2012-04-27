//****************************************//
// mTouch-PDFReader library
// Extended gradient toolbar with shadow
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
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
		/// <summary>
		/// Width of the toolbar 
		/// </summary>
		private int mToolbarWidth;
		
		/// <summary>
		/// Instruct that needed a CAGradientLayer (not the default CALayer) for the Layer property 
		/// </summary>
		[Export("layerClass")]
		public static Class LayerClass()
		{			
			return new Class(typeof(CAGradientLayer));
		}
		
		/// <summary>
		/// Work constructor
		/// </summary>
		/// <param name="frame">Toolbar frame</param>
		/// <param name="fromWhite">From white index</param>
		/// <param name="toWhite">To white index</param>
		/// <param name="alpha">Alpha channel</param>
        public UIXToolbarView(RectangleF frame, float fromWhite, float toWhite, float alpha) : base(frame)
        {
        	mToolbarWidth = -1;
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
		
		/// <summary>
		/// Calls when view is drawing
		/// </summary>
		public override void Draw(RectangleF rect)
		{
			base.Draw(rect);
			// Recalc size of the layer shadow, if the toolbar width was changed
			if (mToolbarWidth != rect.Width) {
				Layer.ShadowPath = UIBezierPath.FromRect(Bounds).CGPath;
			}
        }
    }
}
