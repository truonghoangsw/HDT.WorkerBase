namespace HDT.EventBus
{
    public class TransientEventHandlerFactory<THandler> : TransientEventHandlerFactory, IEventHandlerFactory
    where THandler : IEventHandler, new()
    {
        public TransientEventHandlerFactory()
        : base(typeof(THandler))
        {

        }
        protected override IEventHandler CreateHandler()
        {
            return new THandler();
        }
    }
    public class TransientEventHandlerFactory : IEventHandlerFactory
    {
        public Type HandlerType { get;}
        public TransientEventHandlerFactory(Type handlerType)
        {
            HandlerType = handlerType;
        }
        public virtual IEventHandlerDisposeWrapper GetHandler()
        {
            var handler = CreateHandler();
            return new EventHandlerDisposeWrapper(
                handler,
                () => (handler as IDisposable)?.Dispose()
            );
        }

        public bool IsInFactories(List<IEventHandlerFactory> handlerFactories)
        {
            return  handlerFactories
                    .OfType<TransientEventHandlerFactory>()
                    .Any(f=>f.HandlerType == HandlerType);
        }
        protected virtual IEventHandler CreateHandler()
        {
            return (IEventHandler)Activator.CreateInstance(HandlerType);
        }
    }
}