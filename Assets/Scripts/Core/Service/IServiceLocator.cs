namespace Klaesh.Core
{
    public interface IServiceLocator
    {
        T GetService<T>();

        void RegisterSingleton<TService, TImplementation>(TImplementation singleton) where TImplementation : TService;
    }
}
