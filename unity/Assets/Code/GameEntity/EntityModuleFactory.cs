//using System;
//using System.Collections.Generic;
//using Klaesh.GameEntity.Descriptor;

//namespace Klaesh.GameEntity
//{
//    public interface IEntityModuleFactory
//    {
//        void CreateModules(IEntity entity, ModuleDescriptorBase desc);

//        void RegisterCreator<T>(Action<IEntity, T> creator) where T : ModuleDescriptorBase;
//    }

//    public class EntityModuleFactory : IEntityModuleFactory
//    {
//        private Dictionary<Type, List<Action<IEntity, ModuleDescriptorBase>>> _creators;

//        public EntityModuleFactory()
//        {
//            _creators = new Dictionary<Type, List<Action<IEntity, ModuleDescriptorBase>>>();
//        }

//        public void CreateModules(IEntity entity, ModuleDescriptorBase desc)
//        {
//            Type t = desc.GetType();
//            if (_creators.ContainsKey(t))
//            {
//                foreach (var creator in _creators[t])
//                {
//                    creator(entity, desc);
//                }
//            }
//        }

//        public void RegisterCreator<T>(Action<IEntity, T> creator)
//            where T : ModuleDescriptorBase
//        {
//            Type t = typeof(T);
//            if (!_creators.ContainsKey(t))
//                _creators.Add(t, new List<Action<IEntity, ModuleDescriptorBase>>());
//            var list = _creators[t];
//            list.Add((e, desc) => creator(e, (T)desc));
//        }
//    }
//}
