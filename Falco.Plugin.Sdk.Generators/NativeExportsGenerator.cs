using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace Falco.Plugin.Sdk.Generators
{
    [Generator]
    public class NativeExportsGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var tree = context.Compilation.SyntaxTrees.Where(
                st => st.GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Any(p => p.DescendantNodes().OfType<AttributeSyntax>().Any()))
                .FirstOrDefault();

            if (tree == null) return;

            var semanticModel = context.Compilation.GetSemanticModel(tree);

            var pluginClass = tree
                .GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(cd => cd.DescendantNodes()
                    .OfType<AttributeSyntax>()
                    .Any(asy => semanticModel.GetTypeInfo(
                        asy, context.CancellationToken).Type?.Name == "FalcoPluginAttribute"))
                .FirstOrDefault();

            var attribute = pluginClass
                .DescendantNodes()
                .OfType<AttributeSyntax>()
                .FirstOrDefault(asy => semanticModel.GetTypeInfo(
                    asy, context.CancellationToken).Type?.Name == "FalcoPluginAttribute");

            if (pluginClass == null) return;

            var pluginClassSymbol = semanticModel.GetDeclaredSymbol(pluginClass);

            if (pluginClassSymbol == null) return;

            var hasEventSourceCapability = false;
            var hasFieldExtractionCapability = false;

            foreach (var iface in pluginClassSymbol.Interfaces)
            {
                if (iface.Name == "IEventSource")
                {
                    hasEventSourceCapability = true;
                }

                if (iface.Name == "IFieldExtractor")
                {
                    hasFieldExtractionCapability = true;
                }
            }

            var className = pluginClassSymbol.Name;

            var ns = pluginClassSymbol.ContainingNamespace;

            var namespaces = new List<string>();
            do
            {
                namespaces.Insert(0, ns.Name);
                ns = ns?.ContainingNamespace;
            }
            while (ns != null);

            var fullNamespace = namespaces.Aggregate((a, b) => $"{a}.{b}");

            Debug.WriteLine($"hasEventSourceCapability={hasEventSourceCapability}\nhasFieldExtractionCapability={hasFieldExtractionCapability}");
       
            Debug.WriteLine($"Generating '{className}NativeExports.g.cs' under '{fullNamespace}' namespace");

            if (fullNamespace.StartsWith("."))
            {
                fullNamespace = fullNamespace.Substring(1);
            }

            var source = _nativeExportsTemplate
                .Replace("NAMESPACE_PLACEHOLDER", fullNamespace);

            source = source
                .Replace("CLASSNAME_PLACEHOLDER", className)
                .Replace("HAS_EVENT_SOURCE_CAP", hasEventSourceCapability ? "true" : "false")
                .Replace("HAS_FIELD_EXTRACT_CAP", hasFieldExtractionCapability ? "true" : "false");

            if (hasEventSourceCapability == false)
            {
                source = source
                    .Replace("// BEGIN_PLUGIN_CAP_DATA_SOURCE", "/*")
                    .Replace("// END_PLUGIN_CAP_DATA_SOURCE", "*/");
            }

            if (hasFieldExtractionCapability == false)
            {
                source = source
                    .Replace("// BEGIN_PLUGIN_CAP_FIELD_EXTRACTION", "/*")
                    .Replace("// END_PLUGIN_CAP_FIELD_EXTRACTION", "*/");
            }


            context.AddSource($"{className}NativeExports.g.cs", source);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            #if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
            #endif 
        }

        private const string _nativeExportsTemplate = @"using Falco.Plugin.Sdk;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using DNNE;

namespace NAMESPACE_PLACEHOLDER
{
    using PluginStateOpaquePtr = IntPtr;
    using EventSourceInstanceOpaquePtr = IntPtr;

    public static unsafe class CLASSNAME_PLACEHOLDER_NativeExports
    {
        private static IntPtr _pluginName;

        private static IntPtr _pluginDescription;

        private static IntPtr _pluginRequiredApiVersion;

        private static IntPtr _pluginVersion;

        private static IntPtr _pluginContact;

        private static IntPtr _eventSourceName;

        private static IntPtr _pluginSchema = IntPtr.Zero;

        private static uint _pluginId = 0;

        private static IntPtr _openParamsJsonArray;

        private static CLASSNAME_PLACEHOLDER _plugin = new CLASSNAME_PLACEHOLDER();

        private const bool _pluginHasEventSourceCapability = HAS_EVENT_SOURCE_CAP;

        private const bool _pluginHasFieldExtractCapability = HAS_FIELD_EXTRACT_CAP;

        // must be non-null, or some funcs like list_opem_params will early-exit
        private static IntPtr _pluginState = Marshal.AllocHGlobal(1);

        static CLASSNAME_PLACEHOLDER_NativeExports()
        {
            var pluginInfo = (Falco.Plugin.Sdk.FalcoPluginAttribute) Attribute.GetCustomAttribute(typeof(CLASSNAME_PLACEHOLDER),      
                                   typeof(Falco.Plugin.Sdk.FalcoPluginAttribute));
            _pluginId = pluginInfo.Id;
            _pluginDescription = Marshal.StringToCoTaskMemUTF8(pluginInfo.Description);
            _pluginRequiredApiVersion = Marshal.StringToCoTaskMemUTF8(pluginInfo.RequiredApiVersion);
            _pluginVersion = Marshal.StringToCoTaskMemUTF8(pluginInfo.Version);
            _pluginContact = Marshal.StringToCoTaskMemUTF8(pluginInfo.Contacts);
            _pluginName = Marshal.StringToCoTaskMemUTF8(pluginInfo.Name);
            Console.WriteLine($""EVENT_SOURCE_CAP={_pluginHasEventSourceCapability}"");

            if (_pluginHasEventSourceCapability)
            {
                var eventSource = (Falco.Plugin.Sdk.IEventSource) _plugin;
                _eventSourceName = Marshal.StringToCoTaskMemUTF8(eventSource.EventSourceName);
                var openParams = eventSource.OpenParameters;
                var openParamsJson = JsonSerializer.Serialize(openParams);
                Console.WriteLine($""OPEN_PARAMS='{openParamsJson}'"");
                _openParamsJsonArray = Marshal.StringToCoTaskMemUTF8(openParamsJson);
            }
            else 
            {
               _openParamsJsonArray = Marshal.StringToCoTaskMemUTF8(""[]"");
            } 
        }

        [UnmanagedCallersOnly(EntryPoint = ""plugin_get_required_api_version"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetRequiredApiVersion()
        {
            return _pluginRequiredApiVersion;
        }

        [UnmanagedCallersOnly(EntryPoint = ""plugin_get_init_schema"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetInitSchema(IntPtr schemaType)
        {

            if (_pluginSchema != IntPtr.Zero)
            {
                Marshal.WriteInt32(schemaType, (int)PluginSchemaType.Json);
            }
            else
            {
                Marshal.WriteInt32(schemaType, (int)PluginSchemaType.None);
            }

            return _pluginSchema;
        }

        [UnmanagedCallersOnly(EntryPoint = ""plugin_init"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static PluginStateOpaquePtr Init(IntPtr configString, IntPtr returnCode)
        {
            Marshal.WriteInt32(returnCode, (int)PluginReturnCode.Success);
            return _pluginState;
        }

        [UnmanagedCallersOnly(EntryPoint = ""plugin_destroy"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void Destroy(PluginStateOpaquePtr pluginState)
        {
            Marshal.FreeHGlobal(_pluginRequiredApiVersion);
            Marshal.FreeHGlobal(_pluginSchema);
            Marshal.FreeHGlobal(pluginState);
        }

        [UnmanagedCallersOnly(EntryPoint = ""plugin_get_last_error"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetLastError(PluginStateOpaquePtr pluginState)
        {
            return Marshal.StringToCoTaskMemAnsi(""Test"");
        }

        [UnmanagedCallersOnly(EntryPoint = ""plugin_get_name"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetPluginName()
        {
            return _pluginName;
        }

        [UnmanagedCallersOnly(EntryPoint = ""plugin_get_description"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetPluginDescription()
        {
            return _pluginDescription;
        }

        [UnmanagedCallersOnly(EntryPoint = ""plugin_get_contact"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetPluginAuthorsContact()
        {
            return _pluginContact;
        }

        [UnmanagedCallersOnly(EntryPoint = ""plugin_get_version"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetPluginVersion()
        {
            return _pluginVersion;
        }

        #region Event sourcing capability API

        [UnmanagedCallersOnly(EntryPoint = ""plugin_get_id"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static uint GetId()
        {
            return _pluginId;
        }

        // BEGIN_PLUGIN_CAP_DATA_SOURCE
        [UnmanagedCallersOnly(EntryPoint = ""plugin_get_event_source"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetEventSourceName()
        {
            return _eventSourceName;
        }
        // END_PLUGIN_CAP_DATA_SOURCE

        [UnmanagedCallersOnly(EntryPoint = ""plugin_open"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static EventSourceInstanceOpaquePtr Open(PluginStateOpaquePtr pluginState, IntPtr paramsString, IntPtr returnCode)
        {
            Marshal.WriteInt32(returnCode, (int)PluginReturnCode.Success);
            return IntPtr.Zero;
        }

        [UnmanagedCallersOnly(EntryPoint = ""plugin_close"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void Close(PluginStateOpaquePtr pluginState, EventSourceInstanceOpaquePtr instance)
        {
            Marshal.FreeHGlobal(instance);
        }

        [UnmanagedCallersOnly(EntryPoint = ""plugin_list_open_params"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr ListOpenParams(UIntPtr pluginState, IntPtr returnCode)
        {
            Marshal.WriteInt32(returnCode, (int)PluginReturnCode.Success);
            return _openParamsJsonArray;
        }

        [UnmanagedCallersOnly(EntryPoint = ""plugin_get_progress"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetReadProgress(PluginStateOpaquePtr pluginState, EventSourceInstanceOpaquePtr instance, IntPtr progress)
        {
            Marshal.WriteInt32(progress, 5000);
            return IntPtr.Zero;
        }

        [UnmanagedCallersOnly(EntryPoint = ""plugin_event_to_string"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr EventToString(PluginStateOpaquePtr pluginState, IntPtr pluginEvent)
        {
            return _eventSourceName;
        }

        [UnmanagedCallersOnly(EntryPoint = ""plugin_next_batch"", CallConvs = new[] { typeof(CallConvCdecl) })]

        public static int GetNextEventsBatch(PluginStateOpaquePtr pluginState, EventSourceInstanceOpaquePtr instance, IntPtr numEvents, IntPtr events)
        {
            return (int)PluginReturnCode.Eof;
        }

        #endregion

        #region Field extraction capability API

        // BEGIN_PLUGIN_CAP_FIELD_EXTRACTION
        [UnmanagedCallersOnly(EntryPoint = ""plugin_get_fields"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetFields()
        {
            return Marshal.StringToCoTaskMemUTF8(string.Empty);
           
        }
        // END_PLUGIN_CAP_FIELD_EXTRACTION

        [UnmanagedCallersOnly(EntryPoint = ""plugin_extract_fields"", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static int ExtractFields(PluginStateOpaquePtr pluginState, IntPtr pluginEvent, IntPtr numFields, IntPtr fields)
        {
            return (int)PluginReturnCode.Failure;
        }

        #endregion
    }
}";
    }
}
