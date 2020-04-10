using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class StatsManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager = null;
    [SerializeField] MapLoader mapLoader = null;
    [SerializeField] Stats statsAsset = null;


    // DATA COMPUTING
    [Header("DATA COMPUTING")]
    Game currentGame = new Game();
    Round currentRound = new Round();
    float currentRoundStartTime = 0;
    float currentGameStartTime = 0;




    [Header("SAVE DATA")]
    [SerializeField] string statsSaveFileName = "StatsAsset.txt";






    // Start is called before the first frame update
    void Start()
    {
        InitalizeNewGame(0);
        //Debug.Log(statsAsset.globalStats[0].gamesList.Count);

        // Intializing variables to prevent null reference exceptions
        InitializeNewRound();
        LoadStats();
    }





    #region GAME
    // GAME
    public void InitalizeNewGame(int gameType)
    {
        currentGame.index = statsAsset.globalStats[gameType].gamesList.Count;
        currentGame.type = statsAsset.globalStats[gameType].statName;
        currentGame.stage = mapLoader.currentMapIndex;
        currentGame.scoreToWin = gameManager.scoreToWin;
        currentGame.roundsPlayed = 0;
        currentGame.duration = 0;
        currentGame.gameFinished = false;
        currentGame.finalScore = new Vector2(0, 0);


        // Date
        currentGame.date.year = DateTime.Now.Year;
        currentGame.date.month = DateTime.Now.Month;
        currentGame.date.day = DateTime.Now.Day;
        currentGame.date.hour = DateTime.Now.Hour;
        currentGame.date.minute = DateTime.Now.Minute;



        // Characters
        currentGame.character1 = 0;
        currentGame.character2 = 0;


        // Rounds
        currentGame.rounds = new List<Round>(0);


        currentGameStartTime = Time.time;
    }

    public void FinalizeGame(bool finished, int gameType)
    {
        try
        {
            currentGame.roundsPlayed = currentGame.rounds.Count; // Bug ici (Null reference exception)
        }
        catch
        {
            Debug.Log("Problem when finalizing the recording of the game, can't save the number of rounds played for some reason");
            currentGame.roundsPlayed = 1;
        }
        currentGame.duration = Time.time - currentGameStartTime;
        currentGame.gameFinished = finished;
        currentGame.finalScore = gameManager.score;


        GlobalStat newGlobalStats = statsAsset.globalStats[gameType];
        try
        {
            newGlobalStats.gamesList.Add(currentGame);
        }
        catch
        {
            Debug.Log("Problem when finalizing the recording of the game, can't save the game element for some reason");
        }

        statsAsset.globalStats[gameType] = newGlobalStats;

        Debug.Log("Finalize game");
        ComputeGameDatas();
    }
    #endregion








    #region ROUNDS
    // ROUNDS
    public void InitializeNewRound()
    {
        try
        {
            currentRound.index = currentGame.rounds.Count; // Bug ici aussi (Null reference exception)
        }
        catch
        {
            Debug.Log("Problem initializing the round, can't set the round's index accessing the current game's round list count for some reason");
            currentRound.index = 0;
        }


        currentRound.duration = 0;
        currentRound.winner = 0;
        currentRound.actions = new List<ActionsList>();
        currentRound.actions.Add(new ActionsList());
        currentRound.actions.Add(new ActionsList());
        currentRound.actions[0].actionsList = new List<Action>();
        currentRound.actions[1].actionsList = new List<Action>();


        currentRoundStartTime = Time.time;

        //Debug.Log("Initialized round");
    }

    public void FinalizeRound(int winner)
    {
        currentRound.duration = Time.time - currentRoundStartTime;
        currentRound.winner = winner;

        try
        {
            currentGame.rounds.Add(currentRound);   // Problème ici (Null reference exception)
        }
        catch
        {
            Debug.Log("Problem when finalizing the recording of the round, can't add the current round to the game's list of rounds for some reason");
        }
    }
    #endregion







    // ACTIONS
    public void AddAction(ACTION action, int player, int level)
    {
        Action newAction = new Action();
        newAction.name = action;


        try
        {
            newAction.index = currentRound.actions[player].actionsList.Count; // Ici il y a un problème que je ne comprend pas (Null reference exception)
        }
        catch
        {
            newAction.index = 0;
            Debug.Log("Problem recording the action, can't set its index depending on the size of the round's action list for this player for some reason");
        }
        newAction.timeCode = Time.time - currentRoundStartTime;
        newAction.level = level;


        try
        {
            currentRound.actions[player].actionsList.Add(newAction); // Problème ici :/ (Null reference exception)
        }
        catch
        {
            Debug.Log("Problem recording the action, can't add it to the current round's action list for some reason");
        }
    }





    // COMPUTING DATA
    void ComputeGameDatas()
    {
        GlobalStat newGlobalStat = new GlobalStat();
        float totalPlayTime = 0;
        int totalGamesFinished = 0;
        int totalRoundsPlayed = 0;
        float averageFinishedGameDuration = 0;
        float averageRoundDuration = 0;
        int averageRoundNumberPerFinishedGame = 0;
















        // ONLINE & LOCAL
        for (int o = 1; o < 3; o++)
        {
            newGlobalStat.statName = statsAsset.globalStats[o].statName;
            newGlobalStat.gamesList = statsAsset.globalStats[o].gamesList;


            // Total play time
            totalPlayTime = 0;


            for (int i = 0; i < statsAsset.globalStats[o].gamesList.Count; i++)
            {
                totalPlayTime += statsAsset.globalStats[o].gamesList[i].duration;
            }


            newGlobalStat.totalPlayTime = totalPlayTime;


            newGlobalStat.totalGamesPlayed = statsAsset.globalStats[o].gamesList.Count;


            // Total games finished
            totalGamesFinished = 0;


            for (int i = 0; i < statsAsset.globalStats[o].gamesList.Count; i++)
            {
                if (statsAsset.globalStats[o].gamesList[i].gameFinished)
                    totalGamesFinished++;
            }


            newGlobalStat.totalGamesFinished = totalGamesFinished;





            // Total rounds played
            totalRoundsPlayed = 0;


            for (int i = 0; i < statsAsset.globalStats[o].gamesList.Count; i++)
            {
                totalRoundsPlayed += statsAsset.globalStats[o].gamesList[i].roundsPlayed;
            }


            newGlobalStat.totalRoundsPlayed = totalRoundsPlayed;



            // Average game duration
            averageFinishedGameDuration = 0;


            for (int i = 0; i < statsAsset.globalStats[o].gamesList.Count; i++)
            {
                if (statsAsset.globalStats[o].gamesList[i].gameFinished)
                    averageFinishedGameDuration += statsAsset.globalStats[o].gamesList[i].duration;
            }

            if (averageFinishedGameDuration / newGlobalStat.totalGamesFinished >= 0)
                newGlobalStat.averageFinishedGameDuration = averageFinishedGameDuration / newGlobalStat.totalGamesFinished;
            else
                newGlobalStat.averageFinishedGameDuration = 0;


            // Average round duration
            averageRoundDuration = 0;


            for (int i = 0; i < statsAsset.globalStats[o].gamesList.Count; i++)
            {
                for (int y = 0; y < statsAsset.globalStats[o].gamesList[i].rounds.Count; y++)
                {
                    averageRoundDuration += statsAsset.globalStats[o].gamesList[i].rounds[y].duration;
                }
            }

            if (averageRoundDuration / newGlobalStat.totalRoundsPlayed >= 0)
                newGlobalStat.averageRoundDuration = averageRoundDuration / newGlobalStat.totalRoundsPlayed;
            else
                newGlobalStat.averageRoundDuration = 0;



            // Average rounds number per finished game
            averageRoundNumberPerFinishedGame = 0;


            for (int i = 0; i < statsAsset.globalStats[o].gamesList.Count; i++)
            {
                if (statsAsset.globalStats[o].gamesList[i].gameFinished)
                    averageRoundNumberPerFinishedGame += statsAsset.globalStats[o].gamesList[i].roundsPlayed;
            }

            if (averageRoundNumberPerFinishedGame / newGlobalStat.totalGamesFinished >= 0)
                newGlobalStat.averageRoundsNumberPerFinishedGame = averageRoundNumberPerFinishedGame / newGlobalStat.totalGamesFinished;
            else
                newGlobalStat.averageRoundsNumberPerFinishedGame = 0;


            // Actions
            int
                charge = 0,
                forwardAttack = 0,
                backwardsAttack = 0,
                neutralAttack = 0,
                death = 0,
                forwardDash = 0,
                backwardsDash = 0,
                pommel = 0,
                successfulPommel = 0,
                parry = 0,
                successfulParry = 0,
                clash = 0,
                dodge = 0;



            for (int i = 0; i < newGlobalStat.gamesList.Count; i++)
            {
                for (int y = 0; y < newGlobalStat.gamesList[i].rounds.Count; y++)
                {
                    try
                    {
                        for (int z = 0; z < newGlobalStat.gamesList[i].rounds[y].actions.Count; z++)    // Problème que j'arrive pas à identifier ici qui softlock le jeu (Null reference exception)
                        {
                            for (int w = 0; w < newGlobalStat.gamesList[i].rounds[y].actions[z].actionsList.Count; w++)
                            {
                                switch (newGlobalStat.gamesList[i].rounds[y].actions[z].actionsList[w].name)
                                {
                                    case ACTION.charge:
                                        charge++;
                                        break;

                                    case ACTION.forwardAttack:
                                        forwardAttack++;
                                        break;

                                    case ACTION.backwardsAttack:
                                        backwardsAttack++;
                                        break;

                                    case ACTION.neutralAttack:
                                        neutralAttack++;
                                        break;

                                    case ACTION.death:
                                        death++;
                                        break;

                                    case ACTION.forwardDash:
                                        forwardDash++;
                                        break;

                                    case ACTION.backwardsDash:
                                        backwardsDash++;
                                        break;

                                    case ACTION.pommel:
                                        pommel++;
                                        break;

                                    case ACTION.successfulPommel:
                                        successfulPommel++;
                                        break;

                                    case ACTION.parry:
                                        parry++;
                                        break;

                                    case ACTION.successfulParry:
                                        successfulParry++;
                                        break;

                                    case ACTION.clash:
                                        clash++;
                                        break;

                                    case ACTION.dodge:
                                        dodge++;
                                        break;
                                }
                            }
                        }
                    }
                    catch
                    {
                        Debug.Log("Problem counting the number of each actions for local or online stats, can't access the list of list of actions for some reason");
                    }
                }
            }


            newGlobalStat.charge = charge;
            newGlobalStat.forwardAttack = forwardAttack;
            newGlobalStat.backwardsAttack = backwardsAttack;
            newGlobalStat.neutralAttack = neutralAttack;
            newGlobalStat.death = death;
            newGlobalStat.forwardDash = forwardDash;
            newGlobalStat.backwardsDash = backwardsDash;
            newGlobalStat.pommel = pommel;
            newGlobalStat.successfulPommel = successfulPommel;
            newGlobalStat.parry = parry;
            newGlobalStat.successfulParry = successfulParry;
            newGlobalStat.clash = clash;
            newGlobalStat.dodge = dodge;



            statsAsset.globalStats[o] = newGlobalStat;
        }





















        // GLOBAL
        newGlobalStat.totalPlayTime = statsAsset.globalStats[1].totalPlayTime + statsAsset.globalStats[2].totalPlayTime;
        newGlobalStat.totalGamesPlayed = statsAsset.globalStats[1].gamesList.Count + statsAsset.globalStats[2].gamesList.Count;
        newGlobalStat.totalGamesFinished = statsAsset.globalStats[1].totalGamesFinished + statsAsset.globalStats[2].totalGamesFinished;

        newGlobalStat.totalRoundsPlayed = statsAsset.globalStats[1].totalRoundsPlayed + statsAsset.globalStats[2].totalRoundsPlayed;






        // Games list
        newGlobalStat.gamesList = statsAsset.globalStats[1].gamesList;


        for (int i = 0; i < statsAsset.globalStats[2].gamesList.Count; i++)
        {
            newGlobalStat.gamesList.Add(statsAsset.globalStats[2].gamesList[i]);
        }





        // Average game duration
        averageFinishedGameDuration = 0;


        for (int i = 0; i < statsAsset.globalStats[0].gamesList.Count; i++)
        {
            if (statsAsset.globalStats[0].gamesList[i].gameFinished)
                averageFinishedGameDuration += statsAsset.globalStats[0].gamesList[i].duration;
        }

        if (averageFinishedGameDuration / newGlobalStat.totalGamesFinished >= 0)
            newGlobalStat.averageFinishedGameDuration = averageFinishedGameDuration / newGlobalStat.totalGamesFinished;
        else
            newGlobalStat.averageFinishedGameDuration = 0;







        // Average round duration
        averageRoundDuration = 0;


        for (int i = 0; i < statsAsset.globalStats[0].gamesList.Count; i++)
        {
            for (int y = 0; y < statsAsset.globalStats[0].gamesList[i].rounds.Count; y++)
            {
                averageRoundDuration += statsAsset.globalStats[0].gamesList[i].rounds[y].duration;
            }
        }

        if (averageRoundDuration / newGlobalStat.totalRoundsPlayed >= 0)
            newGlobalStat.averageRoundDuration = averageRoundDuration / newGlobalStat.totalRoundsPlayed;
        else
            newGlobalStat.averageRoundDuration = 0;







        // Average rounds number per finished game
        averageRoundNumberPerFinishedGame = 0;


        for (int i = 0; i < statsAsset.globalStats[0].gamesList.Count; i++)
        {
            if (statsAsset.globalStats[0].gamesList[i].gameFinished)
                averageRoundNumberPerFinishedGame += statsAsset.globalStats[0].gamesList[i].roundsPlayed;
        }

        if (averageRoundNumberPerFinishedGame / newGlobalStat.totalGamesFinished >= 0)
            newGlobalStat.averageRoundsNumberPerFinishedGame = averageRoundNumberPerFinishedGame / newGlobalStat.totalGamesFinished;
        else
            newGlobalStat.averageRoundsNumberPerFinishedGame = 0;





        // Actions
        newGlobalStat.charge = statsAsset.globalStats[1].charge + statsAsset.globalStats[2].charge;
        newGlobalStat.forwardAttack = statsAsset.globalStats[1].forwardAttack + statsAsset.globalStats[2].forwardAttack;
        newGlobalStat.backwardsAttack = statsAsset.globalStats[1].backwardsAttack + statsAsset.globalStats[2].backwardsAttack;
        newGlobalStat.neutralAttack = statsAsset.globalStats[1].neutralAttack + statsAsset.globalStats[2].neutralAttack;
        newGlobalStat.death = statsAsset.globalStats[1].death + statsAsset.globalStats[2].death;
        newGlobalStat.forwardDash = statsAsset.globalStats[1].forwardDash + statsAsset.globalStats[2].forwardDash;
        newGlobalStat.backwardsDash = statsAsset.globalStats[1].backwardsDash + statsAsset.globalStats[2].backwardsDash;
        newGlobalStat.pommel = statsAsset.globalStats[1].pommel + statsAsset.globalStats[2].pommel;
        newGlobalStat.successfulPommel = statsAsset.globalStats[1].successfulPommel + statsAsset.globalStats[2].successfulPommel;
        newGlobalStat.parry = statsAsset.globalStats[1].parry + statsAsset.globalStats[2].parry;
        newGlobalStat.successfulParry = statsAsset.globalStats[1].successfulParry + statsAsset.globalStats[2].successfulParry;
        newGlobalStat.clash = statsAsset.globalStats[1].clash + statsAsset.globalStats[2].clash;
        newGlobalStat.dodge = statsAsset.globalStats[1].dodge + statsAsset.globalStats[2].dodge;






        // Save the calculations
        statsAsset.globalStats[0] = newGlobalStat;
        SaveStats();
    }







    public void ReinitializeAllData()
    {
        GlobalStat newGlobalStat;


        for (int i = 0; i < statsAsset.globalStats.Count; i++)
        {
            newGlobalStat.statName = statsAsset.globalStats[i].statName;

            newGlobalStat.totalPlayTime = 0;
            newGlobalStat.totalGamesPlayed = 0;
            newGlobalStat.totalGamesFinished = 0;
            newGlobalStat.totalRoundsPlayed = 0;
            newGlobalStat.averageFinishedGameDuration = 0;
            newGlobalStat.averageRoundDuration = 0;
            newGlobalStat.averageRoundsNumberPerFinishedGame = 0;

            newGlobalStat.gamesList = new List<Game>();


            newGlobalStat.charge = 0;
            newGlobalStat.forwardAttack = 0;
            newGlobalStat.backwardsAttack = 0;
            newGlobalStat.neutralAttack = 0;
            newGlobalStat.death = 0;
            newGlobalStat.forwardDash = 0;
            newGlobalStat.backwardsDash = 0;
            newGlobalStat.pommel = 0;
            newGlobalStat.successfulPommel = 0;
            newGlobalStat.parry = 0;
            newGlobalStat.successfulParry = 0;
            newGlobalStat.clash = 0;
            newGlobalStat.dodge = 0;

            statsAsset.globalStats[i] = newGlobalStat;
        }


        SaveStats();
    }






    // SAVE & LOADS
    void SaveStats()
    {
        /*
        // Save forever
        SaveGameManager.Load();
        JsonSave save = SaveGameManager.GetCurrentSave();


        save.stats = statsAsset;
        //save.hasBeenSavedOnce = true;

        Debug.Log("Saved user data");
        Debug.Log(Application.persistentDataPath);
        SaveGameManager.Save();
        */

        string json = JsonUtility.ToJson(statsAsset);
        File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + statsSaveFileName, json);
    }

    void LoadStats()
    {
        /*
        
        // Loads actual saves
        SaveGameManager.Load();
        JsonSave save = SaveGameManager.GetCurrentSave();

        if (save.stats != null)
            statsAsset = save.stats;
        else
        {
            ReinitializeAllData();
            SaveStats();
        }

        Debug.Log("Loaded user data");
        */


        //Stats data = null;
        //Debug.Log(File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + statsSaveFilName));

        if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + statsSaveFileName))
        {
            //data = ScriptableObject.CreateInstance<Stats>();
            string json = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + statsSaveFileName);
            JsonUtility.FromJsonOverwrite(json, statsAsset);

            //statsAsset = data;
        }
        else
        {
            //data = Resources.Load<Stats>("Stats Data");
            ReinitializeAllData();
        }
    }
}
