using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;

namespace WilliamDenton.AwsRdsPostgresDemo
{
	class MyHostedService : IHostedService, IDisposable
	{
		readonly CancellationTokenSource _shutdownTokenSource;
		Task _longRunningTask;
		readonly IServiceScopeFactory _scopeFactory;

		public MyHostedService(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
			_shutdownTokenSource = new CancellationTokenSource();
		}

		public void Dispose()
		{
			_shutdownTokenSource.Dispose();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_longRunningTask = Task.Run(() => RunUntilShutdown(_shutdownTokenSource.Token));
			return Task.CompletedTask;
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			_shutdownTokenSource.Cancel();
			if (_longRunningTask != null) {
				await _longRunningTask;
			}
		}

		public async Task RunUntilShutdown(CancellationToken shutdownToken)
		{
			try {
				while (true) {
					shutdownToken.ThrowIfCancellationRequested();

					using var scope = _scopeFactory.CreateScope();
					var clock = scope.ServiceProvider.GetRequiredService<IClock>();
					try {
						await DoWork(clock);
					} catch (Exception e) {
						Console.WriteLine(e);
					} finally {
						await Task.Delay(TimeSpan.FromSeconds(10), shutdownToken);
					}
				}

			} catch (TaskCanceledException) {
				//swallow
			}
		}

		Task DoWork(IClock clock)
		{
			Console.WriteLine($"Hello world {clock.GetCurrentInstant().ToString()}");
			return Task.CompletedTask;
		}
	}
}
