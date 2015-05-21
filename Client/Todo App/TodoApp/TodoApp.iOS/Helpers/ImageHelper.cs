using System;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace TodoApp.iOS.Helpers {
	internal class ImageHelper {
		/// <summary>
		///    Creates grayscaled image from existing image.
		/// </summary>
		/// <param name="oldImage">Image to convert.</param>
		/// <returns>Returns grayscaled image.</returns>
		public static UIImage GrayscaleImage( UIImage oldImage ) {
			var imageRect = new RectangleF(PointF.Empty, (SizeF) oldImage.Size);

			CGImage grayImage;

			// Create gray image.
			using (CGColorSpace colorSpace = CGColorSpace.CreateDeviceGray()) {
				using (var context = new CGBitmapContext(IntPtr.Zero, (int) imageRect.Width, (int) imageRect.Height, 8, 0, colorSpace, CGImageAlphaInfo.None)) {
					context.DrawImage(imageRect, oldImage.CGImage);
					grayImage = context.ToImage();
				}
			}

			// Create mask for transparent areas.
			using (var context = new CGBitmapContext(IntPtr.Zero, (int) imageRect.Width, (int) imageRect.Height, 8, 0, CGColorSpace.Null, CGBitmapFlags.Only)) {
				context.DrawImage(imageRect, oldImage.CGImage);
				CGImage alphaMask = context.ToImage();
				var newImage = new UIImage(grayImage.WithMask(alphaMask));

				grayImage.Dispose();
				alphaMask.Dispose();

				return newImage;
			}
		}
	}
}