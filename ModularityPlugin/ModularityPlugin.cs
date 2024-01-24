using System.Reflection;
using System.Runtime.Loader;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Plugin.Host;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.Logging;
using Modularity;

namespace ModularityPlugin;

public class ModularityPlugin : BasePlugin
{
    public override string ModuleName => "Modularity";
    public override string ModuleVersion => "1.0.1";

    private readonly IPluginManager _pluginManager;
    private readonly List<(AssemblyName name, Assembly assembly)> _sharedAssemblies = new();

    private bool _loaded;

    public ModularityPlugin()
    {
        var app = typeof(Application).GetProperty("Instance")!.GetValue(null);
        _pluginManager =
            (IPluginManager)typeof(Application).GetField("_pluginManager",
                BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(app)!;

        AssemblyLoadContext.Default.Resolving += (context, assemblyName) =>
        {
            if (!_loaded)
            {
                LoadSharedLibs();
                _loaded = true;
            }

            var assembly = _sharedAssemblies.FirstOrDefault(a => a.name.Name == assemblyName.Name);
            if (assembly.assembly == null)
            {
                var name = assemblyName.Name;
                if (name != null && !name.EndsWith(".resources"))
                    Logger.LogError($"Cant find {assemblyName}");

                return null;
            }

            return assembly.assembly;
        };
    }

    public override void Load(bool hotReload)
    {
        var pluginsFolder = Path.Combine(ModuleDirectory, "plugins");

        var apiProvider = new PluginApis();
        var loadedPlugins = new List<IPlugin>();

        foreach (var pluginDir in Directory.EnumerateDirectories(pluginsFolder))
        {
            _pluginManager.LoadPlugin(pluginDir + "/" + Path.GetFileName(pluginDir) + ".dll");
            var loadedPlugin = _pluginManager.GetLoadedPlugins().Last();
            Logger.LogInformation($"Load modular plugin: {loadedPlugin.Plugin.ModuleName}");

            loadedPlugins.Add(loadedPlugin.Plugin);
        }

        foreach (var plugin in loadedPlugins)
        {
            if (plugin is ICorePlugin corePlugin)
            {
                corePlugin.LoadCore(apiProvider);
            }
        }

        foreach (var plugin in loadedPlugins)
        {
            if (plugin is IModulePlugin modulePlugin)
                modulePlugin.LoadModule(apiProvider);
        }
    }

    private void LoadSharedLibs()
    {
        var sharedFolder = Path.Combine(ModuleDirectory, "shared");

        foreach (var dir in Directory.EnumerateDirectories(sharedFolder))
        {
            var dirName = Path.GetFileName(dir);
            var dllPath = Path.Combine(dir, dirName + ".dll");
            if (!File.Exists(dllPath)) continue;
            LoadLib(dllPath);
        }
    }

    private void LoadLib(string path)
    {
        var loader = PluginLoader.CreateFromAssemblyFile(path, new[] { typeof(IPlugin) });
        var assembly = loader.LoadDefaultAssembly();

        _sharedAssemblies.Add((assembly.GetName(), assembly));

        Logger.LogInformation($"Load shared library: {assembly.FullName}");
    }
}