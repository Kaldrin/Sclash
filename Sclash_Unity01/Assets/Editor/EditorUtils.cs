using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorUtils
{
    [MenuItem("Custom Commands/Enable All #%e")]
    public static void EnableAll()
    {
        SetSelectionState(true);
    }

    [MenuItem("Custom Commands/Disable All #%d")]
    public static void DisableAll()
    {
        SetSelectionState(false);
    }

    static void SetSelectionState(bool newState)
    {
        GameObject[] selectedObjs = Selection.gameObjects;
        for (int i = 0; i < selectedObjs.Length; i++)
        {
            selectedObjs[i].SetActive(newState);
        }
    }
}