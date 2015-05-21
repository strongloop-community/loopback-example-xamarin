using TodoApp.Controls.DefaultControls;
using System.Collections.Generic;
using Xamarin.Forms;
using TodoApp.Controls;
using TodoApp.Styles;
using System;

namespace TodoApp.Pages {
	internal class MenuPage : ContentPage {
		public MenuPage() {
			Padding = new Thickness(0, Device.OnPlatform(0, 0, 0), 0, 0);
			Title = "Menu";

            var perfectedLogo = new Image {
                Source = "perfectedtech-logo-white.png",
                HeightRequest = 60
            };
			var perfectedLabel = new ItalicLabel {Text = "Created in PerfectedTech 2015", CustomFontSize = NamedSize.Medium, TextColor = Color.White};
			var perfectedLinkGestureRecognizer = new TapGestureRecognizer {
				Command = new Command (() => Device.OpenUri (new Uri ("http://perfectedtech.com/")))
			};
			perfectedLogo.GestureRecognizers.Add (perfectedLinkGestureRecognizer);
			perfectedLabel.GestureRecognizers.Add (perfectedLinkGestureRecognizer);


			var strongloopLogo = new Image {
				Source = "strongloop.png",
				HeightRequest = 60
			};
			var strongloopLabel = new ItalicLabel {Text = "Powered by Strongloop", CustomFontSize = NamedSize.Medium, TextColor = Color.White};
			var strongloopLinkGestureRecognier = new TapGestureRecognizer {
				Command = new Command (() => Device.OpenUri (new Uri ("https://strongloop.com/")))
			};
			strongloopLogo.GestureRecognizers.Add (strongloopLinkGestureRecognier);
			strongloopLabel.GestureRecognizers.Add (strongloopLinkGestureRecognier);


			var logoLayout = new StackLayout {
                Padding = new Thickness(0, 20),
				Spacing = 30,
				Children = {perfectedLogo, perfectedLabel, strongloopLogo, strongloopLabel},
				BackgroundColor = StyleManager.MainColor
            };

			var menuTemplate = new DataTemplate(typeof(ImageCell));
			menuTemplate.SetBinding (TextCell.TextProperty, "Text");
			menuTemplate.SetBinding (ImageCell.ImageSourceProperty, "ImageSource");
			menuTemplate.SetBinding (TextCell.CommandProperty, "Command");

			var menuList = new ListView {
				ItemsSource = new List<object>{new {
						Text = "Logout",
						Command = new Command(() => Navigation.PopAsync()),
						ImageSource = "logout.png"
					}
				},
				ItemTemplate = menuTemplate,
				SeparatorVisibility = SeparatorVisibility.None
			};
			BackgroundColor = Color.White;
			Content = new StackLayout {Children = {logoLayout, menuList}};
		}
	}
}