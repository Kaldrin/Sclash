using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using Photon.Realtime;

// Created for Unity 2019.1.1f1
public class GameManager : MonoBehaviourPun
{
    #region VARIABLES
    #region MANAGERS
    // MANAGERS
    public static GameManager Instance;
    [Header("MANAGERS")]
    [Tooltip("The AudioManager script instance reference")]
    [SerializeField] AudioManager audioManager = null;

    [Tooltip("The MenuManager script instance reference")]
    [SerializeField] MenuManager menuManager = null;

    [Tooltip("The CameraManager script instance reference")]
    [SerializeField] CameraManager cameraManager = null;

    [Tooltip("The MapLoader script instance reference")]
    [SerializeField] MapLoader mapLoader = null;

    [Tooltip("The CameraShake scripts instances references in the scene")]
    [SerializeField] public CameraShake
        deathCameraShake = null,
        clashCameraShake = null,
        pommelCameraShake = null,
        finalCameraShake = null;

    [SerializeField] StatsManager statsManager = null;
    #endregion





    #region GAME STATE
    public enum GAMESTATE
    {
        menu,
        loading,
        intro,
        game,
        paused,
        roundFinished,
        finished,
    }

    [Header("GAME STATE")]
    [SerializeField] public GAMESTATE gameState = GAMESTATE.menu;
    [HideInInspector] public GAMESTATE oldState = GAMESTATE.menu;
    [HideInInspector] public bool gameStarted;
    # endregion





    #region START
    // START
    [Header("START")]
    [Tooltip("The delay before the battle camera values are entered in the camera parameters to make it reactive for battle once the game started, because the smooth camera values stay some time to smooth the zoom towards the scene")]
    [SerializeField] float timeBeforeBattleCameraActivationWhenGameStarts = 2f;
    # endregion





    # region MENU
    // MENU
    [Header("MENU")]
    [Tooltip("The main menu object reference")]
    [SerializeField] GameObject mainMenu = null;
    [Tooltip("The blur panel object reference")]
    [SerializeField] GameObject blurPanel = null;
    # endregion





    # region SCORE
    // SCORE
    [Header("SCORE")]
    [Tooltip("The score display text mesh pro component reference")]
    [SerializeField] public Text scoreTextComponent = null;
    [SerializeField] TextMeshProUGUI maxScoreTextDisplay = null;

    [Tooltip("The score display game object reference")]
    [SerializeField] public GameObject scoreObject = null;
    [HideInInspector] public Vector2 score = new Vector2(0, 0);
    [Tooltip("The duration the score lasts on screen when a round has finished")]
    [SerializeField] float betweenRoundsScoreShowDuration = 4f;
    [Tooltip("The score to reach to win")]
    [SerializeField] public int scoreToWin = 10;
    [Tooltip("The slider component reference in the options menu to change the number of rounds to win")]
    [SerializeField] Slider scoreToWinSliderComponent = null;
    # endregion





    # region ROUNDS & MATCH
    // ROUND
    [Header("ROUND & MATCH")]
    [Tooltip("The delay before a new round starts when one has finished and players are waiting")]
    [SerializeField] float timeBeforeNextRoundTransitionTriggers = 3;
    [SerializeField] float resetGameDelay = 1.5f;
    #endregion





    # region WIN
    // WIN
    [Header("WIN")]
    [Tooltip("The delay before the win menu screen appears when a player has won")]
    [SerializeField] float timeBeforeWinScreenAppears = 2f;
    # endregion





    #region PLAYERS
    // PLAYERS
    [Header("PLAYERS")]
    [Tooltip("The player prefab reference")]
    [SerializeField] GameObject player = null;
    [Tooltip("The references to the spawn objects of the players in the scene")]
    [SerializeField] public GameObject[] playerSpawns = { null, null };
    public List<GameObject> playersList = new List<GameObject>(2);

    [Tooltip("The colors to identify the players")]
    [SerializeField]
    public Color[]
        playersColors = { Color.red, Color.yellow },
        attackSignColors = { Color.red, Color.yellow },
        playerLightsColors = { Color.red, Color.yellow };
    [Tooltip("The names to identify the players")]
    [SerializeField] string[] playerNames = {"Aka", "Ao"};
    [HideInInspector] public bool playerDead = false;
    bool allPlayersHaveDrawn = false;
    # endregion





    # region FX
    // FX
    [Header("FX")]
    [Tooltip("The level of time slow down that is activated when a player dies")]
    [SerializeField] public float roundEndSlowMoTimeScale = 0.2f;
    [SerializeField]
    public float
        minTimeScale = 0.05f,
        roundEndSlowMoDuration = 1.3f,
        roundEndTimeScaleFadeSpeed = 0.05f,
        gameEndSlowMoTimeScale = 0.1f,
        gameEndSlowMoDuration = 0.5f,
        gameEndTimeScaleFadeSpeed = 0.2f,
        clashSlowMoTimeScale = 0.1f,
        clashSlowMoDuration = 0.5f,
        clashTimeScaleFadeSpeed = 0.2f,
        parrySlowMoTimeScale = 0.2f,
        parrySlowMoDuration = 2f,
        parryTimeScaleFadeSpeed = 0.2f,
        dodgeSlowMoTimeScale = 0.2f,
        dodgeSlowMoDuration = 2f,
        dodgeTimeScaleFadeSpeed = 0.2f,
        deathCameraShakeDuration = 0.3f,
        clashCameraShakeDuration = 0.3f,
        pommelCameraShakeDuration = 0.3f,
        finalCameraShakeDuration = 0.7f;
    


    float
        actualTimeScaleUpdateSmoothness = 0.05f,
        baseTimeScale = 1,
        timeScaleObjective = 1;

    bool runTimeScaleUpdate = true;

    [Tooltip("The round transition leaves effect object reference")]
    [SerializeField] public ParticleSystem
        roundTransitionLeavesFX = null,
        animeLinesFx = null;
    [SerializeField] ParticleSystem hajimeFX = null;
    # endregion





    # region DEATH VFX
    // DEATH VFX
    [Header("DEATH VFX")]
    [Tooltip("The duration of the death VFX black & orange screen before it comes back to normal")]
    [SerializeField] float deathVFXFilterDuration = 3.5f;
    [Tooltip("The material that is put on the sprites when the death VFX orange & black screen appears")]
    [SerializeField] Material deathFXSpriteMaterial = null;
    [SerializeField] Color deathVFXElementsColor = Color.black;
    [SerializeField] Gradient deathVFXGradientForParticles = null;

    // List of all renderers for the death VFX
    SpriteRenderer[] spriteRenderers = null;
    MeshRenderer[] meshRenderers = null;
    ParticleSystem[] particleSystems = null;
    Light[] lights = null;

    // All renderers' original properties storage for the death VFX reset
    List<Color> originalSpriteRenderersColors = new List<Color>();
    List<Material> originalSpriteRenderersMaterials = new List<Material>();
    List<Color> originalMeshRenderersColors = new List<Color>();
    List<Color> originalParticleSystemsColors = new List<Color>();
    List<Gradient> originalParticleSystemsGradients = new List<Gradient>();
    List<float> originalLightsIntensities = new List<float>();
    #endregion






    #region CHEATS FOR DEVELOPMENT PURPOSES
    // CHEATS FOR DEVELOPMENT PURPOSES
    [Header("CHEATS")]
    [Tooltip("Use cheat codes ?")]
    [SerializeField] public bool cheatCodes = false;
    bool slowedDownTime = false;

    int timeSlowDownLevel = 0;

    [SerializeField] float[] timeSlowDownSteps = null;

    [Tooltip("The key to activate the slow motion cheat")]
    [SerializeField] KeyCode slowTimeKey = KeyCode.Alpha5;
    #endregion
    #endregion





















    # region FUNCTIONS
    # region BASE FUNCTIONS
    // BASE FUNCTIONS
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    public void Start()
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
        switch (gameState)
        {
            case GAMESTATE.menu:
                break;

            case GAMESTATE.loading:
                break;

            case GAMESTATE.intro:
                break;

            case GAMESTATE.game:
                break;

            case GAMESTATE.paused:
                break;

            case GAMESTATE.finished:
                break;
        }


        if (cheatCodes)
        {
            if (Input.GetKeyUp(slowTimeKey))
            {
                if (timeSlowDownSteps != null)
                {
                    timeSlowDownLevel++;
                    runTimeScaleUpdate = false;


                    if (timeSlowDownLevel >= timeSlowDownSteps.Length)
                    {
                        timeSlowDownLevel = -1;
                        runTimeScaleUpdate = true;
                    }


                    if (timeSlowDownLevel >= 0)
                        Time.timeScale = timeSlowDownSteps[timeSlowDownLevel];
                    else
                        Time.timeScale = 1;
                }
            }
        }
    }

    // FixedUpdate is called 50 times per second
    private void FixedUpdate()
    {
        // EFFECTS
        RunTimeScaleUpdate();


        scoreToWin = Mathf.FloorToInt(scoreToWinSliderComponent.value);
    }
    #endregion








    #region GAME STATE
    // GAMESTATE
    public void SwitchState(GAMESTATE newState)
    {
        oldState = gameState;
        gameState = newState;


        switch (gameState)
        {
            case GAMESTATE.menu:
                break;

            case GAMESTATE.loading:
                playerDead = false;
                menuManager.pauseMenu.SetActive(false);
                menuManager.winScreen.SetActive(false);
                scoreObject.GetComponent<Animator>().SetBool("On", false);
                Cursor.visible = false;
                break;

            case GAMESTATE.intro:
                break;

            case GAMESTATE.game:
                if (oldState == GAMESTATE.paused)
                {
                    for (int i = 0; i < playersList.Count; i++)
                    {
                        playersList[i].GetComponent<Player>().SwitchState(playersList[i].GetComponent<Player>().oldState);
                        playersList[i].GetComponent<PlayerAnimations>().animator.speed = 1;
                    }
                }
                cameraManager.SwitchState(CameraManager.CAMERASTATE.battle);
                mainMenu.SetActive(false);
                blurPanel.SetActive(false);
                Cursor.visible = false;
                break;

            case GAMESTATE.paused:
                for (int i = 0; i < playersList.Count; i++)
                {
                    playersList[i].GetComponent<Player>().SwitchState(Player.STATE.frozen);
                    playersList[i].GetComponent<PlayerAnimations>().animator.speed = 0;
                }
                break;

            case GAMESTATE.finished:
                menuManager.winMessage.SetActive(true);
                if (oldState == GAMESTATE.paused)
                    menuManager.SwitchPause();
                break;
        }
    }
    # endregion








    #region BEGIN GAME
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
    public IEnumerator StartMatchCoroutine()
    {
        // FX
        hajimeFX.Play();


        // AUDIO
        audioManager.FindPlayers();


        maxScoreTextDisplay.text = scoreToWin.ToString();


        for (int i = 0; i < playersList.Count; i++)
        {
            playersList[i].GetComponent<Player>().SwitchState(Player.STATE.sneathed);
        }


        yield return new WaitForSeconds(0.1f);


        SwitchState(GAMESTATE.game);
        cameraManager.SwitchState(CameraManager.CAMERASTATE.battle);


        // AUDIO
        audioManager.ActivateWind();


        yield return new WaitForSeconds(0.5f);


        //cameraManager.FindPlayers();


        // AUDIO
        //audioManager.matchBeginsRandomSoundSource.Play();


        yield return new WaitForSeconds(timeBeforeBattleCameraActivationWhenGameStarts);


        cameraManager.actualXSmoothMovementsMultiplier = cameraManager.battleXSmoothMovementsMultiplier;
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
        yield return new WaitForSecondsRealtime(playersList[playerNum].GetComponent<Player>().drawDuration + 0.5f);


        if (!audioManager.battleMusicOn)
        {
            allPlayersHaveDrawn = true;

            for (int i = 0; i < playersList.Count; i++)
            {
                if (playersList[i].GetComponent<Player>().playerState != Player.STATE.normal)
                    allPlayersHaveDrawn = false;
            }


            if (allPlayersHaveDrawn)
            {
                audioManager.ActivateBattleMusic();


                // STATS
                statsManager.InitalizeNewGame(1);
                statsManager.InitializeNewRound();
            }
        }
    }

    #endregion







    #region PLAYERS
    // PLAYERS
    // Spawns the players
    void SpawnPlayers()
    {
        for (int i = 0; i < playerSpawns.Length; i++)
        {
            //PlayerStats playerStats;
            PlayerAnimations playerAnimations;
            //PlayerAttack playerAttack;
            Player playerScript = null;

            playersList.Add(Instantiate(player, playerSpawns[i].transform.position, playerSpawns[i].transform.rotation));
            //playerStats = playersList[i].GetComponent<PlayerStats>();
            playerAnimations = playersList[i].GetComponent<PlayerAnimations>();
            //playerAttack = playersList[i].GetComponent<PlayerAttack>();
            playerScript = playersList[i].GetComponent<Player>();

            //playerStats.playerNum = i + 1;
            //playerStats.ResetValues();
            playerScript.playerNum = i;
            playerScript.ResetAllPlayerValuesForNextMatch();





            // ANIMATIONS
            playerAnimations.spriteRenderer.color = playersColors[i];
            playerAnimations.legsSpriteRenderer.color = playersColors[i];

            playerAnimations.spriteRenderer.sortingOrder = 10 * i;
            playerAnimations.legsSpriteRenderer.sortingOrder = 10 * i;
            playerAnimations.legsMask.GetComponent<SpriteMask>().frontSortingOrder = 10 * i + 2;
            playerAnimations.legsMask.GetComponent<SpriteMask>().backSortingOrder = 10 * i - 2;


            // FX
            //ParticleSystem attackSignParticles = playerAttack.attackSign.GetComponent<ParticleSystem>();
            ParticleSystem attackSignParticles = playerScript.attackRangeFX.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule attackSignParticlesMain = attackSignParticles.main;
            attackSignParticlesMain.startColor = attackSignColors[i];


            playerScript.playerLight.color = playerLightsColors[i];
        }
    }

    // Reset all the players' variables for next round
    void ResetPlayersForNextMatch()
    {
        GameObject[] playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");


        for (int i = 0; i < playersList.Count; i++)
        {
            GameObject p = playersList[i];


            playersList[i].GetComponent<PlayerAnimations>().TriggerSneath();
            playersList[i].GetComponent<PlayerAnimations>().ResetDrawText();
            //playersList[i].GetComponent<PlayerAttack>().hasDrawn = false;
            //playersList[i].GetComponent<Player>().SwitchState(Player.STATE.frozen);


            p.transform.position = playerSpawns[i].transform.position;
            p.transform.rotation = playerSpawns[i].transform.rotation;
            //p.GetComponent<PlayerStats>().ResetValues();
            p.GetComponent<PlayerAnimations>().ResetAnimsForNextRound();


            p.GetComponent<Player>().ResetAllPlayerValuesForNextMatch();
        }


        playerDead = false;
    }

    // Reset all the players' variables for next round
    void ResetPlayersForNextRound()
    {
        GameObject[] playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");


        for (int i = 0; i < playersList.Count; i++)
        {
            GameObject p = playersList[i];


            p.transform.position = playerSpawns[i].transform.position;
            p.transform.rotation = playerSpawns[i].transform.rotation;
            //p.GetComponent<PlayerStats>().ResetValues();
            p.GetComponent<PlayerAnimations>().ResetAnimsForNextRound();
            p.GetComponent<Player>().SwitchState(Player.STATE.normal);


            p.GetComponent<Player>().ResetAllPlayerValuesForNextRound();
        }


        playerDead = false;
    }

    public GameObject GetOtherPlayer(GameObject o)
    {
        for (int i = 0; i < playersList.Count; i++)
        {
            if (playersList[i] == o)
            {
                return o;
            }
        }

        return null;
    }

    # endregion








    #region ROUND TO ROUND & SCORE
    // ROUND TO ROUND & SCORE
    // Executed when a player dies, starts the score display and next round parameters
    public void APlayerIsDead(int winningPlayerIndex)
    {
        // STATS
        try
        {
            statsManager.FinalizeRound(winningPlayerIndex);
        }
        catch
        {
            Debug.Log("Error while finalizing the recording of the current round, ignoring");
        }



        playerDead = true;
        UpdatePlayersScoreValues(winningPlayerIndex);
        SwitchState(GAMESTATE.roundFinished);


        if (CheckIfThePlayerWon(winningPlayerIndex))
        {
            StartCoroutine(APlayerWon(winningPlayerIndex));
        }
        else
        {
            StartCoroutine(NextRoundCoroutine());
        }
    }

    void UpdatePlayersScoreValues(int winningPlayerIndex)
    {
        score[winningPlayerIndex] += 1;
        scoreTextComponent.text = ScoreBuilder();
    }

    // Builds the score display message
    string ScoreBuilder()
    {
        string scoreString = "<color=#FF0000>" + score[0].ToString() + "</color> / <color=#0000FF>" + score[1].ToString() + "</color>";
        return scoreString;
    }

    bool CheckIfThePlayerWon(int winningPlayerIndex)
    {
        if (score[winningPlayerIndex] >= scoreToWin)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Start next round
    IEnumerator NextRoundCoroutine()
    {
        Debug.Log("Next round");
        yield return new WaitForSecondsRealtime(timeBeforeNextRoundTransitionTriggers);


        StartCoroutine(ShowScoreBetweenRoundsCoroutine());


        // FX
        roundTransitionLeavesFX.Play();


        // AUDIO
        audioManager.adjustBattleMusicVolumeDepdendingOnPlayersDistance = true;


        yield return new WaitForSeconds(1.5f);


        ResetPlayersForNextRound();


        // AUDIO
        audioManager.UpdateMusicPhaseThatShouldPlayDependingOnScore();

        
        // STATS
        try
        {
            statsManager.InitializeNewRound();
        }
        catch
        {
            Debug.Log("Error while initializing a new round, ignoring");
        }


        yield return new WaitForSeconds(1f);


        SwitchState(GAMESTATE.game);


        // AUDIO
        audioManager.roundBeginsRandomSoundSource.Play();
    }


    // SCORE
    // Displays the current score for a given amount of time
    IEnumerator ShowScoreBetweenRoundsCoroutine()
    {
        scoreObject.GetComponent<Animator>().SetBool("On", true);
        yield return new WaitForSeconds(betweenRoundsScoreShowDuration);
        scoreObject.GetComponent<Animator>().SetBool("On", false);
    }


    // Reset the score and its display
    void ResetScore()
    {
        score = new Vector2(0, 0);
        scoreTextComponent.text = ScoreBuilder();
    }
    #endregion







    #region RESTART GAME
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
    IEnumerator ResetGameCoroutine(bool rematchRightAfter)
    {
        // STATS
        if (gameState != GAMESTATE.finished && allPlayersHaveDrawn)
        {
            try
            {
                statsManager.FinalizeGame(false, 1);
            }
            catch
            {
                Debug.Log("Error while finilizing the recording of the current game, ignoring");
            }
        }


        // ONLINE
        if (photonView != null && ConnectManager.Instance.enableMultiplayer)
        {
            PhotonNetwork.LeaveRoom();
        }


        SwitchState(GAMESTATE.loading);


        // Activates the menu blur panel if it is not supposed to start a new match right after
        if (!rematchRightAfter)
            blurPanel.SetActive(true);


        // FX
        roundTransitionLeavesFX.Play();


        yield return new WaitForSecondsRealtime(resetGameDelay);

        SwitchState(GAMESTATE.menu);
        ResetPlayersForNextMatch();
        TriggerMatchEndFilterEffect(false);
        //TriggerMatchEndFilterEffect(true);
        //TriggerMatchEndFilterEffect(false);
        ResetScore();


        // CAMERA
        cameraManager.SwitchState(CameraManager.CAMERASTATE.inactive);
        cameraManager.actualXSmoothMovementsMultiplier = cameraManager.cinematicXSmoothMovementsMultiplier;
        cameraManager.actualZoomSmoothDuration = cameraManager.cinematicZoomSmoothDuration;
        cameraManager.gameObject.transform.position = cameraManager.cameraArmBasePos;
        cameraManager.cameraComponent.transform.position = cameraManager.cameraBasePos;


        // AUDIO
        audioManager.ResetBattleMusicPhase();


        // Restarts a new match right after it is finished being set up
        if (rematchRightAfter)
            StartCoroutine(StartMatchCoroutine());
        else
        {
            // Activates the main menu if it is not supposed to start a new match right after
            menuManager.mainMenu.SetActive(true);
            Cursor.visible = true;


            // AUDIO
            audioManager.ActivateMenuMusic();
        }
    }
    # endregion








    # region MATCH END
    // MATCH END
    IEnumerator APlayerWon(int winningPlayerIndex)
    {
        // STATS
        try
        {
            statsManager.FinalizeGame(true, 1);
        }
        catch
        {
            Debug.Log("Error while finilizing the recording of the current game, ignoring");
        }


        // AUDIO
        audioManager.ActivateWind();


        yield return new WaitForSecondsRealtime(4f);


        // GAME STATE
        SwitchState(GAMESTATE.finished);


        // PLAYER STATE
        playersList[winningPlayerIndex].GetComponent<Player>().SwitchState(Player.STATE.sneathing);


        // MENU
        menuManager.winName.text = playerNames[winningPlayerIndex];
        menuManager.winName.color = playersColors[winningPlayerIndex];


        // AUDIO
        audioManager.adjustBattleMusicVolumeDepdendingOnPlayersDistance = false;


        // ANIMATION
        //playersList[winningPlayerIndex].GetComponent<PlayerAnimations>().TriggerSneath();
        //playersList[winningPlayerIndex].GetComponent<PlayerAnimations>().ResetDrawText();


        yield return new WaitForSecondsRealtime(timeBeforeWinScreenAppears);


        // MENU
        blurPanel.SetActive(false);
        menuManager.winScreen.SetActive(true);
        Cursor.visible = true;


        // AUDIO
        audioManager.ActivateWinMusic();
    }
    # endregion








    # region EFFECTS
    // EFFECTS
    public void TriggerMatchEndFilterEffect(bool on)
    {
        if (on)
        {
            // List of all renderers for the death VFX
            spriteRenderers = GameObject.FindObjectsOfType<SpriteRenderer>();
            meshRenderers = GameObject.FindObjectsOfType<MeshRenderer>();
            particleSystems = GameObject.FindObjectsOfType<ParticleSystem>();
            lights = GameObject.FindObjectsOfType<Light>();


            // All renderers' original properties storage for the death VFX reset
            originalSpriteRenderersColors = new List<Color>();
            originalSpriteRenderersMaterials = new List<Material>();
            originalMeshRenderersColors = new List<Color>();
            originalParticleSystemsColors = new List<Color>();
            originalLightsIntensities = new List<float>();
            originalParticleSystemsGradients = new List<Gradient>();


            // Sets all black
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (!spriteRenderers[i].CompareTag("NonBlackFX"))
                {
                    originalSpriteRenderersColors.Add(spriteRenderers[i].color);
                    spriteRenderers[i].color = Color.black;

                    originalSpriteRenderersMaterials.Add(spriteRenderers[i].material);
                    spriteRenderers[i].material = deathFXSpriteMaterial;
                }
            }
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                if (!meshRenderers[i].CompareTag("NonBlackFX"))
                {
                    originalMeshRenderersColors.Add(meshRenderers[i].material.color);
                    meshRenderers[i].material.color = Color.black;
                }
            }
            /*
            for (int i = 0; i < particleSystems.Length; i++)
            {
                if (!particleSystems[i].CompareTag("NonBlackFX"))
                {
                    ParticleSystem.MainModule particleSystemMain = particleSystems[i].main;

                    originalParticleSystemsColors.Add(particleSystemMain.startColor.color);
                    particleSystemMain.startColor = Color.black;
                    originalParticleSystemsGradients.Add(particleSystemMain.startColor.gradient);
                    particleSystemMain.startColor = deathVFXGradientForParticles; 
                }
            }
            */


            for (int i = 0; i < lights.Length; i++)
            {
                if (!lights[i].CompareTag("NonBlackFX"))
                {
                    originalLightsIntensities.Add(lights[i].intensity);
                    lights[i].intensity = 0;
                }
            }


            // Deactivates background elements for only orange color
            for (int i = 0; i < mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements.Length; i++)
            {
                mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements[i].SetActive(false);
            }
        }
        else
        {
            // Resets all
            if (spriteRenderers != null && spriteRenderers.Length > 0)
            {
                try
                {
                    for (int i = 0; i < spriteRenderers.Length; i++)
                    {
                        if (!spriteRenderers[i].CompareTag("NonBlackFX"))
                        {
                            //spriteRenderers[i].color = Color.Lerp(spriteRenderers[i].color, spriteRenderersColors[i], (1 / 100 * j));
                            spriteRenderers[i].color = originalSpriteRenderersColors[i];
                            spriteRenderers[i].material = originalSpriteRenderersMaterials[i];
                        }
                    }
                }
                catch
                {
                }
            }
            if (meshRenderers != null && meshRenderers.Length > 0)
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    if (!meshRenderers[i].CompareTag("NonBlackFX"))
                    {
                        meshRenderers[i].material.color = originalMeshRenderersColors[i];
                    }
                }
            }
            /*
            for (int i = 0; i < particleSystems.Length; i++)
            {
                if (!particleSystems[i].CompareTag("NonBlackFX"))
                {
                    try
                    {
                        ParticleSystem.MainModule particleSystemMain = particleSystems[i].main;
                        particleSystemMain.startColor = originalParticleSystemsGradients[i];


                        particleSystemMain.startColor = originalParticleSystemsColors[i];
                    }
                    catch
                    {
                    }
                } 
            }
            */
            if (lights != null && lights.Length > 0)
            {
                for (int i = 0; i < lights.Length; i++)
                {
                    if (!lights[i].CompareTag("NonBlackFX"))
                    {
                        lights[i].intensity = originalLightsIntensities[i];
                    }
                }
            }


            // Reactivates the background of the map if it's referenced
            if (mapLoader.currentMap.GetComponent<MapPrefab>() && mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements.Length > 0)
            {
                for (int i = 0; i < mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements.Length; i++)
                {
                    mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements[i].SetActive(true);
                }
            }
        }
    }

    // Starts the SlowMo coroutine
    public void TriggerSlowMoCoroutine(float slowMoEffectDuration, float slowMoTimeScale, float fadeSpeed)
    {
        StartCoroutine(SlowMoCoroutine(slowMoEffectDuration, slowMoTimeScale, fadeSpeed));
    }

    // Slow motion and zoom for a given duration
    IEnumerator SlowMoCoroutine(float slowMoEffectDuration, float slowMoTimeScale, float fadeSpeed)
    {
        // CAMERA STATE
        cameraManager.SwitchState(CameraManager.CAMERASTATE.eventcam);


        // TIME
        actualTimeScaleUpdateSmoothness = fadeSpeed;
        timeScaleObjective = slowMoTimeScale;


        // FX
        animeLinesFx.Play();


        // AUDIO
        for (int i = 0; i < audioManager.battleMusicPhaseSources.Length; i++)
        {
            audioManager.battleMusicPhaseSources[i].pitch = slowMoTimeScale;
            audioManager.battleMusicStrikesSources[i].pitch = slowMoTimeScale;
        }


        yield return new WaitForSecondsRealtime(slowMoEffectDuration);


        // TIME
        actualTimeScaleUpdateSmoothness = roundEndTimeScaleFadeSpeed;
        timeScaleObjective = baseTimeScale;


        // CAMERA
        cameraManager.SwitchState(CameraManager.CAMERASTATE.battle);


        // AUDIO
        for (int i = 0; i < audioManager.battleMusicPhaseSources.Length; i++)
        {
            audioManager.battleMusicPhaseSources[i].pitch = 1;
            audioManager.battleMusicStrikesSources[i].pitch = 1;
        }


        yield return new WaitForSecondsRealtime(0.5f);


        // TIME
        Time.timeScale = timeScaleObjective;
        // FX
        animeLinesFx.Stop();
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
    # endregion








    # region SECONDARY FUNCTIONS
    // SECONDARY FUNCTIONS
    // Compares 2 floats with a range of tolerance
    public static bool FastApproximately(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }

    #endregion
   
    #endregion

}
