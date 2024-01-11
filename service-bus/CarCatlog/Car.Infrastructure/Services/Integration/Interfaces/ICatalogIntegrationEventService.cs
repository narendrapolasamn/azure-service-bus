
using EventBus.Events;
using System.Threading.Tasks;

namespace Car.Infrastructure.Services.Integration.Interfaces
{
    public interface ICatalogIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(IntegrationEvent @event);
        Task AddAndSaveEventAsync(IntegrationEvent @event);
    }
}
