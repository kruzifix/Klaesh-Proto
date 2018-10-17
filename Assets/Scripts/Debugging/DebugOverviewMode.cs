using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klaesh.Debugging
{
    public class DebugOverviewMode : AbstractDebugMode
    {
        public override string Name => "Overview";
        public override string Description => string.Empty;

        private DebugView _view;
        private List<AbstractDebugMode> _modes;

        public DebugOverviewMode(DebugView view)
        {
            _view = view;
            _modes = view.Modes.ToList();
        }

        public override void PrintScrollContent()
        {
            foreach (var m in _modes)
            {
                if (GUILayout.Button(m.Name))
                {
                    _view.SetMode(m);
                }
            }
        }

        public override void PrintStaticContent()
        {
            GUILayout.Label(string.Format("Version: {0}", Application.version));
            GUILayout.Label(string.Format("Unity Version: {0}", Application.unityVersion));
            GUILayout.Label(string.Format("Platform: {0}", Application.platform));
        }
    }
}
