using System.Text.Json.Serialization;

namespace FalcoSecurity.Plugin.Sdk
{
    public record FieldArg
    {
        [JsonPropertyName("isRequired")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsRequired { get; init; } = null;

        [JsonPropertyName("isIndex")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsIndex { get; init; } = null;

        [JsonPropertyName("isKey")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsKey { get; init; } = null;
    }
}
