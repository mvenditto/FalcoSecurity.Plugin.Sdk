namespace Falco.Plugin.Sdk.Events
{
    public interface IEventBatch : IDisposable
    {
        IEventWriter Get(int eventIndex);

        int Length { get; }

        IntPtr UnderlyingArray { get; }
    }
}
