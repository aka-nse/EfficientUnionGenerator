namespace EfficientUnionGenerator;

public sealed class UnionTypeDefinitionEqualityComparer
    : IEqualityComparer<UnionTypeDefinition?>
{
    public static UnionTypeDefinitionEqualityComparer Instance { get; } = new ();

    bool IEqualityComparer<UnionTypeDefinition?>.Equals(UnionTypeDefinition? x, UnionTypeDefinition? y) =>
            Equals(x, y);

    int IEqualityComparer<UnionTypeDefinition?>.GetHashCode(UnionTypeDefinition? obj) =>
                GetHashCode(obj);

    public static bool Equals(UnionTypeDefinition? x, UnionTypeDefinition? y)
    {
        switch ((x, y))
        {
        case (null, null):
            return true;
        case (null, _):
        case (_, null):
            return false;
        default:
            break;
        }

        if (x.TypeName != y.TypeName)
        {
            return false;
        }

        if (!Enumerable.SequenceEqual(x.CandidateUnmanagedTypes, y.CandidateUnmanagedTypes))
        {
            return false;
        }
        if (!Enumerable.SequenceEqual(x.CandidateManagedTypes, y.CandidateManagedTypes))
        {
            return false;
        }
        if (x.BitMask != y.BitMask)
        {
            return false;
        }
        if (x.TypeIdentifierValueMode != y.TypeIdentifierValueMode)
        {
            return false;
        }
        return true;
    }

    public static int GetHashCode(UnionTypeDefinition? obj)
    {
        if (obj is null)
        {
            return 0;
        }

        var hash = (uint)obj.TypeName.GetHashCode();
        foreach (var candidate in obj.CandidateUnmanagedTypes)
        {
            hash = (hash << 13) | (hash >> 19);
            hash ^= (uint)candidate.GetHashCode();
        }
        foreach (var candidate in obj.CandidateManagedTypes)
        {
            hash = (hash << 13) | (hash >> 19);
            hash ^= (uint)candidate.GetHashCode();
        }
        return (int)hash;
    }
}