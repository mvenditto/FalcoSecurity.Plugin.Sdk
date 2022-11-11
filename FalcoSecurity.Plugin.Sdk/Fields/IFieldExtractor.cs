using FalcoSecurity.Plugin.Sdk.Events;

namespace FalcoSecurity.Plugin.Sdk.Fields
{
    public interface IFieldExtractor
    {
        IEnumerable<ExtractionField> Fields { get; }

        IEnumerable<string> EventSourcesToExtract { get; }

        void Extract(IExtractionRequest extraction, IEventReader evt);
    }
}
