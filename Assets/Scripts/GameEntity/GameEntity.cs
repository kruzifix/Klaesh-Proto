using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.GameEntity.Module;
using Klaesh.GameEntity.Descriptor;
using UnityEngine;

namespace Klaesh.GameEntity
{
    public interface IGameEntity
    {
        int Id { get; }
        GameEntityDescriptor Descriptor { get; }
        Vector3 Position { get; }

        void Initialize(int id, GameEntityDescriptor descriptor);

        void InitModules();

        void AddModule(object module);
        IEnumerable<T> GetModules<T>();
        T GetModule<T>();
    }

    public class GameEntity : MonoBehaviour, IGameEntity
    {
        private List<object> _modules;

        private bool _initialized = false;

        public int Id { get; private set; }
        public GameEntityDescriptor Descriptor { get; private set; }
        public Vector3 Position => transform.position;

        public void Initialize(int id, GameEntityDescriptor descriptor)
        {
            if (_initialized)
                throw new Exception("GameEntity already initialized");

            _initialized = true;
            Id = id;
            Descriptor = descriptor;

            _modules = new List<object>();
        }

        public void InitModules()
        {
            foreach (var mod in _modules.OfType<IGameEntityModule>())
                mod.Init();
        }

        public void AddModule(object module)
        {
            if (_modules.Contains(module))
                return;
            if (module is IGameEntityModule)
                (module as IGameEntityModule).Owner = this;
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

        //public static void CopyAllComponents<T>(GameObject original, GameObject destination)
        //    where T : Component
        //{
        //    var comps = original.GetComponents<T>();

        //    foreach (var c in comps)
        //    {
        //        CopyComponent(c, destination);
        //    }
        //}

        //public static T CopyComponent<T>(T orig, GameObject dest)
        //    where T : Component
        //{
        //    var type = orig.GetType();
        //    var copy = dest.AddComponent(type);
        //    var fields = type.GetFields();
        //    foreach (var field in fields)
        //    {
        //        field.SetValue(copy, field.GetValue(orig));
        //    }
        //    return copy as T;
        //}
    }
}
