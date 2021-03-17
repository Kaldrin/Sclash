using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;




public class TestWizard : ScriptableWizard
{
    public GameObject gameObjectToCreateAPrefabOf = null;

    [MenuItem("E.S. Tools/Test wizard")]
    static void CreateWizard()                                                                                                                              // CREATE WIZARD
    {
        ScriptableWizard.DisplayWizard<TestWizard>("Test wizard");
    }



    void OnWizardCreate()
    {
        PrefabUtility.SaveAsPrefabAsset(gameObjectToCreateAPrefabOf, "Assets/Resources/" + gameObjectToCreateAPrefabOf.name + ".prefab");
    }
}
