using System;
using System.Linq;
using System.Reflection;
using Rebus.Bus;
using Rebus.DataBus;

namespace Rebus.TestHelpers.Internals
{
    static class TestBackdoor
    {
        public static void EnableTestMode(IDataBusStorage dataBusStorage) => Invoke(nameof(EnableTestMode), dataBusStorage);

        public static void Reset() => Invoke(nameof(Reset));

        public static IDataBusStorage TestDataBusStorage
        {
            get => (IDataBusStorage)InvokeGet(nameof(TestDataBusStorage));
            set => InvokeSet(nameof(TestDataBusStorage), value);
        }

        static object InvokeGet(string name) => Invoke($"get_{name}");

        static void InvokeSet(string name, object value) => Invoke($"set_{name}", value);

        static object Invoke(string methodName, params object[] args)
        {
            const string className = nameof(TestBackdoor);
            var assembly = typeof(IBus).Assembly;

            var type = assembly.GetTypes().FirstOrDefault(t => t.Name == className)
                       ?? throw new ArgumentException($"Could not find type named '{className}' in assembly {assembly}");

            var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public)
                ?? throw new ArgumentException($"Could not find method named '{methodName}' on class {type}");

            return method.Invoke(null, args);
        }
    }
}