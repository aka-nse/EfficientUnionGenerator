using EfficientUnion;

namespace EfficientUnionGenerator.SampleApp.PositiveOnlyIntOrFloat;

[EfficientUnion(unmanagedFieldMask: 0x80_00_00_00u)]
public readonly partial struct PositiveOnlyIntOrFloat
{
    public partial PositiveOnlyIntOrFloat(int value);
    public partial PositiveOnlyIntOrFloat(float value);
}