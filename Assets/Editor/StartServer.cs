using UnityEditor;
using UnityEngine;

public class StartServer
{
    [MenuItem("Klaesh/Open Server Dir")]
    public static void OnOpenServerDir()
    {
        string path = Application.dataPath + "/../Klaesh-Server/";
        EditorUtility.RevealInFinder(path);
    }
}
