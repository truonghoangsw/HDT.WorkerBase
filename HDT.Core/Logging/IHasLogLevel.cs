using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace HDT.Core.Logging
{
    public interface IHasLogLevel
    {
        /// <summary>
        /// Log severity.
        /// </summary>
        LogLevel LogLevel { get; set; }
    }
}
