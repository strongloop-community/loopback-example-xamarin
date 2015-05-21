using System;
using System.Collections.Generic;
using System.Globalization;
using TodoApp.Controls.Abstract;
using TodoApp.Controls.DefaultControls;
using TodoApp.Helpers;
using Xamarin.Forms;

namespace TodoApp.Controls {
	public class HomeTopPanel : TopPanel {
        public readonly TopPanelItem monthButton;
		public DateTime SelectedDate { get; private set; }
		public event EventHandler SelectedDateChanged;
		public TopPanelItem yearButton;
		public ContentView BottomPanel{ get; }

		private readonly TopPanelItem todayButton;
		private readonly CalendarView calendarView;
		private DayPicker dayPicker;
		private int selectedMonth;
		private int selectedYear;

		public HomeTopPanel( DateTime selectedDate) {
			SelectedDate = selectedDate;
			selectedMonth = SelectedDate.Month;
			selectedYear = selectedDate.Year;
			// Today's content view.
			var currentDayLayout = new ContentView {
				Padding = 6,
				Content = new DefaultLabel {
					HorizontalOptions = LayoutOptions.Center,
					Text = selectedDate.ToString("dddd", new CultureInfo("en-US"))
				}
			};

			calendarView = new CalendarView {
				VerticalOptions = LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.CenterAndExpand
			};

			// Home page tabs.
			todayButton = new TopPanelItem("Today", currentDayLayout, true);
			monthButton = new TopPanelItem(SelectedDate.ToString("MMMM", new CultureInfo("en-US")));
			yearButton = new TopPanelItem(SelectedDate.Year.ToString(), calendarView);

			Items = new List<TopPanelItem> {todayButton, monthButton, yearButton};

			Children.Add(todayButton);
			Children.Add(monthButton);
			Children.Add(yearButton);

			DefaultView = todayButton.ContentView;
			LastView = todayButton.ContentView;
			todayButton.OnTap += SelectToday;
			yearButton.OnTap += ShowCalender;
			monthButton.OnTap += ShowMonthLayout;

			calendarView.MonthChanged += (sender, newMonth) => ChangeMonth(newMonth);
			calendarView.DateSelected += (sender, newDate) => {
				SelectDate (newDate);
				ShowMonthLayout();
			};

			BottomPanel = new ContentView ();
			OnItemSelected = view => {
				if (view != null)
					BottomPanel.Content = view;
			};
			BottomPanel.Content = DefaultView;	
		}

		private void ChangeMonth (DateTime newMonth)
		{
			selectedMonth = newMonth.Month;
			selectedYear = newMonth.Year;
			monthButton.Title = newMonth.ToString("MMMM", new CultureInfo("en-US"));
			yearButton.Title = selectedYear.ToString();
		}

		public View LastView { get; private set; }

		protected override void SelectItem( TopPanelItem item ) {
			base.SelectItem(item);
			OnItemSelected.Dispatch (item.ContentView);
		}

		void SelectToday ()
		{
			SelectItem (todayButton);
			SelectDate (DateTime.Today);
		}

		private void ShowMonthLayout ()
		{
			dayPicker = new DayPicker (CalendarHelper.CreateDayLayouts (CalendarHelper.GetDaysInMonth (selectedMonth, selectedYear)));
			monthButton.ContentView = dayPicker;
			SelectItem(monthButton);
			dayPicker.DateChanged += (sender, e) => SelectDate (new DateTime (SelectedDate.Year, SelectedDate.Month, dayPicker.SelectedItem, 0, 0, 0, DateTimeKind.Local));
			if (SelectedDate.Month == selectedMonth) {
				dayPicker.SelectDay (SelectedDate.Day);
			}
		}

		private void SelectDate (DateTime date)
		{
			SelectedDate = date;
			ChangeMonth (date);
			calendarView.SelectedDate = date;
			SelectedDateChanged.Dispatch (this, new EventArgs ());
		}

		private void ShowCalender ()
		{
			SelectItem (yearButton);
		}
	}
}