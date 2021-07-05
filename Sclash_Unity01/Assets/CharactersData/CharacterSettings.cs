using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// For Sclash

/// <summary>
/// Scriptable object that stores settings for each characters (Range, images and stuff)
/// </summary>

// UNITY 2019.4
[CreateAssetMenu(fileName = "Character01Settings01", menuName = "Scriptable objects/Character settings")]
public class CharacterSettings : ScriptableObject
{
    public float pommelRange = 0.68f;
    public float attackingMovementsSpeed = 2.2f;
    public Vector2 attack01RangeRange = new Vector2(1.8f, 3.2f);
    public float lightAttackSwordTrailScale = 0.95f;
    public float heavyAttackSwordTrailScale = 1.44f;
    public string nameKey = "";
    public Gradient saberFXColor = new Gradient();
    public Color maxChargeTrailColor = new Color();
    public bool amaterasuHair = false;
}
