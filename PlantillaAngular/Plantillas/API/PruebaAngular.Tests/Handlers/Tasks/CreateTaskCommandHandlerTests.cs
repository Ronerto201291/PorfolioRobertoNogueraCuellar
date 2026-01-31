using FluentAssertions;
using PruebaAngular.Application.Commands;
using PruebaAngular.Application.Handlers;
using PruebaAngular.Domain.AggregateModels.Portfolio;
using PruebaAngular.Tests.Fixtures;
using Xunit;

namespace PruebaAngular.Tests.Handlers.Tasks;

/// <summary>
/// Tests para CreateTaskCommandHandler.
/// Verifica la creación de tareas con validaciones.
/// </summary>
public class CreateTaskCommandHandlerTests
{
    [Fact]
    public async Task Handle_ConDatosValidos_DebeCrearTarea()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var proyecto = Project.Create("Proyecto Test", "Para pruebas");
        context.Projects.Add(proyecto);
        await context.SaveChangesAsync();

        var logger = TestDatabaseFixture.CreateLoggerMock<CreateTaskCommandHandler>();
        var handler = new CreateTaskCommandHandler(context, logger.Object);

        var command = new CreateTaskCommand
        {
            ProjectId = proyecto.ProjectId,
            Title = "Nueva Tarea",
            Description = "Descripción de la tarea",
            Status = "Pending",
            Priority = "High"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Title.Should().Be("Nueva Tarea");
        result.Status.Should().Be("Pending");
        result.Priority.Should().Be("High");
    }

    [Fact]
    public async Task Handle_ProyectoNoExiste_DebeRetornarError()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var logger = TestDatabaseFixture.CreateLoggerMock<CreateTaskCommandHandler>();
        var handler = new CreateTaskCommandHandler(context, logger.Object);

        var command = new CreateTaskCommand
        {
            ProjectId = Guid.NewGuid(), // No existe
            Title = "Tarea huérfana",
            Status = "Pending",
            Priority = "Medium"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("No se encontró el proyecto");
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("InProgress")]
    [InlineData("Completed")]
    public async Task Handle_EstadosValidos_DebeCrearTarea(string status)
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var proyecto = Project.Create("Proyecto", "Desc");
        context.Projects.Add(proyecto);
        await context.SaveChangesAsync();

        var logger = TestDatabaseFixture.CreateLoggerMock<CreateTaskCommandHandler>();
        var handler = new CreateTaskCommandHandler(context, logger.Object);

        var command = new CreateTaskCommand
        {
            ProjectId = proyecto.ProjectId,
            Title = $"Tarea {status}",
            Status = status,
            Priority = "Medium"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Status.Should().Be(status);
    }

    [Fact]
    public async Task Handle_EstadoInvalido_DebeRetornarError()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var proyecto = Project.Create("Proyecto", "Desc");
        context.Projects.Add(proyecto);
        await context.SaveChangesAsync();

        var logger = TestDatabaseFixture.CreateLoggerMock<CreateTaskCommandHandler>();
        var handler = new CreateTaskCommandHandler(context, logger.Object);

        var command = new CreateTaskCommand
        {
            ProjectId = proyecto.ProjectId,
            Title = "Tarea",
            Status = "EstadoInventado",
            Priority = "Medium"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Estado inválido");
    }

    [Theory]
    [InlineData("Low")]
    [InlineData("Medium")]
    [InlineData("High")]
    public async Task Handle_PrioridadesValidas_DebeCrearTarea(string priority)
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var proyecto = Project.Create("Proyecto", "Desc");
        context.Projects.Add(proyecto);
        await context.SaveChangesAsync();

        var logger = TestDatabaseFixture.CreateLoggerMock<CreateTaskCommandHandler>();
        var handler = new CreateTaskCommandHandler(context, logger.Object);

        var command = new CreateTaskCommand
        {
            ProjectId = proyecto.ProjectId,
            Title = $"Tarea {priority}",
            Status = "Pending",
            Priority = priority
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Priority.Should().Be(priority);
    }

    [Fact]
    public async Task Handle_PrioridadInvalida_DebeRetornarError()
    {
        // Arrange
        var context = TestDatabaseFixture.CreateContext();
        var proyecto = Project.Create("Proyecto", "Desc");
        context.Projects.Add(proyecto);
        await context.SaveChangesAsync();

        var logger = TestDatabaseFixture.CreateLoggerMock<CreateTaskCommandHandler>();
        var handler = new CreateTaskCommandHandler(context, logger.Object);

        var command = new CreateTaskCommand
        {
            ProjectId = proyecto.ProjectId,
            Title = "Tarea",
            Status = "Pending",
            Priority = "SuperAlta"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Prioridad inválida");
    }
}
