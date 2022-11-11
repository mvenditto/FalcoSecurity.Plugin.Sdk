using System.Text.Json.Serialization;

namespace FalcoSecurity.Plugin.Sdk
{
    public record ExtractionField
    {
        public ExtractionField(string type, string name, string desc, string display="")
        {
            Type = type;
            Name = name;
            Description = desc;
            Display = display;
        }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("type")]
        public string Type { get; init; }

        [JsonPropertyName("isList")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsList { get; init; } = null;


        [JsonPropertyName("display")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Display { get; init; } = null;


        [JsonPropertyName("desc")]
        public string Description { get; init; }

    }
}
