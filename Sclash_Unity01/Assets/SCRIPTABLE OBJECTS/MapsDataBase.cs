using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;



// HEADER
// For Sclash

// REQUIREMENTS
// Require the PostProcessingStack package to work

/// <summary>
/// Scriptable object to store a list of stages and their properties
/// </summary>

// VERSION
// Originally made for Unity 2019.1.1f1
[CreateAssetMenu(fileName = "MapsDatabase01", menuName = "Scriptable objects/Maps database")]
public class MapsDataBase : ScriptableObject
{
    public List<Map> stagesLists = null;
}


[System.Serializable]
public enum STAGETYPE
{
    day,
    night,
}

[System.Serializable]
public struct Map
{
    public string stageName;
    public string stageNameKey;
    public GameObject mapObject;
    public string prefabName;
    public Sprite mapImage;
    public int musicIndex;
    public STAGETYPE type;
    public bool inCustomList;
    public PostProcessProfile postProcessProfile;
    public int particleSet;
    public int walkStepSFXSet;
}