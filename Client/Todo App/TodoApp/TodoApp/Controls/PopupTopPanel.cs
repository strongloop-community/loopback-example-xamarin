using System.Collections.Generic;
using TodoApp.Controls.Abstract;
using Xamarin.Forms;

namespace TodoApp.Controls {
	internal class PopupTopPanel : TopPanel {
		public PopupTopPanel(List<TopPanelItem> items) {
			Padding = new Thickness(95, 0);
			Spacing = 0;
			
			foreach (TopPanelItem item in items)
				Children.Add(item);

			Items = new List<TopPanelItem>(items);
		}
	}
}