using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PruebaAngular.Domain.Events;
using PruebaAngular.Infrastructure.Messaging;
using RabbitMQ.Client;

namespace PruebaAngular.Api.Extensions
{
    /// <summary>
    /// Extensiones para configurar RabbitMQ con Aspire.
    /// </summary>
    public static class RabbitMqExtensions
    {
        /// <summary>
        /// Añade RabbitMQ al contenedor de servicios.
        /// Configura conexión, EventBus, Consumer y Health Checks.
        /// </summary>
        public static IServiceCollection AddRabbitMqEventBus(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Obtener connection string de Aspire (inyectado automáticamente)
            var connectionString = configuration.GetConnectionString("rabbitmq");

            if (string.IsNullOrEmpty(connectionString))
            {
                // Fallback para desarrollo local sin Aspire
                connectionString = "amqp://guest:guest@localhost:5672";
            }

            // Configurar ConnectionFactory
            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString),
                ClientProvidedName = "PortfolioApi",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            // Registrar conexión como singleton
            services.AddSingleton<IConnection>(sp =>
            {
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });

            // Registrar EventBus
            services.AddSingleton<IEventBus, RabbitMqEventBus>();

            // Registrar Consumer como HostedService
            services.AddSingleton<RabbitMqEventConsumer>();
            services.AddHostedService(sp => sp.GetRequiredService<RabbitMqEventConsumer>());

            return services;
        }

        /// <summary>
        /// Añade health checks para RabbitMQ.
        /// </summary>
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
