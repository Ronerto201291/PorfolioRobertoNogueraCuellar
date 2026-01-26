using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
namespace PruebaAngular.Api.Custom
{
    public static class AutofacExtensionsMethods
    {
        
        
        public static void RegisterQueryDbContext<TContext>(this ContainerBuilder builder)
            where TContext : DbContext
                    {
                        builder.Register(componentContext =>
                        {
                            var serviceProvider = componentContext.Resolve<IServiceProvider>();
                            var configuration = componentContext.Resolve<IConfiguration>();
                            var dbContextOptions = new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>());
                            if (configuration.GetValue<bool>("ApiUnitTest:UseSqlLite"))
                            {
                                //SqlLIte (unit testing)
                                var optionsBuilder = new DbContextOptionsBuilder<TContext>(dbContextOptions)
                                    .UseApplicationServiceProvider(serviceProvider)
                                    .UseSqlite(configuration["ApiUnitTest:LiteConnectionString"],
                                        sqlLiteOptions => {
                                        
                                        });

                                return optionsBuilder.Options;
                            }
                            else
                            {
                                //SqlServer
                                var optionsBuilder = new DbContextOptionsBuilder<TContext>(dbContextOptions)
                                    .UseApplicationServiceProvider(serviceProvider)
                                    .UseSqlServer(configuration["QueryConnectionString"],
                                        serverOptions => serverOptions.EnableRetryOnFailure(5, TimeSpan.FromMilliseconds(30), null));

                                return optionsBuilder.Options;
                            }
                        }).As<DbContextOptions<TContext>>()
                            .InstancePerDependency();

                        builder.Register(context => context.Resolve<DbContextOptions<TContext>>())
                            .As<DbContextOptions>()
                            .InstancePerDependency();

                        builder.RegisterType<TContext>()
                            .AsSelf()
                            .InstancePerDependency();
        }
        /// <summary>
        /// Metodo para registrar el command dbContext mediante autofac. 
        /// Si se utiliza este método no utilizar AddCommandDbContext
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="builder"></param>
        public static void RegisterCommandDbContext<TContext>(this ContainerBuilder builder)
           where TContext : DbContext
        {
            builder.Register(componentContext =>
            {
                var serviceProvider = componentContext.Resolve<IServiceProvider>();
                var configuration = componentContext.Resolve<IConfiguration>();
                var dbContextOptions = new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>());
                if (configuration.GetValue<bool>("ApiUnitTest:UseSqlLite"))
                {
                    //SqlLIte (unit testing)
                    var optionsBuilder = new DbContextOptionsBuilder<TContext>(dbContextOptions)
                        .UseApplicationServiceProvider(serviceProvider)
                        .UseSqlite(configuration["ApiUnitTest:LiteConnectionString"],
                            sqlLiteOptions => {

                            });

                    return optionsBuilder.Options;
                }
                else
                {
                    //SqlServer
                    var optionsBuilder = new DbContextOptionsBuilder<TContext>(dbContextOptions)
                        .UseApplicationServiceProvider(serviceProvider)
                        .UseSqlServer(configuration["ConnectionString"],
                            serverOptions => serverOptions.EnableRetryOnFailure(5, TimeSpan.FromMilliseconds(30), null));

                    return optionsBuilder.Options;
                }
                
            }).As<DbContextOptions<TContext>>()
                .InstancePerDependency();

            builder.Register(context => context.Resolve<DbContextOptions<TContext>>())
                .As<DbContextOptions>()
                .InstancePerDependency();

            builder.RegisterType<TContext>()
                .AsSelf()
                .InstancePerDependency();
        }
       
    }
}
