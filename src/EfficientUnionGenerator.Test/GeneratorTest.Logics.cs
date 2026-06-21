using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EfficientUnionGenerator.Test;

public partial class GeneratorTest
{
    private static AttributeData GetAttributeData(string source, CancellationToken canceller)
    {
        var tree = CSharpSyntaxTree.ParseText(source, cancellationToken: canceller);
        var structDecl = tree
            .GetRoot(canceller)
            .DescendantNodes()
            .OfType<StructDeclarationSyntax>()
            .First();
        var comp = CSharpCompilation.Create(
            "TestAssembly",
            [tree,],
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            ],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );
        var typeSym = comp
            .GetSemanticModel(tree)
            .GetDeclaredSymbol(structDecl, canceller)!;
        return typeSym.GetAttributes().First();
    }

    [Fact]
    public void IsEfficientUnionType_ShouldReturnFalseForUnrelatedTypes()
    {
        const string source = $$"""
            internal class UnrelatedAttribute : System.Attribute;

            [UnrelatedAttribute]
            public partial struct S;
            """;
        var attrData = GetAttributeData(source, CancellationToken);
        Assert.False(Generator.IsEfficientUnionAttribute(attrData));
    }

    [Fact]
    public void IsEfficientUnionType_ShouldReturnTrueForEfficientUnionType()
    {
        const string source = $$"""
            namespace {{Constants.AttributeNamespace}}
            {
                internal class {{Constants.EfficientUnionAttributeName}} : System.Attribute;
            }

            [{{Constants.EfficientUnionAttributeFullName}}]
            public partial struct S;
            """;
        var attrData = GetAttributeData(source, CancellationToken);
        Assert.True(Generator.IsEfficientUnionAttribute(attrData));
    }
}
