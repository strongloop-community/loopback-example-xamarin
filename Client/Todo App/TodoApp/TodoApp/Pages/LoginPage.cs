using TodoApp.Controls.DefaultControls;
using TodoApp.Pages.Base;
using TodoApp.Styles;
using Xamarin.Forms;

namespace TodoApp.Pages {
	internal class LoginPage : BaseContentPage {
		private enum Mode { Login, Signup }
		private Mode currentMode;
		private DefaultButton submitButton;
		private FormattedString signUpString;
		private DefaultEntry emailTextBox;
		private DefaultEntry passwordTextBox;

		public LoginPage() {
			NavigationPage.SetHasNavigationBar(this, false);

			// Status bar for iOS.
			var statusBar = new BoxView();
			if (Device.OS == TargetPlatform.iOS) {
				statusBar.BackgroundColor = StyleManager.DarkAccentColor;
				statusBar.HeightRequest = 20;
			}

			// Application title & subtitle.
			var titleString = new FormattedString();
			titleString.Spans.Add(new Span {ForegroundColor = StyleManager.AccentColor, FontAttributes = FontAttributes.Italic, FontSize = 40, Text = "Todo "});
			titleString.Spans.Add(new Span {ForegroundColor = StyleManager.DarkAccentColor, FontAttributes = FontAttributes.Italic, FontSize = 32, Text = "app"});
			var title = new DefaultLabel {FormattedText = titleString, HorizontalOptions = LayoutOptions.Center};

			var subtitle = new ItalicLabel {Text = "Helping you doing everything", TextColor = StyleManager.AccentColor};

			var titleLayout = new StackLayout {
				Children = {title, subtitle},
				Spacing = 2,
				Padding = new Thickness(0, 30)
			};

			// Start layout (down arrow).
			var login = new ItalicLabel {Text = "Log in", CustomFontSize = NamedSize.Medium};
			var downArrow = new Image {
				Source = "down_button.png",
				HeightRequest = 60
			};
			var startLayout = new StackLayout {
				Padding = new Thickness(0, 100),
				Spacing = 18,
				Children = {login, downArrow}
			};

			// Login layout.
			emailTextBox = new DefaultEntry {Placeholder = "Email"};
			passwordTextBox = new DefaultEntry {Placeholder = "Password", IsPassword = true};
			var entryLayout = new StackLayout {
				Spacing = 15,
				Children = {emailTextBox, passwordTextBox}
			};

			submitButton = new DefaultButton {Command = new Command (Submit)};
			signUpString = new FormattedString();
			signUpString.Spans.Add(new Span {ForegroundColor = StyleManager.AccentColor, FontAttributes = FontAttributes.Italic});
			signUpString.Spans.Add(new Span {ForegroundColor = StyleManager.DarkAccentColor, FontAttributes = FontAttributes.Bold});
			var toggleModeLabel = new DefaultLabel {FormattedText = signUpString, HorizontalOptions = LayoutOptions.Center};

			var signInLayout = new StackLayout {
				Padding = new Thickness(0, 20),
				Spacing = 30,
				Children = {submitButton, toggleModeLabel}
			};

			var loginLayout = new StackLayout {
				Padding = new Thickness(0, 35),
				Children = {entryLayout, signInLayout},
				IsVisible = false
			};

			downArrow.GestureRecognizers.Add(new TapGestureRecognizer {
				Command = new Command(() => {
					loginLayout.IsVisible = true;
					startLayout.IsVisible = false;
				})
			});

			var toggleModeGestureRecognizer = new TapGestureRecognizer ();
			toggleModeGestureRecognizer.Tapped += (sender, ev) => toggleMode ();
			toggleModeLabel.GestureRecognizers.Add (toggleModeGestureRecognizer);

			// Main content layout.
			var contentLayout = new StackLayout {
				Padding = 25,
				HorizontalOptions = LayoutOptions.Center,
				Children = {
					titleLayout,
					startLayout,
					loginLayout
				}
			};

			Content = new StackLayout {
				Children = {statusBar, contentLayout}
			};

			setLoginMode ();
		}

		private void setLoginMode(){
			currentMode = Mode.Login;
			submitButton.Text = "Login";
			signUpString.Spans [0].Text = "First time in Todo? ";
			signUpString.Spans [1].Text = "Sign up ";
		}

		private void setSignupMode(){
			currentMode = Mode.Signup;
			submitButton.Text = "Signup";
			signUpString.Spans [0].Text = "Already have an account? ";
			signUpString.Spans [1].Text = "Login ";
		}

		private void toggleMode(){
			switch(currentMode){
			case Mode.Login:
				setSignupMode ();
				break;
			case Mode.Signup:
				setLoginMode ();
				break;
			}
		}

		private void Submit ()
		{
			var email = emailTextBox.Text;
			var password = passwordTextBox.Text;

			if (string.IsNullOrEmpty (email) || string.IsNullOrEmpty (password)) {
				DisplayAlert("Error", "Please input your email and password before loggin in", "OK");
				return;
			}

			switch (currentMode) {
			case Mode.Login:
				Login (email, password);
				break;
			case Mode.Signup:
				Signup (email, password);
				break;
			}
		}

		async void Signup(string email, string password){
			if (await LoginHelper.Signup (email, password)) {
				await Navigation.PushAsync (new HomePage ());	
			} else {
				await DisplayAlert ("Error", "Invalid signup credentials given.\nPlease make sure your email is properly formatted.", "OK");
			}
		}

		async void Login (string email, string password)
		{
			if (await LoginHelper.Login (email, password)) {
				await Navigation.PushAsync (new HomePage ());	
			} else {
				await DisplayAlert ("Error", "Invalid Login credentials", "OK");
			}
		}
	}
}