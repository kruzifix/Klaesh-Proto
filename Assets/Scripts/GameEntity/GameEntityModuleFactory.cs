using System;
using System.Collections.Generic;
using Klaesh.GameEntity.Descriptor;

namespace Klaesh.GameEntity
{
    public interface IGameEntityModuleFactory
    {
        void CreateModules(IGameEntity entity, ModuleDescriptorBase desc);

        void RegisterCreator<T>(Action<IGameEntity, T> creator) where T : ModuleDescriptorBase;
    }

    public class GameEntityModuleFactory : IGameEntityModuleFactory
    {
        private Dictionary<Type, List<Action<IGameEntity, ModuleDescriptorBase>>> _creators;

        public GameEntityModuleFactory()
        {
            _creators = new Dictionary<Type, List<Action<IGameEntity, ModuleDescriptorBase>>>();
        }

        public void CreateModules(IGameEntity entity, ModuleDescriptorBase desc)
        {
            Type t = desc.GetType();
            if (_creators.ContainsKey(t))
            {
                foreach (var creator in _creators[t])
                {
                    creator(entity, desc);
                }
            }
        }

        public void RegisterCreator<T>(Action<IGameEntity, T> creator)
            where T : ModuleDescriptorBase
        {
            Type t = typeof(T);
            if (!_creators.ContainsKey(t))
                _creators.Add(t, new List<Action<IGameEntity, ModuleDescriptorBase>>());
            var list = _creators[t];
            list.Add((e, desc) => creator(e, (T)desc));
        }
    }
}
