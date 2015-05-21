using Xamarin.Forms;

namespace TodoApp.Controls.DefaultControls {
	internal class ItalicLabel : CustomFontLabel {
		public ItalicLabel() {
			HorizontalOptions = LayoutOptions.Center;
			FontAttributes = FontAttributes.Italic;
		}
	}
}