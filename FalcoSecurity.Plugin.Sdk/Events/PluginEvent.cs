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

        public IntPtr Data { get; set; }

        public uint DataLen { get; set; }

        public ulong Timestamp { get; set; }

        public bool Equals(PluginEvent other)
        {
            return other.EventNum == EventNum;
        }
    }
}
