using CounterStrikeSharp.API.Core;
using Modularity;
using TestCoreApi;

namespace TestModule;

public class TestModule : BasePlugin, IModulePlugin
{
    public override string ModuleName => "Test Module";
    public override string ModuleVersion => "1.0.0";
    
    public void LoadModule(IApiProvider provider)
    {
        Console.WriteLine("LOAD MODULE");
        provider.Get<ITestCoreApi>().PrintSomething("MODULE WORKS!!");
    }
}
