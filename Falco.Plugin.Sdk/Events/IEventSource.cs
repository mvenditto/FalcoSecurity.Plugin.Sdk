namespace Falco.Plugin.Sdk.Events
{
    public interface IEventSource
    {
        string EventSourceName { get; }

        IList<OpenParam> OpenParameters { get; }

        IEventSourceInstance Open(IList<OpenParam>? openParams);

        void Close(IEventSourceInstance instance);
    }
}
