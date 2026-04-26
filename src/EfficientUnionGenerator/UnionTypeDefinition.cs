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

    public override int GetHashCode()
    {
        var hash = (uint)TypeName.GetHashCode();
        foreach (var candidate in CandidateUnmanagedTypes)
        {
            hash = (hash << 13) | (hash >> 19);
            hash ^= (uint)candidate.GetHashCode();
        }
        foreach (var candidate in CandidateManagedTypes)
        {
            hash = (hash << 13) | (hash >> 19);
            hash ^= (uint)candidate.GetHashCode();
        }
        return (int)hash;
    }

    public virtual bool Equals(UnionTypeDefinition? other)
    {
        if (other is null)
        {
            return false;
        }

        if (TypeName != other.TypeName)
        {
            return false;
        }

        if (CandidateUnmanagedTypes.Length != other.CandidateUnmanagedTypes.Length)
        {
            return false;
        }

        for (int i = 0; i < CandidateUnmanagedTypes.Length; i++)
        {
            if (CandidateUnmanagedTypes[i] != other.CandidateUnmanagedTypes[i])
            {
                return false;
            }
        }

        if (CandidateManagedTypes.Length != other.CandidateManagedTypes.Length)
        {
            return false;
        }

        for (int i = 0; i < CandidateManagedTypes.Length; i++)
        {
            if (CandidateManagedTypes[i] != other.CandidateManagedTypes[i])
            {
                return false;
            }
        }

        if (BitMask != other.BitMask)
        {
            return false;
        }

        if (TypeIdentifierValueMode != other.TypeIdentifierValueMode)
        {
            return false;
        }

        // SourceBuilder is not considered for equality as it is used for code generation and does not affect the identity of the union type definition.
        return true;
    }
}
