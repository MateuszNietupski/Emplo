using Emplot.Data.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Emplo.Tests.Common;

public abstract class TestBase
{
    protected AppDbContext Context { get; private set; }
    
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        Context = new AppDbContext(options);
    }
    
    [TearDown]
    protected void TearDown()
    {
        Context.Dispose();
        if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            TestContext.Out.WriteLine($"Test {TestContext.CurrentContext.Test.Name} failed.");
            TestContext.Out.WriteLine($"Message: {TestContext.CurrentContext.Result.Message}");
        }
    }
}