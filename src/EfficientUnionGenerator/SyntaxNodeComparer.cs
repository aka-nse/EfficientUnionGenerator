using Microsoft.CodeAnalysis;

namespace EfficientUnionGenerator;

internal class SyntaxNodeComparer : IEqualityComparer<SyntaxNode>
{
    public static SyntaxNodeComparer Instance { get; } = new();

    public bool Equals(SyntaxNode x, SyntaxNode y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

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
        return x.IsEquivalentTo(y);
    }

    public int GetHashCode(SyntaxNode obj)
    {
        return obj.ToFullString().GetHashCode();
    }
}
