using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WilliamDenton.AwsRdsPostgresDemo.Models
{
	interface IDemoReadOnlyDbContext
	{
	}

	public class DemoReadOnlyDbContext : DemoDbContext, IDemoReadOnlyDbContext
	{

		public DemoReadOnlyDbContext(DbContextOptions<DemoReadOnlyDbContext> options)
			: base(options)
		{ }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
		}

		public override int SaveChanges(bool _ = false) => throw ThrowReadOnlyException();
		public override Task<int> SaveChangesAsync(CancellationToken _ = default) => throw ThrowReadOnlyException();
		public override Task<int> SaveChangesAsync(bool _, CancellationToken __ = default) => throw ThrowReadOnlyException();
		Exception ThrowReadOnlyException() => new InvalidOperationException("Readonly DbContext");
	}

}
