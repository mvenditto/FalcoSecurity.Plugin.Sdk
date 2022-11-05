namespace Falco.Plugin.Sdk.Events
{
    public interface IEventSource
    {
        string EventSourceName { get; }

        IEnumerable<OpenParam> OpenParameters { get; }

        IEventSourceInstance Open(IEnumerable<OpenParam>? openParams);

        void Close(IEventSourceInstance instance);
    }
}
