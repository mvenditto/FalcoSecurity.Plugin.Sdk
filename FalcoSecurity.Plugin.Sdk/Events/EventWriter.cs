using System.Runtime.InteropServices;

namespace FalcoSecurity.Plugin.Sdk.Events
{
    public unsafe class EventWriter : IEventWriter
    {
        private readonly PluginEvent* _event;

        private readonly uint _dataSize;

        public PluginEvent* UnderlyingEvent => _event;

        public EventWriter(PluginEvent* pluginEvent, uint dataSize)
        {
            _dataSize = dataSize;
            _event = pluginEvent;
            _event->Timestamp = ulong.MaxValue;
            _event->Data = NativeMemory.Alloc(dataSize);
            _event->DataLen = 0;
        }

        public void SetTimestamp(ulong timestamp)
        {
            _event->Timestamp = timestamp;
        }

        public void Write(ReadOnlySpan<byte> bytes)
        {
            var offset = (nuint)_event->Data + _event->DataLen;

            var span = new Span<byte>((void*) offset, bytes.Length);

            bytes.CopyTo(span);

            _event->DataLen += (uint) bytes.Length;
        }

        public void Free()
        {
            NativeMemory.Free(_event->Data);
            _event->Data = null;
            _event->DataLen = 0;
        }

    }
}
