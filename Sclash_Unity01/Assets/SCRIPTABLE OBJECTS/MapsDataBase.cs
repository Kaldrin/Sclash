using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Map
{
    public GameObject mapObject;
    public string mapName;
    public Sprite mapImage;
    public int musicIndex;
}

[CreateAssetMenu(fileName = "MapsDatabase01", menuName = "Scriptable objects/Maps database")]
public class MapsDataBase : ScriptableObject
{
    public List<Map> mapsList = null;
}