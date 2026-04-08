using ConnectHub.Media.Services;

namespace ConnectHub.Media.BackgroundServices
{
    // Background service that runs daily to clean up expired files
    public class ExpiredMediaCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExpiredMediaCleanupService> _logger;
        
        public ExpiredMediaCleanupService(IServiceProvider serviceProvider, ILogger<ExpiredMediaCleanupService> logger)
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
                    // Run daily at 2 AM
                    var now = DateTime.UtcNow;
                    var nextRun = now.Date.AddDays(1).AddHours(2);
                    var delay = nextRun - now;
                    
                    await Task.Delay(delay, stoppingToken);
                    
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediaService = scope.ServiceProvider.GetRequiredService<IMediaService>();
                        var result = await mediaService.CleanupExpiredFilesAsync();
                        
                        if (result)
                        {
                            _logger.LogInformation("Expired media files cleaned up successfully");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error cleaning up expired media files");
                    await Task.Delay(3600000, stoppingToken); // Wait 1 hour before retry
                }
            }
        }
    }
}