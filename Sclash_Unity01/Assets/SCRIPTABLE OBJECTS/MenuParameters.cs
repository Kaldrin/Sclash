using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MenuParameters01", menuName = "Scriptable objects/Menu parameters")]
public class MenuParameters : ScriptableObject
{
    public float masterVolume = 50;
    public float musicVolume = 50;
    public float menuFXVolume = 50;
    public float fxVolume = 50;
    public float voiceVolume = 50;
    public int roundToWin = 10;
}
