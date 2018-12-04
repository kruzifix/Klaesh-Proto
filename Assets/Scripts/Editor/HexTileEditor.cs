using Klaesh.Hex;
using UnityEditor;

[CustomEditor(typeof(HexTile))]
public class HexTileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var tile = (HexTile)target;

        EditorGUILayout.LabelField("height:", tile.Height.ToString());
        EditorGUILayout.LabelField("Cube Coords (x, y, z):", tile.Position.ToString());
        EditorGUILayout.LabelField("Offset Coords (col, row):", tile.Position.OffsetCoord.ToString());
    }
}
