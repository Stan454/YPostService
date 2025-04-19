using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using YPostService.Logic;

public class RabbitMQConsumer : BackgroundService
{
    private readonly string _hostname = "localhost";
    private readonly string _queueName = "LikeUpdate";
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Step 1: Create a connection to RabbitMQ
        var factory = new ConnectionFactory
        {
            HostName = _hostname
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        // Step 2: Declare the queue
        channel.QueueDeclare(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        // Step 3: Set up the consumer
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            // Step 4: Process the received message
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received message: {message}");

            // Deserialize the message
            var likeEvent = JsonSerializer.Deserialize<LikeEvent>(message);

            // Process the event
            await HandleLikeEvent(likeEvent);
        };

        // Step 5: Start consuming messages
        channel.BasicConsume(
            queue: _queueName,
            autoAck: true,
            consumer: consumer
        );

        Console.WriteLine("Listening for messages on the LikeUpdate queue...");
        return Task.CompletedTask; // Keep the service running
    }

    private async Task HandleLikeEvent(LikeEvent likeEvent)
    {
        if (likeEvent == null) return;

        // Resolve IPostLogic within a scope
        using (var scope = _serviceProvider.CreateScope())
        {
            var postLogic = scope.ServiceProvider.GetRequiredService<IPostLogic>();

            if (likeEvent.IsLike)
            {
                await postLogic.IncrementLikeCountAsync(likeEvent.PostId);
            }
            else
            {
                await postLogic.DecrementLikeCountAsync(likeEvent.PostId);
            }
        }
    }
}
