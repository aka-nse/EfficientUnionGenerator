using System.Text;

namespace EfficientUnionGenerator;

public static class Helpers
{
    /// <summary>
    /// Enumerates all bit patterns that can be formed by the bits set in the given mask.
    /// </summary>
    /// <param name="mask"></param>
    /// <returns></returns>
    public static IEnumerable<ulong> GetBitPatterns(ulong mask)
    {
        if (mask == 0UL)
        {
            yield return 0UL;
            yield break;
        }

        var positions = new List<int>();
        for (int bit = 0; bit < 64; bit++)
        {
            if ((mask & (1UL << bit)) != 0UL)
            {
                positions.Add(bit);
            }
        }

        int n = positions.Count;
        if (n == 0)
        {
            yield return 0UL;
            yield break;
        }

        static IEnumerable<ulong> recurse(List<int> positions, int idx, ulong current)
        {
            if (idx < 0)
            {
                yield return current;
                yield break;
            }

            foreach (var v in recurse(positions, idx - 1, current))
                yield return v;

            ulong bitValue = 1UL << positions[idx];
            foreach (var v in recurse(positions, idx - 1, current | bitValue))
                yield return v;
        }

        foreach (var p in recurse(positions, n - 1, 0UL))
            yield return p;
    }

    /// <summary>
    /// Sanitizes the given name into letter characters and digits.
    /// The prefix '__' is added.
    /// Not letter nor digit characters will be encoded as '_uXXXX_'.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string CreateSafeName(string name)
    {
        var sb = new StringBuilder();
        sb.Append("__");
        foreach (var c in name)
        {
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(c);
            }
            else
            {
                sb.Append($"_u{(int)c:X04}_");
            }
        }
        return sb.ToString();
    }

}
