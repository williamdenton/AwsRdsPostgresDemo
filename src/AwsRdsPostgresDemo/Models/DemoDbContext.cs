
using Microsoft.EntityFrameworkCore;

namespace WilliamDenton.AwsRdsPostgresDemo.Models
{
	public abstract class DemoDbContext : DbContext
	{

		protected DemoDbContext(DbContextOptions options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.UseSnakeCaseNames();

			modelBuilder.Entity<Customer>()
				.HasIndex(payment => payment.CustomerCode)
				.IsUnique();

			modelBuilder.Entity<Customer>()
				.HasIndex(e => new { e.Name });

		}
	}
}
