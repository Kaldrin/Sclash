using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// This scriptable object is to centralize the settings to control the characters to avoid & replace when setting the texts for the localization
// Originally m de for Unity 2019.14
[CreateAssetMenu(fileName = "DiacriticsReplacementSettings01", menuName = "Scriptable objects/Diacritics replacement settings")]
public class DiacriticsReplacementSettings : ScriptableObject
{
    [System.Serializable] public struct DiacriticReplacement
    {
        public char characterToReplace;
        public char replacementCharacter;
    }

    [SerializeField] public List<DiacriticReplacement> charactersToReplace;
}
