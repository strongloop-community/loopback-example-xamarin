using System;
using TodoApp.Controls.DefaultControls;
using TodoApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer( typeof (CustomFontLabel), typeof (LabelFontRenderer) )]

namespace TodoApp.iOS.Renderers {
	internal class LabelFontRenderer : LabelRenderer {
		protected override void OnElementChanged( ElementChangedEventArgs<Label> e ) {
			base.OnElementChanged(e);

			UIFont font = Control.Font;
			float? size = font.FontDescriptor.Size;
			if (size == null)
				return;

			try {
				switch (Element.FontAttributes) {
					case FontAttributes.None:
						Control.Font = UIFont.FromName("HelveticaNeue-Light", (nfloat) size);
						break;
					case FontAttributes.Italic | FontAttributes.Bold:
						Control.Font = UIFont.FromName("HelveticaNeue-BoldItalic", (nfloat) size);
						break;
					case FontAttributes.Italic:
						Control.Font = UIFont.FromName("HelveticaNeue-LightItalic", (nfloat) size);
						break;
					case FontAttributes.Bold:
						Control.Font = UIFont.FromName("HelveticaNeue-CondensedBold", (nfloat) size);
						break;
				}
			} catch (Exception ex) {
				string msg = ex.Message;
			}
		}
	}
}