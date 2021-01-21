#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IAScript_Solo))]
public class IAScript_SoloEditor : Editor
{
    public override void OnInspectorGUI()
    {
        IAScript_Solo t = (IAScript_Solo)target;

        base.OnInspectorGUI();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Available actions", EditorStyles.boldLabel);
        foreach (IAScript.Actions a in t.actionsList)
        {
            EditorGUILayout.IntField(a.name, a.weight);
        }

    }
}
#endif