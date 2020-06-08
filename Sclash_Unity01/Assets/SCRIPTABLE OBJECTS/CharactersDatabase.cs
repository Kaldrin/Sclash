using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.Animations;


[System.Serializable]
public struct Character
{
    public string name;
    public Sprite illustration;
    public RuntimeAnimatorController animator;
    public RuntimeAnimatorController legsAnimator;
}

[CreateAssetMenu(fileName = "CharactersDatabase01", menuName = "Scriptable objects/Characters database")]
public class CharactersDatabase : ScriptableObject
{
    public List<Character> charactersList = new List<Character>();
}
