using System.Collections.Generic;
using System.Linq;
using System.Text;
using Klaesh.Game.Cards;
using Klaesh.Hex;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerraformCardData))]
public class TerraformCardDataEditor : Editor
{
    private static GUIContent
        moveButtonContent = new GUIContent("\u21b4", "move down"),
        duplicateButtonContent = new GUIContent("+", "duplicate"),
        deleteButtonContent = new GUIContent("-", "delete"),
        addButtonContent = new GUIContent("+", "add element"),
        upArrowContent = new GUIContent("\u21d1", "raise"),
        downArrowContent = new GUIContent("\u21d3", "lower"),
        deleteBtnContent = new GUIContent("x", "delete");
    private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);

    private const float cellSize = 55f;

    private Texture _hexTexture;
    private Texture _raiseTexture;
    private Texture _lowerTexture;

    private bool _showList = false;

    public override void OnInspectorGUI()
    {
        if (_hexTexture == null)
            _hexTexture = (Texture)EditorGUIUtility.Load("hex_ring_big.png");
        if (_raiseTexture == null)
            _raiseTexture = (Texture)EditorGUIUtility.Load("hex_raise.png");
        if (_lowerTexture == null)
            _lowerTexture = (Texture)EditorGUIUtility.Load("hex_lower.png");
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Art"));

        // https://catlikecoding.com/unity/tutorials/editor/custom-list/
        var list = serializedObject.FindProperty("Tiles");
        EditorGUILayout.PropertyField(list);

        EditorGUI.indentLevel++;

        if (list.isExpanded)
        {
            var tiles = (target as TerraformCardData).Tiles;

            #region Hex grid
            EditorGUILayout.BeginVertical(GUILayout.Height(_hexTexture.height + 20f));

            var cr = EditorGUILayout.GetControlRect();
            var rect = new Rect(cr.width / 2f - _hexTexture.width / 2f, cr.yMax, _hexTexture.width, _hexTexture.height);
            EditorGUI.DrawTextureTransparent(rect, _hexTexture);

            var area = new Rect(0, 0, _raiseTexture.width, _raiseTexture.height);

            foreach (var c in HexFun.Spiral(new HexOffsetCoord(0, 0), 4))
            {
                var p = Hex2Pixel(rect.center, cellSize, c);

                area.x = p.x - area.width / 2f;
                area.y = p.y - area.height / 2f;

                var oc = c.OffsetCoord;
                var vec = new Vector2Int(oc.col, oc.row);

                if (tiles.Where(t => t.coord == vec).Count() == 0 && GUI.Button(area, "+"))
                {
                    list.arraySize += 1;
                    var prop = list.GetArrayElementAtIndex(list.arraySize - 1);

                    var coordProp = prop.FindPropertyRelative("coord");
                    coordProp.vector2IntValue = vec;

                    var amntProp = prop.FindPropertyRelative("amount");
                    amntProp.intValue = 1;
                }
            }

            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i].amount == 0)
                    continue;
                var coord = new HexOffsetCoord(tiles[i].coord.x, tiles[i].coord.y);
                var p = Hex2Pixel(rect.center, cellSize, coord);

                p.y -= 5f;

                area.width = _raiseTexture.width;
                area.x = p.x - area.width / 2f;
                area.y = p.y - area.height * 0.25f;

                EditorGUI.DrawTextureTransparent(area, tiles[i].amount > 0 ? _raiseTexture : _lowerTexture);
                area.y -= area.height * 0.8f;
                area.width += 5f;
                GUI.Label(area, tiles[i].amount.ToString());

                var btn = new Rect(p.x - 8f, p.y + cellSize * 0.3f, 17f, 17f);

                if (GUI.Button(btn, deleteBtnContent))
                {
                    int oldSize = list.arraySize;
                    list.DeleteArrayElementAtIndex(i);
                    if (oldSize == list.arraySize)
                        list.DeleteArrayElementAtIndex(i);
                }
                btn.x = p.x + cellSize * 0.2f;
                btn.y = p.y - 5f;
                if (GUI.Button(btn, upArrowContent))
                {
                    var prop = list.GetArrayElementAtIndex(i);

                    var amntProp = prop.FindPropertyRelative("amount");
                    amntProp.intValue++;
                    if (amntProp.intValue == 0)
                        amntProp.intValue = 1;
                }
                btn.x = p.x - cellSize * 0.47f;
                if (GUI.Button(btn, downArrowContent))
                {
                    var prop = list.GetArrayElementAtIndex(i);

                    var amntProp = prop.FindPropertyRelative("amount");
                    amntProp.intValue--;
                    if (amntProp.intValue == 0)
                        amntProp.intValue = -1;
                }
            }

            EditorGUILayout.EndVertical();
            #endregion

            #region Info / Errors
            var duplicates = new HashSet<Vector2Int>();
            var zeroEntries = new HashSet<Vector2Int>();
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i].amount == 0)
                    zeroEntries.Add(tiles[i].coord);
                for (int j = i + 1; j < tiles.Length; j++)
                {
                    if (tiles[i].coord == tiles[j].coord)
                    {
                        duplicates.Add(tiles[i].coord);
                    }
                }
            }

            if (zeroEntries.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("These coordinates have 0 as change amount:");

                foreach (var d in zeroEntries)
                    sb.AppendLine(d.ToString());

                EditorGUILayout.HelpBox(sb.ToString(), MessageType.Info);
            }

            if (duplicates.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("There are duplicate entries for coordinates:");

                foreach (var d in duplicates)
                    sb.AppendLine(d.ToString());

                EditorGUILayout.HelpBox(sb.ToString(), MessageType.Error);
            }
            #endregion

            #region Listview

            EditorGUILayout.LabelField("Size", list.FindPropertyRelative("Array.size").intValue.ToString());

            _showList = EditorGUILayout.Foldout(_showList, "List", true);
            if (_showList)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < list.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);

                    if (GUILayout.Button(moveButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
                    {
                        list.MoveArrayElement(i, i + 1);
                    }
                    if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonMid, miniButtonWidth))
                    {
                        list.InsertArrayElementAtIndex(i);
                    }
                    if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
                    {
                        int oldSize = list.arraySize;
                        list.DeleteArrayElementAtIndex(i);
                        if (oldSize == list.arraySize)
                            list.DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button(addButtonContent, EditorStyles.miniButton))
                {
                    list.arraySize += 1;
                }
                EditorGUI.indentLevel--;
            }
        }

        EditorGUI.indentLevel--;
        #endregion

        serializedObject.ApplyModifiedProperties();
    }

    private Vector2 Hex2Pixel(Vector2 center, float size, IHexCoord coord)
    {
        var c = coord.OffsetCoord;

        return center + new Vector2(
                c.col + 0.5f * (c.row & 1),
                c.row * 0.87f
            ) * size;
    }
}
