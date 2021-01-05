using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StoryPlayer))]
public class PlayerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        Player myscript = (StoryPlayer)target;
        bool showButton = false;

        for (int i = 0; i < myscript.playerColliders.Length; i++)
            if (myscript.playerColliders[i] == null)
                showButton = true;


        if (showButton)
            if (GUILayout.Button("Get Colliders"))
                myscript.GetColliders();


        base.OnInspectorGUI();
    }

}
