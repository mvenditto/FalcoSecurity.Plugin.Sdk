using Falco.Plugin.Sdk.Events;

namespace Falco.Plugin.Sdk.Fields
{
    public interface IFieldExtractor
    {
        IList<ExtractionField> Fields { get; }

        IList<string> EventSourcesToExtract { get; }

        void Extract(IExtractionRequest extraction, IEventReader evt);
    }
}
