using System.Runtime.InteropServices;

namespace Falco.Plugin.Sdk.Events
{
    /// <summary>
    /// ss_plugin_event 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PluginEvent
    {
        public ulong EventNum { get; set; }

        public IntPtr Data { get; set; }

        public uint DataLen { get; set; }

        public ulong TimeStamp { get; set; }
    }
}
