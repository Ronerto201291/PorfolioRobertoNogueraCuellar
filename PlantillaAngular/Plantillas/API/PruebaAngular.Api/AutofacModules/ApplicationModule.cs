using Autofac;
using Microsoft.Extensions.Configuration;
using PruebaAngular.Api.Custom;
using PruebaAngular.Infrastructure.Data;
using PruebaAngular.Infrastructure.Data.Core;
using PruebaAngular.Infrastructure.Data.Cache;
using Serilog;
using System;

namespace PruebaAngular.Api.AutofacModules
{
    public class ApplicationModule : Module
    {
        private readonly IConfiguration configuration;

        public ApplicationModule(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            try
            {
                var environment = configuration.GetValue<String>("ASPNETCORE_ENVIRONMENT");

                builder.RegisterType<CurrentApiUserSessionProvider>()
                    .As<ICurrentSessionProvider>().InstancePerLifetimeScope();

                builder.RegisterInstance<IDbContextSchema>(new IDbContextSchema(configuration["DefaultDbSchema"])).SingleInstance();

                if (environment == "Local" || environment == "UnitTests")
                {
                    builder.RegisterType<InMemmoryCacheProvider>()
                        .As<ICacheProvider>()
                        .SingleInstance();
                }
                else
                {
                    builder.RegisterType<InMemmoryCacheProvider>()
                        .As<ICacheProvider>()
                        .SingleInstance();
                }
                
                builder.RegisterInstance(configuration).SingleInstance();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"{GetType().FullName} terminated unexpectedly");
            }
        }
    }
}
