using Newtonsoft.Json.Schema.Generation;
using System;
using System.Text.Json;

namespace Falco.Plugin.Sdk
{
    public abstract class ConfigurablePlugin<T>: IConfigurable<T>
    {
        public T? Config { get; set; }

        virtual public PluginSchemaType ConfigSchemaType => PluginSchemaType.None;

        public bool TryGenerateJsonSchema(out string? jsonSchema)
        {
            jsonSchema = string.Empty;

            if (ConfigSchemaType != PluginSchemaType.Json)
            {
                return false;
            }

            var generator = new JSchemaGenerator();

            jsonSchema = generator.Generate(typeof(T)).ToString();

            return string.IsNullOrEmpty(jsonSchema) == false;
        }
    }
}
