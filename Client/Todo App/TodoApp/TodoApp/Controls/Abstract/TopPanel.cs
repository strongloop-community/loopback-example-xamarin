using System;
using System.Collections.Generic;
using TodoApp.Styles;
using Xamarin.Forms;

namespace TodoApp.Controls.Abstract {
	public abstract class TopPanel : StackLayout {
		protected TopPanel() {
			Orientation = StackOrientation.Horizontal;
			BackgroundColor = StyleManager.DarkAccentColor;
			HorizontalOptions = LayoutOptions.Fill;
			HeightRequest = 45;
		}
		protected List<TopPanelItem> Items { get; set; }

		public Action<View> OnItemSelected { get; set; }
		public View DefaultView { get; set; }

		protected virtual void SelectItem( TopPanelItem item ) {
			foreach (TopPanelItem panelItem in Items)
				panelItem.IsSelected = false;

			item.IsSelected = true;
		}
	}
}