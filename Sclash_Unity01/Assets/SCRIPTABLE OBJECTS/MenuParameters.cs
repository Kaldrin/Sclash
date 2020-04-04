using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MenuParameters01", menuName = "Scriptable objects/Menu parameters")]
public class MenuParameters : ScriptableObject
{
    [Header("AUDIO")]
    public float masterVolume = 0;
    public float menuMusicVolume = 0;
    public float battleMusicVolume = 0;
    public float menuFXVolume = 0;
    public float fxVolume = 0;
    public float voiceVolume = 0;


    [Header("GAME")]
    public int roundToWin = 5;
    public bool displayHelp = true;


    [Header("STAGES")]
    public List<bool> customList = new List<bool>();
    public bool dayNightCycle = false;
    public bool randomStage = false;
    public bool useCustomListForRandom = true;
    public bool keepLastLoadedStage = true;
    public bool useCustomListForRandomStartStage = true;
    public int lastLoadedStageIndex = 0;
}
