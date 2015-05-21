using TodoApp.Controls.DefaultControls;
using TodoApp.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer( typeof (SelectableScrollView), typeof (CustomScrollViewRenderer) )]

namespace TodoApp.iOS.Renderers {
	internal class CustomScrollViewRenderer : ScrollViewRenderer {
		protected override void OnElementChanged( VisualElementChangedEventArgs e ) {
			base.OnElementChanged(e);

			ShowsHorizontalScrollIndicator = false;

			var scrollView = Element as SelectableScrollView;
			if (scrollView == null)
				return;

			// User releases finger.
			DraggingEnded += ( sender, args ) => {
				// If speed is not high enough to continue scrolling.
				if (!args.Decelerate)
					scrollView.NotifyScrollEnded();
			};

			// End of deceleation (auto scolling).
			DecelerationEnded += ( sender, args ) => scrollView.NotifyScrollEnded();
		}
	}
}