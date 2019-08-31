using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WilliamDenton.AwsRdsPostgresDemo
{
	public static class HostBuilderExtensions
	{
		public static IHostBuilder UseHostedService<T>(this IHostBuilder hostBuilder) where T : class, IHostedService
			=> hostBuilder.ConfigureServices(services => services.AddHostedService<T>());
	}
}
