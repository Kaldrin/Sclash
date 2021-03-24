using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "WalkSoundsList01", menuName = "Scriptable objects/Walk sounds list")]
public class WalkSoundsLists : ScriptableObject
{
    [System.Serializable]
    public struct AudioClipsList
    {
        public string name;
        public Sprite icon;
        public AudioClip[] audioclips;
    }

    public List<AudioClipsList> audioClipsLists = new List<AudioClipsList>();
}
