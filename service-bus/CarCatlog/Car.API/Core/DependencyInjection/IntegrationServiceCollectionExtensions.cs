using Car.Infrastructure.Configuration.Interfaces;
using Car.Infrastructure.Services.Integration.Interfaces;
using Car.Infrastructure.Services.Integration;
using EventBus;
using EventBus.Events.Interface;
using EventBus.Services;
using EventBus.Services.Interface;
using EventLog.Services;
using EventLog.Services.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;


namespace Car.API.Core.DependencyInjection
{
    public static class IntegrationServiceCollectionExtensions
    {
        public static IServiceCollection AddIntegrationServices(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var azureServiceBusConfiguration = serviceProvider.GetRequiredService<IAzureServiceBusConfiguration>();
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddTransient<ICatalogIntegrationEventService, CatalogIntegrationEventService>();

            services.AddSingleton<IServiceBusConnectionManagementService>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ServiceBusConnectionManagementService>>();
                var serviceBusConnection = new ServiceBusConnectionStringBuilder(azureServiceBusConfiguration.ConnectionString);
                return new ServiceBusConnectionManagementService(logger, serviceBusConnection);
            });

            services.AddSingleton<IEventBus, AzureServiceBusEventBus>(sp =>
            {
                var serviceBusConfiguration = sp.GetRequiredService<IServiceBusConnectionManagementService>();
                var logger = sp.GetRequiredService<ILogger<AzureServiceBusEventBus>>();
                var eventBusSubscriptionManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var eventBus = new AzureServiceBusEventBus(serviceBusConfiguration,
                                eventBusSubscriptionManager, serviceProvider, logger, azureServiceBusConfiguration.SubscriptionClientName);
                eventBus.SetupAsync().GetAwaiter().GetResult();
                return eventBus;
            });
            services.AddTransient<Func<DbConnection, IEventLogService>>(
                sp => (DbConnection conn) => new EventLogService(conn));
            return services;
        }
    }
}
