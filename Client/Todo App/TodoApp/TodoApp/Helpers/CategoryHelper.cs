using System.Collections.Generic;
using TodoApp.Models;
using Xamarin.Forms;

namespace TodoApp.Helpers {
	public static class CategoryHelper {

		private static readonly Dictionary<string, Category> categoryByName;
		static CategoryHelper(){
			categoryByName = new Dictionary<string, Category> {
				{ "Home", HomeCategory },
				{ "Fitness", FitnessCategory },
				{ "Friends", FriendsCategory },
				{ "Free Time", FreeTimeCategory }
			};
		}

		public static Category HomeCategory {get ; } = new Category {
				Name = "Home",
				Color = Color.FromRgb(106, 93, 150),
				IconSource = "home.png"
			};

		public static Category FitnessCategory { get; }= new Category {
					Name = "Fitness",
					Color = Color.FromRgb(221, 106, 158),
					IconSource = "fitness.png"
		};

		public static Category FriendsCategory { get ; } = new Category {
					Name = "Friends",
					Color = Color.FromRgb(141, 199, 89),
					IconSource = "friends.png"
		};

		public static Category FreeTimeCategory { get; }= new Category {
					Name = "Free Time",
					Color = Color.FromRgb(77, 166, 220),
					IconSource = "free_time.png"
		};

		public static List<Category> AllCategories {
			get {
				return new List<Category> {
					HomeCategory, FitnessCategory, FriendsCategory, FreeTimeCategory
				};
			}
		}

		public static Category GetByName(string categoryName){
			return categoryByName [categoryName];
		}
	}
}