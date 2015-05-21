using System;
using TodoApp.Controls.DefaultControls;
using TodoApp.Helpers;
using TodoApp.Styles;
using Xamarin.Forms;

namespace TodoApp.Controls {
	public class TopPanelItem : ContentView {
		private readonly DefaultLabel titleLabel;
		private bool isSelected;

		public TopPanelItem( string title, View contentView = null, bool isSelected = false ) {
			HorizontalOptions = LayoutOptions.FillAndExpand;
			Padding = 8;

			titleLabel = new DefaultLabel {
				Text = title,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				FontAttributes = FontAttributes.Bold,
				CustomFontSize = NamedSize.Small
			};
			IsSelected = isSelected;
			ContentView = contentView;

			GestureRecognizers.Add(new TapGestureRecognizer {
				Command = new Command(() => OnTap.Dispatch())
			});

			Content = titleLabel;
		}

		public bool IsSelected {
			get { return isSelected; }
			set {
				isSelected = value;
				titleLabel.TextColor = isSelected ? StyleManager.AccentColor : StyleManager.UnselectedTabColor;
			}
		}

		public string Title {
			get { return titleLabel.Text; }
			set { titleLabel.Text = value; }
		}

		public View ContentView { get; set; }

		public Action OnTap { get; set; }
	}
}