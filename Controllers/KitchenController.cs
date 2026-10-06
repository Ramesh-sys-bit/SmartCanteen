using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCanteen.Data;

namespace SmartCanteen.Controllers
{
	public class KitchenController : Controller
	{
		private readonly AppDbContext _db;

		public KitchenController(AppDbContext db)
		{
			_db = db;
		}

		public async Task<IActionResult> Index()
		{
			var orders = await _db.Orders
				.Include(x => x.Student)
				.Include(x => x.Items)
					.ThenInclude(x => x.MenuItem)
				.Where(x =>
					x.Status == "Placed" ||
					x.Status == "Preparing" ||
					x.Status == "Ready")
				.OrderBy(x => x.OrderedOn)
				.ToListAsync();

			return View(orders);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateStatus(int id, string status)
		{
			var order = await _db.Orders.FindAsync(id);

			if (order == null)
			{
				return NotFound();
			}

			if (status != "Preparing" &&
				status != "Ready" &&
				status != "Completed")
			{
				return BadRequest();
			}

			order.Status = status;

			await _db.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}
	}
}