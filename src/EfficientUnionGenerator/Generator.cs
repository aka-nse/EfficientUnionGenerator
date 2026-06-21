using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using SourceGeneratorToolkit;
using PathBench;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace EfficientUnionGenerator;

using static Constants;

[Generator(LanguageNames.CSharp)]
public partial class Generator : IIncrementalGenerator
{
    public static CodePathProfiler Profiler =
        CodePathProfiler.Create();

    private static IEqualityComparer<GeneratorAttributeSyntaxContext> AttributeSyntaxContextComparer { get; } =
        new GeneratorAttributeSyntaxContextComparer(Profiler);

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
            .WithComparer(AttributeSyntaxContextComparer)
            .Select(PredicateUnionType)
            .WithComparer(UnitTypeGenerationInfoComparer.Instance);
        context.RegisterSourceOutput(sourceUnion, EmitUnionType);
    }


    private static UnitTypeGenerationInfo? PredicateUnionType(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token)
    {
        using var counter = Profiler.StartMeasurement();

        ((TypeDeclarationSyntax)context.TargetNode).ChildNodes().OfType<ConstructorDeclarationSyntax>().ToList();

        var symbol = (INamedTypeSymbol)context.TargetSymbol;

        counter.MarkCheckpoint("After symbol retrieval");

        var fullName = symbol.ToDisplayString();

        counter.MarkCheckpoint("After full name retrieval");

        var unionTypeDefinitionAttributeData = context.Attributes
            .FirstOrDefault(IsEfficientUnionAttribute);
        if (unionTypeDefinitionAttributeData is null)
        {
            return null;
        }

        counter.MarkCheckpoint("After attribute data retrieval");

        var members = symbol.Constructors;

        counter.MarkCheckpoint("After members retrieval");

        var candidateTypes = members
            .Select(TypeCandidateDefinition.Create)
            .OfType<TypeCandidateDefinition>();

        counter.MarkCheckpoint("After candidate type creation");

        var candidateUnmanagedTypes = ImmutableArray.CreateBuilder<TypeCandidateDefinition>();
        var candidateManagedTypes = ImmutableArray.CreateBuilder<TypeCandidateDefinition>();
        foreach (var type in candidateTypes)
        {
            (type.IsUnmanaged ? candidateUnmanagedTypes : candidateManagedTypes).Add(type);
        }

        counter.MarkCheckpoint("After candidate type separation");

        var (mode, bitMask) = ReadAttributeParameter(unionTypeDefinitionAttributeData);

        counter.MarkCheckpoint("After attribute parameter reading");

        return new(
            new UnionTypeDefinition(
                fullName,
                candidateUnmanagedTypes.ToImmutable(),
                candidateManagedTypes.ToImmutable(),
                bitMask,
                mode),
            new SourceBuilder(context, false));
    }


    private static void EmitUnionType(SourceProductionContext context, UnitTypeGenerationInfo? info)
    {
        if (info is null)
        {
            return;
        }
        var (source, sb) = info;
        var (hintName, sourceCode) = GenerateImplementationSource(source, sb);
        context.AddSource(
            hintName,
            sourceCode
            );
    }


    internal static (string hintName, string sourceCode) GenerateImplementationSource(UnionTypeDefinition source, SourceBuilder sb)
    {
        var mode = source.TypeIdentifierValueMode;
        sb.AppendAutoGeneratedComment();
        sb.AppendLine("#nullable enable");
        sb.AppendNamespaceDeclaration();

        using (var typeDecl = sb.BeginTargetTypeDeclare())
        {
            typeDecl.AddAttribute($"System.Runtime.CompilerServices.Union");
            GenerateTypeSpecifierEnumDeclaration(source, sb);

            if (source.CandidateUnmanagedTypes.Length > 0)
            {
                if (source.BitMask == 0)
                {
                    sb.AppendLine($$"""
                                        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
                                        private struct __UnmanagedField
                                        {
                                            {{source.GetUnmanagedFieldDecl().PreserveIndent()}}
                                        }

                                        private readonly __UnmanagedField __unmanagedField;
                                        private readonly __TypeSpecifier __typeSpecifier;

                                        private bool HasUnmanagedValue => __typeSpecifier != __TypeSpecifier.__Undefined || __defaultTypeSpecifierHasType;
                                        private object? UnmanagedValue => __typeSpecifier switch
                                        {
                                            {{source.CandidateUnmanagedTypes.Select(static t => $"__TypeSpecifier.{t.FieldName} => __unmanagedField.{t.FieldName},").PreserveIndent()}}
                                            _ => null,
                                        };

                                        """);
                }
                else
                {
                    sb.AppendLine($$"""
                                        private const __TypeSpecifier __typeSpecifierBitMask = unchecked((__TypeSpecifier){{source.BitMask}});

                                        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
                                        private struct __UnmanagedField
                                        {
                                            [System.Runtime.InteropServices.FieldOffset(0)] public __TypeSpecifier __typeSpecifier;
                                            {{source.GetUnmanagedFieldDecl().PreserveIndent()}}
                                        }

                                        private readonly __UnmanagedField __unmanagedField;
                                        private readonly __TypeSpecifier __typeSpecifier
                                        {
                                            get => __unmanagedField.__typeSpecifier & __typeSpecifierBitMask;
                                            init => __unmanagedField.__typeSpecifier |= value;
                                        }
                        
                                        private bool HasUnmanagedValue => __typeSpecifier != __TypeSpecifier.__Undefined || __defaultTypeSpecifierHasType;
                                        """);
                    if (mode.IsLeaveWhenGet)
                    {
                        sb.AppendLine($$"""
                                        private object? UnmanagedValue
                                        {
                                            get
                                            {
                                                var maskedField = __unmanagedField;
                                                return __typeSpecifier switch
                                                {
                                                    {{source.CandidateUnmanagedTypes.Select(static t => $"__TypeSpecifier.{t.FieldName} => maskedField.{t.FieldName},").PreserveIndent()}}
                                                    _ => null,
                                                };
                                            }
                                        }
                                        
                                        """);
                    }
                    else
                    {
                        sb.AppendLine($$"""
                                        private object? UnmanagedValue
                                        {
                                            get
                                            {
                                                var maskedField = __unmanagedField;
                                                maskedField.__typeSpecifier &= ~__typeSpecifierBitMask;
                                                return __typeSpecifier switch
                                                {
                                                    {{source.CandidateUnmanagedTypes.Select(static t => $"__TypeSpecifier.{t.FieldName} => maskedField.{t.FieldName},").PreserveIndent()}}
                                                    _ => null,
                                                };
                                            }
                                        }
                                        
                                        """);
                    }

                }
                foreach (var type in source.CandidateUnmanagedTypes)
                {
                    if (!mode.IsLeaveWhenCreate)
                    {
                        sb.AppendLine($$"""
                            public partial {{sb.TargetType.Name}}({{type.TypeName}} {{type.ConstructorParameterName}})
                            {
                                __unmanagedField = new()
                                {
                                    {{type.FieldName}} = {{type.ConstructorParameterName}},
                                };
                                __typeSpecifier = __TypeSpecifier.{{type.FieldName}};
                            }
                            """);
                    }
                    else
                    {
                        sb.AppendLine($$"""
                            public partial {{sb.TargetType.Name}}({{type.TypeName}} {{type.ConstructorParameterName}})
                            {
                                __unmanagedField = new()
                                {
                                    {{type.FieldName}} = {{type.ConstructorParameterName}},
                                };
                            }
                            """);
                    }

                    if (source.BitMask == 0 || mode.IsLeaveWhenGet)
                    {
                        sb.AppendLine($$"""
                            public bool TryGetValue(out {{type.TypeName}} value)
                            {
                                if (__typeSpecifier == __TypeSpecifier.{{type.FieldName}})
                                {
                                    value = __unmanagedField.{{type.FieldName}};
                                    return true;
                                }
                                else
                                {
                                    value = default;
                                    return false;
                                }
                            }

                            """);
                    }
                    else
                    {
                        sb.AppendLine($$"""
                            public bool TryGetValue(out {{type.TypeName}} value)
                            {
                                if (__typeSpecifier == __TypeSpecifier.{{type.FieldName}})
                                {
                                    var maskedField = __unmanagedField;
                                    maskedField.__typeSpecifier &= ~__typeSpecifierBitMask;
                                    value = maskedField.{{type.FieldName}};
                                    return true;
                                }
                                else
                                {
                                    value = default;
                                    return false;
                                }
                            }

                            """);
                    }
                }
            }

            if (source.CandidateManagedTypes.Length > 0)
            {
                sb.AppendLine($$"""
                                        private readonly object? __managedField;

                                        """);

                foreach (var type in source.CandidateManagedTypes)
                {
                    sb.AppendLine($$"""
                                        public partial {{sb.TargetType.Name}}({{type.TypeName}} {{type.ConstructorParameterName}})
                                        {
                                            __managedField = {{type.ConstructorParameterName}};
                                        }

                                        public bool TryGetValue([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out {{type.TypeName}}? value)
                                        {
                                            if (__managedField is {{type.TypeName}} __casted)
                                            {
                                                value = __casted;
                                                return true;
                                            }
                                            else
                                            {
                                                value = default;
                                                return false;
                                            }
                                        }

                                        """);
                }
            }

            switch ((source.CandidateUnmanagedTypes.Length > 0, source.CandidateManagedTypes.Length > 0))
            {
            case (true, true):
                sb.AppendLine($$"""
                                        public bool HasValue => HasUnmanagedValue || __managedField is not null;
                                        public object? Value => HasUnmanagedValue ? UnmanagedValue : __managedField;
                                        """);
                break;
            case (true, false):
                sb.AppendLine($$"""
                                        public bool HasValue => HasUnmanagedValue;
                                        public object? Value => UnmanagedValue;
                                        """);
                break;
            case (false, true):
                sb.AppendLine($$"""
                                        public bool HasValue => __managedField is not null;
                                        public object? Value => __managedField;
                                        """);
                break;
            case (false, false):
                sb.AppendLine($$"""
                                        public bool HasValue => false;
                                        public object? Value => null;
                                        """);
                break;
            default:
                throw new InvalidOperationException("never path");
            }
        }

        var hintName = sb.GetPreferHintName(prefix: "EfficientUnionGenerator-", suffix: ".g");
        var sourceCode = sb.Build();
        return (hintName, sourceCode);
    }


    internal static void GenerateTypeSpecifierEnumDeclaration(UnionTypeDefinition source, ISourceBuilder sb)
    {
        var baseType = source.TypeIdentifierBaseType;
        var mode = source.TypeIdentifierValueMode;
        var bitMask = source.BitMask;
        var types = source.CandidateUnmanagedTypes;
        var defaultTypeSpecifierHasType = "false";
        sb.AppendLine($$"""
                                        private enum __TypeSpecifier{{baseType}}
                                        {
                                        """);

        if (mode.IsExplicitAssign)
        {
            foreach (var type in types)
            {
                sb.AppendLine($$"""
                                            {{type.FieldName}} = {{type.EnumBitPattern}},
                                        """);
                if (type.EnumBitPattern == 0)
                {
                    defaultTypeSpecifierHasType = "true";
                }
            }
        }
        else if (bitMask == 0)
        {
            var i = 1;
            foreach (var type in types)
            {
                sb.AppendLine($$"""
                                            {{type.FieldName}} = {{i}},
                                        """);
                ++i;
            }
        }
        else
        {
            foreach (var (type, pattern) in types.Zip(Helpers.GetBitPatterns(bitMask), static (x, y) => (x, y)))
            {
                sb.AppendLine($$"""
                                            {{type.FieldName}} = {{pattern}},
                                        """);
                if (type.EnumBitPattern == 0)
                {
                    defaultTypeSpecifierHasType = "true";
                }
            }
        }
        sb.AppendLine($$"""
                                        
                                            __Undefined = 0,
                                        }

                                        private const bool __defaultTypeSpecifierHasType = {{defaultTypeSpecifierHasType}};
                                        """);
    }


    internal static bool IsEfficientUnionAttribute(AttributeData attrData) =>
        attrData.AttributeClass?.ToDisplayString() == EfficientUnionAttributeFullName;


    internal static (TypeIdentifierValueMode mode, ulong bitMask) ReadAttributeParameter(AttributeData attrData)
    {
        Debug.Assert(IsEfficientUnionAttribute(attrData));

        TypeIdentifierValueMode mode;
        ulong bitMask;
        if (attrData.ConstructorArguments.Length == 0)
        {
            mode = TypeIdentifierValueMode.AutoAssign;
            bitMask = 0;
        }
        else if (attrData.ConstructorArguments.Length == 2)
        {
            mode = (TypeIdentifierValueMode)(int)attrData.ConstructorArguments[0].Value!;
            bitMask = (ulong)attrData.ConstructorArguments[1].Value!;
        }
        else
        {
            throw new InvalidOperationException($"Unsupported bit mask type");
        }
        return (mode, bitMask);
    }
}


internal record UnitTypeGenerationInfo(UnionTypeDefinition Source, SourceBuilder SourceBuilder);


// determines whether re-generation is needed based on the equality of the source and source builder.
file class GeneratorAttributeSyntaxContextComparer(CodePathProfiler profiler)
    : IEqualityComparer<GeneratorAttributeSyntaxContext>
{
    public bool Equals(GeneratorAttributeSyntaxContext x, GeneratorAttributeSyntaxContext y)
    {
        using var counter = profiler.StartMeasurement();

        if ((x.TargetNode, y.TargetNode) is not (TypeDeclarationSyntax xType, TypeDeclarationSyntax yType))
        {
            throw new InvalidOperationException();
        }

        if (xType.Identifier.ValueText != yType.Identifier.ValueText)
        {
            return false;
        }

        counter.MarkCheckpoint("Identifier check");

        var ctorsY = new HashSet<ConstructorDeclarationSyntax>(SyntaxNodeComparer.Instance);
        foreach (var member in yType.Members)
        {
            if (member is ConstructorDeclarationSyntax ctor)
            {
                ctorsY.Add(ctor);
            }
        }

        counter.MarkCheckpoint("Collect ctors of y");

        foreach (var member in xType.Members)
        {
            if (member is ConstructorDeclarationSyntax ctor)
            {
                if (!ctorsY.Contains(ctor))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public int GetHashCode(GeneratorAttributeSyntaxContext obj)
    {
        if (obj.TargetNode is not TypeDeclarationSyntax typeDecl)
        {
            throw new InvalidOperationException();
        }

        var hash = (uint)typeDecl.Identifier.ValueText.GetHashCode();
        foreach (var member in typeDecl.Members)
        {
            if (member is not ConstructorDeclarationSyntax ctor)
            {
                continue;
            }
            hash ^= (uint)ctor.WithoutTrivia().ToFullString().GetHashCode();
            hash = (hash << 13) | (hash >> 19);
        }
        return (int)hash;
    }
}


file class UnitTypeGenerationInfoComparer : IEqualityComparer<UnitTypeGenerationInfo?>
{
    // SourceBuilder is a string builder for the generated source code.
    // It contains information about the generation target type, but is not part of the type's logical state.
    // Therefore, it should not be considered for equality comparison or hash code generation.

    public static UnitTypeGenerationInfoComparer Instance { get; } = new();

    public bool Equals(UnitTypeGenerationInfo? x, UnitTypeGenerationInfo? y) =>
        UnionTypeDefinitionEqualityComparer.Equals(x?.Source, y?.Source);

    public int GetHashCode(UnitTypeGenerationInfo? obj) =>
        UnionTypeDefinitionEqualityComparer.GetHashCode(obj?.Source);
}