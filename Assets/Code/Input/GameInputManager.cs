using System.Collections.Generic;
using Klaesh.Core;
using UnityEngine;

namespace Klaesh.Input
{
    public interface IGameInputManager
    {
        GameObject CurrentHover { get; }

        void RegisterProcessor(IGameInputProcessor proc);
        void DeRegisterProcessor(IGameInputProcessor proc);

        void OnEnter(GameObject go);
        void OnExit(GameObject go);

        void OnDown(GameObject go);
        void OnUp(GameObject go);

        void OnClick(GameObject go);
    }

    public class GameInputManager : ManagerBehaviour, IGameInputManager
    {
        private List<IGameInputProcessor> _processors;

        public GameObject CurrentHover { get; private set; }

        protected override void OnAwake()
        {
            _processors = new List<IGameInputProcessor>();
            _locator.RegisterSingleton<IGameInputManager>(this);
        }

        public void RegisterProcessor(IGameInputProcessor proc)
        {
            _processors.Add(proc);
        }

        public void DeRegisterProcessor(IGameInputProcessor proc)
        {
            _processors.Remove(proc);
        }

        public void OnEnter(GameObject go)
        {
            CurrentHover = go;

            _processors.ForEach(p => p.OnEnter(go));
        }

        public void OnExit(GameObject go)
        {
            CurrentHover = null;

            _processors.ForEach(p => p.OnExit(go));
        }

        public void OnDown(GameObject go)
        {
            _processors.ForEach(p => p.OnDown(go));
        }

        public void OnUp(GameObject go)
        {
            _processors.ForEach(p => p.OnUp(go));
        }

        public void OnClick(GameObject go)
        {
            _processors.ForEach(p => p.OnClick(go));
        }
    }
}
