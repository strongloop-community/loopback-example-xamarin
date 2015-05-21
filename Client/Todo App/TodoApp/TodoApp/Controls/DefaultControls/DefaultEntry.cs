using TodoApp.Styles;
using Xamarin.Forms;

namespace TodoApp.Controls.DefaultControls {
	public class DefaultEntry : Entry {
		public DefaultEntry() {
			BackgroundColor = StyleManager.DarkAccentColor;
			TextColor = StyleManager.AccentColor;
			HorizontalOptions = LayoutOptions.Fill;
			HeightRequest = 40;
		}
	}
}