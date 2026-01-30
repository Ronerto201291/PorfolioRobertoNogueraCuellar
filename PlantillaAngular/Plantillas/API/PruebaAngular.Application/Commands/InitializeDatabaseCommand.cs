using MediatR;

namespace PruebaAngular.Application.Commands
{
    /// <summary>
    /// Command para inicializar la base de datos (crear tablas y datos semilla).
    /// Sigue el patrón CQRS - operación de escritura.
    /// </summary>
    public class InitializeDatabaseCommand : IRequest<InitializeDatabaseResult>
    {
    }

    /// <summary>
    /// Resultado de la inicialización de la base de datos
    /// </summary>
    public class InitializeDatabaseResult
    {
        public bool Success { get; set; }
        public bool TablesCreated { get; set; }
        public bool DataSeeded { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }

        public static InitializeDatabaseResult Ok(bool tablesCreated, bool dataSeeded)
        {
            return new InitializeDatabaseResult
            {
                Success = true,
                TablesCreated = tablesCreated,
                DataSeeded = dataSeeded,
                Message = "Base de datos inicializada correctamente"
            };
        }

        public static InitializeDatabaseResult Fail(string error)
        {
            return new InitializeDatabaseResult
            {
                Success = false,
                ErrorMessage = error
            };
        }
    }
}
