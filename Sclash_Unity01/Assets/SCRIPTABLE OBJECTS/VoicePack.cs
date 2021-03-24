using System.Collections;
using System.Collections.Generic;
using UnityEngine;




// HEADER
// For Sclash

// REQUIREMENTS

/// <summary>
/// This scriptable object stores settings & references assets for a voice of a character
/// </summary>

// VERSION
// Originally made for Unity 2019.4.14
[CreateAssetMenu(fileName = "VoicePack01", menuName = "Scriptable objects/Voice pack")]
public class VoicePack : ScriptableObject
{
    public AudioClip neutralAttackVoiceClip = null;
    public float neutralAttackPitch = 1;
    public AudioClip frontAttackVoiceClip = null;
    public float frontAttackPitch = 1;
    public AudioClip backAttackVoiceClip = null;
    public float backAttackPitch = 1;
}
