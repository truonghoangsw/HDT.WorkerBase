namespace HDT.EventBus
{
    public class SingleInstanceHandlerFactory : IEventHandlerFactory
    {
        public IEventHandler HandlerInstance { get; }
        public SingleInstanceHandlerFactory(IEventHandler handler)
        {
            HandlerInstance = handler;
        }

        public IEventHandlerDisposeWrapper GetHandler()
        {
            return new EventHandlerDisposeWrapper(HandlerInstance);
        }

        public bool IsInFactories(List<IEventHandlerFactory> handlerFactories)
        {
            return handlerFactories
                     .OfType<SingleInstanceHandlerFactory>()
                     .Any(f => f.HandlerInstance == HandlerInstance);
        }
    }
}