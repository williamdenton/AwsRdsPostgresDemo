using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WilliamDenton.AwsRdsPostgresDemo.Models
{

	interface IDemoReadWriteDbContext
	{

		int SaveChanges(bool acceptAllChangesOnSuccess);

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

		Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

	}

	public class DemoReadWriteDbContext : DemoDbContext, IDemoReadWriteDbContext
	{
		public DemoReadWriteDbContext(DbContextOptions<DemoReadWriteDbContext> options)
			: base(options)
		{ }
	}

}
