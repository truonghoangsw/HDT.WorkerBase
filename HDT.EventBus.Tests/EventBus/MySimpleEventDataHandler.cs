using HDT.Core.DependencyInjection;
using HDT.EventBus.Local;
using System.Threading.Tasks;

namespace HDT.EventBus.Tests.EventBus
{
    public class MySimpleEventDataHandler : ILocalEventHandler<MySimpleEventData>, ISingletonDependency
    {
        public int TotalData { get; private set; }

        public Task HandleEventAsync(MySimpleEventData eventData)
        {
            TotalData += eventData.Value;
            return Task.CompletedTask;
        }
    }
}
