using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Store the list of the characters and all their properties
[System.Serializable]
public struct Character
{
    public string name;
    public Sprite illustration;
    public RuntimeAnimatorController animator;
    public RuntimeAnimatorController legsAnimator;
    public int defaultMask;
    public int defaultWeapon;
    public bool locked;
}

[CreateAssetMenu(fileName = "CharactersDatabase01", menuName = "Scriptable objects/Characters database")]
public class CharactersDatabase : ScriptableObject
{
    public List<Character> charactersList = new List<Character>();
}
