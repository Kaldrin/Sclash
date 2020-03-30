using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MenuParameters01", menuName = "Scriptable objects/Menu parameters")]
public class MenuParameters : ScriptableObject
{
    [Header("AUDIO")]
    public float masterVolume = 50;
    public float musicVolume = 50;
    public float menuFXVolume = 50;
    public float fxVolume = 50;
    public float voiceVolume = 50;
    [Header("GAME")]
    public int roundToWin = 10;

    [Header("STAGES")]
    public List<bool> customList = new List<bool>();
    public bool dayNightCycle = true;
    public bool randomStage = true;
    public bool useCustomListForRandom = false;
    public bool keepLastLoadedStage = true;
    public bool useCustomListForRandomStartStage = false;
    public int lastLoadedStageIndex = 0;
}
