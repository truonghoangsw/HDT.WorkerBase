using System;
using System.Collections.Generic;
using System.Text;

namespace HDT.Core.Extensions
{
    public static class AbpObjectExtensions
    {
        public static T As<T>(this object obj) where T : class
        {
            return (T)obj;
        }
    }
}
