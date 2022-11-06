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
    public record EventSourceInstanceContext
    {
        public uint BatchEventsNum { get; set; }

        public bool IsEof { get; set; }

        public bool HasTimeout { get; set; } 

        public bool HasFailure { get; set; }

        public string? Error { get; set; }

        public static readonly EventSourceInstanceContext Eof = new()
        {
            BatchEventsNum = 0,
            IsEof = true,
            HasFailure = false,
            HasTimeout = false
        };

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
