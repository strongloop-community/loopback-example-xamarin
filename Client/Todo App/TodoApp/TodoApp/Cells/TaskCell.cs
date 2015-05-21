using System;
using TodoApp.Models;
using TodoApp.Helpers;
using Xamarin.Forms;
using LBXamarinSDK;
using LBXamarinSDK.LBRepo;

namespace TodoApp.Cells {
	public class TaskCell : ViewCell {
		public delegate void TaskEvent(TodoTask task);

		public TodoTask Task{get;}

		public TaskCell( TodoTask task) {
			Task = task;
		}

		public string Title {
			get { return Task.title; }
		}

		public string TaskID {
			get{ return Task.id; }
		}

		public bool IsFavorite {
			get { return Task.isFavorite; }
		}

		public DateTime Date {
			get { return Task.date; }
		}

		public string CategoryName {
			get { return Task.category; }
		}

		public Color CategoryColor {
			get { return CategoryHelper.GetByName(Task.category).Color; }
		}

		public string CategoryIcon {
			get { return CategoryHelper.GetByName(Task.category).IconSource; }
		}

		public bool IsDone {
			get { return Task.isDone; }
		}

		public Action OnDone { get; set; }
		public Action OnEdit { get; set; }
		public Action OnDelete { get; set; }
		public Action OnFavorite { get; set; }
	}
}