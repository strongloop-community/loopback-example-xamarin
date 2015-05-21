using System;
using Xamarin.Forms;
using TodoApp.Helpers;

namespace TodoApp.Controls {
	public class CalendarView : View {
		public DateTime? SelectedDate{ get; set;}

		public event EventHandler<DateTime> DateSelected;
		public event EventHandler<DateTime> MonthChanged;
		public event EventHandler HideCalendar;

		public void NotifyDateSelected( DateTime dateSelected ) {
			DateSelected.Dispatch(this, dateSelected);
		}

		public void NotifyMonthChanged( DateTime month ) {
			MonthChanged.Dispatch(this, month);
		}

		public void Hide() {
			HideCalendar.Dispatch(this, new EventArgs());
		}
	}
}