using EfficientUnion;

namespace EfficientUnionGenerator.SampleApp.SimpleUnionOfInt32OrString;

[EfficientUnion]
public readonly partial struct Int32OrString
{
    public partial Int32OrString(int x);
    public partial Int32OrString(string x);
}
