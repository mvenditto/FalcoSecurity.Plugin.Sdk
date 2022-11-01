using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Falco.Plugin.Sdk
{
    /// <summary>
    /// ss_plugin_event 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PluginEvent
    {
        /// <summary>
        /// eventnum
        /// </summary>
        public ulong EventNum { get; set; }

        /// <summary>
        /// data
        /// </summary>
        public IntPtr Data { get; set; }

        /// <summary>
        /// datlen
        /// </summary>
        public uint DataLen { get; set; }

        /// <summary>
        /// ts
        /// </summary>
        public ulong TimeStamp { get; set; }
    }
}
