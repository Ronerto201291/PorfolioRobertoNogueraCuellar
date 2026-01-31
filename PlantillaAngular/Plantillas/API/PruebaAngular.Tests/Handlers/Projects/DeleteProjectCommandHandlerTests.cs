using FluentAssertions;
using PruebaAngular.Application.Commands;
using PruebaAngular.Application.Handlers;
using PruebaAngular.Domain.AggregateModels.Portfolio;
using PruebaAngular.Tests.Fixtures;
using Xunit;

namespace PruebaAngular.Tests.Handlers.Projects;

/// <summary>
/// Tests para DeleteProjectCommandHandler.
/// Verifica la eliminación de proyectos.
/// </summary>
public class DeleteProjectCommandHandlerTests
{
    [Fact]
    public async Task Handle_ProyectoExistente_DebeEliminar()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var logger = TestDatabaseFixture.CreateLoggerMock<DeleteProjectCommandHandler>();
        var handler = new DeleteProjectCommandHandler(context, logger.Object);

        var proyecto = Project.Create("Proyecto a eliminar", "Descripción");
        context.Projects.Add(proyecto);
        await context.SaveChangesAsync();

        var command = new DeleteProjectCommand(proyecto.ProjectId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("eliminado");

        // Verificar que ya no existe
        var proyectoEliminado = await context.Projects.FindAsync(proyecto.ProjectId);
        proyectoEliminado.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ProyectoNoExiste_DebeRetornarNotFound()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var logger = TestDatabaseFixture.CreateLoggerMock<DeleteProjectCommandHandler>();
        var handler = new DeleteProjectCommandHandler(context, logger.Object);

        var command = new DeleteProjectCommand(Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("No se encontró");
    }
}
