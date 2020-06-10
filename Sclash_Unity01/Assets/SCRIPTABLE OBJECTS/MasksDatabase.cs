using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Mask
{
    public string name;
    public Sprite illustration;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "MasksDatabase01", menuName = "Scriptable objects/Masks database")]
public class MasksDatabase : ScriptableObject
{
    public List<Mask> masksList = new List<Mask>();
}
