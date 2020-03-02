using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsMenu : MonoBehaviour
{
    #region VARIABLES
    #region STATMODE
    // STAT MODE
    [Header("STAT MODE")]
    [SerializeField] List<string> statModes = new List<string>() {"GLOBAL", "LOCAL", "ONLINE"};
    [SerializeField] int currentStatMode = 0;
    #endregion




    #region INPUT
    // INPUT
    [Header("INPUT")]
    [SerializeField] string statModeSwitchAxis = "MenuTriggers";
    [SerializeField] float statModeSwitchAxisDeadzone = 0.3f;
    bool canInputModeChange = true;
    #endregion




    [Header("DATA")]
    [SerializeField] Stats stats = null;
    [SerializeField] MapsDataBase mapsDataBase = null;




    [Header("DISPLAYED INFO TYPE")]
    [SerializeField] List<GameObject> statModeDisplayObjects = null;



    #region GAMES INFO
    [Header("GAMES INFO")]
    [Tooltip("The refs to the text mesh pro UGUI components containing the global stats")]
    [SerializeField] TextMeshProUGUI totalPlayTime = null;
    [Tooltip("The refs to the text mesh pro UGUI components containing the global stats")]
    [SerializeField] TextMeshProUGUI
        gamesPlayed = null,
        gamesFinished = null,
        RoundsPlayed = null,
        averageFinishedGameDuration = null,
        AverageRoundDuration = null,
        AverageRoundNumberPerFinishedGame = null,
        charge = null,
        forwardAttack = null,
        backwardsAttacks = null,
        neutralAttack = null,
        death = null,
        forwardDash = null,
        backwardsDash = null,
        pommel = null,
        successfulPommel = null,
        parry = null,
        successfulParry = null,
        clash,
        dodge;

    [SerializeField] Transform gameStatsListParent = null;
    [SerializeField] GameObject gameStatObject = null;
    List<GameStat> gameStatsList = new List<GameStat>();
    #endregion
    #endregion














    #region FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        ChangeStatMode(0);
    }

    // Update is called once per graphic frame
    void Update()
    {
        ManageStatModeChange();
    }






    void ManageStatModeChange()
    {
        if (canInputModeChange && Mathf.Abs(Input.GetAxis(statModeSwitchAxis)) > statModeSwitchAxisDeadzone)
        {
            canInputModeChange = false;


            if (Input.GetAxis(statModeSwitchAxis) < -statModeSwitchAxisDeadzone)
            {
                ChangeStatMode(-1);
            }
            else if (Input.GetAxis(statModeSwitchAxis) > -statModeSwitchAxisDeadzone)
            {
                ChangeStatMode(1);
            } 
        }
        else
        if (Mathf.Abs(Input.GetAxis(statModeSwitchAxis)) < statModeSwitchAxisDeadzone)
        {
            canInputModeChange = true;
        }
    }

    public void ChangeStatMode(int indexIncrementation)
    {
        currentStatMode += indexIncrementation;


        if (currentStatMode < 0)
            currentStatMode = statModes.Count - 1;
        else if (currentStatMode > statModes.Count - 1)
            currentStatMode = 0;


        UpdateAllStatsDisplayedInfos();
    }

    public void UpdateAllStatsDisplayedInfos()
    {
        StartCoroutine(UpdateAllStatsDisplayedInfosCoroutine());
    }

    IEnumerator UpdateAllStatsDisplayedInfosCoroutine()
    {
        yield return new WaitForSeconds(0f);
        UpdateDisplayedInfosTypeIndication();
        UpdateDisplayedGlobalInfos();
        UpdateDisplayedGamesInfos();
    }


    // Changes the indication of the displayed info type (Global, local or online)
    void UpdateDisplayedInfosTypeIndication()
    {
        for (int i = 0; i < statModes.Count; i++)
        {
            if (i == currentStatMode)
                statModeDisplayObjects[i].SetActive(true);
            else
                statModeDisplayObjects[i].SetActive(false);
        }
    }


    public void UpdateDisplayedGlobalInfos()
    {
        // Play time
        float playTimeInSeconds = stats.globalStats[currentStatMode].totalPlayTime;
        int playTimeMinutes = (int)((playTimeInSeconds - playTimeInSeconds % 60) / 60);
        string playTimeMinutesText = playTimeMinutes.ToString("F0");
        int playTimeSeconds = (int)(playTimeInSeconds % 60);
        string playTimeSecondsText = playTimeSeconds.ToString("F0");

        if (playTimeMinutes < 10)
            playTimeMinutesText = "0" + playTimeMinutesText;
        if (playTimeSeconds < 10)
            playTimeSecondsText = "0" + playTimeSecondsText;
        
        totalPlayTime.text = playTimeMinutesText + " : " + playTimeSecondsText;



        gamesPlayed.text = stats.globalStats[currentStatMode].totalGamesPlayed.ToString();
        gamesFinished.text = stats.globalStats[currentStatMode].totalGamesFinished.ToString();
        RoundsPlayed.text = stats.globalStats[currentStatMode].totalRoundsPlayed.ToString();


        

        // Average game duration
        float averageGameDurationInSeconds = stats.globalStats[currentStatMode].averageFinishedGameDuration;
        int averageGameDurationMinutes = (int)((averageGameDurationInSeconds - averageGameDurationInSeconds % 60) / 60);
        string averageGameDurationMinutesText = averageGameDurationMinutes.ToString("F0");
        int averageGameDurationSeconds = (int)(averageGameDurationInSeconds % 60);
        string averageGameDurationSecondsText = averageGameDurationSeconds.ToString("F0");

        if (averageGameDurationMinutes < 10)
            averageGameDurationMinutesText = "0" + averageGameDurationMinutesText;
        if (averageGameDurationSeconds < 10)
            averageGameDurationSecondsText = "0" + averageGameDurationSecondsText;


        averageFinishedGameDuration.text = averageGameDurationMinutesText + " : " + averageGameDurationSecondsText;

        


        // Average round duration
        float averageRoundDurationInSeconds = stats.globalStats[currentStatMode].averageRoundDuration;
        int averageRoundDurationMinutes = (int)((averageRoundDurationInSeconds - averageRoundDurationInSeconds % 60) / 60);
        string averageRoundDurationMinutesText = averageRoundDurationMinutes.ToString("F0");
        int averageRoundDurationSeconds = (int)(averageRoundDurationInSeconds % 60);
        string averageRoundDurationSecondsText = averageRoundDurationSeconds.ToString("F0");

        if (averageRoundDurationMinutes < 10)
            averageRoundDurationMinutesText = "0" + averageRoundDurationMinutesText;
        if (averageRoundDurationSeconds < 10)
            averageRoundDurationSecondsText = "0" + averageRoundDurationSecondsText;

        AverageRoundDuration.text = averageRoundDurationMinutesText + " : " + averageRoundDurationSecondsText;




        AverageRoundNumberPerFinishedGame.text = stats.globalStats[currentStatMode].averageRoundsNumberPerFinishedGame.ToString("F1");




        // Actions
        charge.text = stats.globalStats[currentStatMode].charge.ToString();
        forwardAttack.text = stats.globalStats[currentStatMode].forwardAttack.ToString();
        backwardsAttacks.text = stats.globalStats[currentStatMode].backwardsAttack.ToString();
        neutralAttack.text = stats.globalStats[currentStatMode].neutralAttack.ToString();
        death.text = stats.globalStats[currentStatMode].death.ToString();
        forwardDash.text = stats.globalStats[currentStatMode].forwardDash.ToString();
        backwardsDash.text = stats.globalStats[currentStatMode].backwardsDash.ToString();
        pommel.text = stats.globalStats[currentStatMode].pommel.ToString();
        successfulPommel.text = stats.globalStats[currentStatMode].successfulPommel.ToString();
        parry.text = stats.globalStats[currentStatMode].parry.ToString();
        successfulParry.text = stats.globalStats[currentStatMode].successfulParry.ToString();
        clash.text = stats.globalStats[currentStatMode].clash.ToString();
        dodge.text = stats.globalStats[currentStatMode].dodge.ToString();
    }

    void UpdateDisplayedGamesInfos()
    {
        gameStatObject.SetActive(true);

        GameObject currentlyInstantiatedGameStatObject = null;
        GameStat currentlyInstantiatedGameStat = null;


        for (int i = 0; i < gameStatsListParent.childCount; i++)
        {
            if (gameStatsListParent.GetChild(i).gameObject != gameStatObject)
            {
                Destroy(gameStatsListParent.GetChild(i).gameObject);
            }
        }


        
        for (int i = 0; i < stats.globalStats[currentStatMode].gamesList.Count; i++)
        {
            currentlyInstantiatedGameStatObject = Instantiate(gameStatObject, gameStatsListParent);
            currentlyInstantiatedGameStat = currentlyInstantiatedGameStatObject.GetComponent<GameStat>();





            // Update the game's infos display
            currentlyInstantiatedGameStat.gameIndex.text = (i + 1).ToString();
            currentlyInstantiatedGameStat.stageIllustration.sprite = mapsDataBase.mapsList[stats.globalStats[currentStatMode].gamesList[i].stage].mapImage;
            currentlyInstantiatedGameStat.gameType.text = stats.globalStats[currentStatMode].gamesList[i].type;






            // Date
            currentlyInstantiatedGameStat.hour.text = stats.globalStats[currentStatMode].gamesList[i].date.hour.ToString();

            if (stats.globalStats[currentStatMode].gamesList[i].date.minute < 10)
                currentlyInstantiatedGameStat.minute.text = "0" + stats.globalStats[currentStatMode].gamesList[i].date.minute.ToString();
            else
                currentlyInstantiatedGameStat.minute.text = stats.globalStats[currentStatMode].gamesList[i].date.minute.ToString();


            if (stats.globalStats[currentStatMode].gamesList[i].date.day < 10)
                currentlyInstantiatedGameStat.day.text = "0" + stats.globalStats[currentStatMode].gamesList[i].date.day.ToString();
            else
                currentlyInstantiatedGameStat.day.text = stats.globalStats[currentStatMode].gamesList[i].date.day.ToString();


            if (stats.globalStats[currentStatMode].gamesList[i].date.month < 10)
                currentlyInstantiatedGameStat.month.text = "0" + stats.globalStats[currentStatMode].gamesList[i].date.month.ToString();
            else
                currentlyInstantiatedGameStat.month.text = stats.globalStats[currentStatMode].gamesList[i].date.month.ToString();


            currentlyInstantiatedGameStat.year.text = stats.globalStats[currentStatMode].gamesList[i].date.year.ToString();





            // Characters
            currentlyInstantiatedGameStat.character1Name.text = stats.characters[stats.globalStats[currentStatMode].gamesList[i].character1].name;
            currentlyInstantiatedGameStat.character2Name.text = stats.characters[stats.globalStats[currentStatMode].gamesList[i].character1].name;
            currentlyInstantiatedGameStat.characterIllustration1.sprite = stats.characters[stats.globalStats[currentStatMode].gamesList[i].character1].illustration;
            currentlyInstantiatedGameStat.characterIllustration2.sprite = stats.characters[stats.globalStats[currentStatMode].gamesList[i].character1].illustration;


            currentlyInstantiatedGameStat.finalScore.text = stats.globalStats[currentStatMode].gamesList[i].finalScore.x.ToString() + " / " + stats.globalStats[currentStatMode].gamesList[i].finalScore.y.ToString();
            currentlyInstantiatedGameStat.gameFinished.text = stats.globalStats[currentStatMode].gamesList[i].gameFinished.ToString();
            currentlyInstantiatedGameStat.roundsPlayed.text = stats.globalStats[currentStatMode].gamesList[i].roundsPlayed.ToString();
            currentlyInstantiatedGameStat.scoreToWin.text = stats.globalStats[currentStatMode].gamesList[i].scoreToWin.ToString();



            // Duration
            int minutes = (int)(stats.globalStats[currentStatMode].gamesList[i].duration - (stats.globalStats[currentStatMode].gamesList[i].duration % 60)) % 60;
            int seconds = (int)stats.globalStats[currentStatMode].gamesList[i].duration % 60;
            string minutesText = minutes.ToString();
            string secondsText = seconds.ToString();

            if (minutes < 10)
                minutesText = "0" + minutesText;
            if (seconds < 10)
                secondsText = "0" + secondsText;

            currentlyInstantiatedGameStat.duration.text = minutesText + ":" + secondsText;





            // Rounds
            currentlyInstantiatedGameStat.UpdateRoundsList(stats.globalStats[currentStatMode].gamesList[i]);
        }


        gameStatObject.SetActive(false);
    }
    #endregion
}
