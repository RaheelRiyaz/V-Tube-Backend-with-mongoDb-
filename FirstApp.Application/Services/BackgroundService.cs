using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Application.Abstractions.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Services
{
    public class BackgroundService(IServiceProvider serviceProvider) : IHostedService, IDisposable
    {
        private Timer? _timer;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ProcessData, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Background sevice is stopping");
            await Task.CompletedTask;
        }

        private async void ProcessData(object? state)
        {
            using var scope = serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IVideousRepository>();
            var videos = await repository.GetAllAsync();
            Console.WriteLine(videos.FirstOrDefault()?.Title);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

    }
}
