using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ConnectHub.Presence.Services;

namespace ConnectHub.Presence.BackgroundServices
{
    public class ConnectionCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ConnectionCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(5);

        public ConnectionCleanupService(IServiceProvider serviceProvider, ILogger<ConnectionCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_cleanupInterval, stoppingToken);
                    
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var presenceService = scope.ServiceProvider.GetRequiredService<IPresenceService>();
                        await presenceService.CleanupStaleConnections();
                        _logger.LogInformation("Cleaned up stale connections");
                    }
                }
                catch (OperationCanceledException)
                {
                    // Normal shutdown - don't log as error
                    _logger.LogInformation("Connection cleanup service stopping");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in connection cleanup service");
                }
            }
        }
    }
}
