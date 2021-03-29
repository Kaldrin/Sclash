using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;







public class CustomEditorTest : EditorWindow
{
    Texture2D textureToSelect = null;
    bool checkBox = false;
    int spacing = 20;









    [MenuItem("Window/Custom editor test")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(CustomEditorTest));
    }



    void OnEnable()
    {
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("My custom editor window", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();
        

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Wow cool button!", GUILayout.ExpandWidth(true)))
            DoSomething();
        GUILayout.Button("Wow another button!", GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        textureToSelect = EditorGUILayout.ObjectField("Texture to selected", textureToSelect, typeof(Texture2D), false, GUILayout.ExpandWidth(false)) as Texture2D;
        GUILayout.Space(spacing);
        if (textureToSelect != null)
        {
            GUILayout.Label("Wow a texture has been selected", EditorStyles.textArea);
            spacing = EditorGUILayout.IntField("Spacing", spacing, GUILayout.ExpandWidth(true));
        } 
        GUILayout.EndHorizontal();    
    }
    
    void DoSomething()
    {
        Debug.Log("Something!");
    }
}