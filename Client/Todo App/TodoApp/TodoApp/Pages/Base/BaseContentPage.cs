using TodoApp.Styles;
using Xamarin.Forms;

namespace TodoApp.Pages.Base {
	public class BaseContentPage : ContentPage {
		public BaseContentPage() {
			Title = "Todo app";
			BackgroundColor = StyleManager.MainColor;
			NavigationPage.SetBackButtonTitle(this, "");
		}
	}
}