using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.Interface
{
    public interface IEventBus
    {
        Task PublishAsync(IntegrationEvent @event);

        Task SubscribeAsync<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegraionEventHandler<T>;

        Task UnSubscribeAsync<T, TH>()
             where TH : IIntegraionEventHandler<T>
             where T : IntegrationEvent;

        Task SetupAsync();

    }
}
