using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable IDE0130
namespace PathBench;
#pragma warning restore IDE0130

#if !PROFILING

public class CodePathProfiler
{
    private static readonly CodePathProfiler _instance = new();

    private CodePathProfiler() { }

    public static CodePathProfiler Create() => _instance;

    internal CodePathProfileScope StartMeasurement() => new();

    internal ref struct CodePathProfileScope
    {
        public readonly void MarkCheckpoint(string _) { }
        public readonly void Dispose() { }
    }
}

#endif
