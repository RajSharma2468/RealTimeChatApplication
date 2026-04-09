namespace ConnectHub.Notification.Services
{
    public interface IRabbitMqService
    {
        void PublishMessage(string message, string routingKey);
        void StartConsuming(Func<string, Task> onMessageReceived);
    }
}