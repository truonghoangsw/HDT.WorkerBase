using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDT.TestBase
{
    public abstract class HDTTestBaseWithServiceProvider
    {
        protected IServiceProvider ServiceProvider { get; set; }
        protected virtual T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }
        protected virtual T GetRequiredService<T>()
        {
            return ServiceProvider.GetRequiredService<T>();
        }

    }
}
