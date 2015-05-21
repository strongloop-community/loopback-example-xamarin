using TodoApp.Pages.Base;
using Xamarin.Forms;

namespace TodoApp.Pages {
	internal class HomePage : MasterDetailPage {
		public HomePage() {
			NavigationPage.SetHasNavigationBar(this, false);
			IsGestureEnabled = false;

			Master = new MenuPage();
			Master.Icon = "action_menu.png";

			var todayPage = new TodayPage ();
			Detail = new BaseNavigationPage(todayPage);

			IsPresentedChanged += (sender, e) => {
				if(IsPresented){
					todayPage.FadeOut();
				} else {
					todayPage.FadeIn();
				}
			};
		}
	}
}