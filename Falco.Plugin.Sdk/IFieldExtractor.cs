namespace Falco.Plugin.Sdk
{
    public interface IFieldExtractor
    {
        public IList<ExtractionField> ExtractFields { get; init; }
    }
}
