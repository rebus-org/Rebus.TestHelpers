using System;
using System.Collections.Generic;

namespace Rebus.TestHelpers.Events;

/// <summary>
/// Event emitted when the transport message currently being handled is forwarded to another destination address
/// </summary>
public class TransportMessageForwarded : FakeBusEvent
{
    /// <summary>
    /// Gets the destination address to which the message was sent
    /// </summary>
    public string DestinationAddress { get; }

    /// <summary>
    /// Gets the optional headers if they were supplied, or null if they weren't
    /// </summary>
    public Dictionary<string, string> OptionalHeaders { get; }

    internal TransportMessageForwarded(string destinationAddress, Dictionary<string, string> optionalHeaders, DateTimeOffset time) : base(time)
    {
        OptionalHeaders = optionalHeaders;
        DestinationAddress = destinationAddress ?? throw new ArgumentNullException(nameof(destinationAddress));
    }
}