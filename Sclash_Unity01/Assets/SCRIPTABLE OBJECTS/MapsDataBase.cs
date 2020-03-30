using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum STAGETYPE
{
    day,
    night,
}

[System.Serializable]
public struct Map
{
    public GameObject mapObject;
    public string stageName;
    public Sprite mapImage;
    public int musicIndex;
    public STAGETYPE type;
    public bool inCustomList;
}

[CreateAssetMenu(fileName = "MapsDatabase01", menuName = "Scriptable objects/Maps database")]
public class MapsDataBase : ScriptableObject
{
    public List<Map> stagesLists = null;
}