using System;
using TodoApp.Controls.DefaultControls;
using Xamarin.Forms;

namespace TodoApp.Controls {
	internal class FloatingActionControl : StackLayout {
		private const string FLOATING_BUTTON_IMAGE = "floating_button_add.png";

		private readonly FloatingActionButton _floatingActionButton;
		private readonly CustomFontLabel _tooltipLabel;
		private readonly Frame _tooltipLayout;

		public FloatingActionControl() {
			Spacing = 10;
			Orientation = StackOrientation.Horizontal;
			HorizontalOptions = LayoutOptions.End;

			_floatingActionButton = new FloatingActionButton {
				HorizontalOptions = LayoutOptions.End,
				Source = FLOATING_BUTTON_IMAGE,
				SizeRequest = 30
			};
			_tooltipLabel = new CustomFontLabel {TextColor = Color.Black, CustomFontSize = NamedSize.Micro};

			_tooltipLayout = new Frame {
				BackgroundColor = Color.White,
				Padding = new Thickness(10, 0),
				HasShadow = false,
				Content = _tooltipLabel
			};

			Children.Add(_tooltipLayout);
			Children.Add(_floatingActionButton);
		}
			
		public string Tooltip {
			get { return _tooltipLabel.Text; }
			set {
				_tooltipLabel.Text = value;
				_tooltipLayout.IsVisible = !string.IsNullOrEmpty(value);
			}
		}

		public ImageSource ButtonImageSource {
			get { return _floatingActionButton.Source; }
			set { _floatingActionButton.Source = value; }
		}
			
		public double SizeRequest {
			set { _floatingActionButton.SizeRequest = value; }
		}

		public Action TapAction {
			get { return _floatingActionButton.TapAction; }
			set { _floatingActionButton.TapAction = value; }
		}

		public Action OpenAction {
			get { return _floatingActionButton.OpenAction; }
			set { _floatingActionButton.OpenAction = value; }
		}

		public Action CloseAction {
			get { return _floatingActionButton.CloseAction; }
			set { _floatingActionButton.CloseAction = value; }
		}
	}
}