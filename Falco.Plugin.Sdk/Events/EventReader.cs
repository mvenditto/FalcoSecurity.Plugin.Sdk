namespace Falco.Plugin.Sdk.Events
{
    public readonly unsafe struct EventReader : IEventReader
    {
        private readonly PluginEvent* _pluginEvent;

        public EventReader(PluginEvent* pluginEvent)
        {
            _pluginEvent = pluginEvent;
        }

        public ulong EventNum => 0;

        public ulong Timestamp => _pluginEvent->TimeStamp;

        public ReadOnlySpan<byte> Data => new(
            (void*) _pluginEvent->Data, 
            (int) _pluginEvent->DataLen);
    }
}
