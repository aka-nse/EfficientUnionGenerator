using System.Runtime.CompilerServices;
using EfficientUnion;

namespace EfficientUnionGenerator.SampleApp.PositiveOnlyIntOrFloat;

internal class Sample : ISample
{
    public string Name => "PositiveOnlyIntOrFloat";

    public void Run()
    {
        Console.WriteLine($"Struct size of `PositiveOnlyIntOrFloat` union: {Unsafe.SizeOf<PositiveOnlyIntOrFloat>()}");

        var a = new PositiveOnlyIntOrFloat(123);
        var b = new PositiveOnlyIntOrFloat(1.23f);
        ShowValue(nameof(a), a);
        ShowValue(nameof(b), b);
    }

    private static void ShowValue(string name, PositiveOnlyIntOrFloat x)
    {
#if NET11_OR_GREATER
        switch(x)
        {
            case int aInt:
                Console.WriteLine($"a is an int: {aInt}");
                break;
            case float aFloat:
                Console.WriteLine($"a is a float: {aFloat}");
                break;
        }
#else
        if (x.TryGetValue(out int aInt))
        {
            Console.WriteLine($"{name} is an int: {aInt}");
        }
        else if (x.TryGetValue(out float aFloat))
        {
            Console.WriteLine($"{name} is a float: {aFloat}");
        }
#endif
    }
}

[EfficientUnion(unmanagedFieldMask: 0x80_00_00_00u)]
public readonly partial struct PositiveOnlyIntOrFloat
{
    public partial PositiveOnlyIntOrFloat(int value);
    public partial PositiveOnlyIntOrFloat(float value);
}