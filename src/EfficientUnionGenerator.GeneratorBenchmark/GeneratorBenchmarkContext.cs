using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using BenchmarkDotNet.Attributes;

namespace EfficientUnionGenerator.GeneratorBenchmark;

[MemoryDiagnoser]
public partial class GeneratorBenchmarkContext
{
    [Params([
        "Int32OrString",
        "SignedInteger",
        "PositiveOnlyIntOrFloat",
        "ElfHeader",
    ])]
    public string? TypeName { get; set; } = null!;

    private IIncrementalGenerator _generator = null!;
    private IIncrementalGenerator _negativeControlGenerator = null!;
    private Compilation _baseCompilation = null!;
    private Compilation _changedCompilation = null!;

    [Params("Experimental", "NegativeControl")]
    public string Target { get; set; } = null!;
    private GeneratorDriver TargetDriver { get; set; } = null!;

    public string? SourceCode { get; private set; }

    [GlobalSetup]
    public void Setup()
    {
        if(TypeName is null)
        {
            return;
        }
        var resourceName = $"EfficientUnionGenerator.GeneratorBenchmark.resources.{TypeName}.cs";
        var executingAssembly = Assembly.GetExecutingAssembly();
        using (var stream = executingAssembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                throw new InvalidOperationException($"Resource '{resourceName}' not found.");
            }
            using var reader = new StreamReader(stream);
            SourceCode = reader.ReadToEnd();
        }

        _generator = new Generator();
        _negativeControlGenerator = new NegativeControlGenerator();

        var tree = CSharpSyntaxTree.ParseText(SourceCode);
        _baseCompilation = CSharpCompilation.Create(
            assemblyName: "BenchmarkDummy",
            syntaxTrees: [tree],
            references: [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            ]);

        var modifiedTree = tree.WithRootAndOptions(
            new Visitor(TypeName).Visit(tree.GetRoot()),
            tree.Options);
        _changedCompilation = _baseCompilation.ReplaceSyntaxTree(tree, modifiedTree);
    }

    [IterationSetup]
    public void ResetDriver()
    {
        TargetDriver = Target switch
        {
            "Experimental" => CSharpGeneratorDriver
                .Create(_generator),
            "NegativeControl" => CSharpGeneratorDriver
                .Create(_negativeControlGenerator),
            _ => throw new InvalidOperationException($"Unknown target: {Target}"),
        };
    }

    [Benchmark]
    public void RunGenerator()
    {
        TargetDriver = TargetDriver
            .RunGenerators(_baseCompilation);
    }

    [Benchmark]
    public void IncrementalUpdate_NoChanges()
    {
        TargetDriver = TargetDriver
            .RunGenerators(_baseCompilation)
            .RunGenerators(_baseCompilation)
            .RunGenerators(_baseCompilation)
            .RunGenerators(_baseCompilation)
            .RunGenerators(_baseCompilation)
            .RunGenerators(_baseCompilation)
            .RunGenerators(_baseCompilation)
            .RunGenerators(_baseCompilation)
            .RunGenerators(_baseCompilation)
            .RunGenerators(_baseCompilation);
    }

    [Benchmark]
    public void IncrementalUpdate_Regenerate()
    {
        TargetDriver = TargetDriver
            .RunGenerators(_baseCompilation)
            .RunGenerators(_changedCompilation)
            .RunGenerators(_baseCompilation)
            .RunGenerators(_changedCompilation)
            .RunGenerators(_baseCompilation)
            .RunGenerators(_changedCompilation)
            .RunGenerators(_baseCompilation)
            .RunGenerators(_changedCompilation)
            .RunGenerators(_baseCompilation)
            .RunGenerators(_changedCompilation);
    }
}


file class Visitor(string typeName) : CSharpSyntaxRewriter
{
    public override SyntaxToken VisitToken(SyntaxToken token)
    {
        if(token.IsKind(SyntaxKind.IdentifierToken) && token.Text == typeName)
        {
            return SyntaxFactory.Identifier(typeName + "_Modified");
        }
        return base.VisitToken(token);
    }
}