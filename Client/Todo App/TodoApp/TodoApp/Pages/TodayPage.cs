using System;
using System.Globalization;
using TodoApp.Cells;
using TodoApp.Controls;
using TodoApp.Controls.DefaultControls;
using TodoApp.Helpers;
using TodoApp.Models;
using TodoApp.Pages.Base;
using TodoApp.Styles;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using LBXamarinSDK;

namespace TodoApp.Pages {

	public class TodayPage : BaseContentPage {
		private TaskPopup popup;
		private readonly PopupLayout popupLayout = new PopupLayout();
		private DateTime selectedDate;
		private HomeTopPanel homeTopPanel;
		private StackLayout pageLayout;

		public static TableView tasksTableView = new TableView {RowHeight = 50};

		public TodayPage() {
			homeTopPanel = new HomeTopPanel (DateTime.Today);
			homeTopPanel.SelectedDateChanged += (sender, e) => SetDate(homeTopPanel.SelectedDate);

			pageLayout = new StackLayout {
				BackgroundColor = StyleManager.MainColor,
				Children = {homeTopPanel, homeTopPanel.BottomPanel, tasksTableView}
			};

			var floatingButton = new FloatingActionButton {Source = "floating_button.png"};

			var addTaskButton = new FloatingActionControl {
				Padding = new Thickness(15, 0),
				Tooltip = "Add a task",
				IsVisible = false,
				TapAction = () => {
					floatingButton.CloseAction.Dispatch();

					ShowPopup(new TodoTask {
						title = "", 
						category = CategoryHelper.HomeCategory.Name, 
						isDone = false, 
						date = selectedDate, 
						isFavorite = false, 
						isDeleted = false
					});
					popup.OnPopupShown();
				}
			};

			floatingButton.OpenAction += () => {	
				FadeOut();
				floatingButton.Source = FloatingActionButton.OPENED_BUTTON_IMAGE_SOURCE;
				addTaskButton.IsVisible = true;
			};
			floatingButton.CloseAction += () => {
				FadeIn();
				floatingButton.Source = FloatingActionButton.CLOSED_BUTTON_IMAGE_SOURCE;
				addTaskButton.IsVisible = false;
			};
            
			// Main layout with the popup and floating buttons.
			popupLayout.Content = pageLayout; popupLayout.BackgroundColor = Color.Black;

			// If popup is opened - do not allow tap on floating button.
			popupLayout.AllowAction += value => floatingButton.AllowTap = value;

			popupLayout.AddChild(floatingButton, Constraint.RelativeToParent(layout => layout.Width - 50), Constraint.RelativeToParent(layout => layout.Height - 50));
			popupLayout.AddChild(addTaskButton, Constraint.RelativeToParent(layout => layout.Width - addTaskButton.Width), Constraint.RelativeToParent(layout => layout.Height - 100));

			Content = popupLayout;

			selectedDate = DateTime.Now;
			UpdateList ();
		}

		public void FadeOut(){
			pageLayout.Opacity = 0.5;
		}

		public void FadeIn(){
			pageLayout.Opacity = 1;
		}

		private void SetDate (DateTime selectedDate)
		{
			this.selectedDate = selectedDate;
			UpdateList ();
		}

		public void ShowPopup(TodoTask task)
		{
			FadeIn ();
			popup = new TaskPopup(
				task,
				popupLayout.DismissPopup,
				() => OnTaskSaved(popup.Task));

			Debug.WriteLine("ShowPopup");
			popupLayout.ReliableShowPopup(popup, popupLayout, PopupLayout.PopupLocation.Top, 0, 25);
			popup.OnPopupShown();
		}

		private async void UpdateList ()
		{			
			var tasks = await TaskHelper.GetTaskByDay (selectedDate);
			var tableSection = new TableSection ();
			foreach (var task in tasks) {
				tableSection.Add (new TaskCell (task){
					OnDone = () => OnDone (task),
					OnDelete = () => OnDelete (task),
					OnFavorite = () => OnFavorite (task)
				});
			}
			tasksTableView.Root.Clear ();
			tasksTableView.Root.Add (tableSection);
		}

		private async void OnDone(TodoTask task){
			await TaskHelper.ToggleDone (task);
		}

		private async void OnTaskSaved(TodoTask task){
			if (task.IsValid ()) {
				await TaskHelper.SaveTask (task);
				popupLayout.DismissPopup (); 
				UpdateList ();
			} else {
				await DisplayAlert ("Error", "Please make sure you filled in all task fields", "OK");
				ShowPopup (task);
			}
		}

		private async void OnFavorite(TodoTask task){
			await TaskHelper.ToggleFavorite (task);		
		}

		private async void OnDelete(TodoTask task){
			await TaskHelper.DeleteTask (task);
		}
	}
}