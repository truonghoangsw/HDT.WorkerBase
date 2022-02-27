using HDT.EventBus.Local;
using Microsoft.Extensions.DependencyInjection;

namespace HDT.EventBus
{
    public abstract class EventBusBase : IEventBus
    {
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        protected IEventHandlerInvoker EventHandlerInvoker { get; }
        protected EventBusBase(IServiceScopeFactory serviceScopeFactory, IEventHandlerInvoker eventHandlerInvoker)
        {
            ServiceScopeFactory = serviceScopeFactory;
            EventHandlerInvoker = eventHandlerInvoker;
        }

        #region Optional
        public Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete = true)
        where TEvent : class
        {
            return PublishAsync(typeof(TEvent), eventData, onUnitOfWorkComplete);
        }
        public virtual async Task PublishAsync(
        Type eventType,
        object eventData,
        bool onUnitOfWorkComplete = true)
        {
            if (onUnitOfWorkComplete)
            {
                // TODO: version 2
                return;
            }

            await PublishToEventBusAsync(eventType, eventData);
        }

        /// <inheritdoc/>
        public virtual void Unsubscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : class
        {
            Unsubscribe(typeof(TEvent), handler);
        }

        public abstract void Unsubscribe(Type eventType, IEventHandler handler);
        public virtual void UnsubscribeAll<TEvent>() where TEvent : class
        {
            UnsubscribeAll(typeof(TEvent));
        }
        public virtual void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            Unsubscribe(typeof(TEvent), factory);
        }
        public IDisposable Subscribe<TEvent, THandler>()
           where TEvent : class
           where THandler : IEventHandler, new()
        {
            return Subscribe(typeof(TEvent), new TransientEventHandlerFactory<THandler>());
        }
        public virtual IDisposable Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
        {
            return Subscribe(typeof(TEvent), new ActionEventHandler<TEvent>(action));
        }
        public virtual IDisposable Subscribe(Type eventType, IEventHandler handler)
        {
            return Subscribe(eventType, new SingleInstanceHandlerFactory(handler));
        }
        public virtual IDisposable Subscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            return Subscribe(typeof(TEvent), factory);
        }
        #endregion

        #region Base abstract
        protected abstract Task PublishToEventBusAsync(Type eventType, object eventData);
        public abstract IDisposable Subscribe(Type eventType, IEventHandlerFactory factory);
        public abstract void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class;
        public abstract void Unsubscribe(Type eventType, IEventHandlerFactory factory);
        public abstract void UnsubscribeAll(Type eventType);

       

        #endregion


    }

    
}