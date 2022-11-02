using System.Runtime.InteropServices;

namespace Falco.Plugin.Sdk.Events
{
    public unsafe class EventPool : IEventPool
    {
        private int _size;

        private readonly int _dataSize;

        private readonly IntPtr _eventsPtr;

        private bool _disposed = false;

        public EventPool(
            int size = EventSourceConsts.DefaultBatchSize,
            int dataSize = EventSourceConsts.DefaultEventSize)
        {
            _size = size;

            _dataSize = dataSize;

            _eventsPtr = Marshal.AllocHGlobal(sizeof(PluginEvent) * size);
        }

        public int Length => _size;

        public IntPtr UnderlyingArray => _eventsPtr;

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            for (var i = 0; i < _size; i++)
            {
                Marshal.FreeHGlobal(((PluginEvent*)_eventsPtr)[i].Data);
            }

            Marshal.FreeHGlobal(_eventsPtr);

            _size = 0;

            _disposed = true;
        }

        public IEventWriter Get(int eventIndex)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(EventPool));
            }

            if (eventIndex >= _size)
            {
                throw new IndexOutOfRangeException($"{eventIndex} is greater or equal than {_size}");
            }

            var evt = (PluginEvent*)_eventsPtr + sizeof(PluginEvent) * eventIndex;

            return new EventWriter(evt, (uint) _dataSize);
        }
    }
}
