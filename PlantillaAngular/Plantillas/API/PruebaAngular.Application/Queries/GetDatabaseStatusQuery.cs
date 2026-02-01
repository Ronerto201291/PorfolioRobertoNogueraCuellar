using MediatR;

namespace PruebaAngular.Application.Queries
{
    public class GetDatabaseStatusQuery : IRequest<DatabaseStatusResult>
    {
    }

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
