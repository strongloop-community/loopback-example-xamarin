using TodoApp.Controls.DefaultControls;
using TodoApp.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer( typeof (DefaultButton), typeof (DefaultButtonRenderer) )]

namespace TodoApp.iOS.Renderers {
	internal class DefaultButtonRenderer : ButtonRenderer {
		protected override void OnElementChanged( ElementChangedEventArgs<Button> e ) {
			base.OnElementChanged(e);

			if (Control == null)
				return;

			Control.Layer.CornerRadius = 0;
		}
	}
}