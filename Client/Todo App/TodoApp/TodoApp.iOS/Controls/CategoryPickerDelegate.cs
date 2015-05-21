using System;
using CoreGraphics;
using TodoApp.Helpers;
using TodoApp.Models;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace TodoApp.iOS.Controls {
	internal class CategoryPickerDelegate : UIPickerViewDelegate {
		private readonly IUIPickerViewDelegate viewDelegate;

		public CategoryPickerDelegate( IUIPickerViewDelegate viewDelegate ) {
			this.viewDelegate = viewDelegate;
		}

		public override UIView GetView( UIPickerView pickerView, nint row, nint component, UIView view ) {
			nfloat width = pickerView.RowSizeForComponent(component).Width;
			nfloat height = pickerView.RowSizeForComponent(component).Height;

			var pickerCustomView = new UIView {
				Frame = new CGRect(0, 0, width - 10, height)
			};
			var pickerImageView = new UIImageView {
				Frame = new CGRect(20, height/4, height/2, height/2)
			};
			var pickerViewLabel = new UILabel {
				Frame = new CGRect(20 + height, 0, width - 10, height)
			};

			pickerCustomView.AddSubview(pickerImageView);
			pickerCustomView.AddSubview(pickerViewLabel);

			Category category = CategoryHelper.AllCategories[(int) row];

			pickerImageView.Image = new UIImage(category.IconSource);
			pickerViewLabel.BackgroundColor = UIColor.Clear;
			pickerViewLabel.Text = category.Name;
			pickerViewLabel.TextColor = category.Color.ToUIColor();

			return pickerCustomView;
		}

		public override void Selected( UIPickerView pickerView, nint row, nint component ) {
			viewDelegate.Selected(pickerView, row, component);
		}
	}
}