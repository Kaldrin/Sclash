using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores settings for each characters (Range, images and stuff)
[CreateAssetMenu(fileName = "Character01Settings01", menuName = "Scriptable objects/Character settings")]
public class CharacterSettings : ScriptableObject
{
    public float pommelRange = 0.68f;
}
