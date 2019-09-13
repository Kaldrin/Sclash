using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TipsDatabase01", menuName = "Scriptable objects/Tips database")]
public class TipsDatabase : ScriptableObject
{
    [TextArea]
    public List<string> tipsList = null;
}
