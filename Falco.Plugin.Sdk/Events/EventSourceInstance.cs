namespace Falco.Plugin.Sdk.Events
{
    public abstract class BaseEventSourceInstance: IEventSourceInstance
    {
        public IEventBatch EventBatch { get; init; }

        public long TimeoutMs { get; set; } = 
            EventSourceConsts.DefaultInstanceTimeoutMs;

        public BaseEventSourceInstance(int batchSize, int eventSize)
        {
            EventBatch = new EventBatch(batchSize, eventSize);
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
            EventBatch.Dispose();
        }

        virtual public string GetReadProgress(out uint progress) 
        { 
            progress = 0; 
            return string.Empty; 
        }

        abstract public EventSourceInstanceContext NextBatch();
    }
}
