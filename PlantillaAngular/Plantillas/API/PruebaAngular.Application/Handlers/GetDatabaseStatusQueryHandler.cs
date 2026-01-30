using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PruebaAngular.Application.Queries;
using PruebaAngular.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PruebaAngular.Application.Handlers
{
    /// <summary>
    /// Handler para el query GetDatabaseStatusQuery.
    /// Verifica el estado de conexión a la base de datos.
    /// Flujo: API ? Query ? Handler ? Infrastructure ? PostgreSQL
    /// </summary>
    public class GetDatabaseStatusQueryHandler : IRequestHandler<GetDatabaseStatusQuery, DatabaseStatusResult>
    {
        private readonly PruebaAngularContext _context;
        private readonly ILogger<GetDatabaseStatusQueryHandler> _logger;

        public GetDatabaseStatusQueryHandler(
            PruebaAngularContext context,
            ILogger<GetDatabaseStatusQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DatabaseStatusResult> Handle(GetDatabaseStatusQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
                
                if (!canConnect)
                {
                    _logger.LogWarning("No se pudo conectar a la base de datos");
                    return DatabaseStatusResult.Fail("No se pudo establecer conexión con la base de datos");
                }

                var connectionString = _context.Database.GetConnectionString();
                var connectionInfo = connectionString?.Length > 50 
                    ? connectionString.Substring(0, 50) + "..." 
                    : connectionString ?? "No disponible";

                _logger.LogInformation("Conexión a base de datos verificada correctamente");
                return DatabaseStatusResult.Ok(connectionInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar estado de la base de datos: {Message}", ex.Message);
                return DatabaseStatusResult.Fail(ex.Message);
            }
        }
    }
}
