using FluentAssertions;
using PruebaAngular.Application.Commands;
using PruebaAngular.Application.Handlers;
using PruebaAngular.Tests.Fixtures;
using Xunit;

namespace PruebaAngular.Tests.Handlers.Projects;

/// <summary>
/// Tests para CreateProjectCommandHandler.
/// Verifica la creación de proyectos con diferentes escenarios.
/// </summary>
public class CreateProjectCommandHandlerTests
{
    #region Casos Exitosos

    [Fact]
    public async Task Handle_ConDatosValidos_DebeCrearProyecto()
    {
        // Arrange - Preparar
        var context = TestDatabaseFixture.CreateContext();
        var logger = TestDatabaseFixture.CreateLoggerMock<CreateProjectCommandHandler>();
        var handler = new CreateProjectCommandHandler(context, logger.Object);

        var command = new CreateProjectCommand
        {
            Name = "Mi Proyecto",
            Description = "Descripción del proyecto"
        };

        // Act - Ejecutar
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert - Verificar
        result.Success.Should().BeTrue();
        result.Name.Should().Be("Mi Proyecto");
        result.Description.Should().Be("Descripción del proyecto");
        result.ProjectId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_SinDescripcion_DebeCrearProyecto()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var logger = TestDatabaseFixture.CreateLoggerMock<CreateProjectCommandHandler>();
        var handler = new CreateProjectCommandHandler(context, logger.Object);

        var command = new CreateProjectCommand
        {
            Name = "Proyecto sin descripción",
            Description = null
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Description.Should().BeNull();
    }

    #endregion

    #region Casos de Error

    [Fact]
    public async Task Handle_SinNombre_DebeRetornarError()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var logger = TestDatabaseFixture.CreateLoggerMock<CreateProjectCommandHandler>();
        var handler = new CreateProjectCommandHandler(context, logger.Object);

        var command = new CreateProjectCommand
        {
            Name = "",
            Description = "Tiene descripción pero no nombre"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("obligatorio");
    }

    [Fact]
    public async Task Handle_NombreSoloEspacios_DebeRetornarError()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var logger = TestDatabaseFixture.CreateLoggerMock<CreateProjectCommandHandler>();
        var handler = new CreateProjectCommandHandler(context, logger.Object);

        var command = new CreateProjectCommand { Name = "   " };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_NombreMuyLargo_DebeRetornarError()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var logger = TestDatabaseFixture.CreateLoggerMock<CreateProjectCommandHandler>();
        var handler = new CreateProjectCommandHandler(context, logger.Object);

        var nombreLargo = new string('a', 201); // Más de 200 caracteres
        var command = new CreateProjectCommand { Name = nombreLargo };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("200 caracteres");
    }

    #endregion
}
