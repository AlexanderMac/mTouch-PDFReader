//
// mTouch-PDFReader library
// PageView.cs (Page View )
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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using mTouchPDFReader.Library.Utils;
using mTouchPDFReader.Library.Interfaces;
using mTouchPDFReader.Library.Managers;
using mTouchPDFReader.Library.Data.Enums;

namespace mTouchPDFReader.Library.Views.Core
{
	public class PageView : UIScrollView
	{
		#region Constants		
		/// <summary>
		/// Content view padding
		/// </summary>
		private const float ContentViewPadding = 5.0f;
				
		/// <summary>
		/// Thumb content size
		/// </summary>
		private const int ThumbContentSize = 500;		
		#endregion
			
		#region Fields		
		/// <summary>
		/// The page content view.
		/// </summary>
		private PageContentView _PageContentView;
		
		/// <summary>
		/// The page thumb view.
		/// </summary>
		private ThumbView _ThumbView;
		
		/// <summary>
		/// The page view container.
		/// </summary>
		private UIView _PageContentContainerView;	
		
		/// <summary>
		/// The zoom scale step.
		/// </summary>
		private float _ZoomScaleStep;
		#endregion

		#region Properties		
		/// <summary>
		/// Gets or sets the opened page number.
		/// </summary>
		public int PageNumber {
			get {
				return _PageContentView.PageNumber;
			}
			set {
				_PageContentView.PageNumber = value;
			}
		}		

		/// <summary>
		/// Gets or sets the auto scale modes.
		/// </summary>
		public AutoScaleModes AutoScaleMode { get; set; }
		#endregion
		
		#region UIScrollView		
		/// <summary>
		/// Working.
		/// </summary>
		public PageView(RectangleF frame, AutoScaleModes autoScaleMode, int pageNumber) : base(frame)
		{
			// Init page scroll view
			ScrollsToTop = false;
			DelaysContentTouches = false;
			ShowsVerticalScrollIndicator = false;
			ShowsHorizontalScrollIndicator = false;
			ContentMode = UIViewContentMode.Redraw;
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			UserInteractionEnabled = true;
			AutosizesSubviews = false;
			BackgroundColor = UIColor.Clear;
			ViewForZoomingInScrollView = delegate(UIScrollView sender) { 
				return _PageContentContainerView; 
			};
			AutoScaleMode = autoScaleMode;
			
			// Create and init (calc frame size) page content view
			_PageContentView = new PageContentView(PageContentView.GetPageViewSize(pageNumber), pageNumber);
			
			// Create and init page content container view
			_PageContentContainerView = new UIView(_PageContentView.Bounds);
			_PageContentContainerView.AutosizesSubviews = false;
			_PageContentContainerView.UserInteractionEnabled = false;
			_PageContentContainerView.ContentMode = UIViewContentMode.Redraw;
			_PageContentContainerView.AutoresizingMask = UIViewAutoresizing.None;
			_PageContentContainerView.Layer.CornerRadius = 5;
			_PageContentContainerView.Layer.ShadowOffset = new SizeF(2.0f, 2.0f);
			_PageContentContainerView.Layer.ShadowRadius = 4.0f;
			_PageContentContainerView.Layer.ShadowOpacity = 1.0f;
			_PageContentContainerView.Layer.ShadowPath = UIBezierPath.FromRect(_PageContentContainerView.Bounds).CGPath;
			_PageContentContainerView.BackgroundColor = UIColor.White;			
			
			// Create and init page thumb view
			//_ThumbView = new ThumbView(_PageContentView.Bounds, ThumbContentSize, pageNumber);
			
			// Add views to parents
			//_PageContentContainerView.AddSubview(_ThumbView);
			_PageContentContainerView.AddSubview(_PageContentView);						
			AddSubview(_PageContentContainerView);
			
			// Set content size, offset and inset
			ContentSize = _PageContentView.Bounds.Size;
			ContentOffset = new PointF((0.0f - ContentViewPadding), (0.0f - ContentViewPadding));
			ContentInset = new UIEdgeInsets(ContentViewPadding, ContentViewPadding, ContentViewPadding, ContentViewPadding);
			ContentSize = _PageContentContainerView.Bounds.Size;
		}
		
		/// <summary>
		/// Layouts the subviews.
		/// </summary>
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();	

			_UpdateMinimumMaximumZoom();
			_ResetZoom();
			_ResetScrollOffset();

			_AlignPageContentView();						
		}
		
		/// <summary>
		/// Touch begans action handler.
		/// </summary>
		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			
			if (MgrAccessor.OptionsMgr.Options.AllowZoomByDoubleTouch) {
				var touch = touches.AnyObject as UITouch; 
				if (touch.TapCount == 2) { 
					ZoomIncrement(); 
				}
			}			
		}		
		#endregion
		
		#region Logic
		/// <summary>
		/// Aligns the page content view.
		/// </summary>
		private void _AlignPageContentView()
		{
			SizeF boundsSize = Bounds.Size;
			RectangleF viewFrame = _PageContentContainerView.Frame;		
			if (viewFrame.Size.Width < boundsSize.Width) {
				viewFrame.X = (boundsSize.Width - viewFrame.Size.Width) / 2.0f + ContentOffset.X;
			} else {
				viewFrame.X = 0.0f;
			}		
			if (viewFrame.Size.Height < boundsSize.Height) {
				viewFrame.Y = (boundsSize.Height - viewFrame.Size.Height) / 2.0f + ContentOffset.Y;
			} else {
				viewFrame.Y = 0.0f;
			}	
			_PageContentContainerView.Frame = viewFrame;
		}

		/// <summary>
		/// Gets zoom factor for the source rect to fits the target rect.
		/// </summary>
		/// <param name="target">The targer rect.</param>
		/// <param name="source">The source rect.</param>
		/// <returns>The zoom scale factor.</returns>
		private float _GetZoomScaleThatFits(SizeF target, SizeF source)
		{
			float wScale = target.Width / source.Width;
			float hScale = target.Height / source.Height;
			var factor = AutoScaleMode == AutoScaleModes.AutoWidth 
				? (wScale < hScale ? hScale : wScale)
				: (wScale < hScale ? wScale : hScale);
			return factor;
		}
		
		/// <summary>
		/// Updates the minimum/maximum zoom scale factor.
		/// </summary>
		private void _UpdateMinimumMaximumZoom()
		{
			RectangleF targetRect = RectangleFExtensions.Inset(Bounds, ContentViewPadding, ContentViewPadding);
			float zoomScale = _GetZoomScaleThatFits(targetRect.Size, _PageContentView.Bounds.Size);
			MinimumZoomScale = zoomScale; 	
			MaximumZoomScale = zoomScale * MgrAccessor.OptionsMgr.Options.ZoomScaleLevels; 
			_ZoomScaleStep = (MaximumZoomScale - MinimumZoomScale) / MgrAccessor.OptionsMgr.Options.ZoomScaleLevels;
		}

		/// <summary>
		/// Resets the scroll offset.
		/// </summary>
		private void _ResetScrollOffset()
		{
			SetContentOffset(new PointF(0.0f, 0.0f), false);
		}

		/// <summary>
		/// Resets the zoom scale factor.
		/// </summary>
		private void _ResetZoom()
		{
			ZoomScale = MinimumZoomScale;
		}
		
		/// <summary>
		/// Decrements the zoom scale factor.
		/// </summary>
		public void ZoomDecrement()
		{
			float zoomScale = ZoomScale;
			if (zoomScale > MinimumZoomScale) {
				zoomScale -= _ZoomScaleStep;
				if (zoomScale < MinimumZoomScale) {
					zoomScale = MinimumZoomScale;
				}
				SetZoomScale(zoomScale, true);
			}
		}
		
		/// <summary>
		/// Increments the zoom scale factor.
		/// </summary>
		public void ZoomIncrement()
		{
			float zoomScale = ZoomScale;
			if (zoomScale < MaximumZoomScale) {
				zoomScale += _ZoomScaleStep;
				if (zoomScale > MaximumZoomScale) {
					zoomScale = MaximumZoomScale;
				}
				SetZoomScale(zoomScale, true);
			}
		}		
		#endregion
	}
}
