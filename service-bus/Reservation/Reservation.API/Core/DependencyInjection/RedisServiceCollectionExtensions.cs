using Reservation.Domain.Repositories.Interfaces;
using Reservation.Infrastructure.Configuration.Interfaces;
using Reservation.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Security.Authentication;

namespace Reservation.API.Core.DependencyInjection
{
    public static class RedisServiceCollectionExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                var redisConfiguration = sp.GetRequiredService<IRedisConfiguration>();
                var configuration = ConfigurationOptions.Parse(redisConfiguration.ConnectionString, true);
                configuration.SslProtocols = SslProtocols.Tls12;
                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddSingleton<IReservationRepository, ReservationRepository>();

            return services;
        }
    }
}
