using System;
using System.Collections.Generic;
using System.Drawing;
using Foundation;
using SWTableViewCell;
using TodoApp.Cells;
using TodoApp.iOS.Helpers;
using TodoApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Diagnostics;
using TodoApp.Pages;
using TodoApp.Helpers;

[assembly: ExportRenderer(typeof(TaskCell), typeof(TaskCellRenderer))]

namespace TodoApp.iOS.Renderers {
	internal class TaskCellRenderer : ViewCellRenderer {
		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tableView) {
			var taskCell = (TaskCell)item;


			var greenColor = UIColor.FromRGB (43, 182, 115);
			var done = new UIButton {
				Frame = new RectangleF(0, 0, (float)tableView.Bounds.Size.Width / 2, (float)tableView.RowHeight),
				BackgroundColor = UIColor.Gray
			};

			if (taskCell.IsDone) {
				done.BackgroundColor = greenColor;
			} else {
				done.BackgroundColor = UIColor.Gray;
			}




			done.SetImage(new UIImage("done.png"), UIControlState.Normal);

			var buttons = new List<UIButton>();
            
			var box = new UIView {
				BackgroundColor = taskCell.CategoryColor.ToUIColor(),
				Frame = new RectangleF(0, 0, 8, (float)tableView.RowHeight)
			};

			var taskName = new UILabel {
				Font = UIFont.FromName("HelveticaNeue-Light", 14),
				Frame = new RectangleF(20, 5, (float)tableView.Frame.Width - 100, 20),
				TextColor = UIColor.DarkGray
			};

			var categoryName = new UILabel {
				Text = taskCell.CategoryName,
				Font = UIFont.FromName("HelveticaNeue-Light", 10),
				Frame = new RectangleF(45, 30, (float)tableView.Frame.Width - 120, 15),
				TextColor = box.BackgroundColor
			};

			var categoryIcon = new UIImageView {
				Image = new UIImage(taskCell.CategoryIcon),
				Frame = new RectangleF(20, 30, 15, 12)
			};

			SetTaskCompleteStatus (taskCell, taskName, categoryName, categoryIcon);

			tableView.Bounces = false;

			var mainView = new List<UIView> {
				box, taskName, categoryIcon, categoryName
			};

			if (taskCell.Date != DateTime.MinValue) {
				var dueTime = new UILabel {
					Text = taskCell.Date.ToString("hh:mm tt"),
					Font = UIFont.FromName("HelveticaNeue-Light", 10),
					TextAlignment = UITextAlignment.Right,
					TextColor = UIColor.LightGray,
					Frame = new RectangleF((float)tableView.Frame.Width - 60, 30, 50, 15)
				};
				mainView.Add(dueTime);
			}

			if (taskCell.IsFavorite) {
				var favorite = new UIImageView {
					Image = new UIImage("star.png"),
					Frame = new RectangleF((float)tableView.Frame.Width - 25, 10, 12, 12)
				};
				mainView.Add(favorite);
			}

			done.TouchDown += (sender, args) => {
				taskCell.OnDone.Dispatch();

				if(done.BackgroundColor.CGColor == greenColor.CGColor)
				{
					SetTaskCompleteStatus (taskCell, taskName, categoryName, categoryIcon);
					done.BackgroundColor = UIColor.Gray;
				}
				else if(done.BackgroundColor == UIColor.Gray)
				{
					SetTaskCompleteStatus (taskCell, taskName, categoryName, categoryIcon);
					done.BackgroundColor = greenColor;

				}

				tableView.ReloadData();
			};

			var OnFavorite = new UIButton(UIButtonType.Custom) {
				BackgroundColor = UIColor.Gray
			};
			OnFavorite.TouchDown += (sender, args) => {
				Debug.WriteLine ("Favorite hit");

				Device.BeginInvokeOnMainThread(() =>
					{
						Debug.WriteLine ("Favorite hit");
						taskCell.OnFavorite.Dispatch();						

						tableView.ReloadData();
					});
			};
			OnFavorite.SetImage(new UIImage("favorite.png"), UIControlState.Normal);



			var OnEdit = new UIButton(UIButtonType.Custom) {
				BackgroundColor = UIColor.Gray
			};
			OnEdit.TouchDown += (sender, args) => {
				Debug.WriteLine ("edit hit");

				Device.BeginInvokeOnMainThread(() =>
					{
						Debug.WriteLine ("edit hit");

						var mainPage = (NavigationPage) Xamarin.Forms.Application.Current.MainPage;
						var masterDetailPage = (MasterDetailPage) mainPage.CurrentPage;
						var detailPage = (NavigationPage) masterDetailPage.Detail;
						var todayPage = (TodayPage) detailPage.CurrentPage;
						todayPage.ShowPopup(taskCell.Task);
						tableView.ReloadData();
					});
			};
			OnEdit.SetImage(new UIImage("edit.png"), UIControlState.Normal);


			var OnDelete = new UIButton(UIButtonType.Custom) {
				BackgroundColor = UIColor.Gray
			};
			OnDelete.TouchDown += (sender, args) => {
				Debug.WriteLine ("delete hit");
				taskCell.OnDelete.Dispatch();	

				Device.BeginInvokeOnMainThread(() =>
					{
						var it = TodoApp.Pages.TodayPage.tasksTableView.Root.GetEnumerator();

						it.MoveNext();

						it.Current.Remove(taskCell);
					});
			};
			OnDelete.SetImage(new UIImage("delete.png"), UIControlState.Normal);
		
			buttons.Add(OnFavorite);
			buttons.Add(OnEdit);
			buttons.Add(OnDelete);

			var cell = new SWTableViewCell.SWTableViewCell(UITableViewCellStyle.Default, "C", tableView, buttons, done, mainView);
			return cell;
		}

		static void SetTaskCompleteStatus (TaskCell taskCell, UILabel taskName, UILabel categoryName, UIImageView categoryIcon)
		{
			if (taskCell.IsDone) {
				taskName.TextColor = UIColor.LightGray;
				taskName.AttributedText = new NSAttributedString (taskCell.Title, new UIStringAttributes {
					StrikethroughStyle = NSUnderlineStyle.Single
				});
				categoryName.TextColor = UIColor.LightGray;
				categoryIcon.Image = ImageHelper.GrayscaleImage (new UIImage (taskCell.CategoryIcon));
			}
			else
				taskName.Text = taskCell.Title;
		}
	}
}