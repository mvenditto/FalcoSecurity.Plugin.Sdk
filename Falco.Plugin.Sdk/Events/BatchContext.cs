namespace Falco.Plugin.Sdk.Events
{
    /// <summary>
    /// Contains data tied to an EventSourceInstace
    /// </summary>
    /// <remarks>
    /// the data has the scope of the current 
    /// batch and gets cleared by the framework
    /// after the relevant information has been consumed
    /// </remarks>
    public class EventSourceInstanceContext
    {
        public uint BatchEventsNum { get; set; } = 0;

        public bool IsEof { get; set; } = false;

        public bool HasTimeout { get; set; } = false;

        public bool HasFailure { get; set; } = false;

        public string? Error { get; set; }

        public void Reset()
        {
            BatchEventsNum = 0;
            IsEof = false;
            HasTimeout = false;
            HasFailure = false;
            Error = null;
        }
    }
}
