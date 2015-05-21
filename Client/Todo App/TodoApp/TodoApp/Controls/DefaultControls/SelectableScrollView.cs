using System;
using Xamarin.Forms;
using TodoApp.Helpers;

namespace TodoApp.Controls.DefaultControls {
	public class SelectableScrollView : ScrollView {
		public int SelectedItem { get; set; }

		public event EventHandler ScrollEnded;

		public void NotifyScrollEnded() {
			ScrollEnded.Dispatch (this, new EventArgs ());
		}
	}
}