﻿using HDT.Core.Collections;
using HDT.EventBus.Local;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.CompilerServices;
using HDT.Core.Exceptions;

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
        public Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete = false)
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

        public virtual async Task TriggerHandlersAsync(Type eventType, object eventData)
        {
            var exceptions = new List<Exception>();

            await TriggerHandlersAsync(eventType, eventData, exceptions);

            if (exceptions.Any())
            {
                ThrowOriginalExceptions(eventType, exceptions);
            }
        }

        #endregion

        #region protected
        protected virtual async Task TriggerHandlersAsync(Type eventType, object eventData, List<Exception> exceptions)
        {
            await new SynchronizationContextRemover();

            foreach (var handlerFactories in GetHandlerFactories(eventType))
            {
                foreach (var handlerFactory in handlerFactories.EventHandlerFactories)
                {
                    await TriggerHandlerAsync(handlerFactory, handlerFactories.EventType, eventData, exceptions);
                }
            }

            //Implements generic argument inheritance. See IEventDataWithInheritableGenericArgument
            if (eventType.GetTypeInfo().IsGenericType &&
                eventType.GetGenericArguments().Length == 1 &&
                typeof(IEventDataWithInheritableGenericArgument).IsAssignableFrom(eventType))
            {
                var genericArg = eventType.GetGenericArguments()[0];
                var baseArg = genericArg.GetTypeInfo().BaseType;
                if (baseArg != null)
                {
                    var baseEventType = eventType.GetGenericTypeDefinition().MakeGenericType(baseArg);
                    var constructorArgs = ((IEventDataWithInheritableGenericArgument)eventData).GetConstructorArgs();
                    var baseEventData = Activator.CreateInstance(baseEventType, constructorArgs);
                    await PublishToEventBusAsync(baseEventType, baseEventData);
                }
            }
        }
        protected void ThrowOriginalExceptions(Type eventType, List<Exception> exceptions)
        {
            if (exceptions.Count == 1)
            {
                exceptions[0].ReThrow();
            }

            throw new AggregateException(
                "More than one error has occurred while triggering the event: " + eventType,
                exceptions
            );
        }
        protected virtual void SubscribeHandlers(ITypeList<IEventHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                var interfaces = handler.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (!typeof(IEventHandler).GetTypeInfo().IsAssignableFrom(@interface))
                    {
                        continue;
                    }

                    var genericArgs = @interface.GetGenericArguments();
                    if (genericArgs.Length == 1)
                    {
                        Subscribe(genericArgs[0], new IocEventHandlerFactory(ServiceScopeFactory, handler));
                    }
                }
            }
        }


        #endregion

        #region Base abstract
        protected virtual async Task TriggerHandlerAsync(IEventHandlerFactory asyncHandlerFactory, Type eventType,
       object eventData, List<Exception> exceptions)
        {
            using (var eventHandlerWrapper = asyncHandlerFactory.GetHandler())
            {
                try
                {
                    var handlerType = eventHandlerWrapper.EventHandler.GetType();

                    await EventHandlerInvoker.InvokeAsync(eventHandlerWrapper.EventHandler, eventData, eventType);

                }
                catch (TargetInvocationException ex)
                {
                    exceptions.Add(ex.InnerException);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
        }
        protected abstract IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType);
        protected class EventTypeWithEventHandlerFactories
        {
            public EventTypeWithEventHandlerFactories(Type eventType, List<IEventHandlerFactory> eventHandlerFactories)
            {
                EventType = eventType;
                EventHandlerFactories = eventHandlerFactories;
            }

            public Type EventType { get; }
            public List<IEventHandlerFactory> EventHandlerFactories { get; set; }
        }
        protected struct SynchronizationContextRemover : INotifyCompletion
        {
            public bool IsCompleted
            {
                get { return SynchronizationContext.Current == null; }
            }

            public void OnCompleted(Action continuation)
            {
                var prevContext = SynchronizationContext.Current;
                try
                {
                    SynchronizationContext.SetSynchronizationContext(null);
                    continuation();
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(prevContext);
                }
            }

            public SynchronizationContextRemover GetAwaiter()
            {
                return this;
            }

            public void GetResult()
            {
            }
        }
        protected abstract Task PublishToEventBusAsync(Type eventType, object eventData);
        public abstract IDisposable Subscribe(Type eventType, IEventHandlerFactory factory);
        public abstract void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class;
        public abstract void Unsubscribe(Type eventType, IEventHandlerFactory factory);
        public abstract void UnsubscribeAll(Type eventType);
        #endregion


    }

    
}