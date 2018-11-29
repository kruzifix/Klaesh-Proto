using UnityEditor;
using UnityEngine;

namespace Klaesh.Debugging
{
    public static class DStyles
    {
        public static GUIStyle box { get; private set; }
        public static GUIStyle sqrBtn { get; private set; }

        public static GUIStyle boldLabel { get; private set; }
        public static GUIStyle largeLabel { get; private set; }

        public static GUIStyle miniLabel { get; private set; }
        public static GUIStyle miniBoldLabel { get; private set; }

        static DStyles()
        {
#if UNITY_EDITOR
            box = new GUIStyle(EditorStyles.helpBox);
            sqrBtn = new GUIStyle(EditorStyles.miniButtonMid);
            sqrBtn.fixedWidth = 20f;
            sqrBtn.margin.top = 0;
            sqrBtn.margin.right = 10;
            boldLabel = new GUIStyle(EditorStyles.boldLabel);
            largeLabel = new GUIStyle(EditorStyles.largeLabel);
            miniLabel = new GUIStyle(EditorStyles.miniLabel);
            miniBoldLabel = new GUIStyle(EditorStyles.miniBoldLabel);
#else
            box = new GUIStyle();
            box.border = new RectOffset(4, 4, 4, 4);
            box.margin = new RectOffset(4, 3, 4, 3);
            box.padding = new RectOffset(4, 4, 4, 4);
            box.normal.background = new Texture2D(1, 1);
            box.normal.background.SetPixel(0, 0, new Color(1, 1, 1, 0.8f));

            sqrBtn = new GUIStyle();


            boldLabel = new GUIStyle();
            boldLabel.fontSize = 17;
            boldLabel.padding = new RectOffset(3, 3, 3, 3);
            boldLabel.fontStyle = FontStyle.Bold;

            largeLabel = new GUIStyle();
            largeLabel.padding = new RectOffset(3, 3, 3, 3);
            largeLabel.fontSize = 20;

            miniLabel = new GUIStyle();
            miniLabel.padding = new RectOffset(2, 2, 2, 2);
            miniLabel.fontSize = 14;

            miniBoldLabel = new GUIStyle();
            miniBoldLabel.padding = new RectOffset(2, 2, 2, 2);
            miniBoldLabel.fontSize = 14;
            miniBoldLabel.fontStyle = FontStyle.Bold;
#endif
        }
    }
}
