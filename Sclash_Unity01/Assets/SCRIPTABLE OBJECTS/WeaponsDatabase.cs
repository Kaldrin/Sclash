using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Store all the weapons referenced in the game, their names and their sprites
[System.Serializable]
public struct Weapon
{
    public string name;
    public Sprite illustration;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "WeaponsDatabase01", menuName = "Scriptable objects/Weapons database")]
public class WeaponsDatabase : ScriptableObject
{
    public List<Weapon> weaponsList = new List<Weapon>();
}
