using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Player2Mode
{
    public string name;
    public bool AI;
    public IAScript.Difficulty difficulty;
}

[CreateAssetMenu(fileName = "Player2ModesList01", menuName = "Scriptable objects/Player2 modes list")]
public class Player2ModesList : ScriptableObject
{
    public List<Player2Mode> player2modes;
}
