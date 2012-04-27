//****************************************//
// mTouch-PDFReader library
// Page View 
//
// Created by Matsibarov Alexander. 2012.
// Copyright Matsibarov Alexander 2012. All rights reserved.
//
// www.mtouch-pdfreader.com
//****************************************//

using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using mTouchPDFReader.Library.Data.Managers;

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
		/// Page content view
		/// </summary>
		private PageContentView mPageContentView;
		
		/// <summary>
		/// Page thumb view
		/// </summary>
		private ThumbView mThumbView;
		
		/// <summary>
		/// Page view container
		/// </summary>
		private UIView mPageContentContainerView;	
		
		/// <summary>
		/// Zoom scale step
		/// </summary>
		private float mZoomScaleStep;
		
		/// <summary>
		/// Page number displayed in view
		/// </summary>
		public int PageNumber {
			get {
				return mPageContentView.PageNumber;
			}
			set {
				mPageContentView.PageNumber = value;
			}
		}
		
		#endregion
		
		#region UIScrollView methods
		
		/// <summary>
		/// Work constructor
		/// </summary>
		public PageView(RectangleF frame, int pageNumber) : base(frame)
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
				return mPageContentContainerView; 
			};
			
			// Create and init (calc frame size) page content view
			mPageContentView = new PageContentView(PageContentView.GetPageViewSize(pageNumber), pageNumber);
			
			// Create and init page content container view
			mPageContentContainerView = new UIView(mPageContentView.Bounds);
			mPageContentContainerView.AutosizesSubviews = false;
			mPageContentContainerView.UserInteractionEnabled = false;
			mPageContentContainerView.ContentMode = UIViewContentMode.Redraw;
			mPageContentContainerView.AutoresizingMask = UIViewAutoresizing.None;
			mPageContentContainerView.Layer.CornerRadius = 5;
			mPageContentContainerView.Layer.ShadowOffset = new SizeF(2.0f, 2.0f);
			mPageContentContainerView.Layer.ShadowRadius = 4.0f;
			mPageContentContainerView.Layer.ShadowOpacity = 1.0f;
			mPageContentContainerView.Layer.ShadowPath = UIBezierPath.FromRect(mPageContentContainerView.Bounds).CGPath;
			mPageContentContainerView.BackgroundColor = UIColor.White;			
			
			// Create and init page thumb view
			mThumbView = new ThumbView(mPageContentView.Bounds, ThumbContentSize, pageNumber);
			
			// Add views to parents
			mPageContentContainerView.AddSubview(mThumbView);
			mPageContentContainerView.AddSubview(mPageContentView);						
			AddSubview(mPageContentContainerView);
			
			// Set content size, offset and inset
			ContentSize = mPageContentView.Bounds.Size;
			ContentOffset = new PointF((0.0f - ContentViewPadding), (0.0f - ContentViewPadding));
			ContentInset = new UIEdgeInsets(ContentViewPadding, ContentViewPadding, ContentViewPadding, ContentViewPadding);
			ContentSize = mPageContentContainerView.Bounds.Size;
			
			// Setup zoom scale
			UpdateMinimumMaximumZoom();
			ZoomScale = MinimumZoomScale;
		}
		
		/// <summary>
		/// Calls when subview needs to layout - Centers PageContentView in parent view
		/// </summary>
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			
			SizeF boundsSize = Bounds.Size;
			RectangleF viewFrame = mPageContentContainerView.Frame;		
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
			mPageContentContainerView.Frame = viewFrame;
		}
		
		/// <summary>
		/// Calls when users taps view
		/// </summary>
		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			
			if (OptionsManager.Instance.Options.AllowZoomByDoubleTouch) {
				var touch = touches.AnyObject as UITouch; 
				if (touch.TapCount == 2) { 
					ZoomIncrement(); 
				}
			}			
		}
		
		#endregion
		
		#region Logic
		
		/// <summary>
		/// Gets zoom factor for source rect, which fits target rect
		/// </summary>
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <returns> </returns>
		private static float GetZoomScaleThatFits(SizeF target, SizeF source)
		{
			float wScale = target.Width / source.Width;
			float hScale = target.Height / source.Height;
			return ((wScale < hScale) ? wScale : hScale);
		}
		
		/// <summary>
		/// Updates minimum/maximum zoom scale
		/// </summary>
		public void UpdateMinimumMaximumZoom()
		{
			RectangleF targetRect = RectangleFExtensions.Inset(Bounds, ContentViewPadding, ContentViewPadding);
			float zoomScale = GetZoomScaleThatFits(targetRect.Size, mPageContentView.Bounds.Size);
			MinimumZoomScale = zoomScale; 	
			MaximumZoomScale = zoomScale * OptionsManager.Instance.Options.ZoomScaleLevels; 
			mZoomScaleStep = (MaximumZoomScale - MinimumZoomScale) / OptionsManager.Instance.Options.ZoomScaleLevels;
		}
		
		/// <summary>
		/// Resets zoom scale
		/// </summary>
		public void ZoomReset()
		{
			ZoomScale = MinimumZoomScale;
		}
		
		/// <summary>
		/// Decrements zoom scale
		/// </summary>
		public void ZoomDecrement()
		{
			float zoomScale = ZoomScale;
			if (zoomScale > MinimumZoomScale) {
				zoomScale -= mZoomScaleStep;
				if (zoomScale < MinimumZoomScale) {
					zoomScale = MinimumZoomScale;
				}
				SetZoomScale(zoomScale, true);
			}
		}
		
		/// <summary>
		/// Increments zoom scale
		/// </summary>
		public void ZoomIncrement()
		{
			float zoomScale = ZoomScale;
			if (zoomScale < MaximumZoomScale) {
				zoomScale += mZoomScaleStep;
				if (zoomScale > MaximumZoomScale) {
					zoomScale = MaximumZoomScale;
				}
				SetZoomScale(zoomScale, true);
			}
		}
		
		#endregion
	}
}
