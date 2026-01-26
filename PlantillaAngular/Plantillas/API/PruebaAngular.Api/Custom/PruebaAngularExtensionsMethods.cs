using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PruebaAngular.Infrastructure.Data;
using PruebaAngular.Infrastructure.Data.Seeders;

namespace PruebaAngular.Api.Custom
{
    static class PruebaAngularExtensionsMethods
    {
        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        public static void SeedDatabase(this IApplicationBuilder app, ILifetimeScope autofacContainer, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("ApiUnitTest:SeedDatabase") && configuration.GetValue<bool>("ApiUnitTest:UseSqlLite"))
            {
                using var scope = app.ApplicationServices.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<PruebaAngularContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
            }
        }

        /// <summary>
        /// Creates database tables and seeds initial data for local development
        /// </summary>
        public static async Task MigrateAndSeedDatabaseAsync(this IApplicationBuilder app, ILogger logger)
        {
            logger.LogInformation("Starting database initialization...");
            
            using var scope = app.ApplicationServices.CreateScope();
            
            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PruebaAngularContext>();
                
                // Wait for PostgreSQL to be ready
                logger.LogInformation("Waiting for database connection...");
                var retries = 10;
                while (retries > 0)
                {
                    try
                    {
                        if (await dbContext.Database.CanConnectAsync())
                        {
                            logger.LogInformation("Database connection established.");
                            break;
                        }
                    }
                    catch
                    {
                        // Ignore and retry
                    }
                    retries--;
                    logger.LogInformation("Database not ready, retrying in 2 seconds... ({Retries} attempts left)", retries);
                    await Task.Delay(2000);
                }

                if (retries == 0)
                {
                    throw new Exception("Could not connect to database after multiple retries");
                }

                // Create tables using raw SQL
                logger.LogInformation("Creating database tables via SQL...");
                await CreateTablesAsync(dbContext, logger);

                // Seed data
                logger.LogInformation("Seeding initial data...");
                await PortfolioSeeder.SeedAsync(dbContext, logger);
                
                logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database initialization failed: {Message}", ex.Message);
                throw;
            }
        }

        private static async Task CreateTablesAsync(PruebaAngularContext context, ILogger logger)
        {
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

            try
            {
                await context.Database.ExecuteSqlRawAsync(createTablesSql);
                logger.LogInformation("Database tables created successfully.");
            }
            catch (Exception ex)
            {
                logger.LogWarning("Table creation warning (tables may already exist): {Message}", ex.Message);
            }
        }
    }
}
