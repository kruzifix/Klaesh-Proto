using System.Collections;
using System.Collections.Generic;
using Klaesh.Core;
using Klaesh.Core.Message;
using UnityEngine;

namespace Klaesh.Debugging
{
    public abstract class AbstractDebugMode
    {
        public abstract string Name { get; }
        public abstract string Description { get; }

        protected bool _active = false;

        protected IServiceLocator _locator;
        protected IMessageBus _bus;

        /// <summary>
        /// Is called in Awake, after all Modes have been constructed.
        /// </summary>
        public virtual void RegisterServices()
        {
            _locator = ServiceLocator.Instance;
            _bus = _locator.GetService<IMessageBus>();
        }

        /// <summary>
        /// Is called in Start.
        /// </summary>
        public virtual void Init() { }

        /// <summary>
        /// Is called when Debugview is enabled / disabled.
        /// </summary>
        public virtual void DebugViewToggled(bool enabled) { }

        public virtual void Show()
        {
            _active = true;
        }

        public virtual void Hide()
        {
            _active = false;
        }

        public virtual void Update() { }
        public virtual void DrawGizmos() { }

        public abstract void PrintScrollContent();
        public abstract void PrintStaticContent();
    }
}
