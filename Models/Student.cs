using System.ComponentModel.DataAnnotations;

namespace SmartCanteen.Models
{
	public class Student
	{
		public int Id { get; set; }

		[Required]
		[StringLength(20)]
		public string RollNo { get; set; } = "";

		[Required]
		[StringLength(100)]
		public string Name { get; set; } = "";

		[StringLength(150)]
		public string? Email { get; set; }

		[StringLength(15)]
		public string? Phone { get; set; }
	}
}