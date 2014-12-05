//
// mTouch-PDFReader library
// PageView.cs
//
// Copyright (c) 2012-2014 Alexander Matsibarov (amatsibarov@gmail.com)
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

using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using mTouchPDFReader.Library.Managers;
using mTouchPDFReader.Library.Data.Enums;

namespace mTouchPDFReader.Library.Views.Core
{
	public class PageView : UIScrollView
	{
		#region Data		
		private const float ContentViewPadding = 5.0f;
		private const int ThumbContentSize = 500;		

		private readonly PageContentView _pageContentView;
		// TODO: private readonly ThumbView _thumbView;
		private readonly UIView _pageContentContainerView;	
		private float _zoomScaleStep;
	
		public int PageNumber {
			get {
				return _pageContentView.PageNumber;
			}
			set {
				_pageContentView.PageNumber = value;
			}
		}		

		public bool NeedUpdateZoomAndOffset { get; set; }
		public AutoScaleModes AutoScaleMode { get; set; }
		#endregion
		
		#region Logic		
		public PageView(RectangleF frame, AutoScaleModes autoScaleMode, int pageNumber) : base(frame)
		{
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
				return _pageContentContainerView; 
			};
			AutoScaleMode = autoScaleMode;
			NeedUpdateZoomAndOffset = true;
			
			_pageContentView = new PageContentView(PageContentView.GetPageViewSize(pageNumber), pageNumber);
			
			_pageContentContainerView = new UIView(_pageContentView.Bounds);
			_pageContentContainerView.AutosizesSubviews = false;
			_pageContentContainerView.UserInteractionEnabled = false;
			_pageContentContainerView.ContentMode = UIViewContentMode.Redraw;
			_pageContentContainerView.AutoresizingMask = UIViewAutoresizing.None;
			_pageContentContainerView.Layer.CornerRadius = 5;
			_pageContentContainerView.Layer.ShadowOffset = new SizeF(2.0f, 2.0f);
			_pageContentContainerView.Layer.ShadowRadius = 4.0f;
			_pageContentContainerView.Layer.ShadowOpacity = 1.0f;
			_pageContentContainerView.Layer.ShadowPath = UIBezierPath.FromRect(_pageContentContainerView.Bounds).CGPath;
			_pageContentContainerView.BackgroundColor = UIColor.White;			
			
			// TODO: _ThumbView = new ThumbView(_PageContentView.Bounds, ThumbContentSize, pageNumber);
			
			// TODO: _PageContentContainerView.AddSubview(_ThumbView);
			_pageContentContainerView.AddSubview(_pageContentView);						
			AddSubview(_pageContentContainerView);
			
			ContentSize = _pageContentView.Bounds.Size;
			ContentOffset = new PointF((0.0f - ContentViewPadding), (0.0f - ContentViewPadding));
			ContentInset = new UIEdgeInsets(ContentViewPadding, ContentViewPadding, ContentViewPadding, ContentViewPadding);
			ContentSize = _pageContentContainerView.Bounds.Size;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();	

			if (NeedUpdateZoomAndOffset) {
				updateMinimumMaximumZoom();
				resetZoom();
				resetScrollOffset();
				NeedUpdateZoomAndOffset = false;
			}	

			alignPageContentView();
		}
		
		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			
			if (MgrAccessor.SettingsMgr.Settings.AllowZoomByDoubleTouch) {
				var touch = touches.AnyObject as UITouch; 
				if (touch.TapCount == 2) { 
					ZoomIncrement(); 
				}
			}			
		}		

		private void alignPageContentView()
		{
			SizeF boundsSize = Bounds.Size;
			RectangleF viewFrame = _pageContentContainerView.Frame;		
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
			_pageContentContainerView.Frame = viewFrame;
		}

		private float getZoomScaleThatFits(SizeF target, SizeF source)
		{
			float wScale = target.Width / source.Width;
			float hScale = target.Height / source.Height;
			var factor = AutoScaleMode == AutoScaleModes.AutoWidth 
				? (wScale < hScale ? hScale : wScale)
				: (wScale < hScale ? wScale : hScale);
			return factor;
		}
		
		private void updateMinimumMaximumZoom()
		{
			RectangleF targetRect = RectangleFExtensions.Inset(Bounds, ContentViewPadding, ContentViewPadding);
			float zoomScale = getZoomScaleThatFits(targetRect.Size, _pageContentView.Bounds.Size);
			MinimumZoomScale = zoomScale; 	
			MaximumZoomScale = zoomScale * MgrAccessor.SettingsMgr.Settings.ZoomScaleLevels; 
			_zoomScaleStep = (MaximumZoomScale - MinimumZoomScale) / MgrAccessor.SettingsMgr.Settings.ZoomScaleLevels;
		}

		private void resetScrollOffset()
		{
			SetContentOffset(new PointF(0.0f, 0.0f), false);
		}

		private void resetZoom()
		{
			ZoomScale = MinimumZoomScale;
		}

		public void ZoomDecrement()
		{
			float zoomScale = ZoomScale;
			if (zoomScale > MinimumZoomScale) {
				zoomScale -= _zoomScaleStep;
				if (zoomScale < MinimumZoomScale) {
					zoomScale = MinimumZoomScale;
				}
				SetZoomScale(zoomScale, true);
			}
		}
		
		public void ZoomIncrement()
		{
			float zoomScale = ZoomScale;
			if (zoomScale < MaximumZoomScale) {
				zoomScale += _zoomScaleStep;
				if (zoomScale > MaximumZoomScale) {
					zoomScale = MaximumZoomScale;
				}
				SetZoomScale(zoomScale, true);
			}
		}		
		#endregion
	}
}
