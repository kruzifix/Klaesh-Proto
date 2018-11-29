﻿using System;
using System.Collections;
using System.Collections.Generic;
using Klaesh.Utility;
using UnityEditor;
using UnityEngine;

namespace Klaesh.Debugging
{
    public class ObjectPickerDebugMode : AbstractDebugMode
    {
        public override string Name => "Object Picker";

        public override string Description => "View registered Handler";

        private IObjectPicker _picker;

        private int indent;
        private GUIStyle _groupStyle;
        private GUIStyle _keyStyle;
        private GUIStyle _tagStyle;
        private GUIStyle _handlerStyle;
        private GUIStyle _btnStyle;

        public override void RegisterServices()
        {
            base.RegisterServices();

            _picker = _locator.GetService<IObjectPicker>();
        }

        public override void Init()
        {
            _groupStyle = new GUIStyle(EditorStyles.helpBox);

            _keyStyle = new GUIStyle(EditorStyles.largeLabel);
            _keyStyle.normal.textColor = Color.black;

            _tagStyle = new GUIStyle(EditorStyles.boldLabel);
            _tagStyle.normal.textColor = Color.HSVToRGB(0.7f, 0.8f, 0.7f);

            _handlerStyle = new GUIStyle();
            _handlerStyle.normal.textColor = Color.HSVToRGB(0.9f, 0.7f, 0.4f);

            _btnStyle = new GUIStyle(EditorStyles.miniButton);
            _btnStyle.fixedWidth = 17f;
            _btnStyle.margin.right = 10;
            _btnStyle.margin.bottom = 15;
        }

        public override void PrintScrollContent()
        {
            indent = 0;

            foreach (var keyCode in _picker.Handlers)
            {
                GUILayout.BeginVertical(_groupStyle);
                GUILayout.Label(keyCode.Key.ToString(), _keyStyle);

                var handler = keyCode.Value;

                foreach (var pair in handler)
                {
                    indent++;

                    Indent(() => GUILayout.Label(pair.Key, _tagStyle));

                    foreach (var h in pair.Value)
                    {
                        indent++;

                        Indent(() => {
                            _btnStyle.normal.textColor = h.Enabled ? Colors.Forest : Color.red;
                            if (GUILayout.Button(h.Enabled ? "✓" : "✗", _btnStyle))
                                h.Enabled = !h.Enabled;
                            GUILayout.Label(h.ToString(), _handlerStyle);
                            });

                        indent--;
                    }

                    indent--;
                }

                GUILayout.EndVertical();
            }
        }

        public override void PrintStaticContent()
        {
            if (GUILayout.Button("Object Picker enabled: " + (_picker.Enabled ? "✓" : "✗")))
            {
                _picker.Enabled = !_picker.Enabled;
            }
        }

        public override void DebugViewToggled(bool enabled)
        {
            _picker.Enabled = !enabled;
        }

        private void Indent(Action a)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indent * 15f);
            a();
            GUILayout.EndHorizontal();
        }
    }
}
