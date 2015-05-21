using System;
using System.Collections.Generic;
using TodoApp.Controls.DefaultControls;
using Xamarin.Forms;

namespace TodoApp.Controls {
	public class DayPicker : SelectableScrollView {
		private readonly List<DayTile> dayTiles;
		private readonly StackLayout daysLayout;

		public event EventHandler DateChanged;

		public DayPicker( List<DayTile> days ) {
			dayTiles = days;

			Orientation = ScrollOrientation.Horizontal;
			HorizontalOptions = LayoutOptions.Center;
			Padding = 0;

			daysLayout = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Center,
				Padding = new Thickness(135, 0),
				Spacing = 0
			};

			foreach (DayTile day in dayTiles)
				daysLayout.Children.Add(day);

			Content = daysLayout;

			ScrollEnded += ( sender, args ) => {
				var position = (int) Math.Round(ScrollX/DayTile.DefaultItemWidth) + 1;
				System.Diagnostics.Debug.WriteLine(position);

				SelectDay(position);
			};

			Scrolled += ( sender, args ) => {
				var position = (int) Math.Round(args.ScrollX/DayTile.DefaultItemWidth);

				if (position >= 0 && position < dayTiles.Count)
					CenterDay(dayTiles[position]);
			};
		}

		private void CenterDay( DayTile selectedDay ) {
			foreach (DayTile day in dayTiles)
				day.IsHighlighted = false;

			selectedDay.IsHighlighted = true;
		}

		public void SelectDay( int selectedDay ) {
			foreach (DayTile day in dayTiles)
				day.IsSelected = false;
			
			var selectedDayLayout = dayTiles [selectedDay - 1];
			selectedDayLayout.IsSelected = true;
			CenterDay (selectedDayLayout);

			SelectedItem = selectedDay;
			ScrollToAsync((SelectedItem - 1) * DayTile.DefaultItemWidth, 0, false);

			if(DateChanged != null)
				DateChanged (this, new EventArgs ());
		}

		protected override SizeRequest OnSizeRequest( double widthConstraint, double heightConstraint ) {
			// Set padding depending on requested width.
			if (Math.Abs(widthConstraint - (-1)) > 0.1)
				daysLayout.Padding = new Thickness((widthConstraint - DayTile.DefaultItemWidth)/2 - 5, 0);

			return base.OnSizeRequest(widthConstraint, heightConstraint);
		}
	}
}