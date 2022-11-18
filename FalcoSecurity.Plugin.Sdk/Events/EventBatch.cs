using System.Runtime.InteropServices;

namespace FalcoSecurity.Plugin.Sdk.Events
{
    public unsafe class EventBatch : IEventBatch
    {
        private int _size;

        private readonly int _dataSize;

        private readonly PluginEvent* _eventsPtr;

        private bool _disposed = false;

        public EventBatch(int size, int dataSize)
        {
            _size = size;

            _dataSize = dataSize;

            _eventsPtr = (PluginEvent*)NativeMemory.Alloc(
                elementCount: (nuint)_size,
                elementSize: (nuint)sizeof(PluginEvent)
            );

            for (var i = 0; i < _size; i++)
            {
                var evt = &_eventsPtr[i];
                evt->DataLen = 0;
                evt->Data = null;
            }
        }

        public int Length => _size;

        public void* UnderlyingArray => _eventsPtr;

        public void Dispose()
        {
            for (var i = 0; i < _size; i++)
            {
                NativeMemory.Free(_eventsPtr[i].Data);
            }

            NativeMemory.Free(_eventsPtr);
            
            _size = 0;
            _disposed = true;

            GC.SuppressFinalize(this);
        }

        public IEventWriter Get(int eventIndex)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(EventBatch));
            }

            if (eventIndex >= _size)
            {
                throw new IndexOutOfRangeException($"{eventIndex} is greater or equal than {_size}");
            }

            return new EventWriter(
                &_eventsPtr[eventIndex], 
                (uint) _dataSize);
        }
    }
}
