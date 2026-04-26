namespace EfficientUnionGenerator.Test;

public class HelpersTest
{
    public static TheoryData<ulong, ulong[]> GetBitPatternsTestCase() =>
        new()
        {
            { 0, [0] },
            { 0b1, [0b0, 0b1]  },
            { 0b1uL << 63, [0b0uL << 63, 0b1uL << 63]  },
            { 0b101, [0b000, 0b001, 0b100, 0b101] },
            { 0b111, [0b000, 0b001, 0b010, 0b011, 0b100, 0b101, 0b110, 0b111] },
            { 0b1010, [0b0000, 0b0010, 0b1000, 0b1010]  },
        };

    [Theory]
    [MemberData(nameof(GetBitPatternsTestCase))]
    public void GetBitPatterns_CoverPatterns(ulong mask, ulong[] expected)
    {
        var actual = Helpers.GetBitPatterns(mask);
        Assert.Equal(expected, actual);
    }


    public static TheoryData<string, string> CreateSafeNameTestCase() =>
        new()
        {
            { "SimpleName", "__SimpleName" },
            { "Name With Spaces", "__Name_u0020_With_u0020_Spaces" },
            { "Name-With-Dashes", "__Name_u002D_With_u002D_Dashes" },
            { "NameWith$pecial#Chars!", "__NameWith_u0024_pecial_u0023_Chars_u0021_" },
            { "123StartsWithDigits", "__123StartsWithDigits" },
            { "NameWithÜnicode", "__NameWithÜnicode" },
            { "Nameいろはにほへと", "__Nameいろはにほへと" }
        };

    [Theory]
    [MemberData(nameof(CreateSafeNameTestCase))]
    public void CreateSafeName_SanitizesNames(string input, string expected)
    {
        var actual = Helpers.CreateSafeName(input);
        Assert.Equal(expected, actual);
    }
}
