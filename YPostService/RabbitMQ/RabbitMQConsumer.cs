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
    private readonly string _hostname = "rabbitmq";
    private readonly string _queueName = "LikeUpdate";
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _hostname
        };

        IConnection connection = null;
        IModel channel = null;

        while (connection == null && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Attempt to create a connection
                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                // Declare the queue
                channel.QueueDeclare(
                    queue: _queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                Console.WriteLine("Connected to RabbitMQ and queue declared.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to RabbitMQ: {ex.Message}. Retrying in 5 seconds...");
                await Task.Delay(5000, stoppingToken); // Wait 5 seconds before retrying
            }
        }

        if (channel == null)
        {
            Console.WriteLine("Failed to connect to RabbitMQ. Exiting...");
            return;
        }

        // Set up the consumer
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received message: {message}");

            var likeEvent = JsonSerializer.Deserialize<LikeEvent>(message);
            await HandleLikeEvent(likeEvent);
        };

        // Start consuming messages
        channel.BasicConsume(
            queue: _queueName,
            autoAck: true,
            consumer: consumer
        );

        Console.WriteLine("Listening for messages on the LikeUpdate queue...");
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
