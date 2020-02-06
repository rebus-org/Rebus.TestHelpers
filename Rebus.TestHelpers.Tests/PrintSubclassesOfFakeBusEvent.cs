using System;
using System.Linq;
using NUnit.Framework;
using Rebus.TestHelpers.Events;

namespace Rebus.TestHelpers.Tests
{
    [TestFixture]
    public class PrintSubclassesOfFakeBusEvent
    {
        [Test]
        public void DoIt()
        {
            var types = typeof(FakeBusEvent).Assembly.GetTypes()
                .Where(type => typeof(FakeBusEvent).IsAssignableFrom(type));

            Console.WriteLine(string.Join(Environment.NewLine, types.Select(FormatName)));
        }

        static string FormatName(Type type)
        {
            var name = type.Name;

            if (name.Contains('`'))
            {
                var pureName = name.Substring(0, name.IndexOf('`'));

                return $"  * {pureName}<TMessage>";
            }

            return $"* {name}";
        }
    }
}