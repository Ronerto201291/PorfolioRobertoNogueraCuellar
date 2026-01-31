using PruebaAngular.Api.Filters;
using PruebaAngular.Api.Security;
using PruebaAngular.Api.GraphQL;
using PruebaAngular.Api.Extensions;

using Autofac;
using Autofac.Extensions.DependencyInjection;
using Elastic.Apm.AspNetCore;
using Elastic.Apm.Azure.ServiceBus;
using Elastic.Apm.Azure.Storage;
using Elastic.Apm.DiagnosticSource;
using Elastic.Apm.EntityFrameworkCore;
using Elastic.Apm.Instrumentations.SqlClient;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PruebaAngular.Api.AutofacModules;
using PruebaAngular.Api.Custom;
using PruebaAngular.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace PruebaAngular.Api
{
    public class Startup
    {
        // public IConfiguration Configuration { get; }
        static readonly string _RequireAuthenticatedUserPolicy =
            "RequireAuthenticatedUserPolicy";

        private IWebHostEnvironment _env;
        private ILogger<Startup> _logger;
        public Startup(IWebHostEnvironment env)
        {
            _env = env;
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            // In ASP.NET Core 3.0 `env` will be an IWebHostEnvironment, not IHostingEnvironment.
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            //    .AddEnvironmentVariables();
            this.Configuration = GetConfiguration();// builder.Build();
        }

        internal static IConfiguration GetConfiguration()
        {
            string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{envName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.AzureAd.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.CanalesOnline.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.PruebaAngular.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
        public IConfiguration Configuration { get; private set; }

        public ILifetimeScope AutofacContainer { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the collection. Don't build or return
            // any IServiceProvider or the ConfigureContainer method
            // won't get called. Don't create a ContainerBuilder
            // for Autofac here, and don't call builder.Populate() - that
            // happens in the AutofacServiceProviderFactory for you.
            services
                .AddCommandDbContext<PruebaAngularContext>(Configuration, "ConnectionString")
                .AddCustomSwagger(Configuration,_env)
                .AddApplicationInsightsTelemetry(Configuration)
                .AddCustomMvc(Configuration)
                .AddHttpClient()
                .AddHttpContextAccessor()
                .AddMediatR(config=>config.RegisterServicesFromAssembly(typeof(IMediator).GetTypeInfo().Assembly))
                .AddCustomCache(Configuration,_env)
                .AddRabbitMqEventBus(Configuration)
                .AddCustomHealthChecks(Configuration);

            // Añadir health checks de RabbitMQ
            services.AddHealthChecks()
                .AddRabbitMqHealthChecks();

            // Configure GraphQL with HotChocolate
            services
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>()
                .AddProjections()
                .AddFiltering()
                .AddSorting()
                .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = _env.IsDevelopment());

            var container = new ContainerBuilder();
            container.Populate(services);
            ConfigureContainer(container);
            services.AddTransient<IApiKeyValidation, ApiKeyValidation>();
            services.AddHttpContextAccessor();

            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Instance =
                        $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

                    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                    Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                    context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
                };
            });
            services.AddExceptionHandler<CustomExceptionHandler>();
            services.AddScoped<HttpGlobalExceptionFilter>();
            services.AddHttpClient();

            ////HACK: Servicio en segundo plano
            //services.AddHostedService<QueuedHostedService>(factory =>
            //{
            //    return factory.GetRequiredService<QueuedHostedService>();
            //});
            

            //return new AutofacServiceProvider(container.Build());

        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac here. Don't
            // call builder.Populate(), that happens in AutofacServiceProviderFactory
            // for you.
            builder.RegisterModule(new ApplicationModule(Configuration));
            builder.RegisterModule(new MediatorModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Configure is where you add middleware. This is called after
        // ConfigureContainer. You can use IApplicationBuilder.ApplicationServices
        // here if you need to resolve things from the container.
        public void Configure(
            IApplicationBuilder app,
            ILoggerFactory loggerFactory)
        {
            //app.UseMvc();
            // If, for some reason, you need a reference to the built container, you
            // can use the convenience extension method GetAutofacRoot.
            this._logger = loggerFactory.CreateLogger<Startup>();
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();
            app.AddCustomExceptionHandlingPipeline(loggerFactory, _env);

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{pathBase}'", pathBase);
                app.UsePathBase(pathBase);
            }

            // Apply migrations and seed data (always in non-production)
            // NOTE: Uncomment after PostgreSQL is working
            // Auto-inicializar base de datos en desarrollo
            if (_env.EnvironmentName != "Production")
            {
                _logger.LogInformation("Environment: {Environment} - Auto-inicializando base de datos...", _env.EnvironmentName);
                InitializeDatabaseAsync(app).GetAwaiter().GetResult();
            }

            if (Configuration.GetValue<bool>("ApiUnitTest:SeedDatabase")
                && Configuration.GetValue<bool>("ApiUnitTest:UseSqlLite"))
            {
                app.SeedDatabase(this.AutofacContainer,Configuration);
            }

            app.UseStaticFiles();
            app.UseHealthChecks("/hc");
            app.UseRouting();
            app.UseHttpsRedirection();

            // GraphQL endpoint
            app.UseWebSockets();

            app.UseExceptionHandler();
            app.UseStatusCodePages();

            app.UseCors("CorsPolicy");
            if (_env.EnvironmentName != "Local")
            {
                app.UseElasticApm
               (
                Configuration,
                new HttpDiagnosticsSubscriber(),
                new SqlClientDiagnosticSubscriber(),
                new EfCoreDiagnosticsSubscriber(),
                new AzureMessagingServiceBusDiagnosticsSubscriber(),
                new MicrosoftAzureServiceBusDiagnosticsSubscriber(),
                new AzureBlobStorageDiagnosticsSubscriber(),
                new AzureQueueStorageDiagnosticsSubscriber(),
                new AzureFileShareStorageDiagnosticsSubscriber()
               );

            }


            ConfigureAuth(app);
           
            //app.UseMvcWithDefaultRoute();
            if (_env.EnvironmentName != "Production")
            {
                app.UseSwagger(c =>
                {
                    c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;
                })
                .UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "swagger"; 
                    c.SwaggerEndpoint($"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}v1/swagger.json", Configuration.GetValue<string>("Swagger:Title", "API V1"));
                    c.OAuthClientId(Configuration.GetValue<string>("AzureAd:ClientId"));
                    c.OAuthUsePkce();
                    c.OAuthScopeSeparator(" ");
                    c.OAuthScopes(Configuration.GetValue<string>("AzureAd:Scopes"));
                    c.OAuthClientSecret(Configuration.GetValue<string>("AzureAd:ClientSecret"));
                    c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
                });

            }

            // para acceder a la ip cliente del request
            // usado en el token
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto
            });

            ////Servicio en segundo plano
            ////registrado el servicio de cola, lo obtenemos
            //var queueSvc = app.ApplicationServices.GetRequiredService<QueuedHostedService>();

            //// y lo iniciamos. Importante: el AppService debe configurarse como "Siempre activo"
            //queueSvc.StartAsync(new System.Threading.CancellationToken());

            //app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping.Register(() =>
            //{
            //    // Detener el procesamiento de mensajes aqu�
            //    _logger.LogInformation($"PruebaAngular {GetType()} ApplicationStopping... Deteniendo servicio RequestHostedService");
            //    queueSvc.StopAsync(new System.Threading.CancellationToken()).Wait();
            //});
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            var optionSettings = Configuration.GetSection("PruebaAngularSettings").Get<PruebaAngularSettings>();

            if (optionSettings.GlobalAuthorize)
            {
                app.UseAuthentication();
                app.UseAuthorization();

                if (optionSettings.UseLoadTest)
                {
                    // Auto login middleware would be configured here if needed
                }

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapControllers();
                    endpoints.MapDefaultControllerRoute()
                        .RequireAuthorization(_RequireAuthenticatedUserPolicy);
                    
                    // GraphQL endpoint
                    endpoints.MapGraphQL("/graphql/portfolio");
                });
            }
            else
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapControllers()
                    .RequireCors(config =>
                    {
                        config.AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
                    
                    // GraphQL endpoint
                    endpoints.MapGraphQL("/graphql/portfolio");
                });
            }
        }

        /// <summary>
        /// Inicializa la base de datos automáticamente al arrancar (solo en desarrollo).
        /// Crea las tablas y siembra datos si no existen.
        /// </summary>
        private async Task InitializeDatabaseAsync(IApplicationBuilder app)
        {
            try
            {
                using var scope = app.ApplicationServices.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<PruebaAngularContext>();

                // Esperar a que PostgreSQL esté listo
                var maxRetries = 10;
                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        if (await context.Database.CanConnectAsync())
                        {
                            break;
                        }
                    }
                    catch
                    {
                        _logger.LogWarning("Esperando conexión a PostgreSQL... intento {Attempt}/{MaxRetries}", i + 1, maxRetries);
                        await Task.Delay(2000);
                    }
                }

                // Crear tablas
                await context.Database.ExecuteSqlRawAsync(Infrastructure.Data.Queries.DatabaseSetupQueries.CreateTables);
                _logger.LogInformation("✅ Tablas de base de datos verificadas/creadas");

                // Verificar si hay datos
                var hasData = await context.Projects.AnyAsync();
                if (!hasData)
                {
                    var projectId = Guid.NewGuid();
                    await context.Database.ExecuteSqlRawAsync(
                        Infrastructure.Data.Queries.DatabaseSetupQueries.GetSeedDataQuery(projectId.ToString()));
                    _logger.LogInformation("✅ Datos semilla insertados");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al auto-inicializar la base de datos: {Message}", ex.Message);
            }
        }
    }
}
