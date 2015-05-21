using TodoApp.Styles;
using Xamarin.Forms;

namespace TodoApp.Pages.Base {
	internal class BaseNavigationPage : NavigationPage {
		public BaseNavigationPage(Page root) : base(root) {
			BarBackgroundColor = StyleManager.MainColor;
			BarTextColor = StyleManager.AccentColor;
		}
	}
}