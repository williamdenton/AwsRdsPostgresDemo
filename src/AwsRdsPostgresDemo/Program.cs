using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using Npgsql;
using WilliamDenton.AwsRdsPostgresDemo.Models;

namespace WilliamDenton.AwsRdsPostgresDemo
{
	class Program
	{
		static async Task Main()
		{
			var host = BuildHost();

			MigrateDbOnStartup(host);

			await host.RunAsync();
		}

		public static IHost BuildHost()
			=> new HostBuilder()
				.UseEnvironment(Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production")
				.ConfigureAppConfiguration(ConfigureAppConfiguration)
				.ConfigureServices(ConfigureServices)
				.UseConsoleLifetime()
				.UseHostedService<MyHostedService>()
				.Build();

		static void ConfigureAppConfiguration(HostBuilderContext hostContext, IConfigurationBuilder configApp)
		{
			configApp.SetBasePath(Directory.GetCurrentDirectory());
			configApp.AddJsonFile("appsettings.json", optional: false);
			configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: false);
			configApp.AddEnvironmentVariables();
		}

		static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
		{
			ConfigureDbContext(hostContext, services);
			services.AddSingleton<IClock>(p => SystemClock.Instance);
			services.AddSingleton(p => p.GetService<IConfiguration>().GetSection(nameof(AppOptions)).Get<AppOptions>());
		}

		static void ConfigureDbContext(HostBuilderContext hostContext, IServiceCollection services)
		{
			var dbMigrator = hostContext.Configuration.GetConnectionString("DemoDbContextMigrator") ?? throw new ArgumentNullException("DemoDbContextMigrator");
			var dbWrite = hostContext.Configuration.GetConnectionString("DemoDbContextReadWrite") ?? throw new ArgumentNullException("DemoDbContextReadWrite");
			var dbRead = hostContext.Configuration.GetConnectionString("DemoDbContextReadOnly") ?? throw new ArgumentNullException("DemoDbContextReadOnly");

			services.AddEntityFrameworkNpgsql()
				.AddDbContext<DemoMigratorDbContext>(ef => ConfigureDbContextOptions(ef, dbMigrator))
				.AddDbContext<DemoReadWriteDbContext>(ef => ConfigureDbContextOptions(ef, dbWrite))
				.AddDbContext<DemoReadOnlyDbContext>(ef => ConfigureDbContextOptions(ef, dbRead));

			// local setup function for our db contexts above
			static void ConfigureDbContextOptions(DbContextOptionsBuilder efOptions, string connectionString)
			{
				efOptions.UseNpgsql(connectionString, npgsqlOptions => {
					npgsqlOptions.UseAwsIamAuthentication();
					npgsqlOptions.UseNodaTime();
				});
				efOptions.UseSnakeCaseNamingConventions();
			}
			services.AddScoped<IDemoReadWriteDbContext>(provider => provider.GetService<DemoReadWriteDbContext>());
			services.AddScoped<IDemoReadOnlyDbContext>(provider => provider.GetService<DemoReadOnlyDbContext>());
		}

		static void MigrateDbOnStartup(IHost host)
		{
			var options = host.Services.GetService<AppOptions>();
			if (options.MigrateDatabaseOnStartUp) {
				using var dbContext = host.Services.GetService<DemoMigratorDbContext>();
				var database = dbContext.Database;
				if (database.GetPendingMigrations().Any()) {
					database.Migrate();
				}
			}
		}
	}
}
