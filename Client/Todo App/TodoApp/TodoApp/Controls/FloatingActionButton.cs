using System;
using TodoApp.Helpers;
using Xamarin.Forms;

namespace TodoApp.Controls {
	public class FloatingActionButton : Image {
		public static string CLOSED_BUTTON_IMAGE_SOURCE = "floating_button.png";

		public static string OPENED_BUTTON_IMAGE_SOURCE = "floating_button_close.png";

		public FloatingActionButton() {
			SizeRequest = 40;
			AllowTap = true;

			GestureRecognizers.Add(new TapGestureRecognizer {
				Command = new Command(() => {
					if (TapAction != null) {
						TapAction.Dispatch();
						return;
					}

					if (IsOpened) {
						IsOpened = false;
						CloseAction.Dispatch();
					} else {
						IsOpened = true;
						OpenAction.Dispatch();
					}
				}, () => AllowTap)
			});
		}

		public bool IsOpened { get; private set; }

		public double SizeRequest {
			set {
				WidthRequest = value;
				HeightRequest = value;
			}
		}

		public Action OpenAction { get; set; }
		public Action CloseAction { get; set; }
		public Action TapAction { get; set; }
		public bool AllowTap { get; set; }
	}
}