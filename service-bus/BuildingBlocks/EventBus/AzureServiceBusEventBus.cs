using EventBus.Events;
using EventBus.Events.Interface;
using EventBus.Services.Interface;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;


namespace EventBus
{
    public class AzureServiceBusEventBus : IEventBus
    {
        private readonly SubscriptionClient _subscriptionClient;
        private readonly IEventBusSubscriptionsManager _eventBusSubscriptionsManager;
        private readonly ILogger<AzureServiceBusEventBus> _logger;
        private readonly IServiceBusConnectionManagementService _serviceBusConnectionManagementService;
        private readonly IServiceProvider _serviceProvider;

        public AzureServiceBusEventBus(IServiceBusConnectionManagementService serviceBusConnectionManagementService,
                         IEventBusSubscriptionsManager subscriptionManager,
                         IServiceProvider serviceProvider,
                         ILogger<AzureServiceBusEventBus> logger,
                         string subscriptionClientName)
        {
            _serviceBusConnectionManagementService = serviceBusConnectionManagementService;
            _eventBusSubscriptionsManager = subscriptionManager;
            _subscriptionClient = _subscriptionClient = new SubscriptionClient(_serviceBusConnectionManagementService.ServiceBusConnectionStringBuilder,
                subscriptionClientName);
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task PublishAsync(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name;
            var jsonMessage = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            var message = new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = body,
                Label = eventName,

            };

            var topicClient = _serviceBusConnectionManagementService.CreateTopicClient();
            await topicClient.SendAsync(message);
        }

        public async Task SubscribeAsync<T, TH>()
                where T : IntegrationEvent
                where TH : IIntegraionEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var containKey = _eventBusSubscriptionsManager.HasSubscriptionForEvent<T>();
            if (!containKey)
            {
                try
                {
                    await _subscriptionClient.AddRuleAsync(new RuleDescription
                    {
                        Filter = new CorrelationFilter {  Label = eventName },
                        Name = eventName
                    });
                }
                catch(ServerBusyException ex)
                {
                    _logger.LogError(ex,$"The messaging entity '{eventName}' already exists",eventName);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, $"The messaging entity '{eventName}' already exists", eventName);
                }
            }
            _logger.LogInformation("Subscribing to event '{EventName}' with '{EventHandler}'", eventName, typeof(TH).Name);
            _eventBusSubscriptionsManager.AddSubscription<T, TH>();
        }

       public async Task UnSubscribeAsync<T, TH>()
               where TH : IIntegraionEventHandler<T>
               where T : IntegrationEvent
        {
            var eventName = typeof(T).Name;
            try
            {
                await _subscriptionClient.RemoveRuleAsync(eventName);
            }
            catch (MessagingEntityNotFoundException ex)
            {
                _logger.LogError(ex, "The messaging entity '{eventName}' could not be found.", eventName);
            }
            _logger.LogInformation("Unsubscribing from event '{EventName}'", eventName);
            _eventBusSubscriptionsManager.RemoveSubscription<T, TH>();
        }

        public async Task SetupAsync()
        {
            try
            {
                await RemoveDefaultRuleAsync();
                RegisterSubscriptionClientMessageHandler();
            }

            catch (MessagingEntityNotFoundException)
            {
                _logger.LogWarning("The messaging entity '{DefaultRuleName}' Could not be found.", RuleDescription.DefaultRuleName);
            }
        }

        private void RegisterSubscriptionClientMessageHandler()
        {
            _subscriptionClient.RegisterMessageHandler(
                async(message,token) =>{
                    var eventName = message.Label;
                    var messageData = Encoding.UTF8.GetString(message.Body);
                    if(await ProcessEvent(eventName, messageData))
                    {
                        await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                    }
                },
                new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 10, AutoComplete = false });
        }
        private async Task RemoveDefaultRuleAsync()
        {
            try
            {
                await _subscriptionClient.RemoveRuleAsync(RuleDescription.DefaultRuleName);
            }
            catch (MessagingEntityNotFoundException)
            {
                _logger.LogWarning("The messaging entity '{DefaultRuleName}' Could not be found.", RuleDescription.DefaultRuleName);
            }
            catch(Exception ex)
            {
                _logger.LogWarning("The messaging entity '{DefaultRuleName}' Could not be found.", RuleDescription.DefaultRuleName);
            }
        }
        private async Task<bool> ProcessEvent(string eventName,string message)
        {
            var processed = false;
            if (_eventBusSubscriptionsManager.HasSubscriptionForEvent(eventName))
            {
                var subscriptions = _eventBusSubscriptionsManager.GetHandlersForEvent(eventName);
                foreach(var subscription in subscriptions)
                {
                    var handler = _serviceProvider.GetRequiredService(subscription.HandlerType);
                    if (handler is null) continue;
                    var eventType = _eventBusSubscriptionsManager.GetEventTypeByName(eventName);
                    var intergrationEvent = JsonConvert.DeserializeObject(message, eventType);
                    var concreteType = typeof(IIntegraionEventHandler<>).MakeGenericType(eventType);
                    await (Task)concreteType.GetMethod("HandleAsync").Invoke(handler, new object[] { intergrationEvent });
                    processed =  true;
                }
            }

            return processed;
        }
        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var ex = exceptionReceivedEventArgs.Exception;
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            _logger.LogError(ex, "ERROR handling message: '{ExceptionMessage}' - Context: '{ExceptionContext}'", ex.Message, context);
            return Task.CompletedTask;
        }
    }
}
