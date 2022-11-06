using System.Runtime.InteropServices;

namespace Falco.Plugin.Sdk.Events
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
            _event->Data = Marshal.AllocHGlobal((int) dataSize);
            _event->DataLen = 0;
        }

        public void SetTimestamp(ulong timestamp)
        {
            _event->Timestamp = timestamp;
        }

        public void Write(ReadOnlySpan<byte> bytes)
        {
            var offset = (void*)(_event->Data + (int)_event->DataLen);

            var span = new Span<byte>(offset, bytes.Length);

            bytes.CopyTo(span);

            _event->DataLen += (uint) bytes.Length;
        }

        public void Free()
        {
            Marshal.FreeHGlobal(_event->Data);
            _event->Data = IntPtr.Zero;
            _event->DataLen = 0;
        }

    }
}
