using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EfficientUnionGenerator.GeneratorBenchmark;

using static Constants;

internal class NegativeControlGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static cxt =>
        {
            cxt.AddSource(
                $"{AttributeNamespace}.{EfficientUnionAttributeName}.g.cs",
                EfficientUnionAttributeSource);
        });

        var sourceAttr = context.CompilationProvider
            .Select(static (compilation, token) => compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.UnionAttribute") is { });
        context.RegisterSourceOutput(sourceAttr, (context, isAvailable) =>
        {
            if (!isAvailable)
            {
                context.AddSource(
                    $"{AttributeNamespace}.UnionAttribute.g.cs",
                    CompilerServicesUnionAttributeSource);
            }
        });

        var sourceInterface = context.CompilationProvider
            .Select(static (compilation, token) => compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.IUnion") is { });
        context.RegisterSourceOutput(sourceInterface, (context, isAvailable) =>
        {
            if (!isAvailable)
            {
                context.AddSource(
                    $"{AttributeNamespace}.IUnion.g.cs",
                    CompilerServicesIUnionSource);
            }
        });

        var sourceUnion = context.SyntaxProvider.ForAttributeWithMetadataName(
                $"{AttributeNamespace}.{EfficientUnionAttributeName}",
                static (node, token) => node is ClassDeclarationSyntax or StructDeclarationSyntax,
                static (context, token) => context)
            .Where(static _ => false);
        context.RegisterSourceOutput(sourceUnion, EmitUnionType);
    }

    private void EmitUnionType(SourceProductionContext context, GeneratorAttributeSyntaxContext source)
    {
    }
}
