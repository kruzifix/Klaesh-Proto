using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaesh.Core
{
    public class ServiceManager : IServiceLocator
    {
        private Dictionary<Type, object> _entries;

        public ServiceManager()
        {
            _entries = new Dictionary<Type, object>();
        }

        public T GetService<T>()
        {
            return (T)GetInstance(typeof(T));
        }

        public void RegisterSingleton<TService, TImplementation>(TImplementation singleton) where TImplementation : TService
        {
            Type t = typeof(TService);
            if (_entries.ContainsKey(t))
            {
                throw new Exception("Service already registered.");
            }
            _entries.Add(t, singleton);
        }

        private object GetInstance(Type type)
        {
            if (!_entries.ContainsKey(type))
            {
                // create service!
                _entries[type] = Activator.CreateInstance(type);
            }

            return _entries[type];
        }
    }
}
