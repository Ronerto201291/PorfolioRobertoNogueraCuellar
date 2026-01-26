using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PruebaAngular.Domain.AggregateModels.Portfolio;
using PruebaAngular.Infrastructure.Data;

namespace PruebaAngular.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SetupController : ControllerBase
    {
        private readonly PruebaAngularContext _context;
        private readonly ILogger<SetupController> _logger;

        public SetupController(PruebaAngularContext context, ILogger<SetupController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Initialize database - creates tables and seeds data
        /// </summary>
        [HttpPost("init")]
        public async Task<IActionResult> InitializeDatabase()
        {
            try
            {
                _logger.LogInformation("Manual database initialization started...");

                // Create tables via SQL
                var createTablesSql = @"
                    CREATE TABLE IF NOT EXISTS public.projects (
                        id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                        name VARCHAR(200) NOT NULL,
                        description VARCHAR(1000),
                        created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
                    );

                    CREATE TABLE IF NOT EXISTS public.tasks (
                        id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                        project_id UUID NOT NULL REFERENCES public.projects(id) ON DELETE CASCADE,
                        title VARCHAR(200) NOT NULL,
                        description VARCHAR(1000),
                        status VARCHAR(20) NOT NULL DEFAULT 'Pending',
                        priority VARCHAR(20) NOT NULL DEFAULT 'Medium',
                        due_date TIMESTAMPTZ,
                        created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
                    );

                    CREATE INDEX IF NOT EXISTS ix_tasks_project_id ON public.tasks(project_id);
                    CREATE INDEX IF NOT EXISTS ix_tasks_status ON public.tasks(status);
                ";

                await _context.Database.ExecuteSqlRawAsync(createTablesSql);
                _logger.LogInformation("Tables created successfully.");

                // Check if data exists
                var hasData = await _context.Database.ExecuteSqlRawAsync("SELECT 1 FROM public.projects LIMIT 1") > 0;
                
                if (!hasData)
                {
                    // Seed data via SQL
                    var projectId = Guid.NewGuid();
                    var seedSql = $@"
                        INSERT INTO public.projects (id, name, description, created_at)
                        VALUES ('{projectId}', 'Portfolio Full-Stack Application', 
                                'A complete full-stack portfolio application demonstrating Clean Architecture, CQRS, GraphQL, and modern frontend development with Angular.',
                                NOW());

                        INSERT INTO public.tasks (id, project_id, title, description, status, priority, created_at)
                        VALUES 
                            (gen_random_uuid(), '{projectId}', 'Setup Clean Architecture structure', 
                             'Implement Domain, Application, Infrastructure, and API layers following Clean Architecture principles.',
                             'Completed', 'High', NOW()),
                            (gen_random_uuid(), '{projectId}', 'Implement GraphQL API', 
                             'Create GraphQL queries and mutations using HotChocolate for flexible data access.',
                             'InProgress', 'High', NOW()),
                            (gen_random_uuid(), '{projectId}', 'Build Angular frontend', 
                             'Develop responsive Angular frontend to consume GraphQL API and display portfolio content.',
                             'Pending', 'Medium', NOW());
                    ";

                    await _context.Database.ExecuteSqlRawAsync(seedSql);
                    _logger.LogInformation("Seed data inserted successfully.");
                }

                return Ok(new { message = "Database initialized successfully", tablesCreated = true, dataSeeded = !hasData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database initialization failed: {Message}", ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Check database status
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                var connectionString = _context.Database.GetConnectionString();
                
                return Ok(new { 
                    connected = canConnect,
                    connectionString = connectionString?.Substring(0, Math.Min(50, connectionString?.Length ?? 0)) + "..."
                });
            }
            catch (Exception ex)
            {
                return Ok(new { connected = false, error = ex.Message });
            }
        }
    }
}
