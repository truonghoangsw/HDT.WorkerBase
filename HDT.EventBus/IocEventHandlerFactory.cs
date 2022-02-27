using Microsoft.Extensions.DependencyInjection;

namespace HDT.EventBus
{
    public class IocEventHandlerFactory : IEventHandlerFactory, IDisposable
    {
        public Type HandlerType { get; set; }
        protected IServiceScopeFactory ScopeFactory { get; }

        public IocEventHandlerFactory(IServiceScopeFactory scopeFactory, Type handlerType)
        {
            ScopeFactory = scopeFactory;
            HandlerType = handlerType;
        }

        public void Dispose()
        {
        }

        public IEventHandlerDisposeWrapper GetHandler()
        {
            var scope = ScopeFactory.CreateScope();
            return new EventHandlerDisposeWrapper(
             (IEventHandler)scope.ServiceProvider.GetRequiredService(HandlerType),
             () => scope.Dispose()
         );
        }
        public bool IsInFactories(List<IEventHandlerFactory> handlerFactories)
        {
            return handlerFactories
                    .OfType<TransientEventHandlerFactory>()
                    .Any(f => f.HandlerType == HandlerType);
        }
    }
}