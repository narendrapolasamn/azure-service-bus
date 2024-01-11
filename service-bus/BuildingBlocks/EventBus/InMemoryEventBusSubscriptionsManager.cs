using EventBus.Events;
using EventBus.Events.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly Dictionary<string , List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _eventTypes;

        public InMemoryEventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string , List<SubscriptionInfo>>(); 
            _eventTypes = new List<Type>();
        }
        public bool IsEmpty =>!_handlers.Keys.Any();

        public event EventHandler<string> OnEventRemoved;

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegraionEventHandler<T>
        {
            var eventName = GetEventByKey<T>();
            if (!HasSubscriptionForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());
            }
            var handlerType = typeof(TH);
            if (_handlers[eventName].Any(s=>s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                    $"Handler type {handlerType.Name} already register for '{eventName}'",
                    nameof(handlerType)
                    );
            }
            _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
            if (!_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }
        }

        public void Clear() => _handlers.Clear();

        public string GetEventByKey<T>() => typeof(T).Name;

        public Type GetEventTypeByName(string eventName) => _eventTypes
                                                            .SingleOrDefault(t => t.Name == eventName);

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventByKey<T>();
            return GetHandlersForEvent(key);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

        public bool HasSubscriptionForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventByKey<T>();
            return HasSubscriptionForEvent(key);
        }

        public bool HasSubscriptionForEvent(string eventName)=>_handlers.ContainsKey(eventName);

        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegraionEventHandler<T>
        {
            var handlersRemove = FindSubscriptionInfo<T, TH>();
            var eventname = GetEventByKey<T>();
            if(handlersRemove is null)
            {
                _handlers[eventname].Remove(handlersRemove);
                var eventType = _eventTypes
                                .SingleOrDefault(e => e.Name == eventname);
                if (eventType is not null) _eventTypes.Remove(eventType);
                RaiseEventRemoved(eventname);
            }
        }
         private void RaiseEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);
        }
        private SubscriptionInfo FindSubscriptionInfo<T,TH>()
                where T : IntegrationEvent
                where TH : IIntegraionEventHandler<T>
        {
            var eventName = GetEventByKey<T>();
            if (!HasSubscriptionForEvent(eventName)) return null;
            return _handlers[eventName]
                   .SingleOrDefault(s => s.HandlerType == typeof(TH));

        }
    }
}
