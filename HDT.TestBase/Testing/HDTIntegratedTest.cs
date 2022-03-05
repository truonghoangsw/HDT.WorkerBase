using Autofac;
using HDT.Core;
using HDT.Core.Autofac;
using HDT.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDT.TestBase.Testing
{
    public class HDTIntegratedTest : HDTTestBaseWithServiceProvider, IDisposable
    {
        protected IServiceProvider RootServiceProvider { get; }
        protected IServiceScope TestServiceScope { get; }
        protected virtual IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }
        protected HDTIntegratedTest()
        {
            var services = CreateServiceCollection();
            BeforeAddApplication(services);
            var options = new HDTApplicationCreationOptions(services);
            SetAbpApplicationCreationOptions(options);
            RootServiceProvider = CreateServiceProvider(services);
            TestServiceScope = RootServiceProvider.CreateScope();
            ServiceProvider = TestServiceScope.ServiceProvider;

        }
        protected virtual void SetAbpApplicationCreationOptions(HDTApplicationCreationOptions options)
        {

        }

        protected virtual void BeforeAddApplication(IServiceCollection services)
        {
           
        }
        protected virtual IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            return services.BuildServiceProviderFromFactory();
        }
        public virtual void Dispose()
        {
            TestServiceScope.Dispose();
        }
    }
}
