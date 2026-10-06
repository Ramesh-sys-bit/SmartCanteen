using SmartCanteen.Models;

namespace SmartCanteen.Data
{
	public static class DbSeeder
	{
		public static void Seed(AppDbContext db)
		{
			if (!db.Categories.Any())
			{
				var categories = new List<Category>
				{
					new Category { Name = "Breakfast" },
					new Category { Name = "Main Course" },
					new Category { Name = "Snacks" },
					new Category { Name = "Drinks" },
					new Category { Name = "Desserts" }
				};

				db.Categories.AddRange(categories);
				db.SaveChanges();
			}

			if (!db.MenuItems.Any())
			{
				var categories = db.Categories.ToList();

				var breakfast = categories.FirstOrDefault(x => x.Name == "Breakfast");
				var mainCourse = categories.FirstOrDefault(x => x.Name == "Main Course");
				var snacks = categories.FirstOrDefault(x => x.Name == "Snacks");
				var drinks = categories.FirstOrDefault(x => x.Name == "Drinks");
				var desserts = categories.FirstOrDefault(x => x.Name == "Desserts");

				var items = new List<MenuItem>();

				if (breakfast != null)
				{
					items.Add(new MenuItem
					{
						Name = "Masala Dosa",
						Price = 40,
						PrepMinutes = 10,
						StockQty = 50,
						IsAvailable = true,
						CategoryId = breakfast.Id
					});

					items.Add(new MenuItem
					{
						Name = "Idli Vada",
						Price = 35,
						PrepMinutes = 8,
						StockQty = 50,
						IsAvailable = true,
						CategoryId = breakfast.Id
					});
				}

				if (mainCourse != null)
				{
					items.Add(new MenuItem
					{
						Name = "Veg Fried Rice",
						Price = 80,
						PrepMinutes = 15,
						StockQty = 40,
						IsAvailable = true,
						CategoryId = mainCourse.Id
					});

					items.Add(new MenuItem
					{
						Name = "Veg Meals",
						Price = 70,
						PrepMinutes = 12,
						StockQty = 40,
						IsAvailable = true,
						CategoryId = mainCourse.Id
					});
				}

				if (snacks != null)
				{
					items.Add(new MenuItem
					{
						Name = "Samosa",
						Price = 20,
						PrepMinutes = 5,
						StockQty = 60,
						IsAvailable = true,
						CategoryId = snacks.Id
					});

					items.Add(new MenuItem
					{
						Name = "Veg Sandwich",
						Price = 45,
						PrepMinutes = 8,
						StockQty = 40,
						IsAvailable = true,
						CategoryId = snacks.Id
					});
				}

				if (drinks != null)
				{
					items.Add(new MenuItem
					{
						Name = "Fresh Lime",
						Price = 30,
						PrepMinutes = 3,
						StockQty = 50,
						IsAvailable = true,
						CategoryId = drinks.Id
					});

					items.Add(new MenuItem
					{
						Name = "Tea",
						Price = 15,
						PrepMinutes = 3,
						StockQty = 100,
						IsAvailable = true,
						CategoryId = drinks.Id
					});
				}

				if (desserts != null)
				{
					items.Add(new MenuItem
					{
						Name = "Gulab Jamun",
						Price = 30,
						PrepMinutes = 3,
						StockQty = 40,
						IsAvailable = true,
						CategoryId = desserts.Id
					});
				}

				db.MenuItems.AddRange(items);
				db.SaveChanges();
			}
		}
	}
}