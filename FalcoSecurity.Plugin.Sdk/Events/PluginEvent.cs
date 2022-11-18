using System.Runtime.InteropServices;

namespace FalcoSecurity.Plugin.Sdk.Events
{
    /// <summary>
    /// ss_plugin_event 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PluginEvent: IEquatable<PluginEvent>
    {
        public ulong EventNum { get; set; }

        public unsafe void* Data { get; set; }

        public uint DataLen { get; set; }

        unsafe public PluginEvent()
        {
            EventNum = 0;
            Data = null;
            DataLen = 0;
            Timestamp = ulong.MaxValue;
        }

        public ulong Timestamp { get; set; }

        public bool Equals(PluginEvent other)
        {
            return other.EventNum == EventNum;
        }

        public override bool Equals(object? obj)
        {
            return obj is PluginEvent @event && Equals(@event);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EventNum);
        }

        public static bool operator ==(PluginEvent left, PluginEvent right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PluginEvent left, PluginEvent right)
        {
            return !(left == right);
        }

    }
}
