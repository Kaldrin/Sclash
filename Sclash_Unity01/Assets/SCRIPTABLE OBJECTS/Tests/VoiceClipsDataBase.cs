using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VoiceClipsDataBase01", menuName = "Scriptable objects/Voice clips databse")]
public class VoiceClipsDataBase : ScriptableObject
{
    [SerializeField] public List<VoiceClip> voiceClips = new List<VoiceClip>();
}
