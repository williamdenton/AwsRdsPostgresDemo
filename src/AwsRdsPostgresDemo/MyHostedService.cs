using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WilliamDenton.AwsRdsPostgresDemo.Models;
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
					var dbContext = scope.ServiceProvider.GetRequiredService<IDemoReadWriteDbContext>();
					var roDbContext = scope.ServiceProvider.GetRequiredService<IDemoReadOnlyDbContext>();
					var clock = scope.ServiceProvider.GetRequiredService<IClock>();
					try {
						await DoWork(dbContext, roDbContext, clock);
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

		async Task DoWork(IDemoReadWriteDbContext rwDbContext, IDemoReadOnlyDbContext roDbContext, IClock clock)
		{
			var c = new Customer {
				Name = Guid.NewGuid().ToString(),
				CustomerCode = Guid.NewGuid().ToString().Substring(0, 10),
				CreatedOn = clock.GetCurrentInstant()
			};
			rwDbContext.Customers.Add(c);

			await rwDbContext.SaveChangesAsync();
			Console.WriteLine($"Inserted Customer Id { c.Id}");

			var customerCount = roDbContext.Customers.Count();
			Console.WriteLine($"there are {customerCount} customers");
		}
	}
}
