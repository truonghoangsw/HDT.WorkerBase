using HDT.Core.Collections;
using HDT.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HDT.Core.LockExtensions;
using Microsoft.Extensions.Options;

namespace HDT.EventBus.Local
{
    public interface ILocalEventBus : IEventBus
    {
        IDisposable Subscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : class;
    }
    public class LocalEventBus : EventBusBase, ILocalEventBus, ISingletonDependency
    {
        public ILogger<LocalEventBus> Logger { get; set; }
        protected HDTLocalEventBusOptions Options { get; }

        protected ConcurrentDictionary<Type, List<IEventHandlerFactory>> HandlerFactories { get; }
        public LocalEventBus(
           IServiceScopeFactory serviceScopeFactory,
           IOptions<HDTLocalEventBusOptions> options,
           IEventHandlerInvoker eventHandlerInvoker)
       : base(serviceScopeFactory, eventHandlerInvoker)
        {
            Logger = NullLogger<LocalEventBus>.Instance;
            Options = options.Value;
            HandlerFactories = new ConcurrentDictionary<Type, List<IEventHandlerFactory>>();
            SubscribeHandlers(Options.Handlers);
        }

        public virtual IDisposable Subscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : class
        {
            return Subscribe(typeof(TEvent), handler);
        }
        public override IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType)
                .Locking(factories =>
                {
                    if (!factory.IsInFactories(factories))
                    {
                        factories.Add(factory);
                    }
                });
            return new EventHandlerFactoryUnregistrar(this, eventType, factory);

        }
        public override void Unsubscribe(Type eventType, IEventHandler handler)
        {
            GetOrCreateHandlerFactories(eventType)
            .Locking(factories =>
            {
                factories.RemoveAll(
                    factory =>
                        factory is SingleInstanceHandlerFactory &&
                        (factory as SingleInstanceHandlerFactory).HandlerInstance == handler
                );
            });
        }

        protected override async Task PublishToEventBusAsync(Type eventType, object eventData)
        {
            await PublishAsync(new LocalEventMessage(Guid.NewGuid(), eventData, eventType));
        }

        public virtual async Task PublishAsync(LocalEventMessage localEventMessage)
        {
            await TriggerHandlersAsync(localEventMessage.EventType, localEventMessage.EventData);
        }

        
        public override void Unsubscribe<TEvent>(Func<TEvent, Task> action)
        {
            GetOrCreateHandlerFactories(typeof(TEvent))
                .Locking(factories =>
                {
                    factories.RemoveAll(factory =>
                    {
                        var singleInstanceFactory = factory as SingleInstanceHandlerFactory;
                        if (singleInstanceFactory == null)
                        {
                            return false;
                        }

                        var actionHandler = singleInstanceFactory.HandlerInstance as ActionEventHandler<TEvent>;
                        if (actionHandler == null)
                        {
                            return false;
                        }

                        return actionHandler.Action == action;
                    });
                });
        }

        public override void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Remove(factory));
        }

        public override void UnsubscribeAll(Type eventType)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Clear());
        }

        private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
        {
            return HandlerFactories.GetOrAdd(eventType, (type) => new List<IEventHandlerFactory>());
        }

        protected override IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
        {
            var handlerFactoryList = new List<EventTypeWithEventHandlerFactories>();

            foreach (var handlerFactory in HandlerFactories.Where(hf => ShouldTriggerEventForHandler(eventType, hf.Key)))
            {
                handlerFactoryList.Add(new EventTypeWithEventHandlerFactories(handlerFactory.Key, handlerFactory.Value));
            }

            return handlerFactoryList.ToArray();
        }

        private static bool ShouldTriggerEventForHandler(Type targetEventType, Type handlerEventType)
        {
            //Should trigger same type
            if (handlerEventType == targetEventType)
            {
                return true;
            }

            //Should trigger for inherited types
            if (handlerEventType.IsAssignableFrom(targetEventType))
            {
                return true;
            }

            return false;
        }
    }
}
