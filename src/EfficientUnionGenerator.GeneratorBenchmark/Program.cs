// #define PROFILING

using BenchmarkDotNet.Running;
using EfficientUnionGenerator;
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
context.ResetDriver();
context.IncrementalUpdate_NoChanges();

Console.WriteLine("[loaded source code]:");
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine(context.SourceCode);
Console.ForegroundColor = defaultConsoleColor;

#elif PROFILING

var context = new GeneratorBenchmarkContext()
{
    TypeName = "Int32OrString",
};
context.Setup();
for(var i = 0; i < (1 << 16); ++i)
{
    context.ResetDriver();
    context.IncrementalUpdate_NoChanges();
}

var reports = Generator.Profiler.CreateProfileReports();
var report = reports["PredicateUnionType"];
Console.WriteLine(report);

#else

BenchmarkRunner.Run<GeneratorBenchmarkContext>();

#endif
