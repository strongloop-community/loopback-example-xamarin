using TodoApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer( typeof (TimePicker), typeof (CustomTimePickerRenderer) )]

namespace TodoApp.iOS.Renderers {
	internal class CustomTimePickerRenderer : TimePickerRenderer {
		protected override void OnElementChanged( ElementChangedEventArgs<TimePicker> e ) {
			base.OnElementChanged(e);

			Control.BackgroundColor = UIColor.FromRGB(229, 83, 101);
			Control.TextColor = UIColor.White;

			var timePicker = (UIDatePicker) Control.InputView;

			timePicker.BackgroundColor = UIColor.FromRGBA(229, 83, 101, 200);
			timePicker.TintColor = UIColor.White;
		}
	}
}