using System.Collections;
using System.Collections.Generic;
using UnityEngine;





// For Sclash

// REQUIREMENTS
// IAScript script

/// <summary>
/// Stores the list of modes for the opponent in duel mode, IA or Player, etc...
/// </summary>

// UNITY 2019.4
[System.Serializable]
public struct Player2Mode
{
    public string name;
    public bool AI;
    public IAScript.Difficulty difficulty;
    public string nameKey;
}

[CreateAssetMenu(fileName = "Player2ModesList01", menuName = "Scriptable objects/Player2 modes list")]
public class Player2ModesList : ScriptableObject
{
    public List<Player2Mode> player2modes;
}
