using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Character
{
    public string name;
    public Sprite illustration;
}

[CreateAssetMenu(fileName = "CharactersDatabase01", menuName = "Scriptable objects/Characters database")]
public class CharactersDatabase : ScriptableObject
{
    public List<Character> charactersList = new List<Character>();
}
