using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatsManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager = null;
    [SerializeField] MapLoader mapLoader = null;
    [SerializeField] Stats statsAsset = null;


    // DATA COMPUTING
    Game currentGame;
    Round currentRound;
    float currentRoundStartTime = 0;
    float currentGameStartTime = 0;








    // Start is called before the first frame update
    void Start()
    {
          LoadStats();
    }

    // Update is called once per frame
    void Update()
    {
        
    }






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
        currentGame.rounds = new List<Round>();


        currentGameStartTime = Time.time;
    }

    public void FinalizeGame(bool finished, int gameType)
    {
        currentGame.roundsPlayed = currentGame.rounds.Count;
        currentGame.duration = Time.time - currentGameStartTime;
        currentGame.gameFinished = finished;
        currentGame.finalScore = gameManager.score;


        GlobalStat newGlobalStats = statsAsset.globalStats[gameType];
        newGlobalStats.gamesList.Add(currentGame);

        statsAsset.globalStats[gameType] = newGlobalStats;


        ComputeGameDatas();
    }






    // ROUNDS
    public void InitializeNewRound()
    {
        currentRound.index = currentGame.rounds.Count;
        currentRound.duration = 0;
        currentRound.winner = 0;
        currentRound.actions = new List<ActionsList>(); // Il semblerai que cette variable soit effacée (= null) à chaque restart
        currentRound.actions.Add(new ActionsList());
        currentRound.actions.Add(new ActionsList());
        currentRound.actions[0].actionsList = new List<Action>();
        currentRound.actions[1].actionsList = new List<Action>();


        currentRoundStartTime = Time.time;
    }

    public void FinalizeRound(int winner)
    {
        currentRound.duration = Time.time - currentRoundStartTime;
        currentRound.winner = winner;


        currentGame.rounds.Add(currentRound);
    }







    // ACTIONS
    public void AddAction(ACTION action, int player, int level)
    {
        Action newAction = new Action();
        newAction.name = action;


        try
        {
            newAction.index = currentRound.actions[player].actionsList.Count; // Ici il y a un problème que je ne comprend pas
        }
        catch
        {
            newAction.index = 0;
            Debug.Log("Action recording didn't work for some reason, ignored");
        }
        newAction.timeCode = Time.time - currentRoundStartTime;
        newAction.level = level;


        try
        {
            currentRound.actions[player].actionsList.Add(newAction);
        }
        catch
        {
            Debug.Log("Action recording didn't work for some reason, ignored");
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


            newGlobalStat.averageFinishedGameDuration = averageFinishedGameDuration / newGlobalStat.totalGamesFinished;


            // Average round duration
            averageRoundDuration = 0;


            for (int i = 0; i < statsAsset.globalStats[o].gamesList.Count; i++)
            {
                for (int y = 0; y < statsAsset.globalStats[o].gamesList[i].rounds.Count; y++)
                {
                    averageRoundDuration += statsAsset.globalStats[o].gamesList[i].rounds[y].duration;
                }
            }


            newGlobalStat.averageRoundDuration = averageRoundDuration / newGlobalStat.totalRoundsPlayed;


            // Average rounds number per finished game
            averageRoundNumberPerFinishedGame = 0;


            for (int i = 0; i < statsAsset.globalStats[o].gamesList.Count; i++)
            {
                if (statsAsset.globalStats[o].gamesList[i].gameFinished)
                    averageRoundNumberPerFinishedGame += statsAsset.globalStats[o].gamesList[i].roundsPlayed;
            }


            newGlobalStat.averageRoundsNumberPerFinishedGame = averageRoundNumberPerFinishedGame / newGlobalStat.totalGamesFinished;



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
                        for (int z = 0; z < newGlobalStat.gamesList[i].rounds[y].actions.Count; z++)    // Problème que j'arrive pas à identifier ici qui softlock le jeu
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
                                        successfulParry++;
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
            statsAsset.globalStats[0].gamesList.Add(statsAsset.globalStats[2].gamesList[i]);
        }


        // Average game duration
        averageFinishedGameDuration = 0;


        for (int i = 0; i < statsAsset.globalStats[0].gamesList.Count; i++)
        {
            if (statsAsset.globalStats[0].gamesList[i].gameFinished)
                averageFinishedGameDuration += statsAsset.globalStats[0].gamesList[i].duration;
        }


        newGlobalStat.averageFinishedGameDuration = averageFinishedGameDuration / newGlobalStat.totalGamesFinished;


        // Average round duration
        averageRoundDuration = 0;


        for (int i = 0; i < statsAsset.globalStats[0].gamesList.Count; i++)
        {
            for (int y = 0; y < statsAsset.globalStats[0].gamesList[i].rounds.Count; y++)
            {
                averageRoundDuration += statsAsset.globalStats[0].gamesList[i].rounds[y].duration;
            }
        }

        newGlobalStat.averageRoundDuration = averageRoundDuration / newGlobalStat.totalRoundsPlayed;


        // Average rounds number per finished game
        averageRoundNumberPerFinishedGame = 0;


        for (int i = 0; i < statsAsset.globalStats[0].gamesList.Count; i++)
        {
            if (statsAsset.globalStats[0].gamesList[i].gameFinished)
                averageRoundNumberPerFinishedGame += statsAsset.globalStats[0].gamesList[i].roundsPlayed;
        }


        newGlobalStat.averageRoundsNumberPerFinishedGame = averageRoundNumberPerFinishedGame / newGlobalStat.totalGamesFinished;


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
    }






    // SAVE & LOADS
    void SaveStats()
    {
        // Save forever
        JsonSave save = SaveGameManager.GetCurrentSave();


        save.stats = statsAsset;
        save.hasBeenSavedOnce = true;

        SaveGameManager.Save();
    }

    void LoadStats()
    {
        // Loads actual saves
        JsonSave save = SaveGameManager.GetCurrentSave();

        if (save.stats != null)
            statsAsset = save.stats;
        else
        {
            ReinitializeAllData();
            SaveStats();
        }
    }
}
