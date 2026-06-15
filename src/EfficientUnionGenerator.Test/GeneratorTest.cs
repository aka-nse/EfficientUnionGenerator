using System;
using System.Collections.Generic;
using System.Text;

namespace EfficientUnionGenerator.Test;

public partial class GeneratorTest(ITestContextAccessor accessor)
{
    private readonly ITestContextAccessor _accessor = accessor;

    private ITestContext TestContext => _accessor.Current;
    private CancellationToken CancellationToken => TestContext.CancellationToken;
}
