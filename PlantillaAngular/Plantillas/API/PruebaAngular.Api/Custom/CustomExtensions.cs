using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PruebaAngular.Infrastructure.Data;
using PruebaAngular.Infrastructure.Data.Core;

namespace PruebaAngular.Api.Custom
{
    public static class CustomExtensionsMethods
    {
        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Placeholder for custom configuration
            return services;
        }

        public static IServiceCollection AddQueryDbContext<TContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionStringName = "DefaultConnection") where TContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionStringName);
            services.AddDbContext<TContext>(options =>
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    options.UseSqlServer(connectionString);
                }
            });
            return services;
        }

        public static IServiceCollection AddCommandDbContext<TContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionStringName = "ConnectionString") where TContext : DbContext
        {
            // Try Aspire connection string first (injected as "portfolio")
            var connectionString = configuration.GetConnectionString("portfolio") 
                ?? configuration.GetConnectionString(connectionStringName);

            Console.WriteLine($"[DbContext] Connection string: {(string.IsNullOrEmpty(connectionString) ? "NOT FOUND" : "Found")}");
            
            // Register required dependencies
            services.AddScoped<IDbContextSchema>(_ => new DefaultDbContextSchema());
            services.AddScoped<ICurrentSessionProvider, CurrentApiUserSessionProvider>();
            
            services.AddDbContext<TContext>(options =>
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    Console.WriteLine($"[DbContext] Configuring PostgreSQL connection...");
                    // Use PostgreSQL
                    options.UseNpgsql(connectionString, npgsqlOptions =>
                    {
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorCodesToAdd: null);
                    });
                }
            });
            return services;
        }

        public static IApplicationBuilder AddCustomExceptionHandlingPipeline(this IApplicationBuilder app, ILoggerFactory loggerFactory, IHostEnvironment env)
        {
            // Placeholder for custom exception handling
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            return app;
        }

        public static IServiceCollection AddCustomSwagger(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment env)
        {
            // Placeholder for Swagger configuration
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }

        public static IServiceCollection AddCustomMvc(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Placeholder for MVC configuration
            services.AddControllers();
            return services;
        }

        public static IServiceCollection AddCustomCache(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment env)
        {
            // Placeholder for cache configuration
            services.AddMemoryCache();
            return services;
        }

        public static IServiceCollection AddCustomHealthChecks(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Placeholder for health checks configuration
            services.AddHealthChecks();
            return services;
        }
    }

    // Default schema implementation for PostgreSQL (public schema)
    public class DefaultDbContextSchema : IDbContextSchema
    {
        public DefaultDbContextSchema() : base("public") { }
    }
}
