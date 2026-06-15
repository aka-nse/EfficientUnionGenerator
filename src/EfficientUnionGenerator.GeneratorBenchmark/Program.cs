using BenchmarkDotNet.Running;
using EfficientUnionGenerator.GeneratorBenchmark;

#if DEBUG

var defaultConsoleColor = Console.ForegroundColor;
Console.WriteLine("This application is running in DEBUG mode.");
Console.WriteLine("This build mode is only for debugging benchmark context.");
Console.WriteLine("Rebuild release mode for benchmarking.");
Console.WriteLine();

var context = new GeneratorBenchmarkContext()
{
    TypeName = "Int32OrString",
};
context.Setup();

Console.WriteLine("[loaded source code]:");
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine(context.SourceCode);
Console.ForegroundColor = defaultConsoleColor;

#else

BenchmarkRunner.Run<GeneratorBenchmarkContext>();

#endif
