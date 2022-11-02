namespace Falco.Plugin.Sdk
{
    public interface IPlugin
    {
        string? LastError { get; set; }

        void Init();

        void Destroy();
    }
}
