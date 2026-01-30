using MediatR;

namespace PruebaAngular.Application.Queries
{
    /// <summary>
    /// Query para obtener el estado de conexión de la base de datos.
    /// Sigue el patrón CQRS - operación de lectura.
    /// </summary>
    public class GetDatabaseStatusQuery : IRequest<DatabaseStatusResult>
    {
    }

    /// <summary>
    /// Resultado del estado de la base de datos
    /// </summary>
    public class DatabaseStatusResult
    {
        public bool Connected { get; set; }
        public string? ConnectionInfo { get; set; }
        public string? ErrorMessage { get; set; }

        public static DatabaseStatusResult Ok(string connectionInfo)
        {
            return new DatabaseStatusResult
            {
                Connected = true,
                ConnectionInfo = connectionInfo
            };
        }

        public static DatabaseStatusResult Fail(string error)
        {
            return new DatabaseStatusResult
            {
                Connected = false,
                ErrorMessage = error
            };
        }
    }
}
