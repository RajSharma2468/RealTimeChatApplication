using ConnectHub.Notification.Services;

namespace ConnectHub.Notification.BackgroundServices
{
    // Background service to consume RabbitMQ messages
    public class RabbitMqConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMqConsumerService> _logger;
        
        public RabbitMqConsumerService(IServiceProvider serviceProvider, ILogger<RabbitMqConsumerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RabbitMQ Consumer Service started");
            
            await Task.Run(() =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var rabbitMqService = scope.ServiceProvider.GetRequiredService<IRabbitMqService>();
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    
                    rabbitMqService.StartConsuming(async (message) =>
                    {
                        await notificationService.ProcessQueueMessageAsync(message);
                    });
                }
            }, stoppingToken);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}