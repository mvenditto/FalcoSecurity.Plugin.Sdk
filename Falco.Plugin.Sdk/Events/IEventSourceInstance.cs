namespace Falco.Plugin.Sdk.Events
{
    public interface IEventSourceInstance: IDisposable
    {
        IEventBatch EventBatch { get; init; }

        EventSourceInstanceContext NextBatch();

        string GetReadProgress(out uint progress);
    }
}
