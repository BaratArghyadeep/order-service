using OrderService.Domain.Entities;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderService.Application.Service
{
    public class RabbitMqPublisher
    {
        public async Task PublishAsync(
            OrderCreatedEvent orderEvent)
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq"
            };

            await using var connection =
                await factory.CreateConnectionAsync();

            await using var channel =
                await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "order-created",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var message =
                JsonSerializer.Serialize(orderEvent);

            var body =
                Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "order-created",
                body: body);
        }
    }
}
