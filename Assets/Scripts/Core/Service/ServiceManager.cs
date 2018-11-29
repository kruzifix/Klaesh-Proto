using System;
using System.Collections.Generic;
using System.Linq;

namespace Klaesh.Core
{
    public class ServiceManager : IServiceLocator
    {
        private Dictionary<Type, object> _entries;

        public ServiceManager()
        {
            _entries = new Dictionary<Type, object>();
        }

        public bool HasService<T>()
        {
            return _entries.ContainsKey(typeof(T));
        }

        public T GetService<T>()
        {
            return (T)GetInstance(typeof(T));
        }

        public void RegisterSingleton<TService, TImplementation>(TImplementation singleton)
            where TImplementation : TService
        {
            Type t = typeof(TService);
            if (_entries.ContainsKey(t))
            {
                throw new Exception("Service already registered.");
            }
            _entries.Add(t, singleton);
        }

        public void DeregisterSingleton<TService>()
        {
            _entries.Remove(typeof(TService));
        }

        private object GetInstance(Type type)
        {
            if (!_entries.ContainsKey(type))
            {
                bool isInitializable = type.CustomAttributes.Any(a => a.AttributeType == typeof(InitializableFromServiceManager));

                if (!isInitializable)
                    throw new Exception($"No service of Type {type.FullName} registered and can't create one lazyly");

                // create service!
                _entries[type] = Activator.CreateInstance(type);
            }

            return _entries[type];
        }
    }
}
