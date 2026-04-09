using ConnectHub.Notification.Services;

namespace ConnectHub.Notification.BackgroundServices
{
    // Background service that runs daily to clean up old notifications and send emails
    // WHY: Remove old notifications to keep database clean, send emails to offline users
    public class EmailCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailCleanupService> _logger;
        
        public EmailCleanupService(IServiceProvider serviceProvider, ILogger<EmailCleanupService> logger)
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
                    // Run daily at 3 AM
                    var now = DateTime.UtcNow;
                    var nextRun = now.Date.AddDays(1).AddHours(3);
                    var delay = nextRun - now;
                    
                    await Task.Delay(delay, stoppingToken);
                    
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        // TODO: Implement email sending for offline users
                        _logger.LogInformation("Email cleanup service running at {Time}", DateTime.UtcNow);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in email cleanup service");
                    await Task.Delay(3600000, stoppingToken); // Wait 1 hour before retry
                }
            }
        }
    }
}
