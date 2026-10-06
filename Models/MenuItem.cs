using System.ComponentModel.DataAnnotations;

namespace SmartCanteen.Models
{
	public class MenuItem
	{
		public int Id { get; set; }

		[Required, StringLength(80)]
		public string Name { get; set; } = "";

		[Range(1, 1000)]
		public decimal Price { get; set; }

		[Range(1, 30)]
		public int PrepMinutes { get; set; }

		[Range(0, 500)]
		public int StockQty { get; set; }

		public bool IsAvailable { get; set; } = true;

		public int CategoryId { get; set; }

		public Category? Category { get; set; }

		[Timestamp]
		public byte[]? RowVersion { get; set; }
	}
}