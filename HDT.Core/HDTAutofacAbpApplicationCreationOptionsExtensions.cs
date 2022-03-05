using Autofac;
using HDT.Core.Autofac;
using HDT.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HDT.Core
{
    public static class HDTAutofacAbpApplicationCreationOptionsExtensions
    {
        public static void UseAutofac(this HDTApplicationCreationOptions options)
        {
            options.Services.AddAutofacServiceProviderFactory();
        }

        public static HDTAutofacServiceProviderFactory AddAutofacServiceProviderFactory(this IServiceCollection services)
        {
            return services.AddAutofacServiceProviderFactory(new ContainerBuilder());
        }

        public static HDTAutofacServiceProviderFactory AddAutofacServiceProviderFactory(this IServiceCollection services, ContainerBuilder containerBuilder)
        {
            var factory = new HDTAutofacServiceProviderFactory(containerBuilder);

            services.AddObjectAccessor(containerBuilder);
            services.AddSingleton((IServiceProviderFactory<ContainerBuilder>)factory);

            return factory;
        }
    }
}
