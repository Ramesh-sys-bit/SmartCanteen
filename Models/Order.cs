using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SmartCanteen.Models
{
	[Index(nameof(PickupSlot))]
	public class Order
	{
		public int Id { get; set; }
		public int StudentId { get; set; }
		public Student? Student { get; set; }

		public DateTime OrderedOn { get; set; } = DateTime.Now;

		public DateTime EstimatedReadyOn { get; set; }
		public DateTime PickupSlot { get; set; }

		public int TokenNo { get; set; }
		public decimal TotalAmount { get; set; }

		public string Status { get; set; } = "Placed";
		public string PaymentStatus { get; set; } = "Pending";

		public List<OrderItem> Items { get; set; } = new();
	}

	public class OrderItem
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public int MenuItemId { get; set; }

		public MenuItem? MenuItem { get; set; }

		[Range(1, 10)]
		public int Quantity { get; set; }

		public decimal UnitPrice { get; set; }
	}
}