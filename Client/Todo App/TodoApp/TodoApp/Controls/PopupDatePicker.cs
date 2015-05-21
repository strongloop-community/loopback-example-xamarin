using System;
using System.Collections.Generic;
using System.Globalization;
using TodoApp.Controls.DefaultControls;
using TodoApp.Helpers;
using TodoApp.Styles;
using Xamarin.Forms;

namespace TodoApp.Controls {
	internal class PopupDatePicker : SelectableScrollView {
		private readonly List<TopPanelItem> monthTiles;
		private PopupTopPanel monthPicker;
		private int SelectedDay{ get; set; }
		private int SelectedMonth{ get { return SelectedItem + 1; } }
		private TopPanelItem currentMonthTile;

		public event EventHandler DateChanged;
		public Action<View> OnItemSelected { get; set; }
		public View DefaultView { get; private set; }

		public DateTime SelectedDate{ get { return new DateTime (DateTime.Today.Year, SelectedMonth, SelectedDay, 0, 0, 0, DateTimeKind.Local); } }

		public PopupDatePicker() {
			Orientation = ScrollOrientation.Horizontal;
			HorizontalOptions = LayoutOptions.Center;
			Padding = 0;
			BackgroundColor = StyleManager.DarkAccentColor;

			var now = DateTime.Now;
			monthTiles = new List<TopPanelItem>();

			for (int i = 1; i <= 12; i++) {
				IEnumerable<DateTime> days = CalendarHelper.GetDaysInMonth(i, now.Year);
				var dayPicker = new DayPicker(CalendarHelper.CreateDayLayouts(days));
				dayPicker.DateChanged += (sender, e) => {
					SelectedDay = dayPicker.SelectedItem;
					DateChanged (sender, e);
				};

				var month = new DateTime (now.Year, i, 1, 0, 0, 0, DateTimeKind.Local);
				var tab = new TopPanelItem(month.ToString("MMMM", new CultureInfo("en-US")), dayPicker) {
					WidthRequest = 100
				};

				if (month.Month == now.Month) {
					tab.IsSelected = true;
					currentMonthTile = tab;
				}
				monthTiles.Add(tab);
			}

			monthPicker = new PopupTopPanel(monthTiles);

			monthPicker.DefaultView = currentMonthTile.ContentView;
			DefaultView = monthPicker.DefaultView;
			Content = monthPicker;

			ScrollEnded += ( sender, args ) => {
				var position = (int) Math.Round(ScrollX/115);

				if (position >= 0 && position < monthTiles.Count)
					SelectMonthTile(monthTiles[position]);
			};

			Scrolled += ( sender, args ) => {
				var position = (int) Math.Round(args.ScrollX/115);

				if (position >= 0 && position < monthTiles.Count)
					HighlightItem(position);
			};
		}

		public void SelectDate(DateTime date){
			SelectMonth (date.Month);
			SelectDay (date.Day);
		}

		private void SelectMonth( int month ) {
			month = month - 1;
			var monthTile = monthTiles [month];
			SelectMonthTile (monthTile);
		}

		private void SelectDay(int day){
			var dayPicker = monthTiles[SelectedItem].ContentView as DayPicker;
			if (dayPicker != null)
				dayPicker.SelectDay(day);
		}

		private void SelectMonthTile(TopPanelItem selectedMonthTile) {
			monthTiles [SelectedItem].IsSelected = false;

			selectedMonthTile.IsSelected = true;
			SelectedItem = monthTiles.IndexOf(selectedMonthTile);
			ScrollToAsync(SelectedItem*115, 0, false);

			OnItemSelected.Dispatch(selectedMonthTile.ContentView);

			SelectDay (1);
		}

		private void HighlightItem( int index ) {
			foreach (TopPanelItem item in monthTiles)
				item.IsSelected = false;

			monthTiles[index].IsSelected = true;
		}
	}
}