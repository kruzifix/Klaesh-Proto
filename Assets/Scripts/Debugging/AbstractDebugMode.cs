using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaesh.Debugging
{
    public abstract class AbstractDebugMode
    {
        public abstract string Name { get; }
        public abstract string Description { get; }

        protected bool _active = false;

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
