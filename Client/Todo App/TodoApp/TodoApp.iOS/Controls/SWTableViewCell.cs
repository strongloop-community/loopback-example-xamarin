using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;
using TodoApp.Helpers;

namespace SWTableViewCell {
	public enum SWCellState {
		Center,
		Left,
		Right
	}

	public class ScrollingEventArgs : EventArgs {
		public SWCellState CellState { get; set; }
		public NSIndexPath IndexPath { get; set; }
	}

	public class CellUtilityButtonClickedEventArgs : EventArgs {
		public int UtilityButtonIndex { get; set; }
		public NSIndexPath IndexPath { get; set; }
	}

	public class SWTableViewCell : UITableViewCell {
		public const float UtilityButtonsWidthMax = 260;
		public const float UtilityButtonWidthDefault = 53.3f;
		public const float SectionIndexWidth = 15;
		private readonly List<UIView> mainView;


		private readonly float additionalRightPadding;


		// Scroll view to be added to UITableViewCell
		private readonly UIScrollView cellScrollView;
		private readonly UITableView containingTableView;

		// The cell's height
		private readonly float height;

		// Views that live in the scroll view
		private readonly SWUtilityButtonView scrollViewButtonViewRight;
		private readonly UIView scrollViewContentView;
		private readonly UIView scrollViewLeft;

		private SWCellState cellState; // The state of the cell within the scroll view, can be left, right or middle
		private UIButton[] rightUtilityButtons;
		private UIScrollViewDelegate scrollViewDelegate;

		public SWTableViewCell(UITableViewCellStyle style, string reuseIdentifier,
			UITableView containingTable, IEnumerable<UIButton> rightUtilityButtons,
			UIView leftView, List<UIView> mainView)
			: base(style, reuseIdentifier) {
			scrollViewLeft = leftView;
			this.rightUtilityButtons = rightUtilityButtons.ToArray();
			scrollViewButtonViewRight = new SWUtilityButtonView(this.rightUtilityButtons, this);

			containingTableView = containingTable;
			height = (float)containingTableView.RowHeight;
			scrollViewDelegate = new SWScrollViewDelegate(this);


			// Check if the UITableView will display Indices on the right. If that's the case, add a padding
			if (containingTableView.RespondsToSelector(new Selector("sectionIndexTitlesForTableView:"))) {
				string[] indices = containingTableView.Source.SectionIndexTitles(containingTableView);
				additionalRightPadding = indices == null || indices.Length == 0 ? 0 : SectionIndexWidth;
			}

			// Set up scroll view that will host our cell content
			cellScrollView = new UIScrollView(new RectangleF(0, 0, (float)Bounds.Width, height));
			cellScrollView.ContentSize = new SizeF((float)Bounds.Width + UtilityButtonsPadding, height);
			cellScrollView.ContentOffset = ScrollViewContentOffset;
			cellScrollView.Delegate = scrollViewDelegate;
			cellScrollView.ShowsHorizontalScrollIndicator = false;
			cellScrollView.ScrollsToTop = false;
			var tapGestureRecognizer = new UITapGestureRecognizer(OnScrollViewPressed);
			cellScrollView.AddGestureRecognizer(tapGestureRecognizer);

			// Set up the views that will hold the utility buttons
			scrollViewLeft.Frame = new RectangleF(ScrollLeftViewWidth, 0, ScrollLeftViewWidth, height);
			cellScrollView.AddSubview(scrollViewLeft);

			scrollViewButtonViewRight.Frame = new RectangleF((float)Bounds.Width, 0, RightUtilityButtonsWidth, height);

			cellScrollView.AddSubview(scrollViewButtonViewRight);


			// Populate the button views with utility buttons
			scrollViewButtonViewRight.PopulateUtilityButtons();
			// Create the content view that will live in our scroll view
			scrollViewContentView = new UIView(new RectangleF(ScrollLeftViewWidth, 0, (float)Bounds.Width, height));
			cellScrollView.AddSubview(scrollViewContentView);
			this.mainView = mainView;
			BuildMainView();

			AddSubview(cellScrollView);
			HideSwipedContent(false);
		}


		private float ScrollLeftViewWidth {
			get { return (float)scrollViewLeft.Frame.Width; }
		}

		private float RightUtilityButtonsWidth {
			get { return scrollViewButtonViewRight.UtilityButtonsWidth + additionalRightPadding; }
		}

		private float UtilityButtonsPadding {
			get { return ScrollLeftViewWidth + RightUtilityButtonsWidth; }
		}

		private PointF ScrollViewContentOffset {
			get { return new PointF(ScrollLeftViewWidth, 0); }
		}


		public SWCellState State {
			get { return cellState; }
		}

		public override UIColor BackgroundColor {
			get { return base.BackgroundColor; }
			set {
				base.BackgroundColor = value;
				scrollViewContentView.BackgroundColor = value;
			}
		}

		private void BuildMainView() {
			foreach (UIView uiView in mainView)
				scrollViewContentView.AddSubview(uiView);
		}

		private void OnScrollViewPressed(UITapGestureRecognizer tap) {
			if (cellState == SWCellState.Center) {
				if (containingTableView.Source != null) {
					NSIndexPath indexPath = containingTableView.IndexPathForCell(this);
					containingTableView.Source.RowSelected(containingTableView, indexPath);
				}
			} else {
				// Scroll back to center
				HideSwipedContent(true);
			}
		}

		public void HideSwipedContent(bool animated) {
			cellScrollView.SetContentOffset(new PointF(ScrollLeftViewWidth, 0), animated);
			cellState = SWCellState.Center;
			OnScrolling();
		}

		protected internal void OnLeftUtilityButtonPressed(UIButton sender) {
			var tag = (int)sender.Tag;
			EventHandler<CellUtilityButtonClickedEventArgs> handler = UtilityButtonPressed;
			if (handler != null) {
				NSIndexPath indexPath = containingTableView.IndexPathForCell(this);
				handler(sender, new CellUtilityButtonClickedEventArgs { IndexPath = indexPath, UtilityButtonIndex = tag });
			}
		}

		private void OnScrolling() {
			EventHandler<ScrollingEventArgs> handler = Scrolling;
			if (handler != null) {
				NSIndexPath indexPath = containingTableView.IndexPathForCell(this);
				handler(this, new ScrollingEventArgs { CellState = cellState, IndexPath = indexPath });
			}
		}

		public event EventHandler<ScrollingEventArgs> Scrolling;
		public event EventHandler<CellUtilityButtonClickedEventArgs> UtilityButtonPressed;

		public override void LayoutSubviews() {
			base.LayoutSubviews();
			cellScrollView.Frame = new RectangleF(0, 0, (float)Bounds.Width, height);
			cellScrollView.ContentSize = new SizeF((float)Bounds.Width + UtilityButtonsPadding, height);
			cellScrollView.ContentOffset = new PointF(ScrollLeftViewWidth, 0);
			scrollViewLeft.Frame = new RectangleF(ScrollLeftViewWidth, 0, ScrollLeftViewWidth, height);
			scrollViewButtonViewRight.Frame = new RectangleF((float)Bounds.Width, 0, RightUtilityButtonsWidth, height);
			scrollViewContentView.Frame = new RectangleF(ScrollLeftViewWidth, 0, (float)Bounds.Width, height);
		}

		private class SWScrollViewDelegate : UIScrollViewDelegate {
			private readonly SWTableViewCell cell;

			public SWScrollViewDelegate(SWTableViewCell cell) {
				this.cell = cell;
			}

			public override void DecelerationEnded(UIScrollView scrollView) {
				switch (cell.cellState) {
					case SWCellState.Center:
						ScrollToCenter(scrollView);
						break;
					case SWCellState.Left:
						ScrollToLeft(scrollView);
						break;
					case SWCellState.Right:
						ScrollToRight(scrollView);
						break;
					default:
						break;
				}
			}

			public override void WillEndDragging(UIScrollView scrollView, CGPoint velocity,
				ref CGPoint targetContentOffset) {
				float rightThreshold = 1.5f * cell.UtilityButtonsPadding;
				float leftThreshold = cell.ScrollLeftViewWidth / 2;

				switch (cell.cellState) {
					case SWCellState.Center:

						if (velocity.X >= 0.5f)
							ScrollToRight(scrollView);
						else if (velocity.X <= -0.5f)
							ScrollToLeft(scrollView);
						else {
							if (scrollView.ContentOffset.X < leftThreshold)
								ScrollToLeft(scrollView);
							else if (scrollView.ContentOffset.X > rightThreshold)
								ScrollToRight(scrollView);
							else ScrollToCenter(scrollView);
						}

						break;
					case SWCellState.Left:
						if (velocity.X >= 0.5f) {
							ScrollToCenter(scrollView);
						} else if (velocity.X <= -0.5f) {
							// No-op
						} else {
							if (scrollView.ContentOffset.X > leftThreshold)
								ScrollToCenter(scrollView);
							else ScrollToLeft(scrollView);
						}
						break;
					case SWCellState.Right:
						if (velocity.X >= 0.5f) {
							// No-op
						} else if (velocity.X <= -0.5f) {
							ScrollToCenter(scrollView);
						} else {
							if (scrollView.ContentOffset.X < rightThreshold)
								ScrollToCenter(scrollView);
							else ScrollToRight(scrollView);
						}
						break;
					default:
						break;
				}
			}

			private void ScrollToCenter(UIScrollView scrollView) {
				if (scrollView != null)
					scrollView.SetContentOffset(new PointF(cell.ScrollLeftViewWidth, 0), true);
				cell.cellState = SWCellState.Center;
				cell.OnScrolling();
			}

			private void ScrollToLeft(UIScrollView scrollView) {
				if (scrollView != null)
					scrollView.SetContentOffset(new PointF(0, 0), true);
				cell.cellState = SWCellState.Left;
				cell.OnScrolling();
			}

			private void ScrollToRight(UIScrollView scrollView) {
				if (scrollView != null)
					scrollView.SetContentOffset(new PointF(cell.ScrollLeftViewWidth * 2, 0), true);
				cell.cellState = SWCellState.Right;
				cell.OnScrolling();
			}

			public override void Scrolled(UIScrollView scrollView) {
				if (scrollView.ContentOffset.X > cell.ScrollLeftViewWidth) {
					//expose the right view
					cell.scrollViewButtonViewRight.Frame =
						new RectangleF(
							(float)(scrollView.ContentOffset.X + cell.Bounds.Width - cell.RightUtilityButtonsWidth),
							0, cell.RightUtilityButtonsWidth, cell.height);
				} else {
					cell.scrollViewLeft.Frame = new RectangleF((float)scrollView.ContentOffset.X, 0,
						cell.ScrollLeftViewWidth, cell.height);
				}
			}
		}
	}

	internal class SWUtilityButtonView : UIView {
		private readonly SWTableViewCell parentCell;
		private readonly float utilityButtonWidth;
		private readonly UIButton[] utilityButtons;

		public SWUtilityButtonView(UIButton[] buttons, SWTableViewCell parentCell) {
			utilityButtons = buttons;
			this.parentCell = parentCell;
			utilityButtonWidth = CalculateUtilityButtonWidth();
			AddSubviews(buttons);
		}

		public float UtilityButtonsWidth {
			get { return utilityButtonWidth * utilityButtons.Length; }
		}

		private float CalculateUtilityButtonWidth() {
			float buttonWidth = SWTableViewCell.UtilityButtonWidthDefault;
			if (buttonWidth * utilityButtons.Length > SWTableViewCell.UtilityButtonsWidthMax) {
				float buffer = buttonWidth * utilityButtons.Length - SWTableViewCell.UtilityButtonsWidthMax;
				buttonWidth -= buffer / utilityButtons.Length;
			}
			return buttonWidth;
		}

		public void PopulateUtilityButtons() {
			for (int i = 0; i < utilityButtons.Length; i++) {
				UIButton button = utilityButtons[i];
				float x = 0;
				if (i >= 1)
					x = utilityButtonWidth * i;
				button.Frame = new RectangleF(x, 0, utilityButtonWidth, (float)Bounds.Height);
				button.Tag = i;
				button.TouchDown += (sender, e) => parentCell.OnLeftUtilityButtonPressed((UIButton)sender);
			}
		}
	}

	public static class SWButtonCellExtensions {
		public static void AddUtilityButton(this List<UIButton> list, string title, UIColor color) {
			var button = new UIButton(UIButtonType.Custom);
			button.BackgroundColor = color;
			button.SetTitle(title, UIControlState.Normal);
			button.SetTitleColor(UIColor.White, UIControlState.Normal);
			list.Add(button);
		}

		public static void AddUtilityButton(this List<UIButton> list, UIImage image, Action onClickAction = null) {
			var button = new UIButton(UIButtonType.Custom) {
				BackgroundColor = UIColor.Gray
			};
			button.TouchDown += (sender, args) => onClickAction.Dispatch ();
			button.SetImage(image, UIControlState.Normal);
			list.Add(button);
		}
	}
}