using FluentAssertions;
using PruebaAngular.Application.Commands;
using PruebaAngular.Application.Handlers;
using PruebaAngular.Domain.AggregateModels.Portfolio;
using PruebaAngular.Tests.Fixtures;
using Xunit;

namespace PruebaAngular.Tests.Handlers.Tasks;

/// <summary>
/// Tests para UpdateTaskCommandHandler.
/// Verifica la actualización de tareas existentes.
/// </summary>
public class UpdateTaskCommandHandlerTests
{
    [Fact]
    public async Task Handle_TareaExistente_DebeActualizar()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        
        var proyecto = Project.Create("Proyecto", "Desc");
        context.Projects.Add(proyecto);
        
        var tarea = PortfolioTask.Create(proyecto.ProjectId, "Tarea Original", "Pending", "Low");
        context.Tasks.Add(tarea);
        await context.SaveChangesAsync();

        var logger = TestDatabaseFixture.CreateLoggerMock<UpdateTaskCommandHandler>();
        var handler = new UpdateTaskCommandHandler(context, logger.Object);

        var command = new UpdateTaskCommand
        {
            TaskId = tarea.TaskId,
            Title = "Tarea Actualizada",
            Description = "Nueva descripción",
            Status = "InProgress",
            Priority = "High"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Title.Should().Be("Tarea Actualizada");
        result.Status.Should().Be("InProgress");
        result.Priority.Should().Be("High");
    }

    [Fact]
    public async Task Handle_TareaNoExiste_DebeRetornarNotFound()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var logger = TestDatabaseFixture.CreateLoggerMock<UpdateTaskCommandHandler>();
        var handler = new UpdateTaskCommandHandler(context, logger.Object);

        var command = new UpdateTaskCommand
        {
            TaskId = Guid.NewGuid(),
            Title = "Tarea",
            Status = "Pending",
            Priority = "Medium"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("No se encontró");
    }

    [Fact]
    public async Task Handle_CambiarStatusACompletado_DebeActualizar()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        
        var proyecto = Project.Create("Proyecto", "Desc");
        context.Projects.Add(proyecto);
        
        var tarea = PortfolioTask.Create(proyecto.ProjectId, "Tarea", "Pending", "Medium");
        context.Tasks.Add(tarea);
        await context.SaveChangesAsync();

        var logger = TestDatabaseFixture.CreateLoggerMock<UpdateTaskCommandHandler>();
        var handler = new UpdateTaskCommandHandler(context, logger.Object);

        var command = new UpdateTaskCommand
        {
            TaskId = tarea.TaskId,
            Title = "Tarea",
            Status = "Completed",
            Priority = "Medium"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Status.Should().Be("Completed");
    }
}
