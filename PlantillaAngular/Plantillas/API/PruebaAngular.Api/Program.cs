using Autofac.Extensions.DependencyInjection;
using Elastic.Apm.SerilogEnricher;
using Elastic.CommonSchema.Serilog;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaAngular.Api
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = "PruebaAngular.API_net10";

        public static async Task Main(string[] args)
        {
            var configuration = Startup.GetConfiguration();

            Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                var hostBuilder = CreateHostBuilder(args,configuration);
               
                //Log.Information("Applying migrations ({ApplicationContext})...", AppName);


                Log.Information("Starting web host ({ApplicationContext})...", AppName);

                var host = hostBuilder.Build();

                await host.RunAsync();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);

            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args,IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
                // ASP.NET Core 3.0+:
                // The UseServiceProviderFactory call attaches the
                // Autofac provider to the generic hosting mechanism.
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseSerilog().UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((config) => {
                    config.AddConfiguration(configuration);
                    })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((config) => {
                        config.AddConfiguration(configuration);
                    })
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>();
                });



        

        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            var logstashUrl = configuration["Serilog:LogstashgUrl"];


            var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            //Local no enviamos a ELastic
            if (enviroment == "Local" || enviroment == "Development")
            {
                //Ficheros y consola
                return new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .Enrich.WithProperty("ApplicationContext", AppName)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.RollingFile(@"log\log-{Date}.txt",
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}")
                    //.ReadFrom.Configuration(configuration)
                    .CreateLogger();

            }
            else
            {
                //Para entornos productivos ELK productivo
                IEnumerable<Uri> clusterELK = configuration["ElasticSearch:ServerUrls"]
                              .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(uriString => new Uri(uriString));

                // Reemplazar configuraciones sensibles con variables de entorno
                var elasticUser = configuration["ElasticSearch:User"] ?? Environment.GetEnvironmentVariable("ELASTICSEARCH_USER");
                var elasticPass = configuration["ElasticSearch:Pass"] ?? Environment.GetEnvironmentVariable("ELASTICSEARCH_PASS");

                return new LoggerConfiguration()
                             .Enrich.WithElasticApmCorrelationInfo()
                             .Enrich.WithProperty("ApplicationContext", AppName)
                             .Enrich.FromLogContext()
                             .ReadFrom.Configuration(configuration)
                             
                             .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(clusterELK)
                             {
                                 CustomFormatter = new EcsTextFormatter(),
                                 IndexFormat = "MGA-NSI-{0:yyyy.MM}",
                                 ModifyConnectionSettings = x => x.BasicAuthentication(elasticUser, elasticPass)
                                                                 .ServerCertificateValidationCallback((o, certificate, chain, errors) => true)
                                                                 .ServerCertificateValidationCallback(CertificateValidations.AllowAll)

                             }).Filter.ByExcluding(logEvent => logEvent.MessageTemplate.Text.Contains("event from elasticsearch"))
                             .Filter.ByExcluding(logEvent=> logEvent.MessageTemplate.Text.Contains("You do not have a valid license key for the Lucky Penny software MediatR"))
                             .CreateLogger();
            }
        }
    }
}
