
using Microsoft.EntityFrameworkCore;

namespace WilliamDenton.AwsRdsPostgresDemo.Models
{
	public abstract class DemoDbContext : DbContext
	{
		public DbSet<Customer> Customers { get; set; }

		protected DemoDbContext(DbContextOptions options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Customer>()
				.HasIndex(payment => payment.CustomerCode)
				.IsUnique();

			modelBuilder.Entity<Customer>()
				.HasIndex(e => new { e.Name });

		}
	}
}
