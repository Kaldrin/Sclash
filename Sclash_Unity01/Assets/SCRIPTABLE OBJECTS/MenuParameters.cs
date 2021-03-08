using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Store all the settings of the menus
[CreateAssetMenu(fileName = "MenuParameters01", menuName = "Scriptable objects/Menu parameters")]
public class MenuParameters : ScriptableObject
{
    [Header("DEFAULT AUDIO VALUES")]
    public float defaultMasterVolume = 0;
    public float defaultMenuMusicVolume = 0;
    public float defaultBattleMusicVolume = 0;
    public float defaultMenuFXVolume = 0;
    public float defaultFxVolume = 0;
    public float defaultVoiceVolume = 0;

    [Header("MAX AUDIO VALUES")]
    public float maxMasterVolume = 10;
    public float maxMenuMusicVolume = 10;
    public float maxBattleMusicVolume = 10;
    public float maxMenuFXVolume = 10;
    public float maxFxVolume = 10;
    public float maxVoiceVolume = 10;

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

    [Header("ERGONOMY")]
    public bool enableRumbles = true;


    [Header("STAGES")]
    public List<bool> customList = new List<bool>();
    public bool dayNightCycle = false;
    public bool randomStage = false;
    public bool useCustomListForRandom = true;
    public bool keepLastLoadedStage = true;
    public bool useCustomListForRandomStartStage = true;
    public int lastLoadedStageIndex = 0;
}
