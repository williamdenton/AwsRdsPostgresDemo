using System;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using WilliamDenton.AwsRdsPostgresDemo.Models;

namespace WilliamDenton.AwsRdsPostgresDemo
{
	class EfCoreDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DemoMigratorDbContext>
	{
		public DemoMigratorDbContext CreateDbContext(string[] args)
		{
			Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "development");
			var host = Program.BuildHost();
			return host.Services.GetService<DemoMigratorDbContext>();
		}
	}
}
