//
//  CalendarMonthView.cs
//
//  Converted to MonoTouch on 1/22/09 - Eduardo Scoz || http://escoz.com
//  Originally reated by Devin Ross on 7/28/09  - tapku.com || http://github.com/devinross/tapkulibrary
//
/*
 
 Permission is hereby granted, free of charge, to any person
 obtaining a copy of this software and associated documentation
 files (the "Software"), to deal in the Software without
 restriction, including without limitation the rights to use,
 copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the
 Software is furnished to do so, subject to the following
 conditions:
 
 The above copyright notice and this permission notice shall be
 included in all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 OTHER DEALINGS IN THE SOFTWARE.
 
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;
using TodoApp.Helpers;

namespace TodoApp.iOS.Controls {
	public delegate void DateSelected( DateTime date );

	public delegate void MonthChanged( DateTime monthSelected );

	public sealed class CalendarMonthView : UIView {
		private readonly int headerHeight;
		private readonly bool showHeader;
		private readonly float width;

		public DateTime CurrentMonthYear;
		public DateTime CurrentSelectedDate;

		public Func<DateTime, bool> IsDateAvailable;
		public Func<DateTime, bool> IsDayMarkedDelegate;

		public Action<DateTime> MonthChanged;
		public Action<DateTime> OnDateSelected;
		public Action<DateTime> OnFinishedDateSelection;
		public Action SwipedUp;

		private bool calendarIsLoaded;

		private MonthGridView monthGridView;
		private UIScrollView scrollView;

		#region Calendar view settings

		public int BoxHeight = 33;
		public int BoxWidth = 46;

		public float DayCellPadding = 1.5f;
		public int HorizontalPadding = 10;
		public int LinesCount = 6;
		public int VerticalPadding = 10;

		#endregion

		public CalendarMonthView( DateTime selectedDate, bool showHeader, float width = 320 ) {
			if (Math.Abs(width - 320) < 0.1)
				width = (float) UIScreen.MainScreen.Bounds.Size.Width;
			this.width = width;
			this.showHeader = showHeader;

			if (showHeader)
				headerHeight = 20;

			BoxWidth = Convert.ToInt32(Math.Ceiling(width/7)) - (int) Math.Ceiling(2*HorizontalPadding/7.0);

			int height = BoxHeight*LinesCount + 2*VerticalPadding + 10;

			// Calendar view frame (header, calendar, paddings).
			Frame = showHeader ? new RectangleF(0, 0, width, height + headerHeight) : new RectangleF(0, 0, width, height);

			BackgroundColor = UIColor.White;
			ClipsToBounds = true;

			CurrentSelectedDate = selectedDate;
			CurrentDate = DateTime.Now.Date;
			CurrentMonthYear = new DateTime(CurrentSelectedDate.Year, CurrentSelectedDate.Month, 1);

			// Actions on swipe.
			var swipeLeft = new UISwipeGestureRecognizer(MonthViewSwipedLeft) {
				Direction = UISwipeGestureRecognizerDirection.Left
			};
			AddGestureRecognizer(swipeLeft);

			var swipeRight = new UISwipeGestureRecognizer(MonthViewSwipedRight) {
				Direction = UISwipeGestureRecognizerDirection.Right
			};
			AddGestureRecognizer(swipeRight);

			var swipeUp = new UISwipeGestureRecognizer(MonthViewSwipedUp) {
				Direction = UISwipeGestureRecognizerDirection.Up
			};
			AddGestureRecognizer(swipeUp);
		}

		private DateTime CurrentDate { get; set; }

		public void SetDate( DateTime newDate ) {
			bool right = true;

			CurrentSelectedDate = newDate;


			int monthsDiff = (newDate.Month - CurrentMonthYear.Month) + 12*(newDate.Year - CurrentMonthYear.Year);
			if (monthsDiff != 0) {
				if (monthsDiff < 0) {
					right = false;
					monthsDiff = -monthsDiff;
				}

				for (int i = 0; i < monthsDiff; i++) {
					MoveCalendarMonths(right, true);
				}
			} else {
				RebuildGrid(right, false);
			}
		}

		private void MonthViewSwipedUp( UISwipeGestureRecognizer ges ) {
			SwipedUp.Dispatch();
		}

		private void MonthViewSwipedRight( UISwipeGestureRecognizer ges ) {
			// Move to previous month.
			MoveCalendarMonths(false, true);
		}

		private void MonthViewSwipedLeft( UISwipeGestureRecognizer ges ) {
			// Move to next month.
			MoveCalendarMonths(true, true);
		}

		/// <summary>
		///    Update calendar month view.
		/// </summary>
		public override void SetNeedsDisplay() {
			base.SetNeedsDisplay();
			if (monthGridView != null)
				monthGridView.Update();
		}

		public override void LayoutSubviews() {
			if (calendarIsLoaded) return;

			scrollView = new UIScrollView {
				ContentSize = new SizeF(width, 260),
				ScrollEnabled = false,
				Frame = new RectangleF(0, 16 + headerHeight, width, (float) Frame.Height - 16),
				BackgroundColor = UIColor.White
			};

			LoadInitialGrids();

			BackgroundColor = UIColor.Clear;

			AddSubview(scrollView);

			scrollView.AddSubview(monthGridView);

			calendarIsLoaded = true;
		}

		public void DeselectDate() {
			if (monthGridView != null)
				monthGridView.DeselectDayView();
		}

		/// <summary>
		///    Change calendar month.
		/// </summary>
		/// <param name="right">Direction of the transition.</param>
		/// <param name="animated">Animate transition.</param>
		public void MoveCalendarMonths( bool right, bool animated ) {
			CurrentMonthYear = CurrentMonthYear.AddMonths(right ? 1 : -1);
			RebuildGrid(right, animated);
		}

		/// <summary>
		///    Rebuild month grid.
		/// </summary>
		/// <param name="right">Direction of the transition.</param>
		/// <param name="animated">Animate transition.</param>
		public void RebuildGrid( bool right, bool animated ) {
			UserInteractionEnabled = false;

			// Get new month grid.
			MonthGridView gridToMove = CreateNewGrid(CurrentMonthYear);
			nfloat pointsToMove = (right ? Frame.Width : -Frame.Width);

			gridToMove.Frame = new RectangleF(new PointF((float) pointsToMove, VerticalPadding/2.0f), (SizeF) gridToMove.Frame.Size);

			scrollView.AddSubview(gridToMove);

			if (animated) {
				BeginAnimations("changeMonth");
				SetAnimationDuration(0.4);
				SetAnimationDelay(0.1);
				SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
			}

			monthGridView.Center = new PointF((float) (monthGridView.Center.X - pointsToMove), (float) monthGridView.Center.Y);
			gridToMove.Center = new PointF((float) (gridToMove.Center.X - pointsToMove + HorizontalPadding), (float) gridToMove.Center.Y);

			monthGridView.Alpha = 0;

			SetNeedsDisplay();

			if (animated)
				CommitAnimations();

			monthGridView = gridToMove;

			UserInteractionEnabled = true;

			if (MonthChanged != null)
				MonthChanged(CurrentMonthYear);
		}

		/// <summary>
		///    Create new month grid.
		/// </summary>
		/// <param name="date">Needed month.</param>
		/// <returns>Return new calendar month grid view.</returns>
		private MonthGridView CreateNewGrid( DateTime date ) {
			var grid = new MonthGridView(this, date) {CurrentDate = CurrentDate};
			grid.BuildGrid();
			grid.Frame = new RectangleF(HorizontalPadding, VerticalPadding/2.0f, width - HorizontalPadding, (float) Frame.Height - 16);
			return grid;
		}

		private void LoadInitialGrids() {
			monthGridView = CreateNewGrid(CurrentMonthYear);
		}

		public override void Draw( CGRect rect ) {
			using (CGContext context = UIGraphics.GetCurrentContext()) {
				context.SetFillColor(UIColor.White.CGColor);
				context.FillRect(new RectangleF(0, 0, width, 18 + headerHeight));
			}

			// Add day of week labels (monday, tuesday, etc).
			DrawDayLabels(rect);

			// Displat month header.
			if (showHeader)
				DrawMonthLabel(rect);
		}

		private void DrawMonthLabel( CGRect rect ) {
			var r = new RectangleF(new PointF(0, 2), new SizeF {Width = width - HorizontalPadding*2, Height = 42});
			UIColor.DarkGray.SetColor();
			new NSString(CurrentMonthYear.ToString("MMMM yyyy")).DrawString(r, UIFont.BoldSystemFontOfSize(16),
				UILineBreakMode.WordWrap, UITextAlignment.Center);
		}

		/// <summary>
		///    Draw day of week labels.
		/// </summary>
		private void DrawDayLabels( CGRect rect ) {
			// Font size.
			UIFont font = UIFont.BoldSystemFontOfSize(11);
			// Font color.
			UIColor.Gray.SetColor();

			CGContext context = UIGraphics.GetCurrentContext();
			context.SaveState();

			int i = 0;
			foreach (string d in Enum.GetNames(typeof (DayOfWeek))) {
				new NSString(d.Substring(0, 3)).DrawString(
					new RectangleF(i*BoxWidth + HorizontalPadding, VerticalPadding/2.0f + headerHeight, BoxWidth, 10),
					font,
					UILineBreakMode.WordWrap,
					UITextAlignment.Center);
				i++;
			}
			context.RestoreState();
		}
	}

	public sealed class MonthGridView : UIView {
		private readonly CalendarMonthView calendarMonthView;

		private readonly IList<CalendarDayView> dayTiles = new List<CalendarDayView>();

		public int WeekdayOfFirst;
		private DateTime currentMonth;

		public MonthGridView( CalendarMonthView calendarMonthView, DateTime month ) {
			this.calendarMonthView = calendarMonthView;
			currentMonth = month.Date;

			var tapped = new UITapGestureRecognizer(OnTap);
			AddGestureRecognizer(tapped);
		}

		public DateTime CurrentDate { get; set; }

		public int Lines { get; set; }
		private CalendarDayView SelectedDayView { get; set; }
		public IList<DateTime> Marks { get; set; }

		private void OnTap( UITapGestureRecognizer tapRecg ) {
			CGPoint loc = tapRecg.LocationInView(this);
			if (SelectDayView((PointF) loc) && calendarMonthView.OnDateSelected != null)
				calendarMonthView.OnDateSelected(new DateTime(currentMonth.Year, currentMonth.Month,
					(int) SelectedDayView.Tag));
		}

		public void Update() {
			foreach (CalendarDayView v in dayTiles)
				UpdateDayView(v);

			SetNeedsDisplay();
		}

		public void UpdateDayView( CalendarDayView dayView ) {
			dayView.Marked = calendarMonthView.IsDayMarkedDelegate != null && calendarMonthView.IsDayMarkedDelegate(dayView.Date);
			dayView.Available = calendarMonthView.IsDateAvailable == null || calendarMonthView.IsDateAvailable(dayView.Date);
		}

		/// <summary>
		///    Build month grid.
		/// </summary>
		public void BuildGrid() {
			DateTime previousMonth = currentMonth.AddMonths(-1);
			int daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);

			int daysInMonth = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);
			WeekdayOfFirst = (int) currentMonth.DayOfWeek;

			int lead = daysInPreviousMonth - (WeekdayOfFirst - 1);

			int boxWidth = calendarMonthView.BoxWidth;
			int boxHeight = calendarMonthView.BoxHeight;

			// Build previous month's days.
			for (int i = 1; i <= WeekdayOfFirst; i++) {
				var viewDay = new DateTime(currentMonth.Year, currentMonth.Month, i);
				var dayView = new CalendarDayView(calendarMonthView) {
					Frame = new RectangleF((i - 1)*boxWidth - 1, 0, boxWidth, boxHeight),
					Date = viewDay,
					Text = lead.ToString()
				};

				AddSubview(dayView);
				dayTiles.Add(dayView);
				lead++;
			}

			int position = WeekdayOfFirst + 1;
			int line = 0;

			// Current month.
			for (int i = 1; i <= daysInMonth; i++) {
				var viewDay = new DateTime(currentMonth.Year, currentMonth.Month, i);
				var dayView = new CalendarDayView(calendarMonthView) {
					Frame = new RectangleF((position - 1)*boxWidth - 1, line*boxHeight, boxWidth, boxHeight),
					Today = (CurrentDate.Date == viewDay.Date),
					Text = i.ToString(),
					Active = true,
					Tag = i,
					Selected = (viewDay.Date == calendarMonthView.CurrentSelectedDate.Date),
					Date = viewDay
				};

				UpdateDayView(dayView);

				if (dayView.Selected)
					SelectedDayView = dayView;

				AddSubview(dayView);
				dayTiles.Add(dayView);

				position++;
				if (position > 7) {
					position = 1;
					line++;
				}
			}

			// Next month.
			int dayCounter = 1;
			if (position != 1) {
				for (int i = position; i < 8; i++) {
					var viewDay = new DateTime(currentMonth.Year, currentMonth.Month, i);
					var dayView = new CalendarDayView(calendarMonthView) {
						Frame = new RectangleF((i - 1)*boxWidth - 1, line*boxHeight, boxWidth, boxHeight),
						Text = dayCounter.ToString(),
						Date = viewDay,
					};
					UpdateDayView(dayView);

					AddSubview(dayView);
					dayTiles.Add(dayView);
					dayCounter++;
				}
			}

			while (line < calendarMonthView.LinesCount - 1) {
				line++;
				for (int i = 1; i < 8; i++) {
					var viewDay = new DateTime(currentMonth.Year, currentMonth.Month, i);
					var dayView = new CalendarDayView(calendarMonthView) {
						Frame = new RectangleF((i - 1)*boxWidth - 1, line*boxHeight, boxWidth, boxHeight),
						Text = dayCounter.ToString(),
						Date = viewDay,
					};
					UpdateDayView(dayView);

					AddSubview(dayView);
					dayTiles.Add(dayView);
					dayCounter++;
				}
			}

			Frame = new RectangleF((PointF) Frame.Location, new SizeF((float) Frame.Width, (line + 1)*boxHeight));

			Lines = (position == 1 ? line - 1 : line);

			if (SelectedDayView != null)
				BringSubviewToFront(SelectedDayView);
		}

		private bool SelectDayView( PointF p ) {
			int index = ((int) p.Y/calendarMonthView.BoxHeight)*7 + ((int) p.X/calendarMonthView.BoxWidth);
			if (index < 0 || index >= dayTiles.Count)
				return false;

			CalendarDayView newSelectedDayView = dayTiles[index];
			if (newSelectedDayView == SelectedDayView)
				return false;

			if (!newSelectedDayView.Active) {
				int day = int.Parse(newSelectedDayView.Text);
				if (day > 15)
					calendarMonthView.MoveCalendarMonths(false, true);
				else
					calendarMonthView.MoveCalendarMonths(true, true);
				return false;
			}
			if (!newSelectedDayView.Active && !newSelectedDayView.Available) {
				return false;
			}

			if (SelectedDayView != null)
				SelectedDayView.Selected = false;

			BringSubviewToFront(newSelectedDayView);
			newSelectedDayView.Selected = true;

			SelectedDayView = newSelectedDayView;
			calendarMonthView.CurrentSelectedDate = SelectedDayView.Date;
			SetNeedsDisplay();
			return true;
		}

		public void DeselectDayView() {
			if (SelectedDayView == null)
				return;

			SelectedDayView.Selected = false;
			SelectedDayView = null;
			SetNeedsDisplay();
		}
	}

	public sealed class CalendarDayView : UIView {
		private readonly CalendarMonthView monthView;
		private bool isActive;
		private bool isAvailable;
		private bool isMarked;
		private bool isSelected;
		private bool isToday;
		private string text;

		public CalendarDayView( CalendarMonthView monthView ) {
			this.monthView = monthView;
			BackgroundColor = UIColor.White;
		}

		public DateTime Date { get; set; }

		public bool Available {
			get { return isAvailable; }
			set {
				isAvailable = value;
				SetNeedsDisplay();
			}
		}

		public string Text {
			get { return text; }
			set {
				text = value;
				SetNeedsDisplay();
			}
		}

		public bool Active {
			get { return isActive; }
			set {
				isActive = value;
				SetNeedsDisplay();
			}
		}

		public bool Today {
			get { return isToday; }
			set {
				isToday = value;
				SetNeedsDisplay();
			}
		}

		public bool Selected {
			get { return isSelected; }
			set {
				isSelected = value;
				SetNeedsDisplay();
			}
		}

		public bool Marked {
			get { return isMarked; }
			set {
				isMarked = value;
				SetNeedsDisplay();
			}
		}

		public override void Draw( CGRect rect ) {
			// Day cell background.
			UIImage img;
			// Text color.
			UIColor color = UIColor.Gray;

			if (!Active || !Available) {
				// Day of next or previous month.
				img = UIImage.FromBundle("Images/Calendar/datecell_unactive.png").CreateResizableImage(new UIEdgeInsets(4, 4, 4, 4));
			} else if (Today && Selected) {
				// Selected today.
				color = UIColor.White;
				img = UIImage.FromBundle("Images/Calendar/todayselected.png").CreateResizableImage(new UIEdgeInsets(4, 4, 4, 4));
			} else if (Today) {
				// Unselected today.
				img = UIImage.FromBundle("Images/Calendar/today.png").CreateResizableImage(new UIEdgeInsets(4, 4, 4, 4));
			} else if (Selected || Marked) {
				// Selected day.
				img = UIImage.FromBundle("Images/Calendar/datecellselected.png").CreateResizableImage(new UIEdgeInsets(4, 4, 4, 4));
			} else {
				// Default day.
				img = UIImage.FromBundle("Images/Calendar/datecell.png").CreateResizableImage(new UIEdgeInsets(4, 4, 4, 4));
			}

			img.Draw(new RectangleF(0, 0, monthView.BoxWidth - monthView.DayCellPadding, monthView.BoxHeight - monthView.DayCellPadding));

			color.SetColor();
			var inflated = new RectangleF(0, 7, (float) Bounds.Width, (float) Bounds.Height);
			new NSString(Text).DrawString(
				inflated,
				UIFont.BoldSystemFontOfSize(12),
				UILineBreakMode.WordWrap,
				UITextAlignment.Center);
		}
	}
}