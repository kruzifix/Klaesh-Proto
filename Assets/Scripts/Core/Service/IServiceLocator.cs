namespace Klaesh.Core
{
    public interface IServiceLocator
    {
        bool HasService<T>();
        T GetService<T>();

        void RegisterSingleton<TService>(TService singleton);

        void DeregisterSingleton<TService>();
    }
}
