using CounterStrikeSharp.API.Core;
using Modularity;
using TestCoreApi;

namespace TestCore;

public class TestCore : BasePlugin, ICorePlugin
{
    public override string ModuleName => "Test Core";
    public override string ModuleVersion => "1.0.0";
    
    public void LoadCore(IApiRegisterer apiRegisterer)
    {
        Console.WriteLine("LOAD CORE");
        apiRegisterer.Register<ITestCoreApi>(new TestApi());
    }
}

public class TestApi : ITestCoreApi
{
    public void PrintSomething(string message)
    {
        Console.WriteLine(message);
    }
}
