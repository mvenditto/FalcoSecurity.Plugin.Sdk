namespace Falco.Plugin.Sdk.Fields
{
    public interface IExtractionRequest : IDisposable
    {
        void SetPtr(IntPtr ptr);

        ulong FieldId { get; }

        PluginFieldType FieldType { get; }

        string? FieldName { get; }

        string? ArgKey { get; }

        ulong ArgIdx { get; }

        bool ArgPresent { get; }

        bool IsList { get; }

        void SetValue(string str);

        void SetValue(ReadOnlySpan<string> strBuff);

        void SetValue(ulong u64);

        void SetValue(ReadOnlySpan<ulong> u64Buff);

        void Free();
    }
}
