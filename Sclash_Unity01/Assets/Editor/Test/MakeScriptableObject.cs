using UnityEngine;
using System.Collections;
using UnityEditor;





public class MakeScriptableObject
{
    [MenuItem("Assets/Create/My Scriptable Object")]
    public static void CreateMyAsset()
    {
        MyScriptableObjectClass asset = ScriptableObject.CreateInstance<MyScriptableObjectClass>();

        asset.objectName = "Super cool name";
        asset.colorIsRandom = true;
        asset.thisColor = Color.green;

        AssetDatabase.CreateAsset(asset, "Assets/SCRIPTABLE OBJECTS/Tests/NewScripableObject.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}