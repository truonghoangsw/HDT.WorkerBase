using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDT.EventBus.Tests.EventBus
{

    public class ActionBasedEventHandlerTest : EventBusTestBase
    {
        [Fact]
        public async Task Should_Call_Action_On_Event_With_Correct_Source()
        {
            var totalData = 0;
            var action = 
            LocalEventBus.Subscribe<MySimpleEventData>(
                eventData =>
                {
                    totalData += eventData.Value;
                    return Task.CompletedTask;
                });

            await LocalEventBus.PublishAsync(new MySimpleEventData(1));
            await LocalEventBus.PublishAsync(new MySimpleEventData(2));
            await LocalEventBus.PublishAsync(new MySimpleEventData(3));
            await LocalEventBus.PublishAsync(new MySimpleEventData(4));

            Assert.Equal(10, totalData);
        }
    }
}
