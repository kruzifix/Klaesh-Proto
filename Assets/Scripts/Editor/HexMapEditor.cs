using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexMap))]
public class HexMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var map = (HexMap)target;

        GUILayout.Label(string.Format("Cell Width: {0}", map.CellWidth));
        GUILayout.Label(string.Format("Cell Height: {0}", map.CellHeight));

        if (GUILayout.Button("Build Map"))
        {
            map.BuildMap();
        }

        if (GUILayout.Button("Clear Map"))
        {
            map.ClearMap();
        }
    }
}
