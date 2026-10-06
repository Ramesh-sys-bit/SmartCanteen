using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCanteen.Data;
using SmartCanteen.Models;

namespace SmartCanteen.Controllers
{
	public class MenuController : Controller
	{
		private readonly AppDbContext _db;

		public MenuController(AppDbContext db)
		{
			_db = db;
		}

		public async Task<IActionResult> Index()
		{
			var menuItems = await _db.MenuItems
				.Where(x => x.IsAvailable && x.StockQty > 0)
				.OrderBy(x => x.CategoryId)
				.ThenBy(x => x.Name)
				.ToListAsync();

			return View(menuItems);
		}

		[HttpGet]
		public async Task<IActionResult> Manage()
		{
			var menuItems = await _db.MenuItems
				.OrderBy(x => x.CategoryId)
				.ThenBy(x => x.Name)
				.ToListAsync();

			return View(menuItems);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateStock(
			int id,
			int stockQty,
			bool isAvailable)
		{
			var item = await _db.MenuItems
				.FirstOrDefaultAsync(x => x.Id == id);

			if (item == null)
			{
				return NotFound();
			}

			if (stockQty < 0 || stockQty > 500)
			{
				TempData["Error"] =
					"Stock quantity must be between 0 and 500.";

				return RedirectToAction(nameof(Manage));
			}

			item.StockQty = stockQty;
			item.IsAvailable = isAvailable;

			await _db.SaveChangesAsync();

			TempData["Success"] =
				$"{item.Name} stock updated successfully.";

			return RedirectToAction(nameof(Manage));
		}
	}
}
