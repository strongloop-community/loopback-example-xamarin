using Xamarin.Forms;
using TodoApp.Models;
using TodoApp.Helpers;

namespace TodoApp.Controls.DefaultControls {
	public class CategoryPicker : Picker {
		public CategoryPicker() {
			BackgroundColor = Color.Transparent;
			this.SelectedIndexChanged += ( sender, args ) => UpdateSelectedCategory();
		}

		public Category SelectedCategory { get; private set; }

		private void UpdateSelectedCategory(){
			SelectedCategory = CategoryHelper.GetByName(this.Items [this.SelectedIndex]);
		}
	}
}