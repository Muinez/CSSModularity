namespace Modularity;

public class PluginApis : IApiProvider, IApiRegisterer
{
    private readonly Dictionary<Type, object> _apiMap = new();

    public T Get<T>()
    {
        if (_apiMap.TryGetValue(typeof(T), out var value))
            return (T)value;

        throw new Exception($"No such api: {typeof(T)}");
    }

    public void Register<T>(T apiObject)
    {
        _apiMap[typeof(T)] = apiObject ?? throw new ArgumentNullException(nameof(apiObject));
    }
}