﻿using HDT.Core.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.ExceptionServices;


namespace HDT.Core.Exceptions
{
    public static class HDTExceptionExtensions
    {
        /// <summary>
        /// Uses <see cref="ExceptionDispatchInfo.Capture"/> method to re-throws exception
        /// while preserving stack trace.
        /// </summary>
        /// <param name="exception">Exception to be re-thrown</param>
        public static void ReThrow(this Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }

        /// <summary>
        /// Try to get a log level from the given <paramref name="exception"/>
        /// if it implements the <see cref="IHasLogLevel"/> interface.
        /// Otherwise, returns the <paramref name="defaultLevel"/>.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="defaultLevel"></param>
        /// <returns></returns>
        public static LogLevel GetLogLevel(this Exception exception, LogLevel defaultLevel = LogLevel.Error)
        {
            return (exception as IHasLogLevel)?.LogLevel ?? defaultLevel;
        }
    }
}
