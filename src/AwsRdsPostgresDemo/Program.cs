using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using Npgsql;

namespace WilliamDenton.AwsRdsPostgresDemo
{
	class Program
	{
		static async Task Main()
		{
			var host = BuildHost();

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
			services.AddSingleton<IClock>(p => SystemClock.Instance);
			services.AddSingleton(p => p.GetService<IConfiguration>().GetSection(nameof(AppOptions)).Get<AppOptions>());
		}
		}
	}
}
