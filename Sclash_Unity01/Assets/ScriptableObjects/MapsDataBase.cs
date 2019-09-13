using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapsDatabase01", menuName = "Scriptable objects/Maps database")]
public class MapsDatabase : ScriptableObject
{
    [TextArea]
    public List<GameObject> mapsList = null;
}

