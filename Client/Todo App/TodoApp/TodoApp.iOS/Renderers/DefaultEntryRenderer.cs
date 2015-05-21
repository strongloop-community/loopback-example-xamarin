using CoreGraphics;
using Foundation;
using TodoApp.Controls.DefaultControls;
using TodoApp.iOS.Renderers;
using TodoApp.Styles;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer( typeof (DefaultEntry), typeof (DefaultEntryRenderer) )]

namespace TodoApp.iOS.Renderers {
	public class DefaultEntryRenderer : EntryRenderer {
		protected override void OnElementChanged( ElementChangedEventArgs<Entry> e ) {
			base.OnElementChanged(e);

			if (Control == null)
				return;
			Control.BorderStyle = UITextBorderStyle.None;

			if (Control.Placeholder != null)
				Control.AttributedPlaceholder = new NSAttributedString(Control.Placeholder, UIFont.ItalicSystemFontOfSize(15), StyleManager.AccentColor.ToUIColor());

			Control.Font = UIFont.FromName("HelveticaNeue-LightItalic", 15);

			// Add left padding. 
			const int paddingLeft = 15;
			Control.LeftView = new UIView(new CGRect(0, 0, paddingLeft, 20));
			Control.LeftViewMode = UITextFieldViewMode.Always;
		}
	}
}