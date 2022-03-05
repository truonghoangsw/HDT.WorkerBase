using HDT.Core;
using HDT.Core.Collections;
using HDT.EventBus.Local;
using HDT.EventBus.Tests.EventBus;
using HDT.TestBase.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDT.EventBus.Tests
{
    public abstract class EventBusTestBase : HDTIntegratedTest
    {
        protected ILocalEventBus LocalEventBus;

        protected EventBusTestBase()
        {
            LocalEventBus = GetRequiredService<ILocalEventBus>();
        }
        protected override void BeforeAddApplication(IServiceCollection services)
        {
           
            services.Configure<HDTLocalEventBusOptions>(options =>
            {
                options.Handlers.Add(typeof(ILocalEventHandler<>));
                options.Handlers.Add(typeof(MySimpleEventDataHandler));

            });

         
            services.AddSingleton<IEventHandlerFactory, IocEventHandlerFactory>();
            services.AddSingleton<IEventHandlerInvoker, EventHandlerInvoker>();
            services.AddSingleton<ILocalEventBus, LocalEventBus>();

        }
        protected override void SetAbpApplicationCreationOptions(HDTApplicationCreationOptions options)
        {
            options.UseAutofac();
            options.Services.AddSingleton<MySimpleEventDataHandler>();
        }
    }

}
