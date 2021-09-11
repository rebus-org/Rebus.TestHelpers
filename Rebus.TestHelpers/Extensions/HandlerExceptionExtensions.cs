using System;
using System.Collections.Generic;
using System.Linq;

namespace Rebus.TestHelpers.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class HandlerExceptionExtensions
    {
        /// <summary>
        /// Throws Exception if the collection of <see cref="HandlerException"/> is not empty.
        /// </summary>
        /// <param name="exceptions"></param>
        /// <exception cref="AggregateException">Containing a list of exceptions inside <see cref="HandlerException"/></exception>
        public static void ThrowIfNotEmpty(this IEnumerable<HandlerException> exceptions)
        {
            if (exceptions.Any())
            {
                throw new AggregateException(exceptions.Select(ex => ex.Exception));
            }
        }
    }
}
