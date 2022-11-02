namespace Falco.Plugin.Sdk.Events
{
    public struct BaseEventSourceInstance: IEventSourceInstance
    {
        public IEventPool EventPool { get; init; }

        public object? State { get; set;}

        public void Dispose()
        {
            EventPool?.Dispose();
        }

        public string GetReadProgress(out uint progress)
        {
            progress = 0;
            return "0%";
        }

        public int NextBatch()
        {
            return 0;
        }
    }
}
