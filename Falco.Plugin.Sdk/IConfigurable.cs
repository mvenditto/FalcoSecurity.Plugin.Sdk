namespace Falco.Plugin.Sdk
{
    public interface IConfigurable<T>
    {
        public T? Config { get; set; }

        public PluginSchemaType ConfigSchemaType => PluginSchemaType.None;

    }
}
