using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var cam = target as CameraController;

        EditorGUILayout.LabelField("Yaw:", cam.Yaw.ToString());
        EditorGUILayout.LabelField("Pitch:", cam.Pitch.ToString());
        EditorGUILayout.LabelField("Distance:", cam.Distance.ToString());
        EditorGUILayout.LabelField("Target Position:", cam.TargetPos.ToString());
    }
}
