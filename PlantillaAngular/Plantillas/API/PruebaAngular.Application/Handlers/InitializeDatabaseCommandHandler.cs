using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PruebaAngular.Application.Commands;
using PruebaAngular.Infrastructure.Data;
using PruebaAngular.Infrastructure.Data.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PruebaAngular.Application.Handlers
{
    /// <summary>
    /// Handler para el comando InitializeDatabaseCommand.
    /// Crea las tablas y siembra datos iniciales en PostgreSQL.
    /// Flujo: API ? Command ? Handler ? Infrastructure ? PostgreSQL
    /// </summary>
    public class InitializeDatabaseCommandHandler : IRequestHandler<InitializeDatabaseCommand, InitializeDatabaseResult>
    {
        private readonly PruebaAngularContext _context;
        private readonly ILogger<InitializeDatabaseCommandHandler> _logger;

        public InitializeDatabaseCommandHandler(
            PruebaAngularContext context,
            ILogger<InitializeDatabaseCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<InitializeDatabaseResult> Handle(InitializeDatabaseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Iniciando inicialización de base de datos...");

                // Crear tablas
                await _context.Database.ExecuteSqlRawAsync(DatabaseSetupQueries.CreateTables, cancellationToken);
                _logger.LogInformation("Tablas creadas correctamente.");

                // Verificar si ya existen datos
                var hasData = await _context.Database.ExecuteSqlRawAsync(
                    DatabaseSetupQueries.CheckProjectsExist, cancellationToken) > 0;

                if (!hasData)
                {
                    var projectId = Guid.NewGuid().ToString();
                    await _context.Database.ExecuteSqlRawAsync(
                        DatabaseSetupQueries.GetSeedDataQuery(projectId), cancellationToken);
                    
                    _logger.LogInformation("Datos semilla insertados correctamente.");
                    return InitializeDatabaseResult.Ok(tablesCreated: true, dataSeeded: true);
                }

                _logger.LogInformation("Los datos ya existen. Omitiendo siembra.");
                return InitializeDatabaseResult.Ok(tablesCreated: true, dataSeeded: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inicializar la base de datos: {Message}", ex.Message);
                return InitializeDatabaseResult.Fail(ex.Message);
            }
        }
    }
}
