using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DestructibleElement : MonoBehaviour
{
    public enum e_Materials { None, Wood, Stone, Fabric }

    public e_Materials m_Material;

    [SerializeField]
    private AudioClip m_DestroySound;

    public bool m_DestroyComponents;
    public Component[] m_Components;

    void Awake()
    {
        if (!gameObject.CompareTag("Destructible"))
            Debug.LogWarning("Destructible tag not found on the object ! Make sure the tag is on one of the children");

        // gameObject.tag = "Destructible";
        SelectDestroySound();
    }

    public void Cut()
    {
        PlayDestroySound();
        if (m_DestroyComponents)
            DestroyComponents();
    }

    //Select sound depending of the material
    private void SelectDestroySound()
    {
        switch (m_Material)
        {
            case e_Materials.None:
                break;

            case e_Materials.Wood:
                m_DestroySound = Resources.Load<AudioClip>("Sounds/S_HeavyWood");
                break;

            case e_Materials.Stone:
                int availableSound = 2;
                m_DestroySound = Resources.Load<AudioClip>("Sounds/S_Stone" + Random.Range(1, availableSound + 1));
                break;

            case e_Materials.Fabric:
                m_DestroySound = Resources.Load<AudioClip>("Sounds/S_Fabric");
                break;
        }
    }

    private void PlayDestroySound()
    {
        DebrisSoundManager.Instance.AddSound(m_DestroySound);
    }

    private void DestroyComponents()
    {
        foreach (Component c in m_Components)
        {
            Debug.Log(c);
            Destroy(c);
        }
    }
}

[CustomEditor(typeof(DestructibleElement))]
public class DestructibleElementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DestructibleElement script = (DestructibleElement)target;

        SerializedProperty test = serializedObject.FindProperty("m_DestroySound");
        EditorGUILayout.PropertyField(test);

        script.m_DestroyComponents = GUILayout.Toggle(script.m_DestroyComponents, "Destroy components");

        if (script.m_DestroyComponents)
        {
            SerializedProperty cmpnts = serializedObject.FindProperty("m_Components");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(cmpnts, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
