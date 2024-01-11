using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.Interface
{
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }

        event EventHandler<string> OnEventRemoved;

        void AddSubscription<T, TH>()
             where T : IntegrationEvent
             where TH : IIntegraionEventHandler<T>;

        void RemoveSubscription<T, TH>()
             where T : IntegrationEvent
             where TH: IIntegraionEventHandler<T>;

        bool HasSubscriptionForEvent<T>() where T : IntegrationEvent;

        bool HasSubscriptionForEvent(string  eventName);

        Type GetEventTypeByName(string eventName);

        void Clear();

        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T :IntegrationEvent;

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        string GetEventByKey<T>();
    }
}
