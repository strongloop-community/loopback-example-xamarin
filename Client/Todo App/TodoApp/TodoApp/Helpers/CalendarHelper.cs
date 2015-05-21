using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TodoApp.Controls;

namespace TodoApp.Helpers {
	internal static class CalendarHelper {
		public static IEnumerable<DateTime> GetDaysInMonth( int month, int year ) {
			return Enumerable.Range (1, DateTime.DaysInMonth (year, month)).Select (day => new DateTime (year, month, day, 0, 0, 0, DateTimeKind.Local));
		}
			
		public static List<DayTile> CreateDayLayouts( IEnumerable<DateTime> days ) {
			return days.Select(day => new DayTile(day.Day, day.ToString("dddd", new CultureInfo("en-US")))).ToList();
		}
	}
}