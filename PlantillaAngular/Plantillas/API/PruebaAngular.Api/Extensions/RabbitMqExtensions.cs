using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PruebaAngular.Domain.Events;
using PruebaAngular.Infrastructure.Messaging;
using RabbitMQ.Client;

namespace PruebaAngular.Api.Extensions
{
    public static class RabbitMqExtensions
    {
        public static IServiceCollection AddRabbitMqEventBus(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("rabbitmq");

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "amqp://guest:guest@localhost:5672";
            }

            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString),
                ClientProvidedName = "PortfolioApi",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            services.AddSingleton<IConnection>(sp =>
            {
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });

            services.AddSingleton<IEventBus, RabbitMqEventBus>();

            services.AddSingleton<RabbitMqEventConsumer>();
            services.AddHostedService(sp => sp.GetRequiredService<RabbitMqEventConsumer>());

            return services;
        }

        public static IHealthChecksBuilder AddRabbitMqHealthChecks(
            this IHealthChecksBuilder builder)
        {
            builder.AddCheck<RabbitMqHealthCheck>(
                "rabbitmq-connection",
                tags: new[] { "rabbitmq", "messaging", "ready" });

            builder.AddCheck<RabbitMqConsumerHealthCheck>(
                "rabbitmq-consumer",
                tags: new[] { "rabbitmq", "messaging", "ready" });

            return builder;
        }
    }
}
