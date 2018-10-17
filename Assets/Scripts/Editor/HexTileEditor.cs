using Klaesh.Hex;
using UnityEditor;

[CustomEditor(typeof(HexTile))]
public class HexTileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var tile = (HexTile)target;

        EditorGUILayout.LabelField("height:", tile.height.ToString());
        EditorGUILayout.LabelField("Cube Coords (x, y, z):", tile.coord.ToString());
        //EditorGUILayout.LabelField("Axial Coords (q, r):", tile.coord.ToAxial().ToString());
        EditorGUILayout.LabelField("Offset Coords (col, row):", tile.coord.ToOffset().ToString());
    }
}
