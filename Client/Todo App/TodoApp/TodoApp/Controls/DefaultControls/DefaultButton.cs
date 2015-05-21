using TodoApp.Styles;
using Xamarin.Forms;

namespace TodoApp.Controls.DefaultControls {
	public class DefaultButton : Button {
		public DefaultButton() {
			BackgroundColor = StyleManager.AccentColor;
			TextColor = StyleManager.MainColor;
			HorizontalOptions = LayoutOptions.Fill;
			HeightRequest = 35;
			FontAttributes = FontAttributes.Bold;
		}
	}
}