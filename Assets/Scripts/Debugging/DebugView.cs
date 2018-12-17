using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klaesh.Debugging
{
    public class DebugView : MonoBehaviour
    {
        public static DebugView Instance { get; private set; }

        public List<AbstractDebugMode> Modes => _modes;

        private bool _enabled = false;
        private List<AbstractDebugMode> _modes;
        private int _activeModeIndex;
        private Vector2 _scrollPos = Vector2.zero;

        private DebugOverviewMode _overviewMode;

        private GUIStyle _bgStyle;
        private Texture2D _bgTexture;

        private GUIStyle _lblStyle;

        private void Awake()
        {
            if (Instance != null)
                throw new Exception("multiple DebugView instances!");
            Instance = this;
            DontDestroyOnLoad(this);

            CreateViews();

            _modes.ForEach(m => m.RegisterServices());
        }

        private void Start()
        {
            _modes.ForEach(m => m.Init());
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F11))
                ToggleEnable();

            if (_enabled)
            {
                _modes.ForEach(m => m.Update());
            }
        }

        private void OnGUI()
        {
            if (!_enabled)
                return;

            if (_lblStyle == null)
                _lblStyle = new GUIStyle(GUI.skin.label) { fontSize = 30 };

            float w = Screen.width / 2;
            var area = new Rect((Screen.width - w) / 2, 10, w, Screen.height - 20);

            GUILayout.BeginArea(area, GetBgStyle());

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            if (_modes[_activeModeIndex] != _overviewMode && GUILayout.Button("Overview", GUILayout.Width(100f)))
                SetMode(_overviewMode);
            if (GUILayout.Button("Close", GUILayout.Width(100f)))
                ToggleEnable();

            GUILayout.EndHorizontal();

            GUILayout.Label(_modes[_activeModeIndex].Name, _lblStyle);
            GUILayout.Label(_modes[_activeModeIndex].Description);

            _modes[_activeModeIndex].PrintStaticContent();

            GUILayout.EndVertical();

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            _modes[_activeModeIndex].PrintScrollContent();

            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        private void OnDrawGizmos()
        {
            if (_enabled)
                _modes.ForEach(m => m.DrawGizmos());
        }

        private void ToggleEnable()
        {
            _enabled = !_enabled;
            _modes.ForEach(m => m.DebugViewToggled(_enabled));
        }

        private void CreateViews()
        {
            var baseType = typeof(AbstractDebugMode);

            _modes = GetType().Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(baseType)
                    && !t.IsAbstract
                    && t.GetConstructor(new Type[] { }) != null)
                .Select(t => (AbstractDebugMode)Activator.CreateInstance(t))
                .OrderBy(t => t.Name)
                .ToList();

            _overviewMode = new DebugOverviewMode(this);

            _modes.Insert(0, _overviewMode);
            _activeModeIndex = 0;
        }

        private GUIStyle GetBgStyle()
        {
            if (_bgTexture == null)
            {
                _bgTexture = new Texture2D(1, 1);
                _bgTexture.SetPixel(0, 0, new Color(0.4f, 0.4f, 0.5f, 0.7f));
                _bgTexture.Apply();
            }

            if (_bgStyle == null)
            {
                _bgStyle = new GUIStyle();
                _bgStyle.normal.background = _bgTexture;
            }

            return _bgStyle;
        }

        public void SetMode(AbstractDebugMode mode)
        {
            _modes[_activeModeIndex].Hide();
            _activeModeIndex = _modes.IndexOf(mode);
            _modes[_activeModeIndex].Show();
        }
    }
}
