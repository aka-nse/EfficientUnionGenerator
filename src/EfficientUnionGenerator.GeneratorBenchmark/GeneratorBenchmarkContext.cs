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
    private Compilation _baseCompilation = null!;
    private Compilation _changedCompilation = null!;
    private GeneratorDriver _driver = null!;

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
        _driver = CSharpGeneratorDriver
            .Create(_generator);
    }

    [Benchmark]
    public void RunGenerator()
    {
        _driver = _driver
            .RunGenerators(_baseCompilation);
    }

    [Benchmark]
    public void IncrementalUpdate()
    {
        _driver = _driver
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