using System;
using TodoApp.Controls;
using TodoApp.iOS.Controls;
using TodoApp.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer( typeof (CalendarView), typeof (CalendarViewRenderer) )]

namespace TodoApp.iOS.Renderers {
	public class CalendarViewRenderer : ViewRenderer {
		private CalendarView view;
        public static DateTime calendarDate;
        CalendarMonthView calendarView;

		protected override void OnElementChanged( ElementChangedEventArgs<View> e ) {
			base.OnElementChanged(e);

			view = (CalendarView) Element;

			if (view != null) {
				calendarView = new CalendarMonthView (view?.SelectedDate ?? DateTime.Today, false);
				calendarView.SwipedUp += view.Hide;
				calendarView.OnDateSelected += view.NotifyDateSelected;
				calendarView.MonthChanged += view.NotifyMonthChanged;

				try {
					SetNativeControl (calendarView);
				} catch {
					// Hiding calendar causes null reference exception.
				}
			}
		}
	}
}