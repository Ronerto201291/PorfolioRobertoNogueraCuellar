using FluentAssertions;
using PruebaAngular.Application.Commands;
using PruebaAngular.Application.Handlers;
using PruebaAngular.Domain.AggregateModels.Portfolio;
using PruebaAngular.Tests.Fixtures;
using Xunit;

namespace PruebaAngular.Tests.Handlers.Tasks;

/// <summary>
/// Tests para DeleteTaskCommandHandler.
/// Verifica la eliminación de tareas.
/// </summary>
public class DeleteTaskCommandHandlerTests
{
    [Fact]
    public async Task Handle_TareaExistente_DebeEliminar()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        
        var proyecto = Project.Create("Proyecto", "Desc");
        context.Projects.Add(proyecto);
        
        var tarea = PortfolioTask.Create(proyecto.ProjectId, "Tarea a eliminar", "Pending", "Low");
        context.Tasks.Add(tarea);
        await context.SaveChangesAsync();

        var logger = TestDatabaseFixture.CreateLoggerMock<DeleteTaskCommandHandler>();
        var handler = new DeleteTaskCommandHandler(context, logger.Object);

        var command = new DeleteTaskCommand(tarea.TaskId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("eliminada");

        var tareaEliminada = await context.Tasks.FindAsync(tarea.TaskId);
        tareaEliminada.Should().BeNull();
    }

    [Fact]
    public async Task Handle_TareaNoExiste_DebeRetornarNotFound()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var logger = TestDatabaseFixture.CreateLoggerMock<DeleteTaskCommandHandler>();
        var handler = new DeleteTaskCommandHandler(context, logger.Object);

        var command = new DeleteTaskCommand(Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("No se encontró");
    }
}
