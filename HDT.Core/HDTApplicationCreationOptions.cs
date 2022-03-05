using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HDT.Core
{
    public class HDTApplicationCreationOptions
    {
        [NotNull]
        public IServiceCollection Services { get; }

      
        public bool SkipConfigureServices { get; set; }

        public HDTApplicationCreationOptions([NotNull] IServiceCollection services)
        {
            Services = services;
        }
    }

}
