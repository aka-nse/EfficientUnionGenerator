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
