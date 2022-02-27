using System.Collections.Generic;

namespace HDT.EventBus
{
    public interface IEventHandlerFactory
    {
        /// <summary>
        /// Gets an event handler.
        /// </summary>
        /// <returns>The event handler</returns>
        IEventHandlerDisposeWrapper GetHandler();
        bool IsInFactories(List<IEventHandlerFactory> handlerFactories);
    }
}