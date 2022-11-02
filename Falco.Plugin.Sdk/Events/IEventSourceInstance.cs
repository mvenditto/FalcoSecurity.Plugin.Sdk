namespace Falco.Plugin.Sdk.Events
{
    public interface IEventSourceInstance: IDisposable
    {
        IEventPool EventPool { get; init; }

        object? State { get; set; }

        int NextBatch();

        string GetReadProgress(out uint progress);
    }
}
