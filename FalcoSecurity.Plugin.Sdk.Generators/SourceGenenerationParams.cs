namespace FalcoSecurity.Plugin.Sdk.Generators
{
    internal class SourceGenenerationParams
    {
        public string ClassName { get; set; }

        public string Namespace { get; set; }

        public bool HasConfiguration { get; set; }

        public bool HasFieldExtractionCapability { get; set; } 

        public bool HasEventSourcingCapability { get; set; }
    }
}
