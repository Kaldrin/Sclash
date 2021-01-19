using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores settings for each characters (Range, images and stuff)
[CreateAssetMenu(fileName = "Character01Settings01", menuName = "Scriptable objects/Character settings")]
public class CharacterSettings : ScriptableObject
{
    public float pommelRange = 0.68f;
    public Vector2 attack01RangeRange = new Vector2(1.8f, 3.2f);
    public float lightAttackSwordTrailScale = 0.95f;
    public float heavyAttackSwordTrailScale = 1.44f;
}
