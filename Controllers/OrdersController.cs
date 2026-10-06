using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCanteen.Data;
using SmartCanteen.Models;
using SmartCanteen.Services;

namespace SmartCanteen.Controllers
{
	public class OrdersController : Controller
	{
		private readonly AppDbContext _db;
		private readonly ISlotService _slotService;

		public OrdersController(
			AppDbContext db,
			ISlotService slotService)
		{
			_db = db;
			_slotService = slotService;
		}

		[HttpGet]
		public async Task<IActionResult> Checkout(
			int? menuItemId,
			int? quantity)
		{
			var menuItems = await _db.MenuItems
				.Where(x => x.IsAvailable && x.StockQty > 0)
				.OrderBy(x => x.CategoryId)
				.ThenBy(x => x.Name)
				.ToListAsync();

			if (menuItemId.HasValue && quantity.HasValue)
			{
				var selectedItem = menuItems
					.FirstOrDefault(x => x.Id == menuItemId.Value);

				if (selectedItem != null)
				{
					ViewBag.SelectedMenuItemId =
						selectedItem.Id;

					ViewBag.SelectedQuantity =
						Math.Min(
							Math.Max(quantity.Value, 1),
							selectedItem.StockQty);
				}
			}

			return View(menuItems);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Checkout(
			CheckoutViewModel model)
		{
			if (!ModelState.IsValid)
			{
				var menuItems = await _db.MenuItems
					.Where(x =>
						x.IsAvailable &&
						x.StockQty > 0)
					.OrderBy(x => x.CategoryId)
					.ThenBy(x => x.Name)
					.ToListAsync();

				return View(menuItems);
			}

			var student = await _db.Students
				.FirstOrDefaultAsync(
					x => x.RollNo == model.RollNo);

			if (student == null)
			{
				ModelState.AddModelError(
					"RollNo",
					"Student roll number not found.");

				var menuItems = await _db.MenuItems
					.Where(x =>
						x.IsAvailable &&
						x.StockQty > 0)
					.ToListAsync();

				return View(menuItems);
			}

			if (model.Lines == null ||
				model.Lines.Count == 0)
			{
				ModelState.AddModelError(
					"",
					"Add at least one item.");

				return View(await _db.MenuItems
					.Where(x =>
						x.IsAvailable &&
						x.StockQty > 0)
					.ToListAsync());
			}

			var itemIds = model.Lines
				.Where(x => x.Quantity > 0)
				.Select(x => x.MenuItemId)
				.Distinct()
				.ToList();

			if (itemIds.Count == 0)
			{
				ModelState.AddModelError(
					"",
					"Add at least one item.");

				var availableItems = await _db.MenuItems
					.Where(x =>
						x.IsAvailable &&
						x.StockQty > 0)
					.ToListAsync();

				return View(availableItems);
			}

			var items = await _db.MenuItems
				.Where(x => itemIds.Contains(x.Id))
				.ToListAsync();

			decimal totalAmount = 0;
			int totalQty = 0;
			int longestPrep = 0;

			var order = new Order
			{
				StudentId = student.Id,
				OrderedOn = DateTime.Now,
				Status = "Placed",
				PaymentStatus = "Pending"
			};

			foreach (var line in model.Lines)
			{
				// Ignore items that were not selected.
				if (line.Quantity == 0)
				{
					continue;
				}

				if (line.Quantity < 0)
				{
					ModelState.AddModelError(
						"",
						"Quantity cannot be negative.");

					return View(items);
				}

				var item = items.FirstOrDefault(
					x => x.Id == line.MenuItemId);

				if (item == null)
				{
					ModelState.AddModelError(
						"",
						"Invalid menu item.");

					return View(items);
				}

				if (!item.IsAvailable ||
					item.StockQty < line.Quantity)
				{
					ModelState.AddModelError(
						"",
						$"{item.Name} does not have enough stock.");

					return View(items);
				}

				totalQty += line.Quantity;

				longestPrep = Math.Max(
					longestPrep,
					item.PrepMinutes);

				totalAmount +=
					item.Price * line.Quantity;

				order.Items.Add(new OrderItem
				{
					MenuItemId = item.Id,
					Quantity = line.Quantity,
					UnitPrice = item.Price
				});

				item.StockQty -= line.Quantity;
			}

			if (totalQty == 0)
			{
				ModelState.AddModelError(
					"",
					"Add at least one item.");

				return View(items);
			}

			var pendingOrders = await _db.Orders
				.CountAsync(x =>
					x.Status == "Placed" ||
					x.Status == "Preparing");

			var slot = _slotService.Calculate(
				DateTime.Now,
				longestPrep,
				totalQty,
				pendingOrders);

			order.EstimatedReadyOn =
				slot.ReadyAt;

			order.PickupSlot =
				model.PreferredPickup ?? slot.Slot;

			order.TotalAmount =
				totalAmount;

			var lastToken = await _db.Orders
				.OrderByDescending(x => x.Id)
				.Select(x => (int?)x.TokenNo)
				.FirstOrDefaultAsync();

			order.TokenNo =
				(lastToken ?? 0) + 1;

			_db.Orders.Add(order);

			await _db.SaveChangesAsync();

			return RedirectToAction(
				"Track",
				new { id = order.Id });
		}

		public async Task<IActionResult> Track(int id)
		{
			var order = await _db.Orders
				.Include(x => x.Student)
				.Include(x => x.Items)
					.ThenInclude(x => x.MenuItem)
				.FirstOrDefaultAsync(
					x => x.Id == id);

			if (order == null)
			{
				return NotFound();
			}

			return View(order);
		}

		[HttpGet]
		public async Task<IActionResult> MyOrders(
			string rollNo)
		{
			if (string.IsNullOrWhiteSpace(rollNo))
			{
				return View(new List<Order>());
			}

			var student = await _db.Students
				.FirstOrDefaultAsync(
					x => x.RollNo == rollNo);

			if (student == null)
			{
				ViewBag.Error =
					"Student roll number not found.";

				return View(new List<Order>());
			}

			var orders = await _db.Orders
				.Where(x => x.StudentId == student.Id)
				.Include(x => x.Items)
					.ThenInclude(x => x.MenuItem)
				.OrderByDescending(x => x.OrderedOn)
				.ToListAsync();

			ViewBag.RollNo = rollNo;

			return View(orders);
		}
	}
}