using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


// Created for Unity 2019.1.1f1
public class GameManager : MonoBehaviour
{
    // MANAGERS
    [Header("MANAGERS")]
    // Audio manager
    [SerializeField] string audioManagerName = "GlobalManager";
    AudioManager audioManager = null;

    // Menu manager
    [SerializeField] string menuManagerName = "GlobalManager";
    MenuManager menuManager = null;

    // Camera manager
    [SerializeField] CameraManager cameraManager = null;






    // START
    [Header("START")]
    [SerializeField] float timeBeforeBattleCamera = 2f;





    // MENU
    [Header("MENU")]
    [SerializeField] GameObject mainMenu = null;
    [SerializeField] GameObject blurPanel = null;
    [HideInInspector] public bool paused = false;





    // ROUND
    [Header("ROUND")]
    [SerializeField] float timeBeforeNextRound = 3;
    [SerializeField] ParticleSystem roundLeaves = null;
    public bool gameStarted = false;






    // PLAYERS
    [Header("PLAYERS")]
    [SerializeField] GameObject player = null;
    [HideInInspector] public List<GameObject> playersList = new List<GameObject>();
    [SerializeField] Color[]
        playersColors = null,
        attackSignColors = null;
    [SerializeField] string[] playerNames = {"Red samuraï", "Blue samuraï"};
    [HideInInspector] public bool playerDead = false;






    // EFFECTS
    [Header("EFFECTS")]
    [SerializeField] public float roundEndSlowMoTimeScale = 0.3f;
    [SerializeField]
    public float
        minTimeScale = 0.05f,
        rounEndSlowMoDuration = 2f,
        roundEndTimeScaleFadeSpeed = 0.05f,
        clashSlowMoTimeScale = 0.2f,
        clashSlowMoDuration = 2f,
        clashTimeScaleFadeSpeed = 0.2f,
        parrySlowMoTimeScale = 0.2f,
        parrySlowMoDuration = 2f,
        parryTimeScaleFadeSpeed = 0.2f;
    float actualTimeScaleUpdateSmoothness = 0.05f;
    float
        baseTimeScale = 1,
        timeScaleObjective = 1;






    // SCORE
    [Header("SCORE")] [SerializeField] public Text scoreText = null;
    [SerializeField] public GameObject scoreObject = null;
    public Vector2 score = new Vector2(0, 0);
    [SerializeField] float scoreShowDuration = 4f;
    [SerializeField] public int scoreToWin = 10;
    [SerializeField] DynamicValueTMP scoreToWinMenuValue = null;




    // WIN
    [Header("WIN")]
    [SerializeField] float timeBeforeWinScreenAppears = 2f;




    // CHEATS FOR DEVELOPMENT PURPOSES
    [Header("CHEATS")]
    [SerializeField] public bool cheatCodes = false;














    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        // Get audio
        audioManager = GameObject.Find(audioManagerName).GetComponent<AudioManager>();

        // Get menu manager
        menuManager = GameObject.Find(menuManagerName).GetComponent<MenuManager>();

        // Set variables
        score = new Vector2(0, 0);
        baseTimeScale = Time.timeScale;
        actualTimeScaleUpdateSmoothness = roundEndTimeScaleFadeSpeed;


        // START GAME
        StartCoroutine(SetupGame());
    }

    // FixedUpdate is called 30 times per second
    private void FixedUpdate()
    {
        // EFFECTS
        RunTimeScaleUpdate();

        scoreToWin = scoreToWinMenuValue.value;
    }









    // BEGIN GAME
    // Setup the game before it starts
    IEnumerator SetupGame()
    {
        // SOUND
        // Set on the menu music
        audioManager.ActivateMenuMusic();

        SpawnPlayers();
        yield return new WaitForSeconds(0.5f);
        cameraManager.FindPlayers();
    }

    // Begins the StartMatch coroutine, this function is called by the menu button Sclash
    public void StartMatch()
    {
        StartCoroutine(StartMatchCoroutine());
    }

    // Starts the match, activates the camera cinematic zoom and then switches to battle camera
    IEnumerator StartMatchCoroutine()
    {
        audioManager.FindPlayers();


        yield return new WaitForSeconds(0.1f);
    
      
        paused = false;
        gameStarted = true;
        cameraManager.cameraState = "Battle";

        mainMenu.SetActive(false);
        blurPanel.SetActive(false);
        audioManager.ActivateWind();

        
        yield return new WaitForSeconds(0.5f);


        audioManager.matchBeginsRandomSoundSource.Play();
        cameraManager.FindPlayers();


        yield return new WaitForSeconds(timeBeforeBattleCamera);


        cameraManager.actualSmoothMovementsMultiplier = cameraManager.battleSmoothMovementsMultiplier;
        cameraManager.actualZoomSpeed = cameraManager.battleZoomSpeed;
        cameraManager.actualZoomSmoothDuration = cameraManager.battleZoomSmoothDuration;
    }

    // Starts the DrawSabersCoroutine
    public void SaberDrawn(int playerNum)
    {
        StartCoroutine(SaberDrawnCoroutine(playerNum));
    }

    // A saber has been drawn, stores it and checks if both players have drawn
    IEnumerator SaberDrawnCoroutine(int playerNum)
    {
        yield return new WaitForSecondsRealtime(playersList[playerNum - 1].GetComponent<PlayerAttack>().drawDuration + 0.5f);


        if (!audioManager.battleMusicOn)
        {
            bool allPlayersHaveDrawn = true;

            for (int i = 0; i < playersList.Count; i++)
            {
                if (!playersList[i].GetComponent<PlayerAttack>().hasDrawn)
                    allPlayersHaveDrawn = false;
            }


            if (allPlayersHaveDrawn)
            {
                audioManager.ActivateBattleMusic();
            }
        }
    }

    // Spawn the players
    void SpawnPlayers()
    {
        GameObject[] playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");

        for (int i = 0; i < playerSpawns.Length; i++)
        {
            PlayerStats playerStats;
            PlayerAnimations playerAnimations;
            PlayerAttack playerAttack;

            playersList.Add(Instantiate(player, playerSpawns[i].transform.position, playerSpawns[i].transform.rotation));
            playerStats = playersList[i].GetComponent<PlayerStats>();
            playerAnimations = playersList[i].GetComponent<PlayerAnimations>();
            playerAttack = playersList[i].GetComponent<PlayerAttack>();

            playerStats.playerNum = i + 1;
            playerStats.ResetValues();

            playerAnimations.spriteRenderer.color = playersColors[i];
            playerAnimations.legsSpriteRenderer.color = playersColors[i];
            playerAnimations.spriteRenderer.sortingOrder += 10 * i;
            playerAnimations.legsMask.GetComponent<SpriteMask>().frontSortingOrder = 10 * i + 2;
            playerAnimations.legsMask.GetComponent<SpriteMask>().backSortingOrder = 10 * i - 2;
            ParticleSystem attackSignParticles = playerAttack.attackSign.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule attackSignParticlesMain = attackSignParticles.main;
            attackSignParticlesMain.startColor = attackSignColors[i];
        }
    }








    // END & NEXT ROUND
    // Start NextRoundCoroutine
    public void NextRound()
    {
        StartCoroutine(NextRoundCoroutine());
    }

    // Start next round
    IEnumerator NextRoundCoroutine()
    {
        StartCoroutine(ShowScore());
        roundLeaves.gameObject.SetActive(true);
        roundLeaves.Play();
        audioManager.adjustBattleMusicVolumeDepdendingOnPlayersDistance = true;

        yield return new WaitForSeconds(1.5f);

        audioManager.UpdateMusicPhaseThatShouldPlayDependingOnScore();
        ResetPlayers();


        yield return new WaitForSeconds(1f);


        audioManager.roundBeginsRandomSoundSource.Play();
    }

    // Executed when a player dies, starts the score display and next round parameters
    public void Death(int playerNum)
    {
        playerDead = true;
        StartCoroutine(UpdatePlayersScores(playerNum));
    }

    // Reset all the players' variables
    void ResetPlayers()
    {
        GameObject[] playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");

        for (int i = 0; i < playersList.Count; i++)
        {
            GameObject p = playersList[i];

            p.transform.position = playerSpawns[i].transform.position;
            p.transform.rotation = playerSpawns[i].transform.rotation;
            p.GetComponent<PlayerStats>().ResetValues();
            p.GetComponent<PlayerAnimations>().ResetAnims();
        }


        playerDead = false;
    }







    // RESTART GAME
    // Calls ResetGame coroutine, called by main menu button at the end of the match
    public void ResetGame()
    {
        StartCoroutine(ResetGameCoroutine());
    }
    
    // Resets the match settings and values for a next match
    IEnumerator ResetGameCoroutine()
    {
        gameStarted = false;
        playerDead = false;

        roundLeaves.gameObject.SetActive(true);
        menuManager.pauseMenu.SetActive(false);
        menuManager.winScreen.SetActive(false);
        scoreObject.SetActive(false);
        blurPanel.SetActive(true);

        roundLeaves.Play();


        yield return new WaitForSecondsRealtime(1.5f);


        for (int i = 0; i < playersList.Count; i++)
        {
            playersList[i].GetComponent<PlayerAnimations>().TriggerSneath();
            playersList[i].GetComponent<PlayerAttack>().hasDrawn = false;
        }

        cameraManager.cameraState = "Inactive";
        cameraManager.actualSmoothMovementsMultiplier = cameraManager.cinematicSmoothMovementsMultiplier;
        cameraManager.actualZoomSmoothDuration = cameraManager.cinematicZoomSmoothDuration;
        cameraManager.gameObject.transform.position = cameraManager.cameraArmBasePos;
        cameraManager.cam.transform.position = cameraManager.cameraBasePos;

        menuManager.mainMenu.SetActive(true);

        audioManager.ResetBattleMusicPhase();
        audioManager.ActivateMenuMusic();
        ResetPlayers();
        ResetScore();
    }







    // SCORE
    // Displays the current score for a given amount of time
    IEnumerator ShowScore()
    {
        scoreObject.SetActive(true);
        yield return new WaitForSeconds(scoreShowDuration);
        scoreObject.SetActive(false);
    }

    // Updates players' scores and checks if one has reached the win score
    IEnumerator UpdatePlayersScores(int playerNum)
    {
        score[playerNum - 1] += 1;
       
        yield return new WaitForSecondsRealtime(0f);
        scoreText.text = ScoreBuilder();

        if (score[playerNum - 1] >= scoreToWin)
        {
            /*
            for (int i =0; i < playersList.Count; i++)
            {
                playersList[i].GetComponent<PlayerStats>().canRegenStamina = false;
            }
            */


            audioManager.ActivateWind();
            yield return new WaitForSecondsRealtime(2f);

            gameStarted = false;
            menuManager.winMessage.SetActive(true);
            menuManager.winName.text = playerNames[playerNum - 1];
            menuManager.winName.color = playersColors[playerNum - 1];
            audioManager.adjustBattleMusicVolumeDepdendingOnPlayersDistance = false;

            for (int i = 0; i < playersList.Count; i++)
            {
                playersList[i].GetComponent<PlayerAttack>().hasDrawn = false;
            }

            playersList[playerNum - 1].GetComponent<PlayerAnimations>().TriggerSneath();

            yield return new WaitForSecondsRealtime(timeBeforeWinScreenAppears);

            blurPanel.SetActive(false);
            menuManager.winScreen.SetActive(true);
        }
        else
        {
            yield return new WaitForSecondsRealtime(timeBeforeNextRound);

            NextRound();
        }     
    }

    // Build the score display message
    string ScoreBuilder()
    {
        string scoreString = "<color=#FF0000>" + score[0].ToString() + "</color> / <color=#0000FF>" + score[1].ToString() + "</color>";
        return scoreString;
    }

    // Reset the score and its display
    void ResetScore()
    {
        score = new Vector2(0, 0);
        scoreText.text = ScoreBuilder();
    }

    








    // EFFECTS
    // Start the SlowMo coroutine
    public void SlowMo(float slownMoDuration, float slowMoTimeScale, float fadeSpeed)
    {
        StartCoroutine(SlowMoCoroutine(slownMoDuration, slowMoTimeScale, fadeSpeed));
    }

    // Slow motion for a given duration
    IEnumerator SlowMoCoroutine(float slowMoDuration, float slowMoTimeScale, float fadeSpeed)
    {
        actualTimeScaleUpdateSmoothness = fadeSpeed;
        timeScaleObjective = slowMoTimeScale;
        cameraManager.cameraState = "Event";


        yield return new WaitForSecondsRealtime(slowMoDuration);


        actualTimeScaleUpdateSmoothness = roundEndTimeScaleFadeSpeed;
        timeScaleObjective = baseTimeScale;
        cameraManager.cameraState = "Battle";


        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = timeScaleObjective;
    } 

    // Update the timescale smoothly for smooth slow mo effects in FixedUpdate
    void RunTimeScaleUpdate()
    {
        if (FastApproximately(Time.timeScale, timeScaleObjective, 0.06f) || timeScaleObjective == Time.timeScale)
            Time.timeScale = timeScaleObjective;
        else
            Time.timeScale += actualTimeScaleUpdateSmoothness * Mathf.Sign(timeScaleObjective - Time.timeScale);


        if (Time.timeScale <= minTimeScale)
            Time.timeScale = minTimeScale;
    }






    // SECONDARY FUNCTIONS
    // Compares 2 floats with a range of tolerance
    public static bool FastApproximately(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }
}
