namespace Klaesh.Core
{
    public interface IServiceLocator
    {
        bool HasService<T>();
        T GetService<T>();

        void RegisterSingleton<TService, TImplementation>(TImplementation singleton) where TImplementation : TService;

        void DeregisterSingleton<TService>();
    }
}
