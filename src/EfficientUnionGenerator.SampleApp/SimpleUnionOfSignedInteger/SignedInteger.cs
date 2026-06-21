using EfficientUnion;

namespace EfficientUnionGenerator.SampleApp.SimpleUnionOfSignedInteger;

[EfficientUnion]
public readonly partial struct SignedInteger
{
    public partial SignedInteger(sbyte x);
    public partial SignedInteger(short x);
    public partial SignedInteger(int x);
    public partial SignedInteger(long x);
}