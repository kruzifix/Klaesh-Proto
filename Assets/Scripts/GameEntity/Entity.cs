using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.GameEntity.Module;
using UnityEngine;

namespace Klaesh.GameEntity
{
    public interface IEntity
    {
        int Id { get; }

        void Initialize(int id);

        void AddModule(object module);
        IEnumerable<T> GetModules<T>();
        T GetModule<T>();
    }

    public class Entity : MonoBehaviour, IEntity
    {
        private List<object> _modules;

        private bool _initialized = false;

        public bool createWidget = false;

        public int Id { get; private set; }

        public void Initialize(int id)
        {
            if (_initialized)
                throw new Exception("GameEntity already initialized");

            _initialized = true;
            Id = id;

            _modules = new List<object>();
        }

        public void InitModules()
        {
            foreach (var mod in _modules.OfType<IEntityModule>())
                mod.Init();
        }

        public void AddModule(object module)
        {
            if (_modules.Contains(module))
                return;
            if (module is IEntityModule)
                (module as IEntityModule).Owner = this;
            _modules.Add(module);
        }

        public IEnumerable<T> GetModules<T>()
        {
            return _modules.OfType<T>();
        }

        public T GetModule<T>()
        {
            return _modules.OfType<T>().FirstOrDefault();
        }
    }
}
