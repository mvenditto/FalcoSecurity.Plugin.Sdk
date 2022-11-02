using System;
using System.Drawing;
using System.Reflection;

namespace Falco.Plugin.Sdk.Events
{
    public interface IEventReader
    {
        ulong EventNum { get; }

        ulong Timestamp { get; }

        ReadOnlySpan<byte> Data { get; }
    }
}
