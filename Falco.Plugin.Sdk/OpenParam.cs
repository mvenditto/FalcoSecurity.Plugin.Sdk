using System.Text.Json.Serialization;

namespace Falco.Plugin.Sdk
{
    public record OpenParam
    {
        public OpenParam(string value, string desc, string? separator=null)
        {
            Value = value;
            Desc = desc;
            Separator = separator;
        }

        [JsonPropertyName("value")]
        public string Value { get; init; }

        [JsonPropertyName("desc")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Desc { get; init; } = null;

        [JsonPropertyName("separator")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Separator { get; init; } = null;
    }
}
