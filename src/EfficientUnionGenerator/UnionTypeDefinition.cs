using System.Collections.Immutable;
using SourceGeneratorToolkit;

namespace EfficientUnionGenerator;

public record UnionTypeDefinition(
    string TypeName,
    ImmutableArray<TypeCandidateDefinition> CandidateUnmanagedTypes,
    ImmutableArray<TypeCandidateDefinition> CandidateManagedTypes,
    ulong BitMask,
    TypeIdentifierValueMode TypeIdentifierValueMode)
{
    // SourceBuilder is a string builder for the generated source code.
    // It contains information about the generation target type, but is not part of the type's logical state.
    // Therefore, it should not be considered for equality comparison or hash code generation.
    internal SourceBuilder SourceBuilder { get; init; } = default!;


    public string TypeIdentifierBaseType => BitMask switch
    {
        <= byte.MaxValue => " : byte",
        <= ushort.MaxValue => " : ushort",
        <= uint.MaxValue => " : uint",
        _ => " : ulong",
    };


    public IEnumerable<string> GetUnmanagedFieldDecl()
    {
        foreach (var type in CandidateUnmanagedTypes)
        {
            yield return $$"""
                    [System.Runtime.InteropServices.FieldOffset(0)] public {{type.TypeName}} {{type.FieldName}};
                    """;
        }
    }


    public override int GetHashCode() =>
        UnionTypeDefinitionEqualityComparer.GetHashCode(this);


    public virtual bool Equals(UnionTypeDefinition? other) =>
        UnionTypeDefinitionEqualityComparer.Equals(this, other);
}
