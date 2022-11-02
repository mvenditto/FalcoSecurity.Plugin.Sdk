using System.Runtime.InteropServices;

namespace Falco.Plugin.Sdk.Events
{
    public readonly unsafe struct EventWriter : IEventWriter
    {
        private readonly PluginEvent* _event;

        private readonly uint _dataSize;

        public EventWriter(PluginEvent* pluginEvent, uint dataSize)
        {
            _dataSize = dataSize;
            _event = pluginEvent;
            _event->TimeStamp = ulong.MaxValue;
            _event->Data = Marshal.AllocHGlobal((int) dataSize);
            _event->DataLen = 0;

        }

        public void SetTimestamp(ulong timestamp)
        {
            _event->TimeStamp = timestamp;
        }

        public void Write(ReadOnlySpan<byte> bytes)
        {
            var span = new Span<byte>(
                (void*)(_event->Data + (int)_event->DataLen),
                (int)_dataSize);

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
