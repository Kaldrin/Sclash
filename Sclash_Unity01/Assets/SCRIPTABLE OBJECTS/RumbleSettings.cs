using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRumbleSettings01", menuName = "Rumble setting")]
public class RumbleSettings : ScriptableObject
{
    public float rumbleDuration = 0.1f;
    public float rumbleStrengthLeft = 0.2f;
    public float rumbleStrengthRight = 0.2f;
    public int rumbleNumber = 2;
    public float betweenRumblesDuration = 0.1f;
    public bool muteRumble = false;
}
