namespace Falco.Plugin.Sdk.Events
{
    public unsafe class EventReader : IEventReader
    {
        private readonly PluginEvent* _pluginEvent;

        public EventReader(PluginEvent* pluginEvent)
        {
            _pluginEvent = pluginEvent;
        }

        public ulong EventNum => _pluginEvent->EventNum;

        public ulong Timestamp => _pluginEvent->Timestamp;

        public ReadOnlySpan<byte> Data => new(
            (void*) _pluginEvent->Data, 
            (int) _pluginEvent->DataLen);
    }
}
