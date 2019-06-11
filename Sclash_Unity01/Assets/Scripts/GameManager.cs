using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // AUDIO MANAGER
    [Header("Audio manager")]
    [SerializeField] string audioManagerName = "GlobalManager";
    AudioManager audioManager = null;





    // CAMERA MANAGER
    [Header("Camera manager")]
    [SerializeField] CameraManager cameraManager = null;





    // MENU
    [Header("Menu")]
    [SerializeField] GameObject mainMenu = null;
    [SerializeField] GameObject blurPanel = null;
    [HideInInspector] public bool paused = false;





    // ROUND
    [Header("Round")]
    [SerializeField] float timeBeforeNextRound = 3;
    [SerializeField] ParticleSystem roundLeaves = null;
    public bool gameStarted = false;






    // PLAYERS
    [Header("Players")]
    [SerializeField] GameObject player = null;
    List<GameObject> playersList = new List<GameObject>();
    [SerializeField] Color[] playersColors = null;






    // EFFECTS
    [Header("Effects")]
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
    [Header("Score")]
    [SerializeField] Text scoreText = null;
    public Vector2 score = new Vector2(0, 0);
    [SerializeField] float scoreShowDuration = 4f;
    [SerializeField] int scoreToWin = 10;
    [SerializeField] DynamicValueTMP scoreToWinMenuValue = null;








    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        // Get audio
        audioManager = GameObject.Find(audioManagerName).GetComponent<AudioManager>();

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
        yield return new WaitForSeconds(0);


        // SOUND
        // Set on the menu music
        audioManager.MenuMusicOn();

        SpawnPlayers();
        yield return new WaitForSeconds(0.5f);
        cameraManager.FindPlayers();
        /*
        yield return new WaitForSeconds(2);
        Play();
        */
    }

    // Begins the StartGame coroutine
    public void Play()
    {
        StartCoroutine(StartGame());
    }

    // Start the game
    IEnumerator StartGame()
    {
        // SOUND
        audioManager.FindPlayers();
        yield return new WaitForSeconds(0.1f);
        audioManager.BattleMusicOn();


        gameStarted = true;

        mainMenu.SetActive(false);
        blurPanel.SetActive(false);

        cameraManager.cameraState = "Battle";
        yield return new WaitForSeconds(0.5f);
        cameraManager.FindPlayers();
        yield return new WaitForSeconds(1);
        cameraManager.actualSmoothMovementsMultiplier = cameraManager.battleSmoothMovementsMultiplier;
    }

    // Spawn the players
    void SpawnPlayers()
    {
        GameObject[] playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");

        for (int i = 0; i < playerSpawns.Length; i++)
        {
            PlayerStats playerStats;
            PlayerAnimations playerAnimations;

            playersList.Add(Instantiate(player, playerSpawns[i].transform.position, playerSpawns[i].transform.rotation));
            playerStats = playersList[i].GetComponent<PlayerStats>();
            playerAnimations = playersList[i].GetComponent<PlayerAnimations>();
            playerStats.playerNum = i + 1;
            playerStats.ResetValues();

            playerAnimations.spriteRenderer.color = playersColors[i];
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

        yield return new WaitForSeconds(1.5f);
        ResetPlayers();
    }

    // Executed when a player dies
    public void Death(int playerNum)
    {
        // Canvas
        for (int i = 0; i < playersList.Count; i++)
        {
            playersList[i].GetComponent<PlayerStats>().staminaSlider.gameObject.SetActive(false);
        }
        

        StartCoroutine(Score(playerNum));
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
    }









    // SCORE
    // Display the current score for a given amount of time
    IEnumerator ShowScore()
    {
        scoreText.gameObject.SetActive(true);
        yield return new WaitForSeconds(scoreShowDuration);
        scoreText.gameObject.SetActive(false);
    }

    // Update score
    IEnumerator Score(int playerNum)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        score[playerNum - 1] += 1;

        scoreText.text = ScoreBuilder();

        yield return new WaitForSecondsRealtime(timeBeforeNextRound);

        
        NextRound();
    }

    // Build the score display message
    string ScoreBuilder()
    {
        string scoreString = "<color=#FF0000>" + score[0].ToString() + "</color> / <color=#0000FF>" + score[1].ToString() + "</color>";
        return scoreString;
    }

    void CheckIfPlayerWon()
    {
        for (int i = 0; i < 2; i++)
        {
            if (score[i] >= scoreToWin)
            {
                Debug.Log("Won");
            }
        }
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
        //cameraManager.cameraState = "Event";

        yield return new WaitForSecondsRealtime(slowMoDuration);

        actualTimeScaleUpdateSmoothness = roundEndTimeScaleFadeSpeed;
        timeScaleObjective = baseTimeScale;
        //cameraManager.cameraState = "Battle";

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

        //Debug.Log(Time.timeScale);
    }






    // SECONDARY FUNCTIONS
    // Compares 2 floats with a range of tolerance
    public static bool FastApproximately(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }
}
