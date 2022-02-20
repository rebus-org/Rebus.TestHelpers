using Rebus.Sagas;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rebus.TestHelpers
{
    public interface ISagaSerializer
    {
        /// <summary>
        /// Serializes the given ISagaData object into a string
        /// </summary>
        string SerializeToString(ISagaData obj);

        /// <summary>
        /// Deserializes the given string and type into a ISagaData object
        /// </summary>
        ISagaData DeserializeFromString(Type type, string str);
    }
}
