using TodoApp.Controls.DefaultControls;
using TodoApp.Helpers;
using TodoApp.iOS.Controls;
using TodoApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer( typeof (CategoryPicker), typeof (CategoryPickerRenderer) )]

namespace TodoApp.iOS.Renderers {
	internal class CategoryPickerRenderer : PickerRenderer {
		protected override void OnElementChanged( ElementChangedEventArgs<Picker> e ) {
			base.OnElementChanged(e);

			Control.TextColor = CategoryHelper.AllCategories[0].Color.ToUIColor();
			Control.BorderStyle = UITextBorderStyle.None;
			Control.Font = UIFont.FromName("HelveticaNeue-Light", 12);

			var picker = Control.InputView as UIPickerView;
			if (picker != null) {
				IUIPickerViewDelegate viewDelegate = picker.Delegate;
				picker.Delegate = new CategoryPickerDelegate(viewDelegate);
			}

			var element = Element as CategoryPicker;
			if (element == null)
				return;
			element.SelectedIndexChanged += ( sender, args ) => {
				Color newColor = CategoryHelper.AllCategories[element.SelectedIndex].Color;
				Control.TextColor = newColor.ToUIColor();
			};
		}
	}
}