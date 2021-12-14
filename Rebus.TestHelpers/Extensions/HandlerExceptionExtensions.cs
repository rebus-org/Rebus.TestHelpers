using System;
using System.Collections.Generic;
using System.Linq;

namespace Rebus.TestHelpers.Extensions;

/// <summary>
/// Extensions
/// </summary>
public static class HandlerExceptionExtensions
{
    /// <summary>
    /// Throws Exception if the collection of <see cref="HandlerException"/> is not empty.
    /// </summary>
    /// <exception cref="AggregateException">Containing a list of exceptions inside <see cref="HandlerException"/></exception>
    public static void ThrowIfNotEmpty(this IEnumerable<HandlerException> exceptions)
    {
        if (exceptions == null) throw new ArgumentNullException(nameof(exceptions));
        
        var list = exceptions.ToList();

        if (!list.Any()) return;
        
        throw new AggregateException(list.Select(ex => ex.Exception));
    }
}