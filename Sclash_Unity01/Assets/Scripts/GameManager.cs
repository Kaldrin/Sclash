using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


// Created for Unity 2019.1.1f1
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // MANAGERS
    [Header("MANAGERS")]
    [Tooltip("The AudioManager script instance reference")]
    [SerializeField] AudioManager audioManager = null;

    [Tooltip("The MenuManager script instance reference")]
    [SerializeField]  MenuManager menuManager = null;

    [Tooltip("The CameraManager script instance reference")]
    [SerializeField] CameraManager cameraManager = null;

    [Tooltip("The MapLoader script instance reference")]
    [SerializeField] MapLoader mapLoader = null;






    // START
    [Header("START")]
    [Tooltip("The delay before the battle camera values are entered in the camera parameters to make it reactive for battle once the game started, because the smooth camera values stay some time to smooth the zoom towards the scene")]
    [SerializeField] float timeBeforeBattleCameraActivationWhenGameStarts = 2f;





    // MENU
    [Header("MENU")]
    [Tooltip("The main menu object reference")]
    [SerializeField] GameObject mainMenu = null;
    [Tooltip("The blur panel object reference")]
    [SerializeField] GameObject blurPanel = null;
    [HideInInspector] public bool paused = false;





    // ROUND
    [Header("ROUND")]
    [Tooltip("The delay before a new round starts when one has finished and players are waiting")]
    [SerializeField] float timeBeforeNextRound = 3;
    public bool gameStarted = false;






    // PLAYERS
    [Header("PLAYERS")]
    [Tooltip("The player prefab reference")]
    [SerializeField] GameObject player = null;
    [HideInInspector] public List<GameObject> playersList = new List<GameObject>();
    [Tooltip("The colors to identify the players")]
    [SerializeField] Color[]
        playersColors = null,
        attackSignColors = null,
        playerLightsColors = null;
    [Tooltip("The names to identify the players")]
    [SerializeField] string[] playerNames = {"Red samuraï", "Blue samuraï"};
    [HideInInspector] public bool playerDead = false;






    // FX
    [Header("FX")]
    [Tooltip("The CameraShake script instance reference in the scene")]
    [SerializeField] public CameraShake cameraShake = null;
    [Tooltip("The level of time slow down that is activated when a player dies")]
    [SerializeField] public float roundEndSlowMoTimeScale = 0.3f;
    [SerializeField] public float
        minTimeScale = 0.05f,
        rounEndSlowMoDuration = 2f,
        roundEndTimeScaleFadeSpeed = 0.05f,
        clashSlowMoTimeScale = 0.2f,
        clashSlowMoDuration = 2f,
        clashTimeScaleFadeSpeed = 0.2f,
        parrySlowMoTimeScale = 0.2f,
        parrySlowMoDuration = 2f,
        parryTimeScaleFadeSpeed = 0.2f,
        deathCameraShakeDuration = 0.3f;
    [SerializeField] float
        deathVFXFilterDuration = 4f;
    float
        actualTimeScaleUpdateSmoothness = 0.05f,
        baseTimeScale = 1,
        timeScaleObjective = 1;

    bool runTimeScaleUpdate = true;

    [Tooltip("The round transition leaves effect object reference")]
    [SerializeField] public ParticleSystem roundTransitionLeavesFX = null;

    [SerializeField] Material deathFXSpriteMaterial = null;






    // SCORE
    [Header("SCORE")]
    [Tooltip("The score display text mesh pro component reference")]
    [SerializeField] public Text scoreText = null;
    [Tooltip("The score display game object reference")]
    [SerializeField] public GameObject scoreObject = null;
    public Vector2 score = new Vector2(0, 0);
    [Tooltip("The duration the score lasts on screen when a round has finished")]
    [SerializeField] float scoreShowDuration = 4f;
    [Tooltip("The score to reach to win")]
    [SerializeField] public int scoreToWin = 10;
    [Tooltip("The slider component reference in the options menu to change the number of rounds to win")]
    [SerializeField] Slider scoreToWinSlider = null;




    // WIN
    [Header("WIN")]
    [Tooltip("The delay before the win menu screen appears when a player has won")]
    [SerializeField] float timeBeforeWinScreenAppears = 2f;




    // CHEATS FOR DEVELOPMENT PURPOSES
    [Header("CHEATS")]
    [Tooltip("Use cheat codes ?")]
    [SerializeField] public bool cheatCodes = false;
    bool slowedDownTime = false;

    [Tooltip("The key to activate the slow motion cheat")]
    [SerializeField] KeyCode slowTimeKey = KeyCode.Alpha5;
    















    // BASE FUNCTIONS
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set variables
        score = new Vector2(0, 0);
        baseTimeScale = Time.timeScale;
        actualTimeScaleUpdateSmoothness = roundEndTimeScaleFadeSpeed;


        // START GAME
        StartCoroutine(SetupGame());
    }

    // Update is called once per graphic frame
    private void Update()
    {
        if (Input.GetKeyUp(slowTimeKey))
        {
            if (!slowedDownTime)
            {
                Time.timeScale = 0.2f;
                slowedDownTime = true;
                runTimeScaleUpdate = false;
            }
            else
            {
                Time.timeScale = 1;
                slowedDownTime = false;
                runTimeScaleUpdate = true;
            }
        }
    }

    // FixedUpdate is called 50 times per second
    private void FixedUpdate()
    {
        // EFFECTS
        RunTimeScaleUpdate();


        scoreToWin = Mathf.FloorToInt(scoreToWinSlider.value);
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

        Cursor.visible = false;

        
        yield return new WaitForSeconds(0.5f);


        audioManager.matchBeginsRandomSoundSource.Play();
        cameraManager.FindPlayers();


        yield return new WaitForSeconds(timeBeforeBattleCameraActivationWhenGameStarts);


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
            playerAnimations.playerLight.color = playerLightsColors[i];

            playerAnimations.spriteRenderer.sortingOrder = 10 * i;
            playerAnimations.legsSpriteRenderer.sortingOrder = 10 * i;
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
        roundTransitionLeavesFX.gameObject.SetActive(true);
        roundTransitionLeavesFX.Play();
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
        StartCoroutine(ResetGameCoroutine(false));
    }

    // Calls ResetGame coroutine, called by main menu button at the end of the match
    public void ResetGameAndRematch()
    {
        StartCoroutine(ResetGameCoroutine(true));
    }

    // Resets the match settings and values for a next match
    IEnumerator ResetGameCoroutine(bool remathRightAfter)
    {
        gameStarted = false;
        playerDead = false;

        roundTransitionLeavesFX.gameObject.SetActive(false);
        roundTransitionLeavesFX.gameObject.SetActive(true);
        menuManager.pauseMenu.SetActive(false);
        menuManager.winScreen.SetActive(false);
        scoreObject.SetActive(false);


        // Activates the menu blur panel if it is not supposed to start a new match right after
        if (!remathRightAfter)
            blurPanel.SetActive(true);


        roundTransitionLeavesFX.Play();


        yield return new WaitForSecondsRealtime(1.5f);


        for (int i = 0; i < playersList.Count; i++)
        {
            playersList[i].GetComponent<PlayerAnimations>().TriggerSneath(true);
            playersList[i].GetComponent<PlayerAttack>().hasDrawn = false;
        }
        

        cameraManager.cameraState = "Inactive";
        cameraManager.actualSmoothMovementsMultiplier = cameraManager.cinematicSmoothMovementsMultiplier;
        cameraManager.actualZoomSmoothDuration = cameraManager.cinematicZoomSmoothDuration;
        cameraManager.gameObject.transform.position = cameraManager.cameraArmBasePos;
        cameraManager.cam.transform.position = cameraManager.cameraBasePos;


        // Activates the main menu if it is not supposed to start a new match right after
        if (!remathRightAfter)
        {
            audioManager.ActivateMenuMusic();
            menuManager.mainMenu.SetActive(true);
        }


        audioManager.ResetBattleMusicPhase();

        
        ResetPlayers();
        ResetScore();


        // Restarts a new match right after it it finished being set up
        if (remathRightAfter)
            StartMatch();
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


            StartCoroutine(Won(playerNum));
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






    // END GAME
    IEnumerator Won(int playerNum)
    {
        audioManager.ActivateWind();
        yield return new WaitForSecondsRealtime(2f);

        gameStarted = false;
        menuManager.winMessage.SetActive(true);
        menuManager.winName.text = playerNames[playerNum - 1];
        menuManager.winName.color = playersColors[playerNum - 1];
        audioManager.adjustBattleMusicVolumeDepdendingOnPlayersDistance = false;


        if (paused)
            menuManager.SwitchPause();


        for (int i = 0; i < playersList.Count; i++)
        {
            playersList[i].GetComponent<PlayerAttack>().hasDrawn = false;
        }

        playersList[playerNum - 1].GetComponent<PlayerAnimations>().TriggerSneath(false);

        yield return new WaitForSecondsRealtime(timeBeforeWinScreenAppears);

        audioManager.ActivateWinMusic();

        blurPanel.SetActive(false);
        menuManager.winScreen.SetActive(true);
        Cursor.visible = true;
    }

    








    // EFFECTS
    public void StartDeathVFXCoroutine()
    {
        StartCoroutine(DeathVisualEffect());
    }

    IEnumerator DeathVisualEffect()
    {
        // List of all renderers
        SpriteRenderer[] spriteRenderers = GameObject.FindObjectsOfType<SpriteRenderer>();
        MeshRenderer[] meshRenderers = GameObject.FindObjectsOfType<MeshRenderer>();
        ParticleSystem[] particleSystems = GameObject.FindObjectsOfType<ParticleSystem>();
        Light[] lights = GameObject.FindObjectsOfType<Light>();


        // All renderers' original properties storage
        List<Color> spriteRenderersColors = new List<Color>();
        List<Material> spriteRenderersMaterials = new List<Material>();
        List<Color> meshRenderersColors = new List<Color>();
        List<Color> particleSystemsColors = new List<Color>();
        List<float> lightsIntensities = new List<float>();


        // Sets all black
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (!spriteRenderers[i].CompareTag("NonBlackFX"))
            {
                spriteRenderersColors.Add(spriteRenderers[i].color);
                spriteRenderers[i].color = Color.black;

                spriteRenderersMaterials.Add(spriteRenderers[i].material);
                spriteRenderers[i].material = deathFXSpriteMaterial;
            }
        }
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (!meshRenderers[i].CompareTag("NonBlackFX"))
            {
                meshRenderersColors.Add(meshRenderers[i].material.color);
                meshRenderers[i].material.color = Color.black;
            }
        }
        for (int i = 0; i < particleSystems.Length; i++)
        {
            /*
            if (!particleSystems[i].CompareTag("NonBlackFX"))
            {
                
                ParticleSystem.MainModule particleSystemMain = particleSystems[i].main;
                particleSystemsColors.Add(particleSystemMain.startColor.color);
                particleSystemMain.startColor = Color.black;
                
            }
        */
        }
        for (int i = 0; i < lights.Length; i++)
        {
            if (!lights[i].CompareTag("NonBlackFX"))
            {
                lightsIntensities.Add(lights[i].intensity);
                lights[i].intensity = 0;
            }
        }


        // Deactivates background elements for only orange color
        mapLoader.currentMap.transform.Find("DECORS & BIG ELEMENTS").Find("NON BATTLE SCENE").gameObject.SetActive(false);



        yield return new WaitForSeconds(deathVFXFilterDuration);



        // Resets all
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (!spriteRenderers[i].CompareTag("NonBlackFX"))
            {
                spriteRenderers[i].color = spriteRenderersColors[i];
                spriteRenderers[i].material = spriteRenderersMaterials[i];
            }
        }
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (!meshRenderers[i].CompareTag("NonBlackFX"))
            {
                meshRenderers[i].material.color = meshRenderersColors[i];
            }
        }
        for (int i = 0; i < particleSystems.Length; i++)
        {
            /*
            if (!particleSystems[i].CompareTag("NonBlackFX"))
            {
                
                ParticleSystem.MainModule particleSystemMain = particleSystems[i].main;
                particleSystemMain.startColor = particleSystemsColors[i];
                
             }
             */
        }
        for (int i = 0; i < lights.Length; i++)
        {
            if (!lights[i].CompareTag("NonBlackFX"))
            {
                lights[i].intensity = lightsIntensities[i];
            }
        }


        mapLoader.currentMap.transform.Find("DECORS & BIG ELEMENTS").Find("NON BATTLE SCENE").gameObject.SetActive(true);
    }

    // Start the SlowMo coroutine
    public void SlowMo(float slownMoDuration, float slowMoTimeScale, float fadeSpeed)
    {
        StartCoroutine(SlowMoCoroutine(slownMoDuration, slowMoTimeScale, fadeSpeed));
    }

    // Slow motion and zoom for a given duration
    IEnumerator SlowMoCoroutine(float slowMoDuration, float slowMoTimeScale, float fadeSpeed)
    {
        actualTimeScaleUpdateSmoothness = fadeSpeed;
        timeScaleObjective = slowMoTimeScale;


        for (int i = 0; i < audioManager.battleMusicPhaseSources.Length; i++)
        {
            audioManager.battleMusicPhaseSources[i].pitch = slowMoTimeScale;
            audioManager.battleMusicStrikesSources[i].pitch = slowMoTimeScale;
        }
        

        cameraManager.cameraState = "Event";


        yield return new WaitForSecondsRealtime(slowMoDuration);


        actualTimeScaleUpdateSmoothness = roundEndTimeScaleFadeSpeed;
        timeScaleObjective = baseTimeScale;


        for (int i = 0; i < audioManager.battleMusicPhaseSources.Length; i++)
        {
            audioManager.battleMusicPhaseSources[i].pitch = 1;
            audioManager.battleMusicStrikesSources[i].pitch = 1;
        }

        cameraManager.cameraState = "Battle";


        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = timeScaleObjective;
    } 

    // Update the timescale smoothly for smooth slow mo effects in FixedUpdate
    void RunTimeScaleUpdate()
    {
        if (runTimeScaleUpdate)
        {
            if (FastApproximately(Time.timeScale, timeScaleObjective, 0.06f) || timeScaleObjective == Time.timeScale)
                Time.timeScale = timeScaleObjective;
            else
                Time.timeScale += actualTimeScaleUpdateSmoothness * Mathf.Sign(timeScaleObjective - Time.timeScale);


            if (Time.timeScale <= minTimeScale)
                Time.timeScale = minTimeScale;
        }
    }






    // SECONDARY FUNCTIONS
    // Compares 2 floats with a range of tolerance
    public static bool FastApproximately(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }
}
