using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


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
    public Sprite mapImage;
    public int musicIndex;
    public STAGETYPE type;
    public bool inCustomList;
    public PostProcessProfile postProcessProfile;
    public int particleSet;
}

[CreateAssetMenu(fileName = "MapsDatabase01", menuName = "Scriptable objects/Maps database")]
public class MapsDataBase : ScriptableObject
{
    public List<Map> stagesLists = null;
}