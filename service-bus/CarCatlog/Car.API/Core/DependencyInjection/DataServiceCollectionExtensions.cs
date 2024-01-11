using Car.Domain.Repositories.Interfaces;
using Car.Infrastructure.Configuration.Interfaces;
using Car.Infrastructure.Repositories;
using CarsIsland.Catalog.Infrastructure.Repositories;
using EventLog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Car.API.Core.DependencyInjection
{
    public static class DataServiceCollectionExtensions
    {
        public static IServiceCollection AddDataService(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var sqlDbConfiguration = serviceProvider.GetRequiredService<ISqlDbDataServiceConfiguration>();
            services.AddDbContext<CarCatalogDbContext>(options =>
            {
                options.UseSqlServer(sqlDbConfiguration.ConnectionString,
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(CarCatalogDbContext).GetTypeInfo().Assembly.GetName().Name);
                                         sqlOptions.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null);
                                     });
            });
            

            services.AddDbContext<EventLogContext>(options =>
            {
                options.UseSqlServer(sqlDbConfiguration.ConnectionString,
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(CarCatalogDbContext).GetTypeInfo().Assembly.GetName().Name);
                                         sqlOptions.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null);
                                     });
            });

            services.AddScoped<ICarsCatalogRepository, CarCatalogRepository>();
           

            return services;
        }
        public static void Migrate<TContext>(IServiceProvider provider) where TContext : DbContext
        {
            var logger = provider.GetRequiredService<ILogger<TContext>>();
            var dbContext = provider.GetRequiredService<TContext>();
            if(dbContext is null)
            {
                logger.LogInformation("Context {DBContextName} is not registered", typeof(TContext).Name);
                throw new ArgumentNullException($"No context of type {typeof(TContext).Name} registerd");
            }
            dbContext.Database.Migrate();
            logger.LogInformation("Database associated with context {DbContextName} migrated", typeof(TContext).Name);
        }
    }
}
