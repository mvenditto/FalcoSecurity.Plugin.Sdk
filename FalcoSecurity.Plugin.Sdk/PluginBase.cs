using FalcoSecurity.Plugin.Sdk.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
using System.Text.Json;

namespace FalcoSecurity.Plugin.Sdk
{
    public abstract class PluginBase: IPlugin
    {
        virtual public PluginSchemaType ConfigSchemaType => PluginSchemaType.None;

        public string? ConfigRaw { get; set; }

        public string? LastError { get; set; }

        virtual public void Init()
        {

        }

        virtual public void Destroy()
        {

        }
    }

    public abstract class PluginBase<T>: PluginBase, IConfigurable<T>
    {
        public T? Config { get; set; }

        public override PluginSchemaType ConfigSchemaType => PluginSchemaType.Json;

        public override void Init()
        {
            base.Init();

            if (!string.IsNullOrEmpty(ConfigRaw))
            {
                try
                {
                    Config = System.Text.Json.JsonSerializer.Deserialize<T>(ConfigRaw);
                }
                catch (System.Text.Json.JsonException jex)
                {
                    Console.Error.WriteLine($"Cannot parse plugin config: {jex}");
                }
            }
        }

        public bool TryGenerateJsonSchema(out string? jsonSchema)
        {
            jsonSchema = string.Empty;

            if (ConfigSchemaType != PluginSchemaType.Json)
            {
                return false;
            }

            var generator = new JSchemaGenerator();
            generator.ContractResolver = new CamelCasePropertyNamesContractResolver();

            jsonSchema = generator.Generate(typeof(T)).ToString();

            return string.IsNullOrEmpty(jsonSchema) == false;
        }
    }
}
