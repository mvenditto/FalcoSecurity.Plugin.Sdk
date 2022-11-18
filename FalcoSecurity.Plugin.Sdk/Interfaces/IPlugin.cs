namespace FalcoSecurity.Plugin.Sdk
{
    public interface IPlugin
    {
        string? LastError { get; set; }

        string? ConfigRaw { get; set; }

        void Init();

        void Destroy();
    }
}
