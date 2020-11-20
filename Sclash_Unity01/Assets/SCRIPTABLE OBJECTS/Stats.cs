using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Store the game st ats at runtime
[System.Serializable]
public struct Date
{
    public int year;
    public int month;
    public int day;
    public int hour;
    public int minute;
}

[System.Serializable]
public enum ACTION
{
    charge,
    forwardAttack,
    backwardsAttack,
    neutralAttack,
    death,
    forwardDash,
    backwardsDash,
    pommel,
    successfulPommel,
    parry,
    successfulParry,
    clash,
    dodge,
}

[System.Serializable]
public struct Actions
{
    public ACTION name;
    public int index;
    public float timeCode;
    public int level;
}

[System.Serializable]
public class ActionsList
{
    public List<Actions> actionsList;
}

[System.Serializable]
public struct Round
{
    public int index;
    public float duration;
    public int winner;
    [SerializeField] public List<ActionsList> actions;
    /*
    [SerializeField] public ActionsList actionsList;
    public int into;
    */
}

[System.Serializable]
public struct Game
{
    public int index;
    public Date date;
    public string type;
    public int stage;
    public int character1;
    public int character2;
    public int scoreToWin;
    public int roundsPlayed;
    public float duration;
    public bool gameFinished;
    public Vector2 finalScore;
    public List<Round> rounds;
}

[System.Serializable]
public struct CharacterStat
{
    public float totalPlayTime;
    public int totalGamesPlayed;
    public int totalGamesFinished;
    public int totalRoundsPlayed;
    public float averageFinishedGameDuration;
    public float averageRoundDuration;
    public float averageRoundsNumberPerFinishedGame;

    public int totalRoundsWon;
    public int totalRoundsLost;
    public int totalGamesWon;
    public int totalGamesLost;

    public int charge;
    public int forwardAttack;
    public int backwardsAttack;
    public int neutralAttack;
    public int death;
    public int forwardDash;
    public int backwardsDash;
    public int pommel;
    public int successfulPommel;
    public int parry;
    public int successfulParry;
}

[System.Serializable]
public struct CharacterStats
{
    public CharacterStat global;
    public CharacterStat local;
    public CharacterStat online;
}

[System.Serializable]
public struct GlobalStat
{
    public string statName;

    public float totalPlayTime;
    public float totalGamesPlayed;
    public float totalGamesFinished;
    public float totalRoundsPlayed;
    public float averageFinishedGameDuration;
    public float averageRoundDuration;
    public float averageRoundsNumberPerFinishedGame;

    public List<Game> gamesList;

    public int charge;
    public int forwardAttack;
    public int backwardsAttack;
    public int neutralAttack;
    public int death;
    public int forwardDash;
    public int backwardsDash;
    public int pommel;
    public int successfulPommel;
    public int parry;
    public int successfulParry;
    public int clash;
    public int dodge;
}

[CreateAssetMenu(fileName = "Stats01", menuName = "Scriptable objects/Stats")]
public class Stats : ScriptableObject
{
    public List<GlobalStat> globalStats = new List<GlobalStat>();

    public List<CharacterStats> characters = new List<CharacterStats>();
}