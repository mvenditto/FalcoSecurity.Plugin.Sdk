namespace Falco.Plugin.Sdk
{
    public interface IEventSource
    {
        public string EventSourceName { get; }

        public IList<string> EventSourcesToConsume { get; }

        public IList<OpenParam> OpenParameters { get; }
    }
}
