using System.Runtime.InteropServices;

namespace Falco.Plugin.Sdk.Fields
{
    /// <summary>
    /// NOTE: This is just an replica of the anonymous union nested inside
    /// ss_plugin_extract_field. The only difference is that each union field has
    /// one pointer level less than its equivalent of ss_plugin_extract_field.
    /// Keep this in sync with plugin_types.h in case new types will be supported.
    /// <example>
    /// typedef union
    /// {
    ///     const char* str;
    ///     uint64_t u64;
    /// } field_result_t;
    /// </example>
    [StructLayout(LayoutKind.Explicit)]
    public struct FieldResult
    {
        [FieldOffset(0)]
        public IntPtr String;   // char*

        [FieldOffset(0)]
        public ulong Unsigned64; // uint64_t
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe public struct PluginExtractField
    {
        /// <remarks>
        /// To be filled in by the extraction function
        /// </remarks>
        public FieldResult* Result { get; set; }

        /// <remarks>
        /// To be filled in by the extraction function
        /// </remarks>
        public ulong ResultLen { get; set; }

        public uint FieldId { get; set; }

        public IntPtr Field { get; set; }  // char*

        public IntPtr ArgKey { get; set; } // char*

        public ulong ArgIdx { get; set; }

        public byte ArgPresent { get; set; }

        public uint FieldType { get; set; }

        public byte FieldList { get; set; }
    }
}
