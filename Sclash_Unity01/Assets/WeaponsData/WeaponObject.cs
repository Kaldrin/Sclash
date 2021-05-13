using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Weapon01", menuName = "Scriptable object/Weapon")]
public class WeaponObject : ScriptableObject
{
    public string nameKey;
    public bool sharp = true;
}
