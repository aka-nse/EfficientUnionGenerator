// See https://aka.ms/new-console-template for more information

using EfficientUnion;
using EfficientUnionGenerator.SampleApp;

ISample[] samples = [
    new EfficientUnionGenerator.SampleApp.SimpleUnionOfInt32OrString.Sample(),
    new EfficientUnionGenerator.SampleApp.SimpleUnionOfSignedInteger.Sample(),
    new EfficientUnionGenerator.SampleApp.PositiveOnlyIntOrFloat.Sample(),
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
