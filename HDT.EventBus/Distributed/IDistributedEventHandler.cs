using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDT.EventBus.Distributed
{
    public interface IDistributedEventHandler<in TEvent> : IEventHandler
    {
        Task HandleEventAsync(TEvent eventData);
    }
}
