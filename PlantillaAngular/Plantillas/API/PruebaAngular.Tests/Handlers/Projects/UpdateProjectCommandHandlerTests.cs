using FluentAssertions;
using PruebaAngular.Application.Commands;
using PruebaAngular.Application.Handlers;
using PruebaAngular.Domain.AggregateModels.Portfolio;
using PruebaAngular.Tests.Fixtures;
using Xunit;

namespace PruebaAngular.Tests.Handlers.Projects;

/// <summary>
/// Tests para UpdateProjectCommandHandler.
/// Verifica la actualización de proyectos existentes.
/// </summary>
public class UpdateProjectCommandHandlerTests
{
    [Fact]
    public async Task Handle_ProyectoExistente_DebeActualizar()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var logger = TestDatabaseFixture.CreateLoggerMock<UpdateProjectCommandHandler>();
        var handler = new UpdateProjectCommandHandler(context, logger.Object);

        // Crear proyecto existente
        var proyecto = Project.Create("Nombre Original", "Descripción Original");
        context.Projects.Add(proyecto);
        await context.SaveChangesAsync();

        var command = new UpdateProjectCommand
        {
            ProjectId = proyecto.ProjectId,
            Name = "Nombre Actualizado",
            Description = "Nueva Descripción"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Name.Should().Be("Nombre Actualizado");
        result.Description.Should().Be("Nueva Descripción");
    }

    [Fact]
    public async Task Handle_ProyectoNoExiste_DebeRetornarNotFound()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var logger = TestDatabaseFixture.CreateLoggerMock<UpdateProjectCommandHandler>();
        var handler = new UpdateProjectCommandHandler(context, logger.Object);

        var command = new UpdateProjectCommand
        {
            ProjectId = Guid.NewGuid(), // ID que no existe
            Name = "Nombre",
            Description = "Descripción"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("No se encontró");
    }

    [Fact]
    public async Task Handle_SinNombre_DebeRetornarError()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var logger = TestDatabaseFixture.CreateLoggerMock<UpdateProjectCommandHandler>();
        var handler = new UpdateProjectCommandHandler(context, logger.Object);

        var proyecto = Project.Create("Original", "Desc");
        context.Projects.Add(proyecto);
        await context.SaveChangesAsync();

        var command = new UpdateProjectCommand
        {
            ProjectId = proyecto.ProjectId,
            Name = "", // Nombre vacío
            Description = "Descripción"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }
}
