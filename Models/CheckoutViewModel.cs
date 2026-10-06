using System.ComponentModel.DataAnnotations;

namespace SmartCanteen.Models
{
	public class CheckoutViewModel
	{
		[Required, StringLength(20)]
		public string RollNo { get; set; } = "";

		[MinLength(1, ErrorMessage = "Add at least one item.")]
		public List<CartLine> Lines { get; set; } = new();

		public DateTime? PreferredPickup { get; set; }
	}

	public class CartLine
	{
		public int MenuItemId { get; set; }

		[Range(0, 10)]
		public int Quantity { get; set; }
	}
}