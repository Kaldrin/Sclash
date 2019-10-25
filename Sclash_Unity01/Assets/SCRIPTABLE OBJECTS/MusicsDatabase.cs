using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stem
{
    public AudioClip stemAudio;
    public AudioClip stemStrikesAudio;
    public List<int> stemsThatConnect;
}

[System.Serializable]
public struct Phase
{
    public List<Stem> stems;
}

[System.Serializable]
public struct Music
{
    public List<Phase> phases;
    public AudioClip winAudio;
}


[CreateAssetMenu(fileName = "MusicsDatabase01", menuName = "Scriptable objects/Music database")]
public class MusicsDatabase : ScriptableObject
{
    public List<Music> musicsList = null;
}