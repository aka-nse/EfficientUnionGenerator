using System.Runtime.CompilerServices;

namespace EfficientUnionGenerator.SampleApp.SimpleUnionOfSignedInteger;

internal class Sample : ISample
{
    public string Name => "SimpleUnionOfSignedInteger";

    public void Run()
    {
        Console.WriteLine($"Struct size of `SignedInteger` union: {Unsafe.SizeOf<SignedInteger>()}");

        var a = new SignedInteger((sbyte)-1);
        var b = new SignedInteger((short)-2);
        var c = new SignedInteger(-3);
        var d = new SignedInteger(-4L);
        ShowValue(nameof(a), a);
        ShowValue(nameof(b), b);
        ShowValue(nameof(c), c);
        ShowValue(nameof(d), d);
    }

    private static void ShowValue(string name, SignedInteger x)
    {
#if NET11_OR_GREATER
        switch(x)
        {
            case sbyte aSByte:
                Console.WriteLine($"a is an sbyte: {aSByte}");
                break;
            case short aShort:
                Console.WriteLine($"a is a short: {aShort}");
                break;
            case int aInt:
                Console.WriteLine($"a is an int: {aInt}");
                break;
            case long aLong:
                Console.WriteLine($"a is a long: {aLong}");
                break;
        }
#else
        if (x.TryGetValue(out sbyte aSByte))
        {
            Console.WriteLine($"{name} is an sbyte: {aSByte}");
        }
        else if (x.TryGetValue(out short aShort))
        {
            Console.WriteLine($"{name} is a short: {aShort}");
        }
        else if (x.TryGetValue(out int aInt))
        {
            Console.WriteLine($"{name} is an int: {aInt}");
        }
        else if (x.TryGetValue(out long aLong))
        {
            Console.WriteLine($"{name} is a long: {aLong}");
        }
#endif
    }
}
