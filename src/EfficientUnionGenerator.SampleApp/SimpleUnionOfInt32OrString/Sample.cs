using System.Runtime.CompilerServices;
using EfficientUnion;

namespace EfficientUnionGenerator.SampleApp.SimpleUnionOfInt32OrString;

internal class Sample : ISample
{
    public string Name => "SimpleUnionOfInt32OrString";

    public void Run()
    {
        Console.WriteLine($"Struct size of `Int32OrString` union: {Unsafe.SizeOf<Int32OrString>()}");

        var x = new Int32OrString(123);
        var y = new Int32OrString("Hello, World!");
        ShowValue(nameof(x), x);
        ShowValue(nameof(y), y);
    }

    private static void ShowValue(string name, Int32OrString x)
    {
#if NET11_OR_GREATER
        switch (x)
        {
            case int xValue:
                Console.WriteLine($"{name} is an int: {xValue}");
                break;
            case string xStr:
                Console.WriteLine($"{name} is a string: {xStr}");
                break;
        }
#else
        if (x.TryGetValue(out int xValue))
        {
            Console.WriteLine($"{name} is an int: {xValue}");
        }
        else if (x.TryGetValue(out string? xStr))
        {
            Console.WriteLine($"{name} is a string: {xStr}");
        }
#endif
    }
}


[EfficientUnion]
public readonly partial struct Int32OrString
{
    public partial Int32OrString(int x);
    public partial Int32OrString(string x);
}
