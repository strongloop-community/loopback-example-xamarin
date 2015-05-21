using Xamarin.Forms;

namespace TodoApp.Controls.DefaultControls {
	public class DefaultLabel : Label {
		public DefaultLabel() {
			VerticalOptions = LayoutOptions.Center;
			TextColor = Color.White;
			FontSize = Device.GetNamedSize(NamedSize.Medium, this);
		}

		public NamedSize CustomFontSize {
			set { FontSize = Device.GetNamedSize(value, this); }
		}
	}
}