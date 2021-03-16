using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// HEADER
// Reusable script
// For Sclash

// REQUIREMENTS
// Commonly used with the LanguageManager and the TextApparition Scripts alongside the TextMeshPro package, but it won't break if those aren't here

/// <summary>
/// This scriptable object is to centralize the settings to control the characters to avoid & replace when setting the texts for the localization
/// </summary>

// VERSION
// Originally made for Unity 2019.14
[CreateAssetMenu(fileName = "DiacriticsReplacementSettings01", menuName = "Scriptable objects/Diacritics replacement settings")]
public class DiacriticsReplacementSettings : ScriptableObject
{
    [System.Serializable] public struct DiacriticReplacement
    {
        public char characterToReplace;
        public char replacementCharacter;
    }

    [Tooltip("List of characters to replace and their replacement characters")]
    [SerializeField] public List<DiacriticReplacement> charactersToReplace;
}
