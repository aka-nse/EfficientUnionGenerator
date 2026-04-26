using System;
using System.Collections.Generic;
using System.Text;

namespace EfficientUnionGenerator.SampleApp;

internal interface ISample
{
    public string Name { get; }
    public void Run();
}
