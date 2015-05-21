using TodoApp.Controls.DefaultControls;
using TodoApp.Styles;
using Xamarin.Forms;

namespace TodoApp.Controls {
	public class DayTile : StackLayout {
		public static float DefaultItemWidth = 40;
		public static float SelectedItemWidth = 60;
		private readonly Label dayLabel;
		private readonly Label dayOfWeekLabel;
		private readonly string dayOfWeek;
		private bool isSelected;
		private bool isCentered;

		public DayTile( int dayNumber, string dayOfWeek, bool isSelected = false ) {
			this.dayOfWeek = dayOfWeek;

			HorizontalOptions = LayoutOptions.FillAndExpand;
			Spacing = 0;

			dayLabel = new DefaultLabel {
				Text = dayNumber.ToString(),
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.Center
			};
			dayOfWeekLabel = new DefaultLabel {HorizontalOptions = LayoutOptions.Center, FontSize = 10};

			IsSelected = isSelected;

			Children.Add(dayLabel);
			Children.Add(dayOfWeekLabel);
		}

		public bool IsSelected {
			get { return isSelected; }
			set {
				isSelected = value;

				if (isSelected) {
					dayOfWeekLabel.Text = dayOfWeek;
					WidthRequest = SelectedItemWidth;
					IsHighlighted = true;
				} else {
					dayOfWeekLabel.Text = dayOfWeek.Substring(0, 3);
					WidthRequest = DefaultItemWidth;
					IsHighlighted = false;
				}
			}
		}

		public bool IsHighlighted {
			get { return isCentered; }
			set {
				isCentered = value;
				SetTextColor(isCentered ? Color.White : StyleManager.UnselectedTabColor);
			}
		}

		private void SetTextColor( Color color ) {
			dayLabel.TextColor = color;
			dayOfWeekLabel.TextColor = color;
		}
	}
}