using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PruebaAngular.Domain.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PruebaAngular.Infrastructure.Messaging
{
    // Background service that listens to NotificationSubscribedEvent and sends emails
    public class NotificationEmailConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly ILogger<NotificationEmailConsumer> _logger;
        private readonly IEmailSender _emailSender;
        private IModel? _channel;
        private const string ExchangeName = "portfolio-events";
        private const string QueueName = "notification-email-consumer";

        public NotificationEmailConsumer(IConnection connection, ILogger<NotificationEmailConsumer> logger, IEmailSender emailSender)
        {
            _connection = connection;
            _logger = logger;
            _emailSender = emailSender;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[NotificationEmailConsumer] Starting...");

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Topic, durable: true);
            _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: "notification.subscribed");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, ea) =>
            {
                await ProcessMessage(ea.Body.ToArray());
            };

            _channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);

            await Task.CompletedTask;
        }

        private async Task ProcessMessage(byte[] body)
        {
            try
            {
                var json = System.Text.Encoding.UTF8.GetString(body);
                var ev = JsonSerializer.Deserialize<NotificationSubscribedEvent>(json);
                if (ev != null)
                {
                    _logger.LogInformation("[NotificationEmailConsumer] Received event for {Email}", ev.Email);
                    // Build a simple email
                    var subject = "Gracias por suscribirte";
                    var bodyText = $"Hola, gracias por suscribirte a las notificaciones.\n\nEmail: {ev.Email}";
                    await _emailSender.SendEmailAsync(ev.Email, subject, bodyText);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error processing notification event: {Message}", ex.Message);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[NotificationEmailConsumer] Stopping...");
            _channel?.Close();
            await base.StopAsync(cancellationToken);
        }
    }
}
