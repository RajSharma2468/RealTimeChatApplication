using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConnectHub.Notification.Services
{
    public class RabbitMqService : IRabbitMqService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;
        private readonly string _exchangeName;
        private readonly ILogger<RabbitMqService> _logger;
        
        public RabbitMqService(IConfiguration configuration, ILogger<RabbitMqService> logger)
        {
            _logger = logger;
            
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            };
            
            _queueName = configuration["RabbitMQ:QueueName"] ?? "notifications";
            _exchangeName = configuration["RabbitMQ:ExchangeName"] ?? "notification_exchange";
            
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            // Declare exchange
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct, durable: true);
            
            // Declare queue
            _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
            
            // Bind queue to exchange
            _channel.QueueBind(_queueName, _exchangeName, routingKey: "notification");
            
            _logger.LogInformation("RabbitMQ connection established");
        }
        
        public void PublishMessage(string message, string routingKey)
        {
            var body = Encoding.UTF8.GetBytes(message);
            
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            
            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);
            
            _logger.LogDebug("Message published to RabbitMQ: {Message}", message);
        }
        
        public void StartConsuming(Func<string, Task> onMessageReceived)
        {
            var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                try
                {
                    await onMessageReceived(message);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                    catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from queue");
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: true);
                }
            };
            
            _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
            _logger.LogInformation("Started consuming from RabbitMQ queue: {QueueName}", _queueName);
        }
        
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}