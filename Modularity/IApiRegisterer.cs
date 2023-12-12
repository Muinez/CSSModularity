namespace Modularity;

public interface IApiRegisterer
{
    void Register<T>(T apiObject);
}