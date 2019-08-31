using Microsoft.EntityFrameworkCore;

namespace WilliamDenton.AwsRdsPostgresDemo.Models
{
	public class DemoMigratorDbContext : DemoDbContext
	{
		public DemoMigratorDbContext(DbContextOptions<DemoMigratorDbContext> options)
			: base(options)
		{ }
	}

}
