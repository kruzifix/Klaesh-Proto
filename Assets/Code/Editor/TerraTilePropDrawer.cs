using Klaesh.Game.Cards;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TerraTile))]
public class TerraTilePropDrawer : PropertyDrawer
{
    private static GUIContent 
        upArrowContent = new GUIContent("\u21d1", "raise"),
        downArrowContent = new GUIContent("\u21d3", "lower");
    private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int oldIndent = EditorGUI.indentLevel;
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPos = EditorGUI.PrefixLabel(position, label);
        if (position.height > 16f)
        {
            position.height = 16f;
            EditorGUI.indentLevel++;
            contentPos = EditorGUI.IndentedRect(position);
            contentPos.y += 18f;
        }
        contentPos.width *= 0.6f;
        EditorGUI.indentLevel = 0;
        EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("coord"), GUIContent.none);

        contentPos.x += contentPos.width + 5f;
        contentPos.width *= 0.6f;
        EditorGUIUtility.labelWidth = 35f;
        var amountProp = property.FindPropertyRelative("amount");
        EditorGUI.PropertyField(contentPos, amountProp, new GUIContent("Amnt"));

        if (GUILayout.Button(downArrowContent, EditorStyles.miniButtonLeft, miniButtonWidth))
        {
            amountProp.intValue--;
            if (amountProp.intValue == 0)
                amountProp.intValue = -1;
        }
        if (GUILayout.Button(upArrowContent, EditorStyles.miniButtonRight, miniButtonWidth))
        {
            amountProp.intValue++;
            if (amountProp.intValue == 0)
                amountProp.intValue = 1;
        }

        EditorGUI.EndProperty();
        EditorGUI.indentLevel = oldIndent;
    }
}
