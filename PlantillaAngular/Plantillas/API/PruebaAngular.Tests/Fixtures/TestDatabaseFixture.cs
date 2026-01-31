using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PruebaAngular.Infrastructure.Data;

namespace PruebaAngular.Tests.Fixtures;

/// <summary>
/// Fixture base para crear contextos de base de datos en memoria.
/// Proporciona utilidades comunes para todos los tests.
/// </summary>
public static class TestDatabaseFixture
{
    /// <summary>
    /// Crea un nuevo contexto de base de datos en memoria.
    /// Cada test obtiene una base de datos aislada.
    /// </summary>
    public static PruebaAngularContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PruebaAngularContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new PruebaAngularContext(options);
    }

    /// <summary>
    /// Crea un mock de ILogger para cualquier tipo.
    /// </summary>
    public static Mock<ILogger<T>> CreateLoggerMock<T>()
    {
        return new Mock<ILogger<T>>();
    }
}
