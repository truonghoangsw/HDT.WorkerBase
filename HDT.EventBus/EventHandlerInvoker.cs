using HDT.Core;
using HDT.Core.DependencyInjection;
using HDT.Core.Exceptions;
using HDT.EventBus.Distributed;
using HDT.EventBus.Local;
using System.Collections.Concurrent;

namespace HDT.EventBus
{
    public class EventHandlerInvoker : IEventHandlerInvoker, ISingletonDependency
    {
        private readonly ConcurrentDictionary<string,EventHandlerInvokerCacheItem> _cache;
        public EventHandlerInvoker()
        {
            _cache = new ConcurrentDictionary<string, EventHandlerInvokerCacheItem>();
        }
        public async Task InvokeAsync(IEventHandler eventHandler, object eventData, Type eventType)
        {
            var cacheItem = _cache.GetOrAdd($"{eventHandler.GetType().FullName}-{eventType.FullName}", _ =>
            {
                var item = new EventHandlerInvokerCacheItem();

                if (typeof(ILocalEventHandler<>).MakeGenericType(eventType).IsInstanceOfType(eventHandler))
                {
                    item.Local = (IEventHandlerMethodExecutor)Activator.CreateInstance(typeof(LocalEventHandlerMethodExecutor<>).MakeGenericType(eventType));
                }
                if (typeof(IDistributedEventHandler<>).MakeGenericType(eventType).IsInstanceOfType(eventHandler))
                {
                    item.Distributed = (IEventHandlerMethodExecutor)Activator.CreateInstance(typeof(DistributedEventHandlerMethodExecutor<>).MakeGenericType(eventType));
                }
                return item;
            });
            if (cacheItem.Local != null)
            {
                await cacheItem.Local.ExecutorAsync(eventHandler, eventData);
            }
            if (cacheItem.Distributed != null)
            {
                await cacheItem.Distributed.ExecutorAsync(eventHandler, eventData);
            }

            if (cacheItem.Local == null && cacheItem.Distributed == null)
            {
                throw new HDTException("The object instance is not an event handler. Object type: " + eventHandler.GetType().AssemblyQualifiedName);
            }

        }
    }
}
