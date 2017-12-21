#if NETSTANDARD1_3
using System;
using System.Reflection;

namespace Rebus.TestHelpers.Internals
{
    static class Shims
    {
        public static PropertyInfo GetProperty(this Type type, string name) => type.GetTypeInfo().GetDeclaredProperty(name);
    }
}
#endif
