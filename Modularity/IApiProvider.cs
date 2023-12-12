namespace Modularity;

public interface IApiProvider
{
    T Get<T>();
}