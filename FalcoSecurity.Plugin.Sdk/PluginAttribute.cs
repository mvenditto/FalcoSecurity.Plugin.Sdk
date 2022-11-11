namespace FalcoSecurity.Plugin.Sdk
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FalcoPluginAttribute: Attribute
    {
        public uint Id { get; init; }

        public string Version { get; init; }

        public string RequiredApiVersion { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public string Contacts { get; init; }
    }
}
