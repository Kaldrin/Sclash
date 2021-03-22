using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;





// HEADER
// Reusable script
// For Sclash


/// <summary>
/// Scriptable object that stores settings to be used in the game for a rumble event.
/// </summary>

// VERSION
// Originally made for Unity 2019.1.1f1
[CreateAssetMenu(fileName = "NewRumbleSettings01", menuName = "Rumble setting")]
public class RumbleSettings : ScriptableObject
{
    public PlayerIndex playerIndex = PlayerIndex.One;
    public float rumbleMultiplier = 1f;
    public float rumbleDuration = 0.1f;
    public float rumbleStrengthLeft = 0.2f;
    public float rumbleStrengthRight = 0.2f;
    public int rumbleNumber = 2;
    public float betweenRumblesDuration = 0.1f;
    public bool muteRumble = false;
}
