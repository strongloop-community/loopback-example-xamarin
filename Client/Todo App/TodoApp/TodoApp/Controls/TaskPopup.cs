using System;
using TodoApp.Controls.DefaultControls;
using TodoApp.Helpers;
using TodoApp.Models;
using TodoApp.Styles;
using Xamarin.Forms;
using System.Diagnostics;
using LBXamarinSDK;

namespace TodoApp.Controls {
	internal class TaskPopup : StackLayout {
		public TodoTask Task { get; }
		public DateTime SelectedDate{ get; private set; }

		private bool isFavorite;
		private readonly PopupDatePicker datePicker;  	
		private readonly ContentView popupFragment = new ContentView();
		private readonly TimePicker timePicker = new TimePicker();
		private readonly ContentView topPanel;
		private readonly StackLayout titlePanel;
		private readonly DefaultLabel titleLabel;
		private readonly DefaultEntry titleEdit;
		private readonly CategoryPicker categoryPicker;

		private const string FAVORITE_IMG = "star.png";
		private const string NOT_FAVORITE_IMG = "starNotSelected.png";

		public TaskPopup( TodoTask task, Action onCancel = null, Action onSubmit = null) {
			Task = task;

			SelectedDate = task.date;
			timePicker.Time = task.date.TimeOfDay;

			BackgroundColor = StyleManager.MainColor;
			WidthRequest = 300;

			titleLabel = new DefaultLabel {Text = Task.title, HorizontalOptions = LayoutOptions.CenterAndExpand};
			titleEdit = new DefaultEntry {Text = Task.title, HorizontalOptions = LayoutOptions.CenterAndExpand, BackgroundColor = Color.Transparent};
            titleEdit.WidthRequest = 250;

			var editButton = new Image {HorizontalOptions = LayoutOptions.End, Source = "edit_task.png"};
			var doneButton = new Image {HorizontalOptions = LayoutOptions.End, Source = "ok.png"};


			titlePanel = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Padding = 10,
				Children = {titleLabel, editButton}
			};

			var editPanel = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Padding = new Thickness(10, 0),
				Children = {titleEdit, doneButton}
			};

			topPanel = new ContentView {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Content = titlePanel,
			};

			editButton.GestureRecognizers.Add(new TapGestureRecognizer {
				Command = new Command(() => Device.BeginInvokeOnMainThread (() => {
					topPanel.Content = editPanel;
					titleEdit.Focus ();
				}))
			});

			doneButton.GestureRecognizers.Add (new TapGestureRecognizer {
				Command = new Command (() => Device.BeginInvokeOnMainThread (UpdateTitle))
			});

			if (string.IsNullOrWhiteSpace (task.title)) {
				Device.BeginInvokeOnMainThread(() =>
					{
						topPanel.Content = editPanel;
						titleEdit.Focus();
						((Entry)editPanel.Children [0]).Focus ();
					});
				((Entry)editPanel.Children [0]).Focus ();
			}

            popupFragment = new ContentView();
			datePicker = new PopupDatePicker {
				OnItemSelected = view => {
                    if (view != null){
                        popupFragment.Content = view;
                    }
				}
			};
			datePicker.DateChanged += (sender, e) => {
				SelectedDate = datePicker.SelectedDate;
			};
			popupFragment.Content = datePicker.DefaultView;

			var categoryIcon = new Image {HorizontalOptions = LayoutOptions.EndAndExpand, HeightRequest = 12, Source = task.GetCategory().IconSource};

			categoryPicker = new CategoryPicker();
			categoryPicker.SelectedIndex = 0;

			foreach (Category category in CategoryHelper.AllCategories) {
				categoryPicker.Items.Add (category.Name);
				if (task.category == category.Name) {
					categoryPicker.SelectedIndex = categoryPicker.Items.Count - 1;
				}
			}				

			categoryPicker.SelectedIndexChanged += ( sender, args ) => categoryIcon.Source = CategoryHelper.AllCategories[categoryPicker.SelectedIndex].IconSource;
			var selectCategoryArrow = new Image { HorizontalOptions = LayoutOptions.End, Source = "select.png", HeightRequest = 10 };
			selectCategoryArrow.GestureRecognizers.Add(new TapGestureRecognizer {
				Command = new Command(() => categoryPicker.Focus ())
			});
				

			var categoryLayout = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					new DefaultLabel {Text = "Category", TextColor = StyleManager.MainColor},
					categoryIcon,
					categoryPicker,
					selectCategoryArrow
				}
			};
					
			isFavorite = task.isFavorite;
			var starResource = isFavorite ? FAVORITE_IMG : NOT_FAVORITE_IMG;

			var favoriteLabel = new DefaultLabel { Text = "Mark as favorite", TextColor = StyleManager.MainColor };
			var favoriteImage = new Image {
				HorizontalOptions = LayoutOptions.EndAndExpand,
				Source = starResource,
				HeightRequest = 15
			};
			var favoriteLayout = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			    Children = { favoriteLabel, favoriteImage}
			};
					
			favoriteImage.GestureRecognizers.Add(new TapGestureRecognizer {
				Command = new Command(() => Device.BeginInvokeOnMainThread (() => {
					isFavorite = !isFavorite;
					favoriteImage.Source = isFavorite ? FAVORITE_IMG : NOT_FAVORITE_IMG;
				}))
			});

			var actionText = "Create";

			if (!string.IsNullOrWhiteSpace (task.title))
				actionText = "Update";

			var buttons = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 0,
				Children = {
					new DefaultButton {
						Text = "Cancel",
						TextColor = Color.White,
						BackgroundColor = Color.Silver,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Command = new Command (onCancel.Dispatch)
					},
					new DefaultButton {
						Text = actionText,
						TextColor = Color.White,
						BackgroundColor = StyleManager.MainColor,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Command = new Command (() => Device.BeginInvokeOnMainThread (() => {
							UpdateTask (task);
							onSubmit.Dispatch ();
						}))
					}
				}
			};

			var mainLayout = new StackLayout {
				Padding = 15,
				BackgroundColor = StyleManager.AccentColor,
				Spacing = 20,
				Children = { // TODO time picker, pass time
					timePicker, categoryLayout, favoriteLayout, buttons
				}
			};

			Children.Add(topPanel);
			Children.Add(datePicker);
			Children.Add(popupFragment);
			Children.Add(mainLayout);
		}

		public void OnPopupShown() {
			datePicker.SelectDate (SelectedDate);
		}

		private void UpdateTitle(){
			Task.title = titleEdit.Text;
			topPanel.Content = titlePanel;
			titleLabel.Text = titleEdit.Text;
		}

		private void UpdateTask (TodoTask task)
		{
			task.date = datePicker.SelectedDate + timePicker.Time;
			task.category = categoryPicker.SelectedCategory.Name;
			task.isFavorite = isFavorite;
			UpdateTitle ();
		}
	}
}