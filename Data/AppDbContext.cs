using Microsoft.EntityFrameworkCore;
using SmartCanteen.Models;

namespace SmartCanteen.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}

		public DbSet<Student> Students { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<MenuItem> MenuItems { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
	}
}