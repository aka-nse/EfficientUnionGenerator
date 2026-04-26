// See https://aka.ms/new-console-template for more information

using EfficientUnion;
using EfficientUnionGenerator.SampleApp;

ISample[] samples = [
    new EfficientUnionGenerator.SampleApp.SimpleUnionOfInt32OrString.Sample(),
    new EfficientUnionGenerator.SampleApp.SimpleUnionOfSignedInteger.Sample(),
    new EfficientUnionGenerator.SampleApp.ElfHeader.Sample(),
];

var color = Console.ForegroundColor;
foreach(var sample in samples)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"[{sample.Name}]");
    Console.ForegroundColor = color;
    sample.Run();
    Console.WriteLine();
}


namespace FooBar
{
    [EfficientUnion()]
    public readonly partial struct MyUnion1
    {
        public partial MyUnion1(int x);
        public partial MyUnion1(float x);
        public partial MyUnion1(string x);
    }

    [EfficientUnion(TypeIdentifierValueMode.AutoAssign | TypeIdentifierValueMode.SetWhenCreate | TypeIdentifierValueMode.ResetWhenGet, 0x80000000)]
    public readonly partial struct MyUnion2
    {
        public partial MyUnion2(int x);
        public partial MyUnion2(float x);
    }

    [EfficientUnion(TypeIdentifierValueMode.ExplicitAssign | TypeIdentifierValueMode.LeaveWhenCreate | TypeIdentifierValueMode.LeaveWhenGet, 0x80000000)]
    public readonly partial struct MyUnion3
    {
        [EnumBitPattern(0x00000000)] public partial MyUnion3(int x);
        [EnumBitPattern(0x80000000)] public partial MyUnion3(float x);
    }
}


