using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.Interface
{
    public interface  IIntegraionEventHandler<in TIntegrationEvent>
                      where TIntegrationEvent: IntegrationEvent  
    {
        Task HandleAsync(TIntegrationEvent @event);
    }
}
