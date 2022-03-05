using System;
using System.Threading.Tasks;

namespace HDT.EventBus
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete = false) where TEvent : class;
        Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true);
        IDisposable Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class;
        IDisposable Subscribe<TEvent, THandler>() where TEvent : class where THandler : IEventHandler, new();
        void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class;
        void Unsubscribe(Type eventType, IEventHandlerFactory factory);
        void Unsubscribe(Type eventType, IEventHandler handler);
        void UnsubscribeAll<TEvent>() where TEvent : class;
        void UnsubscribeAll(Type eventType);
    }
}