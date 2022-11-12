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

namespace FalcoSecurity.Plugin.Sdk.Generators
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
            var hasConfig = false;

            foreach (var iface in pluginClassSymbol.AllInterfaces)
            {
                switch (iface.Name)
                {
                    case "IEventSource":
                        hasEventSourceCapability = true;
                        break;
                    case "IFieldExtractor":
                        hasFieldExtractionCapability = true;
                        break;
                    case "IConfigurable":
                        hasConfig = true;
                        break;
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

            var template = Helpers.GetEmbeddedContent("PluginNativeExports.cssbn");

            var source = template
                .Replace("NAMESPACE_PLACEHOLDER", fullNamespace);

            source = source
                .Replace("CLASSNAME_PLACEHOLDER", className)
                .Replace("HAS_CONFIG", hasConfig ? "true": "false")
                .Replace("HAS_EVENT_SOURCE_CAP", hasEventSourceCapability ? "true" : "false")
                .Replace("HAS_FIELD_EXTRACT_CAP", hasFieldExtractionCapability ? "true" : "false");

            if (hasConfig == false)
            {
                source = source
                    .Replace("// BEGIN_PLUGIN_CONFIG", "/*")
                    .Replace("// END_PLUGIN_CONFIG", "*/");
            }

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
    }
}
