using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using Photon.Realtime;

// Created for Unity 2019.1.1f1
// Global game manager, uses all the other scripts, very important
// A LITTLE MESSY ?
public class GameManager : MonoBehaviourPun
{
    #region VARIABLES
    #region MANAGERS
    public static GameManager Instance;
    [Header("MANAGERS")]
    [Tooltip("The AudioManager script instance reference")]
    [SerializeField] AudioManager audioManager = null;

    [Tooltip("The MenuManager script instance reference")]
    [SerializeField] MenuManager menuManager = null;

    [Tooltip("The CameraManager script instance reference")]
    [SerializeField] CameraManager cameraManager = null;

    [Tooltip("The MapLoader script instance reference")]
    [SerializeField] public MapLoader mapLoader = null;

    InputManager inputManager = null;

    [Tooltip("The CameraShake scripts instances references in the scene")]
    [SerializeField] public CameraShake deathCameraShake = null;
    [SerializeField] public CameraShake clashCameraShake = null;
    [SerializeField] public CameraShake pommelCameraShake = null;
    [SerializeField] public CameraShake finalCameraShake = null;

    [SerializeField] StatsManager statsManager = null;
    #endregion



    #region DATA
    [Header("DATA")]
    [SerializeField] public CharactersDatabase charactersData = null;
    [SerializeField] public MenuParameters gameParameters = null;
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
    [Header("START")]
    [Tooltip("The delay before the battle camera values are entered in the camera parameters to make it reactive for battle once the game started, because the smooth camera values stay some time to smooth the zoom towards the scene")]
    [SerializeField] float timeBeforeBattleCameraActivationWhenGameStarts = 2f;
    [SerializeField] Animator drawTextAnimator = null;
    bool drawTextVisible = false;
    # endregion



    # region MENU STUFF
    [Header("MENU")]
    [Tooltip("The main menu object reference")]
    [SerializeField] GameObject mainMenu = null;
    [Tooltip("The blur panel object reference")]
    [SerializeField] GameObject blurPanel = null;
    #endregion



    #region IN GAME INFOS
    [Header("IN GAME INFOS REFERENCES")]
    [SerializeField] public Animator[] inGameHelp = null;
    [SerializeField] public List<Animator> playerKeysIndicators = new List<Animator>(2);
    [SerializeField] List<TextMeshProUGUI> playerHelpTextIdentifiers = new List<TextMeshProUGUI>(2);
    [SerializeField] List<Image> playerHelpIconIdentifiers = new List<Image>(2);
    [SerializeField] Animator characterSelectionHelpAnimator = null;
    #endregion



    #region SCORE DISPLAY
    [Header("SCORE DISPLAY REFERENCES")]
    [Tooltip("The score display game object reference")]
    [SerializeField] public GameObject scoreObject = null;

    [Tooltip("The score display text mesh pro component reference")]
    [SerializeField] public List<TextMeshProUGUI> scoresNames = new List<TextMeshProUGUI>(2);
    [SerializeField] List<Text> scoresDisplays = new List<Text>(2);
    [SerializeField] TextMeshProUGUI maxScoreTextDisplay = null;
    #endregion

    #region SCORE CALCULATION
    [Header("SCORE CALCULATION")]
    [Tooltip("The duration the score lasts on screen when a round has finished")]
    [SerializeField] float betweenRoundsScoreShowDuration = 4f;
    [Tooltip("The slider component reference in the options menu to change the number of rounds to win")]
    [SerializeField] Slider scoreToWinSliderComponent = null;

    [HideInInspector] public Vector2 score = new Vector2(0, 0);
    [Tooltip("The score to reach to win")]
    [SerializeField] public int scoreToWin = 10;
    # endregion



    # region ROUNDS & MATCH
    [Header("ROUND & MATCH")]
    [Tooltip("The delay before a new round starts when one has finished and players are waiting")]
    [SerializeField] float timeBeforeNextRoundTransitionTriggers = 3;
    [SerializeField] float resetGameDelay = 1.5f;
    #endregion



    [Header("WIN")]
    [Tooltip("The delay before the win menu screen appears when a player has won")]
    [SerializeField] float timeBeforeWinScreenAppears = 2f;



    #region PLAYERS
    [Header("PLAYERS")]
    [Tooltip("The player prefab reference")]
    [SerializeField] GameObject player = null;
    //[SerializeField] GameObject playerAI = null;
    [Tooltip("The references to the spawn objects of the players in the scene")]
    [SerializeField] public GameObject[] playerSpawns = { null, null };
    public List<GameObject> playersList = new List<GameObject>(2);

    [Tooltip("The colors to identify the players")]
    [SerializeField] public Color[] playersColors = { Color.red, Color.yellow },
        attackSignColors = { Color.red, Color.yellow },
        playerLightsColors = { Color.red, Color.yellow };
    [HideInInspector] public bool playerDead = false;
    [HideInInspector] public bool allPlayersHaveDrawn = false;
    bool player2Detected = false;
    # endregion



    # region FX
    [Header("FX")]
    [Tooltip("The level of time slow down that is activated when a player dies")]
    [SerializeField] public float roundEndSlowMoTimeScale = 0.2f;
    [SerializeField] public float minTimeScale = 0.05f,
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



    float actualTimeScaleUpdateSmoothness = 0.05f,
        baseTimeScale = 1,
        timeScaleObjective = 1;

    bool runTimeScaleUpdate = true;

    [Tooltip("The round transition leaves effect object reference")]
    [SerializeField] public ParticleSystem roundTransitionLeavesFX = null;
    [SerializeField] public ParticleSystem animeLinesFx = null;
    [SerializeField] ParticleSystem hajimeFX = null;
    [SerializeField] bool useSlowMotion = true;
    # endregion



    # region DEATH VFX
    [Header("DEATH VFX")]
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



    #region DEMO STUFF
    [Header("DEMO")]
    [SerializeField] public bool demo = false;
    [SerializeField] GameObject demoMark = null;
    [SerializeField] MenuBrowser mainMenuBrowser = null;
    [SerializeField] GameObject stagesButton = null;
    [SerializeField] GameObject demoStagesButton = null;
    #endregion



    #region CHEATS FOR DEVELOPMENT PURPOSES
    [Header("CHEATS")]
    [Tooltip("Use cheat codes ?")]
    [SerializeField] public bool cheatCodes = false;
    [Tooltip("The key to activate the slow motion cheat")]
    [SerializeField] KeyCode slowTimeKey = KeyCode.Alpha5;
    [SerializeField] float[] timeSlowDownSteps = null;
    //bool slowedDownTime = false;

    int timeSlowDownLevel = 0;
    #endregion


    [Header("EDITOR ONLY")]
    [SerializeField] public bool letThemFight;
    int losingPlayerIndex = 0;
    int winningPlayerIndex;


    [HideInInspector] public delegate void OnResetGameEvent();
    [HideInInspector] public event OnResetGameEvent ResetGameEvent;
    #endregion



















    #region FUNCTIONS
    #region BASE FUNCTIONS
    private void Awake()                                        // AWAKE
    {
        Instance = this;
        inputManager = InputManager.Instance;

        // DEMO
        if (demo)
        {
            demoMark.SetActive(true);
            demoStagesButton.SetActive(true);
            mainMenuBrowser.elements[2] = demoStagesButton;
            stagesButton.SetActive(false);
        }
    }


    public void Start()                                     // START
    {
        if (inputManager == null)
            inputManager = InputManager.Instance;

        inputManager.P2Input += ConnectPlayer2;

        // Set variables
        score = new Vector2(0, 0);
        baseTimeScale = Time.timeScale;
        actualTimeScaleUpdateSmoothness = roundEndTimeScaleFadeSpeed;


        // START GAME
        StartCoroutine(SetupGame());
    }

    
    private void Update()                                       // UPDATE
    {
        if (enabled && cheatCodes)
            if (Input.GetKeyUp(slowTimeKey))
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


    private void FixedUpdate()
    {
        // EFFECTS
        RunTimeScaleUpdate();

        if (!ConnectManager.Instance.connectedToMaster)
            scoreToWin = Mathf.FloorToInt(scoreToWinSliderComponent.value);
    }
    #endregion








    #region GAME STATE
    public void SwitchState(GAMESTATE newState)
    {
        oldState = gameState;
        gameState = newState;


        switch (gameState)
        {
            case GAMESTATE.menu:                                        // MENU
                break;

            case GAMESTATE.loading:                                                 // LOADING
                playerDead = false;
                menuManager.TriggerPause(false);
                menuManager.winScreen.SetActive(false);
                scoreObject.GetComponent<Animator>().SetBool("On", false);
                // IN GAME HELP
                characterSelectionHelpAnimator.SetBool("On", false);
                Cursor.visible = false;
                break;

            case GAMESTATE.intro:                                           // INTRO
                break;

            case GAMESTATE.game:                                                    // GAME
                if (oldState == GAMESTATE.paused)
                    for (int i = 0; i < playersList.Count; i++)
                    {
                        playersList[i].GetComponent<Player>().SwitchState(playersList[i].GetComponent<Player>().oldState);
                        playersList[i].GetComponent<PlayerAnimations>().animator.speed = 1;
                    }

                cameraManager.SwitchState(CameraManager.CAMERASTATE.battle);
                mainMenu.SetActive(false);
                
                blurPanel.SetActive(false);
                Cursor.visible = false;
                break;

            case GAMESTATE.paused:                                                      // PAUSED
                for (int i = 0; i < playersList.Count; i++)
                    if (playersList[i] != null)
                    {
                        playersList[i].GetComponent<Player>().SwitchState(Player.STATE.frozen);
                        playersList[i].GetComponent<PlayerAnimations>().animator.speed = 0;
                    }
                break;

            case GAMESTATE.finished:                                                // FINISHED
                menuManager.winMessage.SetActive(true);
                if (oldState == GAMESTATE.paused)
                    menuManager.SwitchPause();
                break;
        }
    }
    # endregion








    #region BEGIN GAME
    // Setup the game before it starts
    IEnumerator SetupGame()
    {
        // SOUND
        // Set on the menu music
        audioManager.SwitchAudioState(AudioManager.AUDIOSTATE.menu);


        SpawnPlayers();
        yield return new WaitForSeconds(0.5f);
        cameraManager.FindPlayers();


        for (int i = 0; i < playersList.Count; i++)
            playersList[i].GetComponent<Player>().ManageOrientation();
    }

    void ConnectPlayer2()
    {
        Debug.Log("Player2 joined");
        player2Detected = true;


        bool removeWarning = player2Detected;


        if (playersList[1].GetComponent<Player>().playerIsAI)
        {
            playersList[1].GetComponent<Player>().playerIsAI = false;
            Destroy(playersList[1].GetComponent<IAScript>());
        }

        inputManager.P2Input -= ConnectPlayer2;
    }

    // Begins the StartMatch coroutine, this function is called by the menu button Sclash
    public void StartMatch()
    {
        StartCoroutine(StartMatchCoroutine());
    }

    public void ManageAI()
    {
        foreach (Player p in FindObjectsOfType<Player>())
            p.gameObject.AddComponent<IAScript>();

        StartMatch();
    }

    // Starts the match, activates the camera cinematic zoom and then switches to battle camera
    public IEnumerator StartMatchCoroutine()
    {
        StopCoroutine(StartMatchCoroutine());


        // FX
        hajimeFX.Play();


        // AUDIO
        audioManager.FindPlayers();
        audioManager.SwitchAudioState(AudioManager.AUDIOSTATE.beforeBattle);


        // SCORE DISPLAY
        UpdateMaxScoreDisplay();
        ResetScore();


        for (int i = 0; i < playersList.Count; i++)
        {
            scoresNames[i].name = charactersData.charactersList[playersList[i].GetComponent<Player>().characterIndex].name;
            scoresNames[i].color = playersColors[i];
            scoresDisplays[i].color = playersColors[i];
            playerHelpTextIdentifiers[i].color = playersColors[i];
            playerHelpIconIdentifiers[i].color = playersColors[i];
        }


        // IN GAME HELP
        characterSelectionHelpAnimator.SetBool("On", true);


        for (int i = 0; i < playersList.Count; i++)
            playersList[i].GetComponent<Player>().SwitchState(Player.STATE.sneathed);


        yield return new WaitForSeconds(0.1f);


        // STATE
        SwitchState(GAMESTATE.game);
        cameraManager.SwitchState(CameraManager.CAMERASTATE.battle);


        yield return new WaitForSeconds(0.5f);

        /*
        for (int i = 0; i < playersList.Count; i++)
            playersList[i].GetComponent<Player>().SwitchState(Player.STATE.sneathed);
            */


        yield return new WaitForSeconds(timeBeforeBattleCameraActivationWhenGameStarts);


        // Change camera speeds
        cameraManager.actualXSmoothMovementsMultiplier = cameraManager.battleXSmoothMovementsMultiplier;
        cameraManager.actualZoomSpeed = cameraManager.battleZoomSpeed;
        cameraManager.actualZoomSmoothDuration = cameraManager.battleZoomSmoothDuration;


        yield return new WaitForSeconds(10f);


        // Appears draw text if both players haven't drawn
        allPlayersHaveDrawn = true;


        for (int i = 0; i < playersList.Count; i++)
            if (playersList[i].GetComponent<Player>().playerState == Player.STATE.sneathed || playersList[i].GetComponent<Player>().playerState == Player.STATE.drawing)
                allPlayersHaveDrawn = false;


        // DRAW DISPLAY
        if (!allPlayersHaveDrawn)
        {
            drawTextVisible = true;
            drawTextAnimator.ResetTrigger("FadeIn");
            drawTextAnimator.SetTrigger("FadeIn");
            drawTextAnimator.ResetTrigger("FadeOut");
        }
    }

    // A saber has been drawn, stores it and checks if both players have drawn
    public void SaberDrawn(int playerNum)
    {
        if (audioManager.audioState == AudioManager.AUDIOSTATE.beforeBattle || audioManager.audioState == AudioManager.AUDIOSTATE.pause)
        {
            allPlayersHaveDrawn = true;

            for (int i = 0; i < playersList.Count; i++)
                if (playersList[i].GetComponent<Player>().playerState == Player.STATE.sneathed || playersList[i].GetComponent<Player>().playerState == Player.STATE.drawing)
                    allPlayersHaveDrawn = false;


            if (allPlayersHaveDrawn)
            {
                //audioManager.ActivateBattleMusic();
                audioManager.SwitchAudioState(AudioManager.AUDIOSTATE.battle);


                // IN GAME HELP
                characterSelectionHelpAnimator.SetBool("On", false);


                // STATS
                statsManager.InitalizeNewGame(1, playersList[0].GetComponent<CharacterChanger>().currentCharacterIndex, playersList[1].GetComponent<CharacterChanger>().currentCharacterIndex);
                statsManager.InitializeNewRound();


                // Makes draw text disappear if it has appeared
                if (drawTextVisible)
                {
                    drawTextAnimator.ResetTrigger("FadeOut");
                    drawTextAnimator.SetTrigger("FadeOut");
                    drawTextAnimator.ResetTrigger("FadeIn");
                }
            }
        }
    }
    #endregion





    void SpawnAI()
    {
        playersList.Clear();
        for (int i = 0; i < playerSpawns.Length; i++)
        {
            //PlayerStats playerStats;
            PlayerAnimations playerAnimations;
            //PlayerAttack playerAttack;
            Player playerScript = null;

            GameObject AI = (GameObject)Resources.Load("PlayerAI");

            playersList.Add(Instantiate(AI, playerSpawns[i].transform.position, playerSpawns[i].transform.rotation));
            playerAnimations = playersList[i].GetComponent<PlayerAnimations>();
            playerScript = playersList[i].GetComponent<Player>();
            playerScript.playerNum = i;
            playerScript.ResetAllPlayerValuesForNextMatch();



            // ANIMATIONS
            playerAnimations.spriteRenderer.color = playersColors[i];
            playerAnimations.legsSpriteRenderer.color = playersColors[i];

            playerAnimations.spriteRenderer.sortingOrder = 10 * i;
            playerAnimations.legsSpriteRenderer.sortingOrder = 10 * i;


            // FX
            ParticleSystem attackSignParticles = playerScript.attackRangeFX.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule attackSignParticlesMain = attackSignParticles.main;
            attackSignParticlesMain.startColor = attackSignColors[i];


            playerScript.playerLight.color = playerLightsColors[i];
        }
    }

    #region PLAYERS
    // Spawns the players
    void SpawnPlayers()
    {
        for (int i = 0; i < playerSpawns.Length; i++)
        {
            PlayerAnimations playerAnimations;
            Player playerScript = null;

            playersList.Add(Instantiate(player, playerSpawns[i].transform.position, playerSpawns[i].transform.rotation));
            //IAScript ia = null;
            /*
#if UNITY_EDITOR
            if (letThemFight || i == 1)
                ia = playersList[i].AddComponent<IAScript>();
#else
            if (i == 1)
                ia = playersList[i].AddComponent<IAScript>();
#endif
            if (ia != null)
                ia.SetDifficulty(IAScript.Difficulty.Hard);
                */
            playerAnimations = playersList[i].GetComponent<PlayerAnimations>();
            playerScript = playersList[i].GetComponent<Player>();
            playerScript.playerNum = i;
            playerScript.ResetAllPlayerValuesForNextMatch();



            // ANIMATIONS
            playerAnimations.spriteRenderer.sortingOrder = 10 * i;
            playerAnimations.legsSpriteRenderer.sortingOrder = 10 * i;


            // FX
            ParticleSystem attackSignParticles = playerScript.attackRangeFX.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule attackSignParticlesMain = attackSignParticles.main;
            attackSignParticlesMain.startColor = attackSignColors[i];


            // VISUAL IDENTIFICATION
            playerScript.characterNameDisplay.text = charactersData.charactersList[0].name;
            playerScript.characterNameDisplay.color = playersColors[i];
            playerScript.characterIdentificationArrow.color = playersColors[i];
            //playerScript.playerLight.color = playerLightsColors[i];
        }
    }

    // Reset all the players' variables for next round
    void ResetPlayersForNextMatch()
    {
        if (playerSpawns.Length < 2)
            playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");

        if (ConnectManager.Instance.connectedToMaster)
            foreach (GameObject p in playersList)
            {
                int _tempPNum = p.GetComponent<Player>().playerNum;

                p.GetComponent<PlayerAnimations>().TriggerSneath();

                p.transform.position = playerSpawns[_tempPNum].transform.position;
                p.transform.rotation = playerSpawns[_tempPNum].transform.rotation;
                p.GetComponent<PlayerAnimations>().ResetAnimsForNextMatch();

                p.GetComponent<Player>().ResetAllPlayerValuesForNextMatch();
            }
        else
            for (int i = 0; i < playersList.Count; i++)
            {
                GameObject p = playersList[i];


                playersList[i].GetComponent<PlayerAnimations>().TriggerSneath();


                p.transform.position = playerSpawns[i].transform.position;
                p.transform.rotation = playerSpawns[i].transform.rotation;
                p.GetComponent<PlayerAnimations>().ResetAnimsForNextMatch();


                p.GetComponent<Player>().ResetAllPlayerValuesForNextMatch();
            }

        playerDead = false;
    }

    // Reset all the players' variables for next round
    void ResetPlayersForNextRound()
    {
        if (playerSpawns.Length < 2)
            playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");

        if (ConnectManager.Instance.connectedToMaster)
            foreach (GameObject p in playersList)
            {
                int _tempPNum = p.GetComponent<Player>().playerNum;

                p.transform.position = playerSpawns[_tempPNum].transform.position;
                p.transform.rotation = playerSpawns[_tempPNum].transform.rotation;
                p.GetComponent<PlayerAnimations>().ResetAnimsForNextRound();
                p.GetComponent<Player>().SwitchState(Player.STATE.normal);

                p.GetComponent<Player>().ResetAllPlayerValuesForNextRound();

                p.GetComponent<PhotonView>().RPC("ResetPos", RpcTarget.AllViaServer);
            }
        else
            for (int i = 0; i < playersList.Count; i++)
            {
                GameObject p = playersList[i];

                p.transform.position = playerSpawns[i].transform.position;
                p.transform.rotation = playerSpawns[i].transform.rotation;
                p.GetComponent<PlayerAnimations>().ResetAnimsForNextRound();
                p.GetComponent<Player>().SwitchState(Player.STATE.normal);


                p.GetComponent<Player>().ResetAllPlayerValuesForNextRound();
            }
        playerDead = false;
    }

    public GameObject GetOtherPlayer(GameObject o)
    {
        for (int i = 0; i < playersList.Count; i++)
            if (playersList[i] == o)
                return o;

        return null;
    }

    #endregion








    #region ROUND TO ROUND & SCORE
    // Executed when a player dies, starts the score display and next round parameters
    public void APlayerIsDead(int incomingWinning)
    {
        winningPlayerIndex = incomingWinning;
        losingPlayerIndex = 1 - winningPlayerIndex;

        // STATS
        try {
            statsManager.FinalizeRound(winningPlayerIndex);
        }
        catch {
            Debug.Log("Error while finalizing the recording of the current round, ignoring");
        }



        playerDead = true;
        UpdatePlayersScoreValues();
        SwitchState(GAMESTATE.roundFinished);


        if (CheckIfThePlayerWon())
            APlayerWon();
        else
            StartCoroutine(NextRoundCoroutine());
    }

    void UpdatePlayersScoreValues()
    {
        score[winningPlayerIndex] += 1;

        for (int i = 0; i < playersList.Count; i++)
            scoresDisplays[i].text = score[i].ToString();
        //scoreTextComponent.text = ScoreBuilder();
    }

    // Builds the score display message
    string ScoreBuilder()
    {
        string scoreString = "<color=#FF0000>" + score[0].ToString() + "</color> / <color=#0000FF>" + score[1].ToString() + "</color>";
        return scoreString;
    }

    bool CheckIfThePlayerWon()
    {
        if (score[winningPlayerIndex] >= scoreToWin)
            return true;
        else
            return false;
    }

    // Start next round
    IEnumerator NextRoundCoroutine()
    {
        //Debug.Log("Next round");
        yield return new WaitForSecondsRealtime(timeBeforeNextRoundTransitionTriggers);


        StartCoroutine(ShowScoreBetweenRoundsCoroutine());


        // FX
        roundTransitionLeavesFX.Play();


        yield return new WaitForSeconds(1.5f);


        ResetPlayersForNextRound();


        // STATS
        try {
            statsManager.InitializeNewRound();
        }
        catch {
            Debug.Log("Error while initializing a new round, ignoring");
        }


        yield return new WaitForSeconds(1f);


        SwitchState(GAMESTATE.game);
        

        // AUDIO
        audioManager.roundBeginsRandomSoundSource.Play();
    }

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

        for (int i = 0; i < playersList.Count; i++)
            scoresDisplays[i].text = "0";
    }

    public void UpdateMaxScoreDisplay()
    {
        maxScoreTextDisplay.text = scoreToWin.ToString();
    }
    #endregion







    #region RESTART GAME
    // Calls ResetGame coroutine, called by main menu button at the end of the match
    public void ResetGame()
    {
        StartCoroutine(ResetGameCoroutine(false));
    }

    // Calls ResetGame coroutine, called by main menu button at the end of the match
    public void ResetGameAndRematch()
    {
        ResetGameEvent();
        StartCoroutine(ResetGameCoroutine(true));
    }

    // Resets the match settings and values for a next match
    IEnumerator ResetGameCoroutine(bool rematchRightAfter)
    {
        // STATS
        if (gameState != GAMESTATE.finished && allPlayersHaveDrawn)
        {
            try {
                statsManager.FinalizeGame(false, 1);
            }
            catch {
                Debug.Log("Error while finalizing the recording of the current game, ignoring");
            }
        }


        // ONLINE
        if (photonView != null && PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();


        // NEXT STAGE
        int newStageIndex = mapLoader.currentMapIndex;

        if (gameState == GAMESTATE.finished)
            if (rematchRightAfter)
                newStageIndex = CalculateNextStageIndex();


        // STATE
        SwitchState(GAMESTATE.loading);


        // Activates the menu blur panel if it is not supposed to start a new match right after
        if (!rematchRightAfter)
            blurPanel.SetActive(true);


        // IN GAME INDICATIONS
        drawTextAnimator.ResetTrigger("FadeOut");
        drawTextAnimator.SetTrigger("FadeOut");
        drawTextAnimator.ResetTrigger("FadeIn");
        for (int i = 0; i < playersList.Count; i++)
        {
            playersList[i].GetComponent<PlayerAnimations>().nameDisplayAnimator.SetBool("On", false);
            inGameHelp[i].SetBool("On", false);
            playerKeysIndicators[i].SetBool("On", false);
        }


        // FX
        roundTransitionLeavesFX.Play();


        // AUDIO
        audioManager.SwitchAudioState(AudioManager.AUDIOSTATE.none);


        yield return new WaitForSecondsRealtime(resetGameDelay);


        ResetPlayersForNextMatch();
        TriggerMatchEndFilterEffect(false);


        // PLAYERS LIGHTS / COLORS
        for (int i = 0; i < playersList.Count; i++)
            if (playersList[i].GetComponent<Player>().playerLight != null)
            {
                playersList[i].GetComponent<Player>().playerLight.color = playerLightsColors[i];
                playersList[i].GetComponent<Player>().playerLight.intensity = 5;
            }


        // NEXT STAGE
        if (demo && mapLoader.halloween) // Halloween stage for demo
            mapLoader.SetMap(0, true);
        else
            mapLoader.SetMap(newStageIndex, false);


        ResetScore();


        // CAMERA
        cameraManager.SwitchState(CameraManager.CAMERASTATE.inactive);
        cameraManager.actualXSmoothMovementsMultiplier = cameraManager.cinematicXSmoothMovementsMultiplier;
        cameraManager.actualZoomSmoothDuration = cameraManager.cinematicZoomSmoothDuration;
        cameraManager.gameObject.transform.position = cameraManager.cameraArmBasePos;
        cameraManager.cameraComponent.transform.position = cameraManager.cameraBasePos;


        // Restarts a new match right after it is finished being set up
        if (rematchRightAfter)
            StartCoroutine(StartMatchCoroutine());
        else
        {
            // STATE
            SwitchState(GAMESTATE.menu);

            // Activates the main menu if it is not supposed to start a new match right after
            menuManager.mainMenu.SetActive(true);
            Cursor.visible = true;


            // AUDIO
            audioManager.SwitchAudioState(AudioManager.AUDIOSTATE.menu);
        }
    }
    #endregion








    #region MATCH END
    // MATCH END
    public void APlayerLeft()
    {
        foreach (GameObject p in playersList)
            if (p != null)
                continue;
            else
            {
                playersList.Remove(p);
                break;
            }

        Debug.Log("<color=red>The opponent left</color>");
        //APlayerWon();
    }


    void APlayerWon()
    {
        // STATS
        try {
            statsManager.FinalizeGame(true, 1);
        }
        catch {
            Debug.Log("Error while finalizing the recording of the current game, ignoring");
        }


        // AUDIO
        audioManager.SwitchAudioState(AudioManager.AUDIOSTATE.won);


        // SCORE
        scoreObject.GetComponent<Animator>().SetBool("On", false);
        for (int i = 0; i < playersList.Count; i++)
        {
            playersList[i].GetComponent<PlayerAnimations>().nameDisplayAnimator.SetBool("On", false);
            inGameHelp[i].SetBool("On", false);
            playerKeysIndicators[i].SetBool("On", false);
        }

        Invoke("EndGame", 4f);
    }
    #endregion

    //RENAME HERE IF WORKING
    void EndGame()
    {
        // GAME STATE
        SwitchState(GAMESTATE.finished);


        // PLAYER STATE
        playersList[winningPlayerIndex].GetComponent<Player>().SwitchState(Player.STATE.sneathing);
        playersList[winningPlayerIndex].GetComponent<Player>().SwitchState(Player.STATE.frozen);


        // WIN MENU
        menuManager.SetUpWinMenu(charactersData.charactersList[playersList[winningPlayerIndex].GetComponent<Player>().characterIndex].name, playersColors[winningPlayerIndex], score, playersColors);


        // ANIMATION
        Invoke("TriggerFallDeadAnimation", 2f);


        Invoke("ShowMenu", 2f + timeBeforeWinScreenAppears);
    }

    // Animation
    void TriggerFallDeadAnimation()
    {
        playersList[losingPlayerIndex].GetComponent<PlayerAnimations>().TriggerRealDeath();
    }

    void ShowMenu()
    {
        // MENU
        blurPanel.SetActive(false);
        menuManager.winScreen.SetActive(true);
        Cursor.visible = true;


        // AUDIO
        audioManager.winMusicAudioSource.Play();
    }

    #region EFFECTS
    public void TriggerMatchEndFilterEffect(bool on)
    {
        if (on)
        {
            // Deactivates background elements for only orange color
            for (int i = 0; i < mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements.Length; i++)
                mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements[i].SetActive(false);


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



            // SET ALL BLACK
            // SPRITES
            for (int i = 0; i < spriteRenderers.Length; i++)
                if (!spriteRenderers[i].CompareTag("NonBlackFX") && spriteRenderers[i].gameObject.activeInHierarchy)
                {
                    originalSpriteRenderersColors.Add(spriteRenderers[i].color);
                    spriteRenderers[i].color = Color.black;

                    originalSpriteRenderersMaterials.Add(spriteRenderers[i].material);
                    spriteRenderers[i].material = deathFXSpriteMaterial;
                }
            // MESHES
            for (int i = 0; i < meshRenderers.Length; i++)
                if (!meshRenderers[i].CompareTag("NonBlackFX") && meshRenderers[i].gameObject.activeInHierarchy)
                {
                    try
                    {
                        // Store original color
                        originalMeshRenderersColors.Add(meshRenderers[i].material.color);
                        // Set black
                        meshRenderers[i].material.color = Color.black;
                    }
                    catch { }
                }
            // PARTICLES
            for (int i = 0; i < particleSystems.Length; i++)
                if (particleSystems[i] != null && !particleSystems[i].CompareTag("NonBlackFX") && particleSystems[i].gameObject.activeInHierarchy && particleSystems[i].isPlaying)
                {
                    // INDIVIDUAL
                    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystems[i].particleCount];
                    particleSystems[i].GetParticles(particles);

                    for (int o = 0; o < particles.Length; o++)
                        particles[o].startColor = Color.black;
                    particleSystems[i].SetParticles(particles, particles.Length);


                    // SYSTEMS
                    ParticleSystem.MainModule particleSystemMain = particleSystems[i].main;
                    originalParticleSystemsColors.Add(particleSystemMain.startColor.color);
                    particleSystemMain.startColor = Color.black;
                    originalParticleSystemsGradients.Add(particleSystemMain.startColor.gradient);
                    particleSystemMain.startColor = deathVFXGradientForParticles; 
                }
            

            // LIGHTS
            for (int i = 0; i < lights.Length; i++)
                if (lights[i] != null && !lights[i].CompareTag("NonBlackFX") && lights[i].gameObject.activeInHierarchy)
                    lights[i].gameObject.SetActive(false);
        }
        else
        {
            // RESET ALL
            // SPRITES
            if (spriteRenderers != null && spriteRenderers.Length > 0)
            {
                try
                {
                    for (int i = 0; i < spriteRenderers.Length; i++)
                        if (!spriteRenderers[i].CompareTag("NonBlackFX") && spriteRenderers[i].gameObject.activeInHierarchy)
                        {
                            spriteRenderers[i].color = originalSpriteRenderersColors[i];
                            spriteRenderers[i].material = originalSpriteRenderersMaterials[i];
                        }
                }
                catch {}
            }
            // MESHES
            if (meshRenderers != null && meshRenderers.Length > 0)
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                    if (meshRenderers[i] != null && !meshRenderers[i].CompareTag("NonBlackFX") && meshRenderers[i].gameObject.activeInHierarchy)
                        meshRenderers[i].material.color = originalMeshRenderersColors[i];
            }
            // PARTICLES
            if (particleSystems != null && particleSystems.Length > 0)
                for (int i = 0; i < particleSystems.Length; i++)
                    if (particleSystems[i] != null && !particleSystems[i].CompareTag("NonBlackFX"))
                    {
                        try
                        {
                            ParticleSystem.MainModule particleSystemMain = particleSystems[i].main;



                            particleSystemMain.startColor = originalParticleSystemsGradients[i];
                            particleSystemMain.startColor = originalParticleSystemsColors[i];
                        }
                        catch { }
                    } 
            // LIGHTS
            if (lights != null && lights.Length > 0)
                for (int i = 0; i < lights.Length; i++)
                    try
                    {
                        if (lights[i] != null && !lights[i].CompareTag("NonBlackFX") && lights[i].gameObject.activeInHierarchy)
                                lights[i].gameObject.SetActive(true);
                    }
                    catch { }


            // Reactivates the background of the map if it's referenced
            if (mapLoader.currentMap.GetComponent<MapPrefab>() && mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements.Length > 0)
                for (int i = 0; i < mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements.Length; i++)
                    mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements[i].SetActive(true);
        }
    }

    // Starts the SlowMo coroutine
    public void TriggerSlowMoCoroutine(float slowMoEffectDuration, float slowMoTimeScale, float fadeSpeed)
    {
        if (useSlowMotion)
        {
            StopCoroutine(SlowMoCoroutine(slowMoEffectDuration, slowMoTimeScale, fadeSpeed));
            StartCoroutine(SlowMoCoroutine(slowMoEffectDuration, slowMoTimeScale, fadeSpeed));
        }
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
        audioManager.TriggerSlowMoAudio(true);


        yield return new WaitForSecondsRealtime(slowMoEffectDuration);


        // TIME
        actualTimeScaleUpdateSmoothness = roundEndTimeScaleFadeSpeed;
        timeScaleObjective = baseTimeScale;


        // CAMERA
        cameraManager.SwitchState(CameraManager.CAMERASTATE.battle);


        // AUDIO
        /*
        for (int i = 0; i < audioManager.battleMusicPhaseSources.Length; i++)
        {
            audioManager.battleMusicPhaseSources[i].pitch = 1;
            audioManager.battleMusicStrikesSources[i].pitch = 1;
        }
        */
        audioManager.TriggerSlowMoAudio(false);


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


            for (int i = 0; i < audioManager.phasesMainAudioSources.Count; i++)
            {
                audioManager.phasesMainAudioSources[i].pitch = Time.timeScale;
                audioManager.phasesStrikesAudioSources[i].pitch = Time.timeScale;
            }
        }
    }
    #endregion





    # region SECONDARY FUNCTIONS
    int CalculateNextStageIndex()
    {
        int nextStageIndex = mapLoader.currentMapIndex;
        int loopCount = 0;

        // DAY NIGHT
        if (gameParameters.dayNightCycle)
        {
            if (!gameParameters.randomStage)
            {
                if (mapLoader.mapsData.stagesLists[mapLoader.currentMapIndex].type == STAGETYPE.day)
                    nextStageIndex = mapLoader.currentMapIndex + 1;
                if (mapLoader.mapsData.stagesLists[mapLoader.currentMapIndex].type == STAGETYPE.night)
                    nextStageIndex = mapLoader.currentMapIndex - 1;
            }
            else
            {
                if (mapLoader.mapsData.stagesLists[mapLoader.currentMapIndex].type == STAGETYPE.day)
                    nextStageIndex = mapLoader.currentMapIndex + 1;
                else
                {
                    nextStageIndex = Random.Range(0, mapLoader.mapsData.stagesLists.Count);

                    if (gameParameters.useCustomListForRandom)
                        while (!mapLoader.mapsData.stagesLists[nextStageIndex].inCustomList || nextStageIndex == mapLoader.currentMapIndex || !(mapLoader.mapsData.stagesLists[nextStageIndex].type == STAGETYPE.day))
                        {
                            nextStageIndex = Random.Range(0, mapLoader.mapsData.stagesLists.Count);

                            loopCount++;
                            if (loopCount >= 100)
                            {
                                nextStageIndex = 0;
                                break;
                            }
                        }
                    else
                        while (nextStageIndex == mapLoader.currentMapIndex || mapLoader.mapsData.stagesLists[nextStageIndex].type == STAGETYPE.night)
                        {
                            nextStageIndex = Random.Range(0, mapLoader.mapsData.stagesLists.Count);
                            loopCount++;
                            if (loopCount >= 100)
                            {
                                Debug.Log("Couldn't find random day map that is not this one, taking index 0 instead");
                                nextStageIndex = 0;
                                break;
                            }
                        }
                }
            }
        }
        // RANDOM
        else if (gameParameters.randomStage)
        {
            nextStageIndex = Random.Range(0, mapLoader.mapsData.stagesLists.Count);

            if (gameParameters.useCustomListForRandom)
                while (!mapLoader.mapsData.stagesLists[nextStageIndex].inCustomList || nextStageIndex == mapLoader.currentMapIndex)
                {
                    nextStageIndex = Random.Range(0, mapLoader.mapsData.stagesLists.Count);

                    loopCount++;
                    if (loopCount >= 100)
                    {
                        nextStageIndex = 0;
                        break;
                    }
                }
            else
                while (nextStageIndex == mapLoader.currentMapIndex)
                {
                    nextStageIndex = Random.Range(0, mapLoader.mapsData.stagesLists.Count);

                    loopCount++;
                    if (loopCount >= 100)
                    {
                        nextStageIndex = 0;
                        break;
                    }
                }
        }

        return nextStageIndex;
    }


    // Compares 2 floats with a range of tolerance
    public static bool FastApproximately(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }
    #endregion
    #endregion

}
