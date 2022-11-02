namespace Falco.Plugin.Sdk.Events
{
    public interface IEventPool : IDisposable
    {
        IEventWriter Get(int eventIndex);

        int Length { get; }

        IntPtr UnderlyingArray { get; }
    }
}
