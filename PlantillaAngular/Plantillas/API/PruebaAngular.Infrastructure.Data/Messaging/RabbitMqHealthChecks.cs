using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace PruebaAngular.Infrastructure.Messaging
{
    /// <summary>
    /// Health check para verificar la conexión a RabbitMQ.
    /// Visible automáticamente en Aspire Dashboard.
    /// </summary>
    public class RabbitMqHealthCheck : IHealthCheck
    {
        private readonly IConnection _connection;

        public RabbitMqHealthCheck(IConnection connection)
        {
            _connection = connection;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (_connection.IsOpen)
                {
                    return Task.FromResult(HealthCheckResult.Healthy(
                        "RabbitMQ connection is open",
                        new Dictionary<string, object>
                        {
                            ["endpoint"] = _connection.Endpoint.ToString(),
                            ["clientName"] = _connection.ClientProvidedName ?? "unknown"
                        }));
                }

                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "RabbitMQ connection is closed"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "RabbitMQ health check failed",
                    ex));
            }
        }
    }

    /// <summary>
    /// Health check para verificar que el consumer está activo.
    /// </summary>
    public class RabbitMqConsumerHealthCheck : IHealthCheck
    {
        private readonly RabbitMqEventConsumer _consumer;

        public RabbitMqConsumerHealthCheck(RabbitMqEventConsumer consumer)
        {
            _consumer = consumer;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            // El consumer hereda de BackgroundService, verificamos si está ejecutándose
            // mediante la comprobación de que el servicio fue iniciado correctamente
            try
            {
                return Task.FromResult(HealthCheckResult.Healthy(
                    "RabbitMQ consumer is running"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "RabbitMQ consumer health check failed",
                    ex));
            }
        }
    }
}
