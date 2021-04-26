using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using UnityEditor;

using Photon.Pun;
using Photon.Realtime;





// HEADER
// A LITTLE MESSY ?
// For Sclash

// REQUIREMENTS
// TextMeshPro package
// Photon unity
// PlayerControls script
// MapLoader script (Single instance)
// MenuManager script (Single instance)
// AudioManager script (Single instance)
// CameraManager script (Single instance)
// Player script
// PLayerAnimations script
// TagsReferences scriptable object
// CharactersDatabase scriptable object
// MenuParameters scriptable object


/// <summary>
/// Global game manager for the duel mode, uses all the other scripts, very important
/// </summary>

// VERSION
// Originally created for Unity 2019.1.1f1
public class GameManager : MonoBehaviourPun
{
    #region VARIABLES
    // SINGLETON
    [HideInInspector] public static GameManager Instance;




    [Header("DATA")]
    [SerializeField] public CharactersDatabase charactersData = null;
    [SerializeField] public MenuParameters gameParameters = null;
    [SerializeField] TagsReferences tagsReferences = null;




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
    [SerializeField] public float timeBeforeBattleCameraActivationWhenGameStarts = 2f;
    [SerializeField] public Animator drawTextAnimator = null;
    bool drawTextVisible = false;
    # endregion



    # region MENU STUFF
    [Header("MENU")]
    [Tooltip("The main menu object reference")]
    [SerializeField] GameObject mainMenu = null;
    [Tooltip("The blur panel object reference")]
    [SerializeField] GameObject blurPanel = null;
    #endregion



    [Header("IN GAME INFOS REFERENCES")]
    [SerializeField] public Animator[] inGameHelp = null;
    [SerializeField] public List<Animator> playerKeysIndicators = new List<Animator>(2);
    [SerializeField] List<TextMeshProUGUI> playerHelpTextIdentifiers = new List<TextMeshProUGUI>(2);
    [SerializeField] List<Image> playerHelpIconIdentifiers = new List<Image>(2);
    [SerializeField] Animator characterSelectionHelpAnimator = null;



    #region SCORE DISPLAY
    [Header("SCORE DISPLAY REFERENCES")]
    [Tooltip("The score display game object reference")]
    [SerializeField] public GameObject scoreObject = null;

    [Tooltip("The score display text mesh pro component reference")]
    [SerializeField] public List<TextMeshProUGUI> scoresNames = new List<TextMeshProUGUI>(2);
    [SerializeField] public List<Text> scoresDisplays = new List<Text>(2);
    [SerializeField] public TextMeshProUGUI maxScoreTextDisplay = null;
    #endregion


    #region SCORE CALCULATION
    [Header("SCORE CALCULATION")]
    [Tooltip("The duration the score lasts on screen when a round has finished")]
    [HideInInspector] float betweenRoundsScoreShowDuration = 4f;
    [Tooltip("The slider component reference in the options menu to change the number of rounds to win")]
    [SerializeField] Slider scoreToWinSliderComponent = null;

    [HideInInspector] public Vector2 score = new Vector2(0, 0);
    [Tooltip("The score to reach to win")]
    [HideInInspector] public int scoreToWin = 10;
    # endregion


    [Header("ROUND & MATCH")]
    [Tooltip("The delay before a new round starts when one has finished and players are waiting")]
    [HideInInspector] float timeBeforeNextRoundTransitionTriggers = 3;
    [HideInInspector] float resetGameDelay = 1.5f;



    [Header("WIN")]
    [Tooltip("The delay before the win menu screen appears when a player has won")]
    [HideInInspector] float timeBeforeWinScreenAppears = 2f;



    #region PLAYERS
    [Header("PLAYERS")]
    [Tooltip("The player prefab reference")]
    [SerializeField] GameObject player = null;
    //[SerializeField] GameObject playerAI = null;
    [Tooltip("The references to the spawn objects of the players in the scene")]
    [SerializeField] public GameObject[] playerSpawns = { null, null };
    [SerializeField] public List<GameObject> playersList = new List<GameObject>(2);

    [Tooltip("The colors to identify the players")]
    [SerializeField]
    public Color[] playersColors = { Color.red, Color.yellow },
        attackSignColors = { Color.red, Color.yellow },
        playerLightsColors = { Color.red, Color.yellow };
    [HideInInspector] public bool playerDead = false;
    [HideInInspector] public bool allPlayersHaveDrawn = false;
    bool player2Detected = false;
    # endregion


    [Header("POST PROCESSING")]
    [SerializeField] PostProcessVolume postProcessVolume = null;
    float kuwaharaMaxXPow = 23f;
    float kuwaharaMinXPow = 11f;
    float kuwaharaXPowObjective = 0;
    float currentKuwaharaXPow = 0;
    Kuwahara kuwahara = null;


    # region FX
    [Header("FX")]
    [Tooltip("The level of time slow down that is activated when a player dies")]
    [SerializeField] public float roundEndSlowMoTimeScale = 0.2f;
    [HideInInspector] public float minTimeScale = 0.05f;
    [SerializeField] public float roundEndSlowMoDuration = 1.3f;
    [HideInInspector] public float roundEndTimeScaleFadeSpeed = 0.05f;
    [SerializeField] public float gameEndSlowMoTimeScale = 0.1f;
    [SerializeField] public float gameEndSlowMoDuration = 0.5f;
    [SerializeField] public float gameEndTimeScaleFadeSpeed = 0.2f;
    [SerializeField] public float clashSlowMoTimeScale = 0.1f;
    [SerializeField] public float clashSlowMoDuration = 0.5f;
    [SerializeField] public float clashTimeScaleFadeSpeed = 0.2f;
    [SerializeField] public float parrySlowMoTimeScale = 0.2f;
    [SerializeField] public float parrySlowMoDuration = 2f;
    [HideInInspector] public float parryTimeScaleFadeSpeed = 0.2f;
    [SerializeField] public float dodgeSlowMoTimeScale = 0.2f;
    [SerializeField] public float dodgeSlowMoDuration = 2f;
    [HideInInspector] public float dodgeTimeScaleFadeSpeed = 0.2f;




    float actualTimeScaleUpdateSmoothness = 0.05f;
    float baseTimeScale = 1;
    float timeScaleObjective = 1;
    bool runTimeScaleUpdate = true;



    [Tooltip("The round transition leaves effect object reference")]
    [SerializeField] public ParticleSystem roundTransitionLeavesFX = null;
    [SerializeField] public ParticleSystem animeLinesFx = null;
    [SerializeField] ParticleSystem hajimeFX = null;
    [SerializeField] bool useSlowMotion = true;
    [SerializeField] bool useAnimeLines = true;
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
    SkinnedMeshRenderer[] skinnedMeshRenderers = null;
    ParticleSystem[] particleSystems = null;
    Light[] lights = null;

    // All renderers' original properties storage for the death VFX reset
    List<Color> originalSpriteRenderersColors = new List<Color>();
    List<Material> originalSpriteRenderersMaterials = new List<Material>();
    List<Color> originalMeshRenderersColors = new List<Color>();
    List<Color> skinnedMeshRenderesColors = new List<Color>();
    List<Color> originalParticleSystemsColors = new List<Color>();
    List<Gradient> originalParticleSystemsGradients = new List<Gradient>();
    List<float> originalLightsIntensities = new List<float>();
    #endregion



    [Header("CAMERA SHAKE")]
    [SerializeField] public float deathCameraShakeDuration = 0.3f;
    [SerializeField] public float clashCameraShakeDuration = 0.3f;
    [SerializeField] public float pommelCameraShakeDuration = 0.3f;
    [SerializeField] public float finalCameraShakeDuration = 0.7f;
    [Tooltip("The CameraShake scripts instances references in the scene")]
    [SerializeField] public CameraShake deathCameraShake = null;
    [SerializeField] public CameraShake clashCameraShake = null;
    [SerializeField] public CameraShake pommelCameraShake = null;
    [SerializeField] public CameraShake finalCameraShake = null;



    #region DEMO STUFF
    [Header("DEMO")]
    [SerializeField] public bool demo = false;
    [SerializeField] GameObject demoMark = null;
    [SerializeField] MenuBrowser mainMenuBrowser = null;
    [SerializeField] GameObject stagesButton = null;
    [SerializeField] GameObject demoStagesButton = null;
    [SerializeField] GameObject storyButton = null;
    [SerializeField] GameObject demoStoryButton = null;
    [SerializeField] GameObject onlineButton = null;
    [SerializeField] GameObject demoOnlineButton = null;
    [SerializeField] public CharactersDatabase demoCharactersData = null;
    [SerializeField] public CharactersDatabase christmasCharactersData = null;
    [SerializeField] public MasksDatabase demoMasksDatabase = null;
    [SerializeField] public MasksDatabase christmasMasksDatabase = null;
    [SerializeField] public WeaponsDatabase demoWeaponsDatabase = null;
    [SerializeField] public WeaponsDatabase christmasWeaponsDatabase = null;
    #endregion



    #region CHEATS FOR DEVELOPMENT PURPOSES
    [Header("CHEATS")]
    [Tooltip("Use cheat codes ?")]
    [SerializeField] public bool cheatCodes = false;
    [Tooltip("The key to activate the slow motion cheat")]
    //[SerializeField] KeyCode slowTimeKey = KeyCode.Alpha5;
    [HideInInspector] float[] timeSlowDownSteps = { 2f, 0.2f, 0.05f};
    int timeSlowDownLevel = 0;
    #endregion


    [Header("EDITOR ONLY")]
    [SerializeField] public bool letThemFight;
    int losingPlayerIndex = 0;
    int winningPlayerIndex;


    // EVENTS
    [HideInInspector] public delegate void OnResetGameEvent();
    [HideInInspector] public event OnResetGameEvent ResetGameEvent;
    #endregion















    public PlayerControls Controls
    {
        get { return _controls; }
    }
    protected PlayerControls _controls;


    #region FUNCTIONS
    #region BASE FUNCTIONS
    public virtual void Awake()                                        // AWAKE
    {
        Instance = this;
        _controls = new PlayerControls();


        // DEMO
        if (demo)
            TriggerDemoVersion();
    }


    private void OnEnable()                                                                                                             // ON ENABLE
    {
        _controls.Enable();
    }

    private void OnDisable()                                                                                                            // ON DISABLE
    {
        _controls.Disable();
    }

    // Start is called before the first frame update
    public virtual void Start()                                                                                                       // START
    {

        // Set variables
        score = new Vector2(0, 0);
        baseTimeScale = Time.timeScale;
        actualTimeScaleUpdateSmoothness = roundEndTimeScaleFadeSpeed;


        // START GAME
        SetupGame();
    }



    // Update is called once per graphic frame
    public virtual void Update()                                                                                                        // UPDATE
    {
        if (enabled && isActiveAndEnabled)
        {
            // POST PROCESS
            if (kuwahara != null && currentKuwaharaXPow != kuwaharaXPowObjective)
            {
                currentKuwaharaXPow = Mathf.Lerp(currentKuwaharaXPow, kuwaharaXPowObjective, Time.deltaTime * 1f);
                kuwahara.powX.value = Mathf.FloorToInt(currentKuwaharaXPow);

                if (Mathf.Abs(currentKuwaharaXPow - kuwaharaXPowObjective) < 0.2f)
                {
                    currentKuwaharaXPow = Mathf.FloorToInt(kuwaharaXPowObjective);
                    kuwahara.powX.value = Mathf.FloorToInt(currentKuwaharaXPow);

                }
            }
            

            /*
            // CHEATS
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
                */
        }        
    }

    // FixedUpdate is called 50 times per second
    public virtual void FixedUpdate()
    {
        if (enabled && isActiveAndEnabled)
        {
            // EFFECTS
            RunTimeScaleUpdate();


            // ONLINE STUFF
            if (!ConnectManager.Instance.connectedToMaster)
                scoreToWin = Mathf.FloorToInt(scoreToWinSliderComponent.value);
        }
    }
    #endregion








    // DEMO
    void TriggerDemoVersion()
    {
        // CHARACTERS DATA
        if (demoCharactersData != null)
            charactersData = demoCharactersData;
        else
            Debug.Log("Can't find demo characters data, ignoring");

        // DEMO INDICATOR
        if (demoMark != null)
            demoMark.SetActive(true);
        else
            Debug.Log("Can't find demo mark object, ignoring");


        // DEMO BUTTONS
        if (demoStagesButton != null)
            demoStagesButton.SetActive(true);
        else
            Debug.Log("Can't find demo stages button, ignoring");
        if (demoStoryButton != null)
            demoStoryButton.SetActive(true);
        else
            Debug.Log("Can't find demo story button, ignoring");
        if (demoOnlineButton != null)
            demoOnlineButton.SetActive(true);
        else
            Debug.Log("Can't find demo online button, ignoring");


        // NORMAL BUTTONS
        if (stagesButton != null)
            stagesButton.SetActive(false);
        else
            Debug.Log("Can't find stages button, ignoring");
        if (storyButton != null)
            storyButton.SetActive(false);
        else
            Debug.Log("Can't find story button, ignoring");
        if (onlineButton != null)
            onlineButton.SetActive(false);
        else
            Debug.Log("Can't find online button, ignoring");


        // BROWSING
        if (mainMenuBrowser != null && mainMenuBrowser.elements.Length > 0)
        {
            if (demoStagesButton != null && mainMenuBrowser.elements.Length > 2)
                mainMenuBrowser.elements[2] = demoStagesButton;
            if (demoOnlineButton != null && mainMenuBrowser.elements.Length > 3)
                mainMenuBrowser.elements[3] = demoOnlineButton;
            if (demoStoryButton != null && mainMenuBrowser.elements.Length > 4)
                mainMenuBrowser.elements[4] = demoStoryButton;
        }
        else
            Debug.Log("Problem with main menu browser, ignoring");
    }











    // GAME STATE
    public void SwitchState(GAMESTATE newState)                                                                                                                         // SWITCH STATE
    {
        oldState = gameState;
        gameState = newState;


        switch (gameState)
        {
            case GAMESTATE.menu:                                                                      // MENU
                break;

            case GAMESTATE.loading:                                                                      // LOADING
                playerDead = false;
                if (MenuManager.Instance != null)
                {
                    MenuManager.Instance.TriggerPause(false);
                    MenuManager.Instance.winScreen.SetActive(false);
                }
                if (scoreObject != null && scoreObject.GetComponent<Animator>())
                    scoreObject.GetComponent<Animator>().SetBool("On", false);
                // IN GAME HELP
                if (characterSelectionHelpAnimator != null)
                    characterSelectionHelpAnimator.SetBool("On", false);
                Cursor.visible = false;
                break;

            case GAMESTATE.intro:                                                                          // INTRO
                break;

            case GAMESTATE.game:                                                                            // GAME
                if (oldState == GAMESTATE.paused)
                    if (playersList != null && playersList.Count > 0)
                        for (int i = 0; i < playersList.Count; i++)
                            if (playersList[i] != null)
                            {
                                // ONLINE
                                if (ConnectManager.Instance != null && ConnectManager.Instance.enableMultiplayer)
                                {
                                    if (playersList[i].GetComponent<PhotonView>() && playersList[i].GetComponent<PhotonView>().IsMine)
                                    {
                                        if (playersList[i].GetComponent<Player>().oldState != Player.STATE.clashed)
                                            playersList[i].GetComponent<Player>().SwitchState(playersList[i].GetComponent<Player>().oldState);
                                        else
                                            playersList[i].GetComponent<Player>().SwitchState(Player.STATE.normal);
                                    }
                                }
                                else
                                    playersList[i].GetComponent<Player>().SwitchState(playersList[i].GetComponent<Player>().oldState);

                                playersList[i].GetComponent<PlayerAnimations>().animator.speed = 1;
                            }
                if (CameraManager.Instance != null)
                    CameraManager.Instance.SwitchState(CameraManager.CAMERASTATE.battle);
                if (mainMenu != null)
                    mainMenu.SetActive(false);

                if (blurPanel != null)
                    blurPanel.SetActive(false);

                Cursor.visible = false;
                break;

            case GAMESTATE.paused:                                                                                     // PAUSED
                if (playersList != null && playersList.Count > 0)
                    for (int i = 0; i < playersList.Count; i++)
                        if (playersList[i] != null)
                        {
                            // ONLINE
                            if (ConnectManager.Instance != null && ConnectManager.Instance.enableMultiplayer)
                            {
                                if (playersList[i].GetComponent<PhotonView>() && playersList[i].GetComponent<PhotonView>().IsMine)
                                    playersList[i].GetComponent<Player>().SwitchState(Player.STATE.onlinefrozen);
                            }
                            else
                            {
                                playersList[i].GetComponent<PlayerAnimations>().animator.speed = 0;
                                playersList[i].GetComponent<Player>().SwitchState(Player.STATE.frozen);
                            }
                        }
                break;

            case GAMESTATE.finished:                                                                                          // FINISHED
                if (MenuManager.Instance != null)
                    MenuManager.Instance.winMessage.SetActive(true);
                if (oldState == GAMESTATE.paused)
                    if (MenuManager.Instance != null)
                        MenuManager.Instance.SwitchPause();
                break;
        }
    }








    #region BEGIN GAME                                                                                                                                                                      
    // SETUP
    // Setup the game before it starts
    void SetupGame()                                                                                                                                                                       // SET UP GAME
    {
        // SOUND
        // Set on the menu music
        if (AudioManager.Instance != null)
            AudioManager.Instance.SwitchAudioState(AudioManager.AUDIOSTATE.menu);


        // PLAYERS
        SpawnPlayers();


        Invoke("SetupGame2", 0.5f);
    }

    void SetupGame2()                                                                                                                                                                     // SET UP GAME 2
    {
        // CAMERA
        if (CameraManager.Instance != null)
            CameraManager.Instance.FindPlayers();


        // PLAYERS
        if (playersList != null && playersList.Count > 0)
            for (int i = 0; i < playersList.Count; i++)
                if (playersList[i] != null)
                    playersList[i].GetComponent<Player>().ManageOrientation();
    }




    void ConnectPlayer2()                                                                                                                                                               // CONNECT PLAYER 2
    {
        Debug.Log("Player2 joined");
        player2Detected = true;


        bool removeWarning = player2Detected;


        if (playersList[1].GetComponent<Player>().playerIsAI)
        {
            playersList[1].GetComponent<Player>().playerIsAI = false;
            Destroy(playersList[1].GetComponent<IAScript>());
        }

    }

    // Begins the StartMatch coroutine, this function is called by the menu button Sclash
    public void StartMatch()                                                                                                                                                                // START MATCH
    {
        StartCoroutine(StartMatchCoroutine());
    }

    public void ManageAI()                                                                                                                                                              // MANAGE AI
    {
        foreach (Player p in FindObjectsOfType<Player>())
            p.gameObject.AddComponent<IAScript>();

        StartMatch();
    }

    // Starts the match, activates the camera cinematic zoom and then switches to battle camera
    public IEnumerator StartMatchCoroutine()                                                                                                                                            // START MATCH COROUTINE
    {
        StopCoroutine(StartMatchCoroutine());

        // POST PROCESSING
        if (postProcessVolume != null)
            postProcessVolume.profile.TryGetSettings<Kuwahara>(out kuwahara);
        kuwaharaXPowObjective = kuwaharaMaxXPow;
        currentKuwaharaXPow = kuwahara.powX.value;



        // FX
        hajimeFX.Play();


        // AUDIO
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.FindPlayers();
            AudioManager.Instance.SwitchAudioState(AudioManager.AUDIOSTATE.beforeBattle);
        }


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


        yield return new WaitForSeconds(0.1f);


        for (int i = 0; i < playersList.Count; i++)
            playersList[i].GetComponent<Player>().SwitchState(Player.STATE.sneathed);


        // STATE
        SwitchState(GAMESTATE.game);
        if (CameraManager.Instance != null)
            CameraManager.Instance.SwitchState(CameraManager.CAMERASTATE.battle);


        yield return new WaitForSeconds(0.5f);

        /*
        for (int i = 0; i < playersList.Count; i++)
            playersList[i].GetComponent<Player>().SwitchState(Player.STATE.sneathed);
            */


        yield return new WaitForSeconds(timeBeforeBattleCameraActivationWhenGameStarts);


        // Change camera speeds
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.actualXSmoothMovementsMultiplier = CameraManager.Instance.battleXSmoothMovementsMultiplier;
            CameraManager.Instance.actualZoomSpeed = CameraManager.Instance.battleZoomSpeed;
            CameraManager.Instance.actualZoomSmoothDuration = CameraManager.Instance.battleZoomSmoothDuration;
        }


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
    public void SaberDrawn(int playerNum)                                                                                                                                                       // SABER DRAWN
    {
        if (AudioManager.Instance != null && AudioManager.Instance.audioState == AudioManager.AUDIOSTATE.beforeBattle || AudioManager.Instance.audioState == AudioManager.AUDIOSTATE.pause)
        {
            allPlayersHaveDrawn = true;

            if (playersList.Count < 2)
                allPlayersHaveDrawn = false;
            else
                for (int i = 0; i < playersList.Count; i++)
                    if (playersList[i].GetComponent<Player>().playerState == Player.STATE.sneathed || playersList[i].GetComponent<Player>().playerState == Player.STATE.drawing)
                        allPlayersHaveDrawn = false;



            if (allPlayersHaveDrawn)
            {
                //audioManager.ActivateBattleMusic();
                AudioManager.Instance.SwitchAudioState(AudioManager.AUDIOSTATE.battle);

                
                // POST PROCESSING
                if (postProcessVolume != null)
                    postProcessVolume.profile.TryGetSettings<Kuwahara>(out kuwahara);
                kuwaharaXPowObjective = kuwaharaMinXPow;
                currentKuwaharaXPow = kuwahara.powX.value;


                // IN GAME HELP
                characterSelectionHelpAnimator.SetBool("On", false);


                // STATS
                StatsManager.Instance.InitalizeNewGame(1, playersList[0].GetComponent<CharacterChanger>().currentCharacterIndex, playersList[1].GetComponent<CharacterChanger>().currentCharacterIndex);
                StatsManager.Instance.InitializeNewRound();


                // STAGE
                if (MapLoader.Instance.currentMap != null && MapLoader.Instance.currentMap.GetComponent<MapPrefab>())
                    MapLoader.Instance.currentMap.GetComponent<MapPrefab>().TriggerStartStage();
                else
                    Debug.Log("Couldn't find current stage script, ignoring");


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





    void SpawnAI()                                                                                                                                                              // SPAWN AI
    {
        playersList.Clear();
        for (int i = 0; i < playerSpawns.Length; i++)
        {
            //PlayerStats playerStats;
            PlayerAnimations playerAnimations;
            //PlayerAttack playerAttack;
            Player playerScript = null;

            GameObject AI = (GameObject)Resources.Load("Prefabs/PlayerAI");

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
    /// <summary>
    /// Instantiates the players in the duel scene
    /// </summary>
    // Spawns the players
    void SpawnPlayers()                                                                                                                                                         // SPAWN PLAYERS
    {
        for (int i = 0; i < playerSpawns.Length; i++)
        {
            PlayerAnimations playerAnimations;
            Player playerScript = null;

            if (player != null && playerSpawns != null && playerSpawns.Length > 0 && playerSpawns[i] != null)
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
            playerAnimations.legsSpriteRenderer.sortingOrder = 10 * i - 1;


            // FX
            ParticleSystem attackSignParticles = playerScript.attackRangeFX.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule attackSignParticlesMain = attackSignParticles.main;
            attackSignParticlesMain.startColor = attackSignColors[i];


            // VISUAL IDENTIFICATION
            playerScript.characterNameDisplay.text = charactersData.charactersList[0].name;
            playerScript.characterNameDisplay.color = playersColors[i];
            playerScript.characterIdentificationArrow.color = playersColors[i];

            if (demo)
            {
                playerScript.characterChanger.charactersDatabase = demoCharactersData;
                playerScript.characterChanger.masksDatabase = demoMasksDatabase;
                playerScript.characterChanger.weaponsDatabase = demoWeaponsDatabase;
            }
            //playerScript.playerLight.color = playerLightsColors[i];
        }
    }

    // Reset all the players' variables for next round
    void ResetPlayersForNextMatch()                                                                                                                                             // RESET PLAYERS FOR NEXT MATCH 
    {
        if (playerSpawns.Length < 2 || playerSpawns == null)
            playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");

        if (ConnectManager.Instance != null && ConnectManager.Instance.connectedToMaster && playersList != null && playersList.Count > 0)
        {
            foreach (GameObject p in playersList)
                if (p != null)
                {
                    int _tempPNum = p.GetComponent<Player>().playerNum;

                    p.GetComponent<PlayerAnimations>().TriggerSneath();

                    p.transform.position = playerSpawns[_tempPNum].transform.position;
                    p.transform.rotation = playerSpawns[_tempPNum].transform.rotation;
                    p.GetComponent<PlayerAnimations>().ResetAnimsForNextMatch();

                    p.GetComponent<Player>().ResetAllPlayerValuesForNextMatch();
                }
        }
        else if (playersList != null && playersList.Count > 0)
            for (int i = 0; i < playersList.Count; i++)
                if (playersList[i] != null)
                {
                    GameObject p = playersList[i];

                    if (p.GetComponent<PlayerAnimations>())
                        playersList[i].GetComponent<PlayerAnimations>().TriggerSneath();

                    /*
                    p.transform.position = playerSpawns[i].transform.position;
                    p.transform.rotation = playerSpawns[i].transform.rotation;
                    */
                    p.transform.SetPositionAndRotation(playerSpawns[i].transform.position, playerSpawns[i].transform.rotation);
                    if (p.GetComponent<PlayerAnimations>())
                        p.GetComponent<PlayerAnimations>().ResetAnimsForNextMatch();

                    if (p.GetComponent<Player>())
                        p.GetComponent<Player>().ResetAllPlayerValuesForNextMatch();
                }

        playerDead = false;
    }

    // Reset all the players' variables for next round
    void ResetPlayersForNextRound()                                                                                                                                             // RESET PLAYERS FOR NEXT ROUND
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

    public GameObject GetOtherPlayer(GameObject o)                                                                                                                                  // GET OTEHR PLAYER
    {
        for (int i = 0; i < playersList.Count; i++)
            if (playersList[i] == o)
                return o;

        return null;
    }
    #endregion








    #region ROUND TO ROUND & SCORE
    // DEAD
    // Executed when a player dies, starts the score display and next round parameters
    public void APlayerIsDead(int incomingWinning)                                                                                                                                      // A PLAYER IS DEAD
    {
        winningPlayerIndex = incomingWinning;
        losingPlayerIndex = 1 - winningPlayerIndex;

        // STATS
        try
        {
            if (StatsManager.Instance != null)
                StatsManager.Instance.FinalizeRound(winningPlayerIndex);
        }
        catch
        {
            Debug.Log("Error while finalizing the recording of the current round, ignoring");
        }



        playerDead = true;
        UpdatePlayersScoreValues();


        // ONLINE
        if (ConnectManager.Instance != null && ConnectManager.Instance.enableMultiplayer)
        {
            if (gameState == GAMESTATE.paused)
                MenuManager.Instance.TriggerPause(false);
        }



        SwitchState(GAMESTATE.roundFinished);






        if (CheckIfThePlayerWon())
            APlayerWon();
        else
            StartCoroutine(NextRoundCoroutine());
    }


    // SCORE
    void UpdatePlayersScoreValues()                                                                                                                                                     // UPDATE PLAYERS SCORE VALUES
    {
        score[winningPlayerIndex] += 1;

        for (int i = 0; i < playersList.Count; i++)
            scoresDisplays[i].text = score[i].ToString();
    }


    // Builds the score display message
    /*
    string ScoreBuilder()
    {
        string scoreString = "<color=#FF0000>" + score[0].ToString() + "</color> / <color=#0000FF>" + score[1].ToString() + "</color>";
        return scoreString;
    }
    */


    // WON ?
    bool CheckIfThePlayerWon()                                                                                                                                                          // CHECK IF THE PLAYER WON
    {
        if (score[winningPlayerIndex] >= scoreToWin)
            return true;
        else
            return false;
    }


    // NEXT ROUND
    // Starts next round
    IEnumerator NextRoundCoroutine()                                                                                                                    // NEXT ROUND COROUTINE
    {
        yield return new WaitForSecondsRealtime(timeBeforeNextRoundTransitionTriggers);


        ShowScoreBetweenRoundsCoroutine();


        // FX
        if (roundTransitionLeavesFX != null)
            roundTransitionLeavesFX.Play();


        yield return new WaitForSeconds(1.5f);


        ResetPlayersForNextRound();


        // STATS
        try
        {
            StatsManager.Instance.InitializeNewRound();
        }
        catch
        {
            Debug.Log("Error while initializing a new round, ignoring");
        }


        yield return new WaitForSeconds(1f);


        SwitchState(GAMESTATE.game);


        // AUDIO
        if (AudioManager.Instance != null)
            AudioManager.Instance.roundBeginsRandomSoundSource.Play();
    }


    // DISPLAY SCORE
    // Displays the current score for a given amount of time
    void ShowScoreBetweenRoundsCoroutine()                                                                                                                  // SHOW SCORE BETWEEN ROUDNS COROUTINE
    {
        scoreObject.GetComponent<Animator>().SetBool("On", true);


        Invoke("HideScoreBetweenRounds", betweenRoundsScoreShowDuration);
    }
    void HideScoreBetweenRounds()                                                                                                                       // HIDE SCORE BETWEEN ROUNDS
    {
        scoreObject.GetComponent<Animator>().SetBool("On", false);
    }


    // RESET
    // Reset the score and its display
    void ResetScore()                                                                                                                                   // RESET SCORE
    {
        score = new Vector2(0, 0);

        for (int i = 0; i < playersList.Count; i++)
            scoresDisplays[i].text = "0";
    }


    // MAX SCORE
    public void UpdateMaxScoreDisplay()                                                                                                                     // UPDATE MAX SCORE DISPLAY
    {
        maxScoreTextDisplay.text = scoreToWin.ToString();
    }
    #endregion







    #region RESTART GAME
    // Calls ResetGame coroutine, called by main menu button at the end of the match
    public void ResetGame()                                                                                                                                     // RESET GAME
    {
        StartCoroutine(ResetGameCoroutine(false));
    }

    // Calls ResetGame coroutine, called by main menu button at the end of the match
    public void ResetGameAndRematch()                                                                                                                               // RESET GAME & REMATCH
    {
        // ONLINE
        if (ConnectManager.Instance != null && ConnectManager.Instance.enableMultiplayer)
            OnlineRestartCall();
        else
        {
            // Disable win screen
            if (MenuManager.Instance != null && MenuManager.Instance.winScreen.activeInHierarchy)
                MenuManager.Instance.winScreen.SetActive(false);


            ResetGameEvent();
            StartCoroutine(ResetGameCoroutine(true));
        }
    }

    // Resets the match settings and values for a next match
    IEnumerator ResetGameCoroutine(bool rematchRightAfter)                                                                                                          // RESET GAME COROUTINE
    {
        // STATS
        if (gameState != GAMESTATE.finished && allPlayersHaveDrawn)
        {
            try
            {
                if (StatsManager.Instance != null)
                    StatsManager.Instance.FinalizeGame(false, 1);
            }
            catch
            {
                Debug.Log("Error while finalizing the recording of the current game, ignoring");
            }
        }


        // ONLINE
        if (photonView != null && PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();


        // NEXT STAGE
        int newStageIndex = 0;
        if (MapLoader.Instance != null)
            newStageIndex = MapLoader.Instance.currentMapIndex;

        if (gameState == GAMESTATE.finished && rematchRightAfter)
            newStageIndex = CalculateNextStageIndex();


        // STATE
        SwitchState(GAMESTATE.loading);


        // Activates the menu blur panel if it is not supposed to start a new match right after
        if (!rematchRightAfter)
            blurPanel.SetActive(true);

        // IN GAME INDICATIONS
        if (drawTextAnimator != null)
        {
            drawTextAnimator.ResetTrigger("FadeOut");
            drawTextAnimator.SetTrigger("FadeOut");
            drawTextAnimator.ResetTrigger("FadeIn");
        }

        if (playersList != null && playersList.Count > 0)
            for (int i = 0; i < playersList.Count; i++)
            {
                if (playersList[i] != null)
                    playersList[i].GetComponent<PlayerAnimations>().nameDisplayAnimator.SetBool("On", false);

                if (inGameHelp != null && inGameHelp.Length > i && inGameHelp[i] != null)
                    inGameHelp[i].SetBool("On", false);

                if (playerKeysIndicators != null && playerKeysIndicators.Count > i && playerKeysIndicators[i] != null)
                    playerKeysIndicators[i].SetBool("On", false);
            }


        // FX
        roundTransitionLeavesFX.Play();


        // AUDIO
        if (AudioManager.Instance != null)
            AudioManager.Instance.SwitchAudioState(AudioManager.AUDIOSTATE.none);


        yield return new WaitForSecondsRealtime(resetGameDelay);


        ResetPlayersForNextMatch();
        TriggerMatchEndFilterEffect(false);


        // PLAYERS LIGHTS / COLORS
        if (playersList != null && playersList.Count > 0 && playerLightsColors != null && playerLightsColors.Length > 0)
            for (int i = 0; i < playersList.Count; i++)
                if (playersList[i] != null && playersList[i].GetComponent<Player>() && playersList[i].GetComponent<Player>().playerLight != null && playerLightsColors[i] != null)
                {
                    playersList[i].GetComponent<Player>().playerLight.color = playerLightsColors[i];
                    playersList[i].GetComponent<Player>().playerLight.intensity = 5;
                }


        // NEXT STAGE
        if (MapLoader.Instance != null)
        {
            if (demo && MapLoader.Instance.halloween) // Halloween stage for demo
                MapLoader.Instance.SetMap(0, true);
            else if (demo && MapLoader.Instance.christmas) // Christmas stage for demo
                MapLoader.Instance.SetMap(1, true);
            else
                MapLoader.Instance.SetMap(newStageIndex, false);
        }


        ResetScore();


        // CAMERA
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.SwitchState(CameraManager.CAMERASTATE.inactive);
            CameraManager.Instance.actualXSmoothMovementsMultiplier = CameraManager.Instance.cinematicXSmoothMovementsMultiplier;
            CameraManager.Instance.actualZoomSmoothDuration = CameraManager.Instance.cinematicZoomSmoothDuration;
            CameraManager.Instance.gameObject.transform.position = CameraManager.Instance.cameraArmBasePos;
            if (CameraManager.Instance.cameraComponent != null)
                CameraManager.Instance.cameraComponent.transform.position = CameraManager.Instance.cameraBasePos;
        }


        // Restarts a new match right after it is finished being set up
        if (rematchRightAfter)
            StartCoroutine(StartMatchCoroutine());
        else
        {
            // STATE
            SwitchState(GAMESTATE.menu);

            // Activates the main menu if it is not supposed to start a new match right after
            if (MenuManager.Instance != null && MenuManager.Instance.mainMenu != null)
                MenuManager.Instance.mainMenu.SetActive(true);
            Cursor.visible = true;


            // AUDIO
            if (AudioManager.Instance != null)
                AudioManager.Instance.SwitchAudioState(AudioManager.AUDIOSTATE.menu);
        }
    }


    // ONLINE
    void OnlineRestartCall()                                                                                                                        // ONLINE RESTART CALL
    {
        if (ConnectManager.Instance != null)
            ConnectManager.Instance.RestartCall();
    }
    #endregion




    #region MATCH END   
    public void APlayerLeft()                                                                                                                               // A PLAYER LEFT
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


    void APlayerWon()                                                                                                                                                // A PLAYER WON
    {
        // STATS
        try
        {
            if (StatsManager.Instance != null)
                StatsManager.Instance.FinalizeGame(true, 1);
        }
        catch
        {
            Debug.Log("Error while finalizing the recording of the current game, ignoring");
        }


        // AUDIO
        if (AudioManager.Instance != null)
            AudioManager.Instance.SwitchAudioState(AudioManager.AUDIOSTATE.won);


        // SCORE
        scoreObject.GetComponent<Animator>().SetBool("On", false);
        if (playersList != null && playersList.Count > 0)
            for (int i = 0; i < playersList.Count; i++)
            {
                if (playersList[i] != null && playersList[i].GetComponent<PlayerAnimations>() && playersList[i].GetComponent<PlayerAnimations>().nameDisplayAnimator != null)
                    playersList[i].GetComponent<PlayerAnimations>().nameDisplayAnimator.SetBool("On", false);
                if (inGameHelp != null && inGameHelp.Length > i && inGameHelp[i] != null)
                    inGameHelp[i].SetBool("On", false);
                if (playerKeysIndicators != null && playerKeysIndicators.Count > i && playerKeysIndicators[i] != null)
                    playerKeysIndicators[i].SetBool("On", false);
            }

        Invoke("EndGame", 4f);
    }
    #endregion



    // END
    // RENAME HERE IF WORKING
    void EndGame()                                                                                                                                                  // END GAME
    {
        // GAME STATE
        SwitchState(GAMESTATE.finished);


        // PLAYER STATE
        if (playersList != null && playersList.Count > 0 && playersList[winningPlayerIndex] != null && playersList[winningPlayerIndex].GetComponent<Player>())
        {
            playersList[winningPlayerIndex].GetComponent<Player>().SwitchState(Player.STATE.sneathing);
            playersList[winningPlayerIndex].GetComponent<Player>().SwitchState(Player.STATE.frozen);
        }


        // WIN MENU
        if (MenuManager.Instance != null)
            MenuManager.Instance.SetUpWinMenu(charactersData.charactersList[playersList[winningPlayerIndex].GetComponent<Player>().characterIndex].name, playersColors[winningPlayerIndex], score, playersColors);


        // ANIMATION
        Invoke("TriggerFallDeadAnimation", 2f);


        Invoke("ShowMenu", 2f + timeBeforeWinScreenAppears);
    }


    // DEATH
    // Animation
    void TriggerFallDeadAnimation()                                                                                                                                     // TRIGGER FALL DEATH ANIMATION
    {
        if (playersList != null && playersList.Count > 0 && playersList[losingPlayerIndex] != null && playersList[losingPlayerIndex].GetComponent<PlayerAnimations>())
            playersList[losingPlayerIndex].GetComponent<PlayerAnimations>().TriggerRealDeath();
    }


    // MENU
    void ShowMenu()                                                                                                                                             // SHOW MENU
    {
        // MENU
        if (blurPanel != null)
            blurPanel.SetActive(false);
        if (MenuManager.Instance != null && MenuManager.Instance.winScreen != null)
            MenuManager.Instance.winScreen.SetActive(true);
        Cursor.visible = true;


        // AUDIO
        if (AudioManager.Instance != null && AudioManager.Instance.winMusicAudioSource != null)
            AudioManager.Instance.winMusicAudioSource.Play();
    }





    #region EFFECTS
    public void TriggerMatchEndFilterEffect(bool on)                                                                                                        // TRIGGER MATCH END FILTER EFFECT
    {
        if (on)
        {
            // Players particles set to none
            if (playersList != null && playersList.Count > 0)
                for (int i = 0; i < playersList.Count; i++)
                    if (playersList[i] != null && playersList[i].GetComponent<Player>())
                        playersList[i].GetComponent<Player>().SetParticleSets(1);



            // Deactivates background elements for only orange color
            /* DONT DELETE
            for (int i = 0; i < mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements.Length; i++)
                mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements[i].SetActive(false);
                */

                    // STAGE ELEMENTS
            if (MapLoader.Instance.currentMap != null && MapLoader.Instance.currentMap != null && MapLoader.Instance.currentMap.GetComponent<MapPrefab>())
                MapLoader.Instance.currentMap.GetComponent<MapPrefab>().TriggerDramaticScreen();



            // List of all renderers for the death VFX
            spriteRenderers = GameObject.FindObjectsOfType<SpriteRenderer>();
            meshRenderers = GameObject.FindObjectsOfType<MeshRenderer>();
            skinnedMeshRenderers = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();
            particleSystems = GameObject.FindObjectsOfType<ParticleSystem>();
            lights = GameObject.FindObjectsOfType<Light>();


            // All renderers' original properties storage for the death VFX reset
            originalSpriteRenderersColors = new List<Color>();
            originalSpriteRenderersMaterials = new List<Material>();
            originalMeshRenderersColors = new List<Color>();
            skinnedMeshRenderesColors = new List<Color>();
            originalParticleSystemsColors = new List<Color>();
            originalLightsIntensities = new List<float>();
            originalParticleSystemsGradients = new List<Gradient>();



            // SET ALL BLACK
            // SPRITES
            if (spriteRenderers != null && spriteRenderers.Length > 0)
                for (int i = 0; i < spriteRenderers.Length; i++)
                    if (!spriteRenderers[i].CompareTag("NonBlackFX"))
                    {

                        originalSpriteRenderersColors.Add(spriteRenderers[i].color);
                        spriteRenderers[i].color = Color.black;

                        if (spriteRenderers[i].tag != tagsReferences.playerTag && spriteRenderers[i].tag != tagsReferences.comesticsTag)
                        {
                            originalSpriteRenderersMaterials.Add(spriteRenderers[i].material);
                            spriteRenderers[i].material = deathFXSpriteMaterial;
                        }
                    }
            // MESHES
            if (meshRenderers != null && meshRenderers.Length > 0)
                for (int i = 0; i < meshRenderers.Length; i++)
                    if (!meshRenderers[i].CompareTag("NonBlackFX") && meshRenderers[i].gameObject.activeInHierarchy)
                    {
                        try
                        {
                            // Store original color DONT DELETE
                            //originalMeshRenderersColors.Add(meshRenderers[i].material.color);
                            // Set black
                            meshRenderers[i].material.color = Color.black;
                        }
                        catch { }
                    }
            // SKINNED MESHES
            if (skinnedMeshRenderers != null && skinnedMeshRenderers.Length > 0)
                for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                    if (!skinnedMeshRenderers[i].CompareTag("NonBlackFX") && skinnedMeshRenderers[i].gameObject.activeInHierarchy)
                    {
                        try
                        {
                            // Store original color
                            skinnedMeshRenderesColors.Add(skinnedMeshRenderers[i].material.color);
                            // Set black
                            skinnedMeshRenderers[i].material.color = Color.black;
                        }
                        catch { }
                    }
            // PARTICLES
            if (particleSystems != null && particleSystems.Length > 0)
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
            if (lights != null && lights.Length > 0)
                for (int i = 0; i < lights.Length; i++)
                    if (lights[i] != null && !lights[i].CompareTag("NonBlackFX") && lights[i].gameObject.activeInHierarchy)
                        lights[i].gameObject.SetActive(false);
        }
        else
        {

            // RESET ALL
            // SPRITES

            if (spriteRenderers != null && spriteRenderers.Length > 0)
                for (int i = 0; i < spriteRenderers.Length; i++)
                    if (spriteRenderers[i] != null && !spriteRenderers[i].CompareTag("NonBlackFX"))
                    {
                        

                        // If player sprite
                        if (spriteRenderers[i].tag == tagsReferences.playerTag || spriteRenderers[i].tag == tagsReferences.comesticsTag)
                        {
                            spriteRenderers[i].color = Color.white;
                            /*
                            if (playerSpriteMaterial != null)
                                spriteRenderers[i].material = playerSpriteMaterial;
                                */
                        }
                        else
                        {
                            if (originalSpriteRenderersColors.Count > i && originalSpriteRenderersColors[i] != null)
                                spriteRenderers[i].color = originalSpriteRenderersColors[i];
                            if (originalSpriteRenderersMaterials.Count > i && originalSpriteRenderersMaterials[i] != null)
                                spriteRenderers[i].material = originalSpriteRenderersMaterials[i];
                        }
                    }
            /*
            // MESHES DONT DELETE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (meshRenderers != null && meshRenderers.Length > 0)
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                    if (meshRenderers[i] != null && !meshRenderers[i].CompareTag("NonBlackFX") && meshRenderers[i].gameObject.activeInHierarchy)
                        meshRenderers[i].material.color = originalMeshRenderersColors[i];
            }
            */
            // SKINNED MESHES
            if (skinnedMeshRenderers != null && skinnedMeshRenderers.Length > 0)
                for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                    if (skinnedMeshRenderers[i] != null && !skinnedMeshRenderers[i].CompareTag("NonBlackFX") && skinnedMeshRenderers[i].gameObject.activeInHierarchy)
                        if (skinnedMeshRenderesColors.Count > i && skinnedMeshRenderesColors[i] != null)
                            skinnedMeshRenderers[i].material.color = skinnedMeshRenderesColors[i];
            // PARTICLES
            if (particleSystems != null && particleSystems.Length > 0)
                for (int i = 0; i < particleSystems.Length; i++)
                    if (particleSystems[i] != null && !particleSystems[i].CompareTag("NonBlackFX"))
                    {
                        try
                        {
                            ParticleSystem.MainModule particleSystemMain = particleSystems[i].main;


                            if (originalParticleSystemsGradients.Count > i && originalParticleSystemsGradients[i] != null)
                                particleSystemMain.startColor = originalParticleSystemsGradients[i];
                            if (originalParticleSystemsColors.Count > i && originalParticleSystemsColors[i] != null)
                                particleSystemMain.startColor = originalParticleSystemsColors[i];
                        }
                        catch { }
                    }
            // LIGHTS
            /* DONT DELETE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (lights != null && lights.Length > 0)
                for (int i = 0; i < lights.Length; i++)
                    try
                    {
                        if (lights[i] != null && !lights[i].CompareTag("NonBlackFX") && lights[i].gameObject.activeInHierarchy)
                            lights[i].gameObject.SetActive(true);
                    }
                    catch { }
                    */

            /* DONT DELETE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Reactivates the background of the map if it's referenced
            if (mapLoader.currentMap.GetComponent<MapPrefab>() && mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements.Length > 0)
                for (int i = 0; i < mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements.Length; i++)
                    mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements[i].SetActive(true);
            */
        }
    }


    // SLOW MO
    // Starts the SlowMo coroutine
    public void TriggerSlowMoCoroutine(float slowMoEffectDuration, float slowMoTimeScale, float fadeSpeed)                                                                  // TRIGGER SLOW ME COROUTINE
    {
        if (useSlowMotion)
        {
            StopCoroutine(SlowMoCoroutine(slowMoEffectDuration, slowMoTimeScale, fadeSpeed));
            StartCoroutine(SlowMoCoroutine(slowMoEffectDuration, slowMoTimeScale, fadeSpeed));
        }
    }


    // Slow motion and zoom for a given duration
    IEnumerator SlowMoCoroutine(float slowMoEffectDuration, float slowMoTimeScale, float fadeSpeed)                                                                         // SLOW MO COROUTINE
    {
        // CAMERA STATE
        if (CameraManager.Instance != null)
            CameraManager.Instance.SwitchState(CameraManager.CAMERASTATE.eventcam);


        // TIME
        actualTimeScaleUpdateSmoothness = fadeSpeed;
        timeScaleObjective = slowMoTimeScale;


        // FX
        if (useAnimeLines)
        {
            if (animeLinesFx != null)
                animeLinesFx.Play();
            else
                Debug.Log("Couldn't find anime lines FX, ignoring");
        }



        // AUDIO
        if (score[winningPlayerIndex] < scoreToWin && AudioManager.Instance != null)
            AudioManager.Instance.TriggerSlowMoAudio(true);


        yield return new WaitForSecondsRealtime(slowMoEffectDuration);


        // TIME
        actualTimeScaleUpdateSmoothness = roundEndTimeScaleFadeSpeed;
        timeScaleObjective = baseTimeScale;


        // CAMERA
        if (CameraManager.Instance != null)
            CameraManager.Instance.SwitchState(CameraManager.CAMERASTATE.battle);


        // AUDIO
        /*
        for (int i = 0; i < audioManager.battleMusicPhaseSources.Length; i++)
        {
            audioManager.battleMusicPhaseSources[i].pitch = 1;
            audioManager.battleMusicStrikesSources[i].pitch = 1;
        }
        */
        if (score != null && score[winningPlayerIndex] < scoreToWin && AudioManager.Instance != null)
            AudioManager.Instance.TriggerSlowMoAudio(false);


        yield return new WaitForSecondsRealtime(0.5f);


        // TIME
        Time.timeScale = timeScaleObjective;
        // FX
        if (useAnimeLines)
        {
            if (animeLinesFx != null)
                animeLinesFx.Stop();
            else
                Debug.Log("Couldn't find anime lines FX, ignoring");
        }
    }


    // Update the timescale smoothly for smooth slow mo effects in FixedUpdate
    void RunTimeScaleUpdate()                                                                                                                            // RUN TIMESCALE UPDATE
    {
        if (runTimeScaleUpdate)
        {
            if (FastApproximately(Time.timeScale, timeScaleObjective, 0.06f) || timeScaleObjective == Time.timeScale)
                Time.timeScale = timeScaleObjective;
            else
                Time.timeScale += actualTimeScaleUpdateSmoothness * Mathf.Sign(timeScaleObjective - Time.timeScale);


            if (Time.timeScale <= minTimeScale)
                Time.timeScale = minTimeScale;

            // AUDIO
            if (AudioManager.Instance != null)
                for (int i = 0; i < AudioManager.Instance.phasesMainAudioSources.Count; i++)
                {
                    AudioManager.Instance.phasesMainAudioSources[i].pitch = Time.timeScale;
                    AudioManager.Instance.phasesStrikesAudioSources[i].pitch = Time.timeScale;
                }
        }
    }
    #endregion





    #region SECONDARY FUNCTIONS
    // STAGE INDEX
    public int CalculateNextStageIndex()                                                                                                            // CALCULATE NEXT STAGE INDEX
    {
        int nextStageIndex = 0;
        if (MapLoader.Instance != null)
            nextStageIndex = MapLoader.Instance.currentMapIndex;

        int loopCount = 0;


        // IF NO DEMO
        if (!demo)
        {
            // DAY NIGHT
            if (gameParameters.dayNightCycle)
            {
                if (!gameParameters.randomStage)
                {
                    if (MapLoader.Instance.mapsData.stagesLists[MapLoader.Instance.currentMapIndex].type == STAGETYPE.day)
                        nextStageIndex = MapLoader.Instance.currentMapIndex + 1;
                    if (MapLoader.Instance.mapsData.stagesLists[MapLoader.Instance.currentMapIndex].type == STAGETYPE.night)
                        nextStageIndex = MapLoader.Instance.currentMapIndex - 1;
                }
                else
                {
                    if (MapLoader.Instance.mapsData.stagesLists[MapLoader.Instance.currentMapIndex].type == STAGETYPE.day)
                        nextStageIndex = MapLoader.Instance.currentMapIndex + 1;
                    else
                    {
                        nextStageIndex = Random.Range(0, MapLoader.Instance.mapsData.stagesLists.Count);

                        if (gameParameters.useCustomListForRandom)
                            while (!MapLoader.Instance.mapsData.stagesLists[nextStageIndex].inCustomList || nextStageIndex == MapLoader.Instance.currentMapIndex || !(MapLoader.Instance.mapsData.stagesLists[nextStageIndex].type == STAGETYPE.day))
                            {
                                nextStageIndex = Random.Range(0, MapLoader.Instance.mapsData.stagesLists.Count);
                                loopCount++;


                                if (loopCount >= 100)
                                {
                                    nextStageIndex = 0;
                                    break;
                                }
                            }
                        else
                            while (nextStageIndex == MapLoader.Instance.currentMapIndex || MapLoader.Instance.mapsData.stagesLists[nextStageIndex].type == STAGETYPE.night)
                            {
                                nextStageIndex = Random.Range(0, MapLoader.Instance.mapsData.stagesLists.Count);
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
            // If not day / night
            else if (gameParameters.randomStage)
            {
                nextStageIndex = Random.Range(0, MapLoader.Instance.mapsData.stagesLists.Count);


                if (gameParameters.useCustomListForRandom)
                    while (!MapLoader.Instance.mapsData.stagesLists[nextStageIndex].inCustomList || nextStageIndex == MapLoader.Instance.currentMapIndex)
                    {
                        nextStageIndex = Random.Range(0, MapLoader.Instance.mapsData.stagesLists.Count);

                        loopCount++;
                        if (loopCount >= 100)
                        {
                            nextStageIndex = 0;
                            break;
                        }
                    }
                else
                    while (nextStageIndex == MapLoader.Instance.currentMapIndex)
                    {
                        nextStageIndex = Random.Range(0, MapLoader.Instance.mapsData.stagesLists.Count);

                        loopCount++;
                        if (loopCount >= 100)
                        {
                            nextStageIndex = 0;
                            break;
                        }
                    }
            }
        }


        return nextStageIndex;
    }


    // COMPARE FLOATS
    // Compares 2 floats with a range of tolerance
    public static bool FastApproximately(float a, float b, float threshold)                                                                                                 // FAST APPROXIMATELY
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }
    #endregion










    // EDITOR STUFF
    private void OnDrawGizmos()
    {
        // Display if it's the demo version
        if (demoMark != null)
        {
            if (demo && !demoMark.activeInHierarchy)
                demoMark.SetActive(true);
            if (!demo && demoMark.activeInHierarchy)
                demoMark.SetActive(false);
        }


#if UNITY_EDITOR
        HandleUtility.Repaint();
#endif
    }




    void RemoveWarnings()
    {
        timeSlowDownLevel += timeSlowDownLevel;
    }
    #endregion
}
