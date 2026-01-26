using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PruebaAngular.Domain.AggregateModels.Portfolio;

namespace PruebaAngular.Infrastructure.Data.Seeders
{
    /// <summary>
    /// Siembra datos iniciales del portfolio (proyectos y tareas)
    /// </summary>
    public static class PortfolioSeeder
    {
        public static async Task SeedAsync(PruebaAngularContext context, ILogger logger)
        {
            // Verificar si ya existen datos
            if (await context.Projects.AnyAsync())
            {
                logger.LogInformation("Los datos del portfolio ya existen. Omitiendo...");
                return;
            }

            logger.LogInformation("Sembrando datos del portfolio...");

            // Crear proyecto de ejemplo
            var project = Project.Create(
                name: "Aplicación Portfolio Full-Stack",
                description: "Aplicación de portfolio completa que demuestra Clean Architecture, CQRS, GraphQL y desarrollo frontend moderno con Angular."
            );

            context.Projects.Add(project);
            await context.SaveChangesAsync();

            // Crear tareas de ejemplo con diferentes estados
            var tasks = new List<PortfolioTask>
            {
                PortfolioTask.Create(
                    projectId: project.ProjectId,
                    title: "Configurar estructura Clean Architecture",
                    status: "Completed",
                    priority: "High",
                    description: "Implementar las capas Domain, Application, Infrastructure y API siguiendo los principios de Clean Architecture."
                ),
                PortfolioTask.Create(
                    projectId: project.ProjectId,
                    title: "Implementar API GraphQL",
                    status: "InProgress",
                    priority: "High",
                    description: "Crear queries y mutations de GraphQL usando HotChocolate para acceso flexible a datos."
                ),
                PortfolioTask.Create(
                    projectId: project.ProjectId,
                    title: "Construir frontend Angular",
                    status: "Pending",
                    priority: "Medium",
                    description: "Desarrollar frontend responsive en Angular para consumir la API GraphQL y mostrar el contenido del portfolio.",
                    dueDate: DateTimeOffset.UtcNow.AddDays(7)
                )
            };

            context.Tasks.AddRange(tasks);
            await context.SaveChangesAsync();

            logger.LogInformation("Datos del portfolio sembrados correctamente. Creado(s) {ProjectCount} proyecto(s) y {TaskCount} tarea(s).", 1, tasks.Count);
        }
    }
}
