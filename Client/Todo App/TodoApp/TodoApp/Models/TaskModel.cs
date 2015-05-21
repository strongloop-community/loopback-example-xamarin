using System;

namespace TodoApp.Models {
	public class TaskModel {
		public string Name { get; set; }
		public Category Category { get; set; }
		public bool IsFavourite { get; set; }
		public bool IsCompleted { get; set; }
		public DateTime DueTime { get; set; }
		public bool IsDeleted { get; set; }
	}
}