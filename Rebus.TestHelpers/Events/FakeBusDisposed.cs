﻿using System;

namespace Rebus.TestHelpers.Events;

/// <summary>
/// Indicates that the bus was disposed
/// </summary>
public class FakeBusDisposed : FakeBusEvent
{
    internal FakeBusDisposed(DateTimeOffset time) : base(time) { }
}