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

    InputManager inputManager = null;

    [Tooltip("The CameraShake scripts instances references in the scene")]
    [SerializeField]
    public CameraShake
        deathCameraShake = null,
        clashCameraShake = null,
        pommelCameraShake = null,
        finalCameraShake = null;

    [SerializeField] StatsManager statsManager = null;
    #endregion





    #region DATA
    [Header("DATA")]
    [SerializeField] CharactersDatabase charactersData = null;
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
    // START
    [Header("START")]
    [Tooltip("The delay before the battle camera values are entered in the camera parameters to make it reactive for battle once the game started, because the smooth camera values stay some time to smooth the zoom towards the scene")]
    [SerializeField] float timeBeforeBattleCameraActivationWhenGameStarts = 2f;
    [SerializeField] Animator drawTextAnimator = null;
    bool drawTextVisible = false;
    # endregion





    # region MENU
    // MENU
    [Header("MENU")]
    [Tooltip("The main menu object reference")]
    [SerializeField] GameObject mainMenu = null;
    [Tooltip("The blur panel object reference")]
    [SerializeField] GameObject blurPanel = null;
    #endregion




    #region IN GAME INFOS
    [Header("IN GAME INFOS")]
    [SerializeField] public Animator[] inGameHelp = null;
    [SerializeField] public List<Animator> playerKeysIndicators = new List<Animator>(2);
    [SerializeField] List<TextMeshProUGUI> playerHelpTextIdentifiers = new List<TextMeshProUGUI>(2);
    [SerializeField] List<Image> playerHelpIconIdentifiers = new List<Image>(2);
    #endregion




    #region SCORE DISPLAY
    [Header("SCORE DISPLAY")]
    [Tooltip("The score display game object reference")]
    [SerializeField] public GameObject scoreObject = null;

    [Tooltip("The score display text mesh pro component reference")]
    [SerializeField] List<TextMeshProUGUI> scoresNames = new List<TextMeshProUGUI>(2);
    [SerializeField] List<Text> scoresDisplays = new List<Text>(2);
    [SerializeField] TextMeshProUGUI maxScoreTextDisplay = null;
    #endregion

    #region SCORE CALCULATION
    [Header("SCORE CALCULATION")]
    [HideInInspector] public Vector2 score = new Vector2(0, 0);
    [Tooltip("The duration the score lasts on screen when a round has finished")]
    [SerializeField] float betweenRoundsScoreShowDuration = 4f;
    [Tooltip("The score to reach to win")]
    public int scoreToWin = 10;
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
    [SerializeField] GameObject playerAI = null;
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
    //[SerializeField] string[] playerNames = { "Aka", "Ao" };
    [HideInInspector] public bool playerDead = false;
    bool allPlayersHaveDrawn = false;
    bool player2Detected = false;
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
    [SerializeField]
    public ParticleSystem
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

    [SerializeField]
    public bool letThemFight;
    int winningPlayerIndex;


















    # region FUNCTIONS
    # region BASE FUNCTIONS
    // BASE FUNCTIONS
    private void Awake()
    {
        Instance = this;

        inputManager = InputManager.Instance;
    }

    // Start is called before the first frame update
    public void Start()
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

    void OnEnable()
    {

    }

    // Update is called once per graphic frame
    private void Update()
    {
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

        if (!ConnectManager.Instance.connectedToMaster)
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
                //menuManager.pauseMenu.SetActive(false);
                menuManager.TriggerPause(false);
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
                    if (playersList[i] != null)
                    {
                        playersList[i].GetComponent<Player>().SwitchState(Player.STATE.frozen);
                        playersList[i].GetComponent<PlayerAnimations>().animator.speed = 0;
                    }
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
        audioManager.SwitchAudioState(AudioManager.AUDIOSTATE.menu);


        SpawnPlayers();
        yield return new WaitForSeconds(0.5f);
        cameraManager.FindPlayers();
    }

    void ConnectPlayer2()
    {
        Debug.Log("Player2 joined");
        player2Detected = true;
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
        {
            p.gameObject.AddComponent<IAScript>();
        }

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


        for (int i = 0; i < playersList.Count; i++)
        {
            playersList[i].GetComponent<Player>().SwitchState(Player.STATE.sneathed);
        }


        yield return new WaitForSeconds(0.1f);


        SwitchState(GAMESTATE.game);
        cameraManager.SwitchState(CameraManager.CAMERASTATE.battle);


        yield return new WaitForSeconds(0.5f);


        yield return new WaitForSeconds(timeBeforeBattleCameraActivationWhenGameStarts);


        cameraManager.actualXSmoothMovementsMultiplier = cameraManager.battleXSmoothMovementsMultiplier;
        cameraManager.actualZoomSpeed = cameraManager.battleZoomSpeed;
        cameraManager.actualZoomSmoothDuration = cameraManager.battleZoomSmoothDuration;


        yield return new WaitForSeconds(10f);


        // Appears draw text if both players haven't drawn
        allPlayersHaveDrawn = true;

        for (int i = 0; i < playersList.Count; i++)
        {
            if (playersList[i].GetComponent<Player>().playerState == Player.STATE.sneathed || playersList[i].GetComponent<Player>().playerState == Player.STATE.drawing)
            {
                allPlayersHaveDrawn = false;
            }
        }


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
        if (audioManager.audioState == AudioManager.AUDIOSTATE.beforeBattle)
        {
            allPlayersHaveDrawn = true;

            for (int i = 0; i < playersList.Count; i++)
            {
                if (playersList[i].GetComponent<Player>().playerState == Player.STATE.sneathed || playersList[i].GetComponent<Player>().playerState == Player.STATE.drawing)
                    allPlayersHaveDrawn = false;
            }


            if (allPlayersHaveDrawn)
            {
                //audioManager.ActivateBattleMusic();
                audioManager.SwitchAudioState(AudioManager.AUDIOSTATE.battle);


                // STATS
                statsManager.InitalizeNewGame(1);
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

    #region PLAYERS
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
            IAScript ia = null;

#if UNITY_EDITOR
            if (letThemFight || i == 1)
                ia = playersList[i].AddComponent<IAScript>();
#else
            if (i == 1)
                ia = playersList[i].AddComponent<IAScript>();
#endif
            if (ia != null)
                ia.SetDifficulty(IAScript.Difficulty.Hard);

            //playerStats = playersList[i].GetComponent<PlayerStats>();*
            playerAnimations = playersList[i].GetComponent<PlayerAnimations>();
            playerScript = playersList[i].GetComponent<Player>();

            playerScript.playerNum = i;
            playerScript.ResetAllPlayerValuesForNextMatch();


            // ANIMATIONS
            //playerAnimations.spriteRenderer.color = playersColors[i];
            //playerAnimations.legsSpriteRenderer.color = playersColors[i];

            playerAnimations.spriteRenderer.sortingOrder = 10 * i;
            playerAnimations.legsSpriteRenderer.sortingOrder = 10 * i;
            playerAnimations.legsMask.GetComponent<SpriteMask>().frontSortingOrder = 10 * i + 2;
            playerAnimations.legsMask.GetComponent<SpriteMask>().backSortingOrder = 10 * i - 2;


            // FX
            ParticleSystem attackSignParticles = playerScript.attackRangeFX.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule attackSignParticlesMain = attackSignParticles.main;
            attackSignParticlesMain.startColor = attackSignColors[i];


            // VISUAL IDENTIFICATION
            playerScript.characterNameDisplay.text = charactersData.charactersList[0].name;
            playerScript.characterNameDisplay.color = playersColors[i];
            playerScript.characterIdentificationArrow.color = playersColors[i];
            playerScript.playerLight.color = playerLightsColors[i];
        }
    }

    // Reset all the players' variables for next round
    void ResetPlayersForNextMatch()
    {
        if (playerSpawns.Length < 2)
            playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");

        if (ConnectManager.Instance.connectedToMaster)
        {
            foreach (GameObject p in playersList)
            {
                int _tempPNum = p.GetComponent<Player>().playerNum;

                p.GetComponent<PlayerAnimations>().TriggerSneath();
                p.GetComponent<PlayerAnimations>().ResetDrawText();

                p.transform.position = playerSpawns[_tempPNum].transform.position;
                p.transform.rotation = playerSpawns[_tempPNum].transform.rotation;
                p.GetComponent<PlayerAnimations>().ResetAnimsForNextMatch();

                p.GetComponent<Player>().ResetAllPlayerValuesForNextMatch();

            }
        }
        else
        {

            for (int i = 0; i < playersList.Count; i++)
            {
                GameObject p = playersList[i];


                playersList[i].GetComponent<PlayerAnimations>().TriggerSneath();
                playersList[i].GetComponent<PlayerAnimations>().ResetDrawText();


                p.transform.position = playerSpawns[i].transform.position;
                p.transform.rotation = playerSpawns[i].transform.rotation;
                p.GetComponent<PlayerAnimations>().ResetAnimsForNextMatch();


                p.GetComponent<Player>().ResetAllPlayerValuesForNextMatch();
            }
        }

        playerDead = false;
    }

    // Reset all the players' variables for next round
    void ResetPlayersForNextRound()
    {
        if (playerSpawns.Length < 2)
            playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");

        if (ConnectManager.Instance.connectedToMaster)
        {
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
        }
        else
        {
            for (int i = 0; i < playersList.Count; i++)
            {
                GameObject p = playersList[i];

                p.transform.position = playerSpawns[i].transform.position;
                p.transform.rotation = playerSpawns[i].transform.rotation;
                p.GetComponent<PlayerAnimations>().ResetAnimsForNextRound();
                p.GetComponent<Player>().SwitchState(Player.STATE.normal);


                p.GetComponent<Player>().ResetAllPlayerValuesForNextRound();
            }
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

    #endregion








    #region ROUND TO ROUND & SCORE
    // Executed when a player dies, starts the score display and next round parameters
    public void APlayerIsDead(int incomingWinning)
    {
        winningPlayerIndex = incomingWinning;

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
        UpdatePlayersScoreValues();
        SwitchState(GAMESTATE.roundFinished);


        if (CheckIfThePlayerWon())
        {
            APlayerWon();
            // StartCoroutine(APlayerWon(winningPlayerIndex));
        }
        else
        {
            StartCoroutine(NextRoundCoroutine());
        }
    }

    void UpdatePlayersScoreValues()
    {
        score[winningPlayerIndex] += 1;

        for (int i = 0; i < playersList.Count; i++)
        {
            scoresDisplays[i].text = score[i].ToString();
        }
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


        // AUDIO
        //audioManager.UpdateMusicPhaseThatShouldPlayDependingOnScore();


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
        {
            scoresDisplays[i].text = "0";
        }
        //scoreTextComponent.text = ScoreBuilder();
    }

    public void UpdateMaxScoreDisplay()
    {
        maxScoreTextDisplay.text = scoreToWin.ToString();
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
                Debug.Log("Error while finalizing the recording of the current game, ignoring");
            }
        }


        // ONLINE
        if (photonView != null && ConnectManager.Instance.enableMultiplayer)
        {
            PhotonNetwork.LeaveRoom();
        }


        // NEXT STAGE
        int newStageIndex = mapLoader.currentMapIndex;

        if (gameState == GAMESTATE.finished)
        {
            if (rematchRightAfter)
            {
                newStageIndex = CalculateNextStageIndex();
            }
        }


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



        // STATE
        SwitchState(GAMESTATE.menu);


        ResetPlayersForNextMatch();
        TriggerMatchEndFilterEffect(false);

        // PLAYERS LIGHTS / COLORS
        for (int i = 0; i < playersList.Count; i++)
        {
            playersList[i].GetComponent<Player>().playerLight.color = playerLightsColors[i];
            playersList[i].GetComponent<Player>().playerLight.intensity = 5;
        }


        // NEXT STAGE
        mapLoader.SetMap(newStageIndex);



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
        {
            if (p != null)
            {
                continue;
            }
            else
            {
                playersList.Remove(p);
                break;
            }
        }

        Debug.Log("<color=red>The opponent left</color>");
        //APlayerWon();
    }


    void APlayerWon()
    {
        // STATS
        try
        {
            statsManager.FinalizeGame(true, 1);
        }
        catch
        {
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
        //menuManager.winName.text = charactersData.charactersList[playersList[winningPlayerIndex].GetComponent<Player>().characterIndex].name;
        //menuManager.winName.color = playersColors[winningPlayerIndex];
        menuManager.SetUpWinMenu(charactersData.charactersList[playersList[winningPlayerIndex].GetComponent<Player>().characterIndex].name, playersColors[winningPlayerIndex], score, playersColors);


        // AUDIO
        //audioManager.adjustBattleMusicVolumeDepdendingOnPlayersDistance = false;


        // ANIMATION
        //playersList[winningPlayerIndex].GetComponent<PlayerAnimations>().TriggerSneath();
        //playersList[winningPlayerIndex].GetComponent<PlayerAnimations>().ResetDrawText();


        //yield return new WaitForSecondsRealtime(timeBeforeWinScreenAppears);
        Invoke("ShowMenu", timeBeforeWinScreenAppears);
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
    // EFFECTS
    public void TriggerMatchEndFilterEffect(bool on)
    {
        if (on)
        {

            // Deactivates background elements for only orange color
            for (int i = 0; i < mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements.Length; i++)
            {
                mapLoader.currentMap.GetComponent<MapPrefab>().backgroundElements[i].SetActive(false);
            }



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
                if (!spriteRenderers[i].CompareTag("NonBlackFX") && spriteRenderers[i].gameObject.activeInHierarchy)
                {
                    originalSpriteRenderersColors.Add(spriteRenderers[i].color);
                    spriteRenderers[i].color = Color.black;

                    originalSpriteRenderersMaterials.Add(spriteRenderers[i].material);
                    spriteRenderers[i].material = deathFXSpriteMaterial;
                }
            }
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                if (!meshRenderers[i].CompareTag("NonBlackFX") && meshRenderers[i].gameObject.activeInHierarchy)
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
                if (!lights[i].CompareTag("NonBlackFX") && lights[i].gameObject.activeInHierarchy)
                {
                    originalLightsIntensities.Add(lights[i].intensity);
                    lights[i].intensity = 0;
                }
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
                        if (!spriteRenderers[i].CompareTag("NonBlackFX") && spriteRenderers[i].gameObject.activeInHierarchy)
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
                    if (meshRenderers[i])
                    {
                        if (!meshRenderers[i].CompareTag("NonBlackFX") && meshRenderers[i].gameObject.activeInHierarchy)
                        {
                            meshRenderers[i].material.color = originalMeshRenderersColors[i];
                        }
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
                    try
                    {
                        if (lights[i])
                        {
                            if (!lights[i].CompareTag("NonBlackFX") && lights[i].gameObject.activeInHierarchy)
                            {
                                lights[i].intensity = originalLightsIntensities[i];
                            }
                        }
                    }
                    catch
                    {

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
        StopCoroutine(SlowMoCoroutine(slowMoEffectDuration, slowMoTimeScale, fadeSpeed));
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

                Debug.Log(mapLoader.currentMapIndex);
            }
            else
            {
                if (mapLoader.mapsData.stagesLists[mapLoader.currentMapIndex].type == STAGETYPE.day)
                    nextStageIndex = mapLoader.currentMapIndex + 1;
                else
                {
                    nextStageIndex = Random.Range(0, mapLoader.mapsData.stagesLists.Count);

                    if (gameParameters.useCustomListForRandom)
                    {
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
                    }
                    else
                    {
                        while (nextStageIndex == mapLoader.currentMapIndex || mapLoader.mapsData.stagesLists[nextStageIndex].type == STAGETYPE.night)
                        {
                            nextStageIndex = Random.Range(0, mapLoader.mapsData.stagesLists.Count);
                            Debug.Log(nextStageIndex == mapLoader.currentMapIndex);
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
        }
        // RANDOM
        else if (gameParameters.randomStage)
        {
            nextStageIndex = Random.Range(0, mapLoader.mapsData.stagesLists.Count);

            if (gameParameters.useCustomListForRandom)
            {
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
            }
            else
            {
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
