namespace Falco.Plugin.Sdk.Events
{
    public interface IEventWriter
    {
        void Write(ReadOnlySpan<byte> bytes);

        void SetTimestamp(ulong timestamp);

        void Free();
    }
}
