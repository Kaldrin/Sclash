using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region VARIABLES
    #region MANAGERS
    // MANAGERS
    [Header("MANAGERS")]
    // Audio manager
    [Tooltip("The name of the object in the scene containing the AudioManager script component, to find its reference")]
    [SerializeField] string audioManagerName = "GlobalManager";
    AudioManager audioManager;

    // Game manager
    [Tooltip("The name of the object in the scene containing the GlobalManager script component, to find its reference")]
    [SerializeField] string gameManagerName = "GlobalManager";
    GameManager gameManager;

    // Input manager
    [Tooltip("The name of the object in the scene containing the InputManager script component, to find its reference")]
    [SerializeField] string inputManagerName = "GlobalManager";
    InputManager inputManager = null;
    # endregion



   

    # region PLAYER'S COMPONENTS
    // PLAYER'S COMPONENTS
    [Header("PLAYER'S COMPONENTS")]
    [SerializeField] Rigidbody2D rb = null;
    [SerializeField] PlayerAnimations playerAnimations = null;
    [Tooltip("The basic collider of the player")]
    [SerializeField] public Collider2D playerCollider;
    [Tooltip("All of the player's 2D colliders")]
    [SerializeField] public Collider2D[] playerColliders = null;
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [Tooltip("The reference to the light component which lits the player with their color")]
    [SerializeField] public Light playerLight = null;
    #endregion






    #region PLAYER STATES
    // PLAYER STATES
    public enum STATE
    {
        frozen,
        sneathing,
        sneathed,
        drawing,
        normal,
        charging,
        attacking,
        pommeling,
        parrying,
        jumping,
        dashing,
        recovering,
        clashed,
        enemyKilled,
        enemyKilledEndMatch,
        dead,
    }

    [SerializeField] public STATE playerState = STATE.normal;
    [HideInInspector] public STATE oldState = STATE.normal;

    [SerializeField] bool hasFinishedAnim = false;
    # endregion






    # region PLAYERS
    // PLAYERS
    [Header("PLAYERS")]
    [HideInInspector] public int playerNum = 0;
    int otherPlayerNum = 0;
    # endregion






    # region HEALTH
    // HEALTH
    [Header("HEALTH")]
    [Tooltip("The maximum health of the player")]
    [SerializeField] float maxHealth = 1;
    float currentHealth;

    [Tooltip("Can the player be hit in the current frames ?")]
    [SerializeField] public bool untouchableFrame = false;

    [Tooltip("The opacity amount of the player's sprite when in untouchable frames")]
    [SerializeField] float untouchableFrameOpacity = 0.3f;
    # endregion





    # region STAMINA
    //STAMINA
    [Header("STAMINA")]
    [Tooltip("The reference to the base stamina slider attached to the player to create the other sliders")]
    [SerializeField] public Slider staminaSlider = null;
    List<Slider> staminaSliders = new List<Slider>();

    [Tooltip("The amount of stamina each move will cost when executed")]
    [SerializeField] public float staminaCostForMoves = 1;
    [Tooltip("The maximum amount of stamina one player can have")]
    [SerializeField] public float maxStamina = 4f;
    [Tooltip("Stamina parameters")]
    [SerializeField]
    float
        durationBeforeStaminaRegen = 0.5f,
        staminaGlobalGainOverTimeMultiplier = 1f,
        idleStaminaGainOverTimeMultiplier = 0.8f,
        backWalkingStaminaGainOverTime = 0.8f,
        frontWalkingStaminaGainOverTime = 0.4f,
        staminaBarBaseOpacity = 0.8f;
    [HideInInspector] public float stamina = 0;
    float
        currentTimeBeforeStaminaRegen = 0,
        staminaBarsOpacity = 1,
        oldStamina = 0;

    [HideInInspector] public bool canRegenStamina = true;

    [Tooltip("Stamina colors depending on how much there is left")]
    [SerializeField]
    Color
        staminaBaseColor = Color.green,
        staminaLowColor = Color.yellow,
        staminaDeadColor = Color.red;
    # endregion





    # region MOVEMENTS
    // MOVEMENTS
    [Header("MOVEMENTS")]
    [Tooltip("The default movement speed of the player")]
    [SerializeField] float baseMovementsSpeed = 2.5f;
    [SerializeField]
    float
        chargeMovementsSpeed = 1.2f,
        sneathedMovementsSpeed = 1.8f,
        attackingMovementsSpeed = 2.2f;
    float actualMovementsSpeed = 1;
    #endregion





    # region ORIENTATION
    // ORIENTATION
    [Header("ORIENTATION")]
    [Tooltip("The duration before the player can orient again towards the enemy if they need to once they applied the orientation")]
    [SerializeField] float orientationCooldown = 0.1f;
    float
        orientationCooldownStartTime = 0;
    
    bool
        orientationCooldownFinished = true,
        canOrientTowardsEnemy = true;
    # endregion





    # region DRAW
    // DRAW
    [Header("DRAW")]
    [Tooltip("The duration the draw animation takes to switch to drawn state")]
    [SerializeField] public float drawDuration = 2f;

    [Tooltip("The reference to the game object containing the text component telling the players to draw their sabers")]
    [SerializeField] public GameObject drawText = null;
    Vector3 drawTextBaseScale = new Vector3(0, 0, 0);
    #endregion




    /*
    #region JUMP
    // JUMP
    [SerializeField]
    float
        fallMultiplier = 2.5f,
        jumpHeight = 10f;
    #endregion
    */





    #region CHARGE
    // CHARGE
    [Header("CHARGE")]
    [Tooltip("The number of charge levels for the attack, so the number of range subdivisions")]
    [SerializeField] public int maxChargeLevel = 4;
    [HideInInspector] public int chargeLevel = 1;

    [Tooltip("Charge duration parameters")]
    [SerializeField] float
        durationToNextChargeLevel = 0.7f,
        maxHoldDurationAtMaxCharge = 2f;
    [SerializeField] float attackReleaseAxisInputDeadZoneForDashAttack = 0.1f;
    float
        maxChargeLevelStartTime = 0,
        chargeStartTime = 0;

    [HideInInspector] public bool canCharge = true;
    # endregion





    # region ATTACK
    // ATTACK
    [Header("ATTACK")]
    [Tooltip("Attack range parameters")]
    [SerializeField] public float lightAttackRange = 1.8f;
    [Tooltip("Attack range parameters")]
    [SerializeField] public float
        heavyAttackRange = 3.2f,
        attackRangeDisjoint = 0.2f;
    [SerializeField] float axisDeadZoneForAttackDash = 0.2f;
    [HideInInspector] public float actualAttackRange = 0;

    [Tooltip("Frame parameters for the attack")]
    [SerializeField] public bool
        activeFrame = false,
        clashFrames = false;
    [HideInInspector] public bool isAttacking = false;



    // ATTACK RECOVERY
    [Header("ATTACK RECOVERY")]
    [SerializeField] bool hasAttackRecoveryAnimFinished = false;

    # endregion





    #region DASH
    // DASH
    [Header("DASH")]
    [SerializeField] public float
        baseDashSpeed = 3;
    [SerializeField] public float
        forwardDashDistance = 3,
        backwardsDashDistance = 2.5f;
    [SerializeField] float
        allowanceDurationForDoubleTapDash = 0.3f,
        forwardAttackDashDistance = 2.5f,
        backwardsAttackDashDistance = 1.5f,
        dashDeadZone = 0.5f,
        shortcutDashDeadZone = 0.5f;
    float
       dashDirection,
       temporaryDashDirectionForCalculation,
       dashInitializationStartTime = 0,
       actualUsedDashDistance,
       dashTime = 0;

    enum DASHSTEP
    {
        rest,
        firstInput,
        firstRelease,
        invalidated,
    }

    DASHSTEP currentDashStep = DASHSTEP.invalidated;
    DASHSTEP currentShortcutDashStep = DASHSTEP.invalidated;

    Vector3
        initPos,
        targetPos;

    bool isDashing = false;
    # endregion





    # region KICK
    // KICK
    [Header("KICK")]
    [Tooltip("Is currently applying the pommel effect to what they touches ?")]
    [SerializeField] public bool kickFrame = false;
    [HideInInspector] public bool canPommel = true;

    [SerializeField]
    float
        kickRange = 0.88f;
    # endregion





    # region KICKED
    // KICKED
    [Header("KICKED")]
    [Tooltip("The distance the player will be pushed on when pommeled")]
    [SerializeField] float kickKnockbackDistance = 1f;
    # endregion






    # region PARRY
    // PARRY
    [Header("PARRY")]
    [Tooltip("Only editable by animator, is currently in parry frames state")]
    [SerializeField] public bool parryFrame = false;
    [HideInInspector] public bool canParry = true;
    
    //[SerializeField] int numberOfFramesToDetectParryInput = 3;
    int currentParryFramesPressed = 0;
    # endregion






    # region CLASHED
    // CLASHED
    [Header("CLASHED")]
    [Tooltip("The distance the player will be pushed on when clashed")]
    [SerializeField] float clashKnockback = 2;
    //[SerializeField] float clashDuration = 2f;
    [Tooltip("The speed at which the knockback distance will be covered")]
    [SerializeField] public float clashKnockbackSpeed = 2;
    #endregion






    #region FX
    // FX
    [Header("FX")]
    [Tooltip("The references to the game objects holding the different FXs")]
    [SerializeField] GameObject clashFXPrefabRef = null;
    [SerializeField] GameObject
        staminaGainFX = null,
        deathBloodFX = null;

    [Tooltip("The attack sign FX object reference, the one that spawns at the range distance before the attack hits")]
    [SerializeField] public ParticleSystem attackRangeFX = null;
    [SerializeField] ParticleSystem
        chargeFlareFX = null,
        chargeFX = null,
        chargeFullFX = null,
        clashKanasFX = null,
        kickKanasFX = null,
        kickedFX = null,
        staminaLossFX = null,
        clashFX = null,
        slashFX = null;

    [Tooltip("The slider component reference to move the charging FX on the katana")]
    [SerializeField] Slider chargeSlider = null;

    [SerializeField] float attackSignDisjoint = 0.4f;
    [Tooltip("The amount to rotate the death blood FX's object because for some reason it takes another rotation when it plays :/")]
    [SerializeField] float deathBloodFXRotationForDirectionChange = 240;
    [Tooltip("The width of the attack trail depending on the range of the attack")]
    [SerializeField] float
        lightAttackSwordTrailWidth = 20f,
        heavyAttackSwordTrailWidth = 65f;
    [Tooltip("The minimum speed required for the walk fx to trigger")]
    [SerializeField] float minSpeedForWalkFX = 0.05f;

    Vector3 deathFXbaseAngles = new Vector3(0, 0, 0);

    [Tooltip("The reference to the TrailRenderer component of the saber")]
    [SerializeField] TrailRenderer swordTrail = null;

    [Tooltip("The colors of the attack trail depending on the range of the attack")]
    [SerializeField] Color
        lightAttackColor = Color.yellow,
        heavyAttackColor = Color.red;

    [SerializeField] Gradient lightAttackGradientColor = null;

    Vector3 deathBloodFXBaseRotation = Vector3.zero;
    #endregion






    # region STAGE DEPENDENT FX
    // STAGFE DEPENDENT FX
    [Header("STAGE DEPENDENT FX")]
    [SerializeField] ParticleSystem
        dashFXFront = null;
    [SerializeField] ParticleSystem
        dashFXBack = null,
        attackDashFXFront = null,
        attackDashFXBack = null,
        attackNeutralFX = null;
    [Tooltip("The references to the particle systems components used for the walk leaves FX")]
    [SerializeField] ParticleSystem
        walkFXFront = null,
        walkFXBack = null;
    # endregion






    # region SOUND
    // SOUND
    [Header("SOUND")]
    [Tooltip("The reference to the stamina charged audio FX AudioSource")]
    [SerializeField] AudioSource staminaBarChargedAudioEffectSource = null;
    #endregion







    #region CHEATS FOR DEVELOPMENT PURPOSES
    // CHEATS FOR DEVELOPMENT PURPOSES
    [Header("CHEATS")]
    [Tooltip("The cheat key to trigger a clash for the player")]
    [SerializeField] KeyCode clashCheatKey = KeyCode.Alpha1;
    [Tooltip("The other cheat keys for other effects")]
    [SerializeField] KeyCode
        deathCheatKey = KeyCode.Alpha2,
        staminaCheatKey = KeyCode.Alpha4;
    #endregion
    #endregion




























    # region FUNCTIONS
    #region BASE FUNCTIONS
    // BASE FUNCTIONS
    void Start()
    {
        // GET MANAGERS
        // Get audio manager to use in the script
        audioManager = GameObject.Find(audioManagerName).GetComponent<AudioManager>();
        // Get game manager to use in the script
        gameManager = GameObject.Find(gameManagerName).GetComponent<GameManager>();
        // Get input manager
        inputManager = GameObject.Find(inputManagerName).GetComponent<InputManager>();


        deathBloodFXBaseRotation = deathBloodFX.transform.localEulerAngles;
        drawTextBaseScale = drawText.transform.localScale;
       

        // Begin by reseting all the player's values and variable to start fresh
        StartCoroutine(GetOtherPlayerNum());
        SetUpStaminaBars();
        deathFXbaseAngles = deathBloodFX.transform.localEulerAngles;
        ResetAllPlayerValuesForNextMatch();
    }

    // Update is called once per graphic frame
    void Update()
    {
        // Action depending on state
        switch (playerState)
        {
            case STATE.frozen:
                ManageOrientation();
                break;

            case STATE.sneathing:
                break;

            case STATE.sneathed:
                ManageDraw();
                break;

            case STATE.drawing:
                break;

            case STATE.normal:
                ManageJump();
                ManageChargeInput();
                ManageDashInput();
                ManagePommel();
                ManageParryInput();
                break;

            case STATE.charging:
                ManageDashInput();
                ManagePommel();
                ManageParryInput();
                ManageCharging();
                break;

            case STATE.attacking:
                break;

            case STATE.recovering:
                break;

            case STATE.pommeling:
                ManageDashInput();
                break;

            case STATE.parrying:
                break;

            case STATE.jumping:
                break;

            case STATE.dashing:
                ManageDashInput();
                ManageChargeInput();
                ManagePommel();
                ManageParryInput();
                break;

            case STATE.clashed:
                break;

            case STATE.enemyKilled:
                break;

            case STATE.enemyKilledEndMatch:
                break;

            case STATE.dead:
                break;
        }


        // Cheatcodes to use for development purposes
        if (gameManager.cheatCodes)
            CheatsInputs();
    }

    // FixedUpdate is called 50 times per second
    void FixedUpdate()
    {
        if (kickFrame)
        {
            ApplyPommelHitbox();
        }


        if (untouchableFrame)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, untouchableFrameOpacity);
        }
        else
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        }


        // Behaviour depending on state
        switch (playerState)
        {
            case STATE.frozen:
                ManageOrientation();
                SetStaminaBarsOpacity(0);
                rb.velocity = Vector3.zero;
                break;

            case STATE.sneathing:
                UpdateStaminaSlidersValue();
                SetStaminaBarsOpacity(staminaBarsOpacity);
                UpdateStaminaColor();
                Debug.Log("Sneathing");
                if (hasFinishedAnim)
                {
                    hasFinishedAnim = false;
                    SwitchState(STATE.sneathed);
                }
                break;

            case STATE.sneathed:
                break;

            case STATE.drawing:
                UpdateStaminaSlidersValue();
                SetStaminaBarsOpacity(staminaBarsOpacity);
                UpdateStaminaColor();
                if (hasFinishedAnim)
                {
                    hasFinishedAnim = false;
                    SwitchState(STATE.normal);
                }
                break;

            case STATE.normal:
                ManageMovementsInputs();
                ManageOrientation();
                ManageStaminaRegen();
                UpdateStaminaSlidersValue();
                SetStaminaBarsOpacity(staminaBarsOpacity);
                UpdateStaminaColor();
                playerAnimations.UpdateIdleStateDependingOnStamina(stamina);
                break;

            case STATE.charging:
                ManageMovementsInputs();
                ManageStaminaRegen();
                UpdateStaminaSlidersValue();
                SetStaminaBarsOpacity(staminaBarsOpacity);
                UpdateStaminaColor();
                break;

            case STATE.attacking:
                RunDash();
                ManageMovementsInputs();
                if (hasFinishedAnim)
                {
                    hasFinishedAnim = false;
                    SwitchState(STATE.recovering);
                }
                UpdateStaminaSlidersValue();
                SetStaminaBarsOpacity(staminaBarsOpacity);
                UpdateStaminaColor();
                // Apply damages if the current attack animation has entered active frame, thus activating the bool in the animation
                if (activeFrame)
                {
                    ApplyAttackHitbox();
                }
                break;

            case STATE.recovering:
                if (hasAttackRecoveryAnimFinished)
                {
                    hasFinishedAnim = false;
                    SwitchState(STATE.normal);
                }
                ManageStaminaRegen();
                UpdateStaminaSlidersValue();
                SetStaminaBarsOpacity(staminaBarsOpacity);
                UpdateStaminaColor();
                RunDash();
                rb.velocity = Vector3.zero;
                break;

            case STATE.pommeling:
                RunDash();
                if (hasFinishedAnim)
                {
                    hasFinishedAnim = false;
                    SwitchState(STATE.normal);
                }
                UpdateStaminaSlidersValue();
                SetStaminaBarsOpacity(staminaBarsOpacity);
                UpdateStaminaColor();
                break;

            case STATE.parrying:
                if (hasFinishedAnim)
                {
                    hasFinishedAnim = false;
                    SwitchState(STATE.normal);
                }
                UpdateStaminaSlidersValue();
                SetStaminaBarsOpacity(staminaBarsOpacity);
                UpdateStaminaColor();
                break;

            case STATE.jumping:
                UpdateStaminaSlidersValue();
                SetStaminaBarsOpacity(staminaBarsOpacity);
                UpdateStaminaColor();
                break;

            case STATE.dashing:
                UpdateStaminaSlidersValue();
                SetStaminaBarsOpacity(staminaBarsOpacity);
                UpdateStaminaColor();
                RunDash();
                break;

            case STATE.clashed:
                ManageStaminaRegen();
                UpdateStaminaSlidersValue();
                SetStaminaBarsOpacity(staminaBarsOpacity);
                UpdateStaminaColor();
                RunDash();
                if (hasFinishedAnim)
                {
                    hasFinishedAnim = false;
                    SwitchState(STATE.normal);
                }
                rb.velocity = Vector3.zero;
                break;

            case STATE.enemyKilled:
                ManageMovementsInputs();
                //ManageOrientation();
                SetStaminaBarsOpacity(0);
                playerAnimations.UpdateIdleStateDependingOnStamina(stamina);
                break;

            case STATE.enemyKilledEndMatch:
                ManageMovementsInputs();
                //ManageOrientation();
                SetStaminaBarsOpacity(0);
                //playerAnimations.UpdateIdleStateDependingOnStamina(stamina);
                break;

            case STATE.dead:
                break;
        }
    }
    #endregion






    #region STATE SWITCH
    public void SwitchState(STATE newState)
    {
        oldState = playerState;
        playerState = newState;


        switch (newState)
        {
            case STATE.frozen:
                SetStaminaBarsOpacity(0);
                attackDashFXFront.Stop();
                attackDashFXBack.Stop();
                dashFXBack.Stop();
                dashFXFront.Stop();
                break;

            case STATE.sneathing:
                rb.velocity = Vector3.zero;
                playerAnimations.TriggerSneath();
                break;

            case STATE.sneathed:
                drawText.SetActive(true);
                staminaBarsOpacity = 0;
                actualMovementsSpeed = sneathedMovementsSpeed;
                rb.simulated = true;
                break;

            case STATE.drawing:
                rb.velocity = Vector3.zero;
                break;

            case STATE.normal:
                actualMovementsSpeed = baseMovementsSpeed;
                dashTime = 0;
                //canCharge = false;
                playerCollider.isTrigger = false;
                for (int i = 0; i < playerColliders.Length; i++)
                {
                    playerColliders[i].isTrigger = false;
                }
                attackDashFXFront.Stop();
                attackDashFXBack.Stop();
                break;

            case STATE.charging:
                chargeLevel = 1;
                chargeStartTime = Time.time;
                actualMovementsSpeed = chargeMovementsSpeed;
                break;

            case STATE.attacking:
                isDashing = true;
                canCharge = false;
                chargeLevel = 1;
                chargeSlider.value = 1;
                actualMovementsSpeed = attackingMovementsSpeed;
                playerCollider.isTrigger = true;
                for (int i = 0; i < playerColliders.Length; i++)
                {
                    playerColliders[i].isTrigger = true;
                }
                PauseStaminaRegen();
                StaminaCost(staminaCostForMoves);
                //swordTrail.startWidth = actualAttackRange;
                break;

            case STATE.pommeling:
                chargeLevel = 1;
                rb.velocity = Vector3.zero;
                PauseStaminaRegen();
                break;

            case STATE.parrying:
                chargeLevel = 1;
                canParry = false;
                PauseStaminaRegen();
                StaminaCost(staminaCostForMoves);
                dashFXBack.Stop();
                dashFXFront.Stop();
                break;

            case STATE.jumping:
                PauseStaminaRegen();
                break;

            case STATE.dashing:
                canCharge = false;
                currentDashStep = DASHSTEP.invalidated;
                currentShortcutDashStep = DASHSTEP.invalidated;
                chargeLevel = 1;
                playerCollider.isTrigger = true;
                isDashing = true;
                for (int i = 0; i < playerColliders.Length; i++)
                {
                    playerColliders[i].isTrigger = true;
                }
                StaminaCost(staminaCostForMoves);
                PauseStaminaRegen();
                break;

            case STATE.recovering:
                rb.velocity = Vector3.zero;
                break;

            case STATE.clashed:
                chargeLevel = 1;
                playerCollider.isTrigger = true;
                for (int i = 0; i < playerColliders.Length; i++)
                {
                    playerColliders[i].isTrigger = true;
                }
                PauseStaminaRegen();
                attackDashFXFront.Stop();
                attackDashFXBack.Stop();
                break;

            case STATE.enemyKilled:
                SetStaminaBarsOpacity(0);
                stamina = maxStamina;
                attackDashFXFront.Stop();
                attackDashFXBack.Stop();
                dashFXBack.Stop();
                dashFXFront.Stop();
                break;

            case STATE.dead:
                rb.velocity = Vector3.zero;
                rb.simulated = false;
                SetStaminaBarsOpacity(0);
                playerCollider.isTrigger = true;
                for (int i = 0; i < playerColliders.Length; i++)
                {
                    playerColliders[i].isTrigger = true;
                }
                chargeFlareFX.gameObject.SetActive(false);
                chargeFlareFX.gameObject.SetActive(true);
                walkFXBack.Stop();
                walkFXFront.Stop();
                drawText.SetActive(false);
                break;
        }
    }
    # endregion







    #region PLAYERS
    // PLAYERS
    IEnumerator GetOtherPlayerNum()
    {
        yield return new WaitForSeconds(0.2f);


        for (int i = 0; i < gameManager.playersList.Count; i++)
        {
            if (i + 1 != playerNum)
            {
                otherPlayerNum = i;
            }
        }
    }
    # endregion







    # region RESET ALL VALUES
    // RESET ALL VALUES
    public void ResetAllPlayerValuesForNextMatch()
    {
        SwitchState(Player.STATE.frozen);


        currentHealth = maxHealth;
        stamina = maxStamina;
        staminaSlider.gameObject.SetActive(true);
        canRegenStamina = true;
        chargeLevel = 1;


        // Restablishes physics
        rb.gravityScale = 1;
        rb.simulated = true;


        // Restablishes colliders
        playerCollider.isTrigger = false;


        for (int i = 0; i < playerColliders.Length; i++)
        {
            playerColliders[i].isTrigger = false;
        }

        
        // ANIMATIONS
        playerAnimations.CancelCharge(true);
        playerAnimations.ResetAnimsForNextMatch();
        playerAnimations.ResetDrawText();
    }


    public void ResetAllPlayerValuesForNextRound()
    {
        SwitchState(STATE.normal);


        currentHealth = maxHealth;
        stamina = maxStamina;
        staminaSlider.gameObject.SetActive(true);
        canRegenStamina = true;
        chargeLevel = 1;


        // Restablishes physics
        rb.gravityScale = 1;
        rb.simulated = true;


        // Restablishes colliders
        playerCollider.isTrigger = false;


        for (int i = 0; i < playerColliders.Length; i++)
        {
            playerColliders[i].isTrigger = false;
        }


        // ANIMATIONS
        playerAnimations.CancelCharge(true);
        playerAnimations.ResetAnimsForNextRound();
    }
    # endregion







    # region RECEIVE AN ATTACK
    // RECEIVE AN ATTACK
    public bool TakeDamage(GameObject instigator, int hitStrength = 1)
    {
        bool hit = false;


        if (playerState != STATE.dead)
        {
            // CLASH
            if (clashFrames)
            {
                TriggerClash();
                instigator.GetComponent<Player>().TriggerClash();


                // FX
                Vector3 fxPos = new Vector3((gameManager.playersList[0].transform.position.x + gameManager.playersList[1].transform.position.x) / 2, clashFX.transform.position.y, clashFX.transform.position.z);
                Instantiate(clashFXPrefabRef, fxPos, clashFX.transform.rotation, null).GetComponent<ParticleSystem>().Play();
            }
            // PARRY
            else if (parryFrame)
            {
                // STAMINA
                stamina += staminaCostForMoves;


                // CLASH
                instigator.GetComponent<Player>().TriggerClash();


                // FX
                clashFX.Play();


                // SOUND
                audioManager.TriggerParriedAudio();
            }
            // UNTOUCHABLE FRAMES
            else if (untouchableFrame)
            {
                gameManager.TriggerSlowMoCoroutine(gameManager.dodgeSlowMoDuration, gameManager.dodgeSlowMoTimeScale, gameManager.dodgeTimeScaleFadeSpeed);


                // SOUND
                audioManager.BattleEventIncreaseIntensity();
            }
            // TOUCHED
            else
            {
                hit = true;
                TriggerHit();
                audioManager.BattleEventIncreaseIntensity();
            }


            // IS DEAD ?
            if (currentHealth <= 0 && playerState != STATE.dead)
            {
                SwitchState(STATE.dead);


                if (gameManager.score[instigator.GetComponent<Player>().playerNum] + 1 >= gameManager.scoreToWin)
                {
                    //gameManager.StartFinalDeathVFXCoroutine();
                    gameManager.TriggerMatchEndFilterEffect(true);
                }


                // ANIMATIONS
                playerAnimations.TriggerDeath();
                playerAnimations.DeathActivated(true);


                // FX
                chargeFX.Stop();


                // CAMERA FX
                gameManager.APlayerIsDead(instigator.GetComponent<Player>().playerNum);
            }
        }


        // FX
        attackRangeFX.gameObject.SetActive(false);
        attackRangeFX.gameObject.SetActive(true);


        return hit;
    }

    // Hit
    void TriggerHit()
    {
        currentHealth -= 1;


        // SOUND
        audioManager.TriggerSuccessfulAttackAudio();
        audioManager.BattleEventIncreaseIntensity();
        

        // FX
        slashFX.Play();


        // CAMERA FX
        gameManager.cameraShake.shakeDuration = gameManager.deathCameraShakeDuration;
        gameManager.TriggerSlowMoCoroutine(gameManager.roundEndSlowMoDuration, gameManager.roundEndSlowMoTimeScale, gameManager.roundEndTimeScaleFadeSpeed);
    }
    # endregion







    #region STAMINA
    // STAMINA
    // Set up stamina bar system
    void SetUpStaminaBars()
    {
        staminaSliders.Add(staminaSlider);


        for (int i = 0; i < maxStamina - 1; i++)
        {
            staminaSliders.Add(Instantiate(staminaSlider.gameObject, staminaSlider.transform.parent).GetComponent<Slider>());
        }
    }

    // Manage stamina regeneration, executed in FixedUpdate
    void ManageStaminaRegen()
    {
        if (stamina < maxStamina && canRegenStamina)
        {
            // If back walking
            if (rb.velocity.x * -transform.localScale.x < 0)
            {
                stamina += Time.deltaTime * backWalkingStaminaGainOverTime * staminaGlobalGainOverTimeMultiplier;
            }
            // If idle walking
            else if (Mathf.Abs(rb.velocity.x) <= 0.5f)
            {
                stamina += Time.deltaTime * idleStaminaGainOverTimeMultiplier * staminaGlobalGainOverTimeMultiplier;
            }
            // If front walking
            else
            {
                stamina += Time.deltaTime * frontWalkingStaminaGainOverTime * staminaGlobalGainOverTimeMultiplier;
            }
        }


        // Small duration before the player can regen stamina again after a move
        if (currentTimeBeforeStaminaRegen <= 0 && !canRegenStamina)
        {
            currentTimeBeforeStaminaRegen = 0;
            canRegenStamina = true;
        }
        else if (!canRegenStamina)
        {
            currentTimeBeforeStaminaRegen -= Time.deltaTime;
        }
    }

    // Trigger the stamina regen pause duration
    public void PauseStaminaRegen()
    {
        canRegenStamina = false;
        currentTimeBeforeStaminaRegen = durationBeforeStaminaRegen;
    }

    // Function to decrement to stamina
    public void StaminaCost(float cost)
    {
        stamina -= cost;


        if (stamina <= 0)
        {
            stamina = 0;
        }


        // FX
        staminaLossFX.Play();
    }

    // Update stamina slider value
    void UpdateStaminaSlidersValue()
    {
        // DETECT STAMINA CHARGE UP
        if (Mathf.FloorToInt(oldStamina) < Mathf.FloorToInt(stamina))
        {
            if (!gameManager.playerDead && gameManager.gameState == GameManager.GAMESTATE.game)
            {
                staminaBarChargedAudioEffectSource.Play();

                staminaGainFX.SetActive(false);
                staminaGainFX.SetActive(true);
                staminaGainFX.GetComponent<ParticleSystem>().Play();
            }
        }


        oldStamina = stamina;


        staminaSliders[0].value = Mathf.Clamp(stamina, 0, 1);
        staminaLossFX.gameObject.transform.position = staminaSliders[(int)Mathf.Clamp((int)(stamina + 0.5f), 0, maxStamina - 1)].transform.position;
        staminaGainFX.gameObject.transform.position = staminaSliders[(int)Mathf.Clamp((int)(stamina - 0.5f), 0, maxStamina - 1)].transform.position + new Vector3(0.2f, 0, 0) * Mathf.Sign(transform.localScale.x);


        for (int i = 1; i < staminaSliders.Count; i++)
        {
            staminaSliders[i].value = Mathf.Clamp(stamina, i, i + 1) - i;
        }


        if (stamina >= maxStamina)
        {
            if (staminaBarsOpacity > 0)
                staminaBarsOpacity -= 0.05f;
        }
        else if (staminaBarsOpacity != staminaBarBaseOpacity)
        {
            staminaBarsOpacity = staminaBarBaseOpacity;
        }
    }

    // Manages stamina bars opacity
    void SetStaminaBarsOpacity(float opacity)
    {
        for (int i = 0; i < staminaSliders.Count; i++)
        {
            Color
                fillColor = staminaSliders[i].fillRect.GetComponent<Image>().color,
                backgroundColor = staminaSliders[i].transform.GetChild(0).GetComponent<Image>().color;


            staminaSliders[i].fillRect.GetComponent<Image>().color = new Color(fillColor.r, fillColor.g, fillColor.b, opacity);
            staminaSliders[i].transform.GetChild(0).GetComponent<Image>().color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, opacity);
        }
    }

    void UpdateStaminaColor()
    {
        if (stamina < staminaCostForMoves)
        {
            SetStaminaColor(staminaDeadColor);
        }
        else if (stamina < staminaCostForMoves * 2)
        {
            SetStaminaColor(staminaLowColor);
        }
        else
        {
            SetStaminaColor(staminaBaseColor);
        }
    }

    void SetStaminaColor(Color color)
    {
        for (int i = 0; i < staminaSliders.Count; i++)
        {
            staminaSliders[i].fillRect.gameObject.GetComponent<Image>().color = Color.Lerp(staminaSliders[i].fillRect.gameObject.GetComponent<Image>().color, color, Time.deltaTime * 10);
        }
    }
    # endregion







    #region MOVEMENTS
    // MOVEMENTS
    void ManageMovementsInputs()
    {
        // The player move if they can in their state
        if (rb.simulated == false)
            rb.simulated = true;


        rb.velocity = new Vector2(inputManager.playerInputs[playerNum].horizontal * actualMovementsSpeed, rb.velocity.y);


        // FX
        if (Mathf.Abs(rb.velocity.x) > minSpeedForWalkFX && GameManager.Instance.gameState == GameManager.GAMESTATE.game && playerState == Player.STATE.normal)
        {
            if ((rb.velocity.x * -transform.localScale.x) < 0)
            {
                walkFXFront.Stop();


                if (!walkFXBack.isPlaying)
                    walkFXBack.Play();
            }
            else
            {
                if (!walkFXFront.isPlaying)
                    walkFXFront.Play();


                walkFXBack.Stop();
            }
        }
        else
        {
            walkFXBack.Stop();
            walkFXFront.Stop();
        }
    }
    #endregion







    # region DRAW
    // DRAW
    // Detects draw input
    void ManageDraw()
    {
        if (inputManager.playerInputs[playerNum].anyKey)
        {
            TriggerDraw();
        }
    }

    // Triggers saber draw and informs the game manager
    void TriggerDraw()
    {
        SwitchState(STATE.drawing);
        gameManager.SaberDrawn(playerNum);


        // ANIMATION
        playerAnimations.TriggerDraw();
        playerAnimations.TriggerDrawText();
    }
    # endregion








    # region JUMP
    // JUMP
    void ManageJump()
    {
        if (!inputManager.playerInputs[playerNum].jump)
        {
            StartCoroutine(JumpCoroutine());
        }
    }

    IEnumerator JumpCoroutine()
    {
        yield return new WaitForSeconds(0);
    }
    # endregion








    #region CHARGE
    //CHARGE
    // Manages the detection of attack charge inputs
    void ManageChargeInput()
    {
        // Player presses attack button
        if (inputManager.playerInputs[playerNum].attack && canCharge)
        {
            if (stamina >= staminaCostForMoves)
            {
                SwitchState(STATE.charging);


                chargeStartTime = Time.time;


                // FX
                chargeFlareFX.Play();


                // ANIMATION
                playerAnimations.CancelCharge(false);
                playerAnimations.TriggerCharge(true);
            }
        }

        
        // Player releases attack button
        if (!inputManager.playerInputs[playerNum].attack)
        {
            canCharge = true;
        }
    }

    void ManageCharging()
    {
        //currentChargeFramesPressed++;
        


        //Player releases attack button
        if (!inputManager.playerInputs[playerNum].attack)
        {
            ReleaseAttack();
        }


        // If the player has waited too long charging
        if (chargeLevel >= maxChargeLevel)
        {
            if (Time.time - maxChargeLevelStartTime >= maxHoldDurationAtMaxCharge)
            {
                ReleaseAttack();
            }
        }
        // Pass charge levels
        else if (Time.time - chargeStartTime >= durationToNextChargeLevel)
        {
            chargeStartTime = Time.time;


            if (chargeLevel < maxChargeLevel)
            {
                chargeLevel++;
                chargeSlider.value = chargeSlider.maxValue - (chargeSlider.maxValue / maxChargeLevel) * chargeLevel;


                // FX
                chargeFX.Play();
            }

            if (chargeLevel >= maxChargeLevel)
            {
                chargeSlider.value = 0;
                chargeLevel = maxChargeLevel;
                maxChargeLevelStartTime = Time.time;


                // FX
                chargeFullFX.Play();
                chargeFlareFX.Stop();


                // ANIMATION
                playerAnimations.TriggerMaxCharge();
            }
        }
    }
    # endregion





    



    # region ATTACK
    // ATTACK
    // Triggers the attack
    void ReleaseAttack()
    {
        // FX
        // Trail color and width depending on attack range
        if (chargeLevel == 1)
        {
            swordTrail.startColor = lightAttackColor;
            swordTrail.startWidth = lightAttackSwordTrailWidth;
        }
        else if (chargeLevel == maxChargeLevel)
        {
            swordTrail.startColor = heavyAttackColor;
            swordTrail.startWidth = heavyAttackSwordTrailWidth;
        }
        else
        {
            swordTrail.startColor = new Color(
                lightAttackColor.r + (heavyAttackColor.r - lightAttackColor.r) * ((float)actualAttackRange - lightAttackRange) / (float)heavyAttackRange,
                lightAttackColor.g + (heavyAttackColor.g - lightAttackColor.g) * ((float)actualAttackRange - lightAttackRange) / (float)heavyAttackRange,
                lightAttackColor.b + (heavyAttackColor.b - lightAttackColor.b) * ((float)actualAttackRange - lightAttackRange) / (float)heavyAttackRange);


            swordTrail.startWidth = lightAttackSwordTrailWidth + (heavyAttackSwordTrailWidth - lightAttackSwordTrailWidth) * (actualAttackRange - lightAttackRange) / (heavyAttackRange - lightAttackRange);
            
        }


        // Calculates attack range depending on level of charge
        if (chargeLevel == 1)
            actualAttackRange = lightAttackRange;
        else if (chargeLevel == maxChargeLevel)
            actualAttackRange = heavyAttackRange;
        else
            actualAttackRange = lightAttackRange + (heavyAttackRange - lightAttackRange) * ((float)chargeLevel - 1) / (float)maxChargeLevel;


        // STATE SWITCH
        StopAllCoroutines();
        SwitchState(STATE.attacking);


        // FX
        Vector3 attackSignPos = attackRangeFX.transform.localPosition;
        attackRangeFX.transform.localPosition = new Vector3(- (actualAttackRange + attackSignDisjoint), attackSignPos.y, attackSignPos.z);
        attackRangeFX.Play();
        chargeFlareFX.gameObject.SetActive(false);
        chargeFlareFX.gameObject.SetActive(true);


        // Dash direction & distance
        Vector3 dashDirection3D = new Vector3(0, 0, 0);
        float dashDirection = 0;


        if (Mathf.Abs(inputManager.playerInputs[playerNum].horizontal) > attackReleaseAxisInputDeadZoneForDashAttack)
        {
            dashDirection = Mathf.Sign(inputManager.playerInputs[playerNum].horizontal) * transform.localScale.x;
            dashDirection3D = new Vector3(Mathf.Sign(inputManager.playerInputs[playerNum].horizontal), 0, 0);


            // Dash distance
            if (Mathf.Sign(inputManager.playerInputs[playerNum].horizontal) == -Mathf.Sign(transform.localScale.x))
            {
                actualUsedDashDistance = forwardAttackDashDistance;


                // FX
                 attackDashFXFront.Play();
            }
            else
            {
                actualUsedDashDistance = backwardsAttackDashDistance;


                // FX
                attackDashFXBack.Play();
            }
        }
        else
        {
            attackNeutralFX.Play();
        }


        dashDirection3D *= actualUsedDashDistance;
        initPos = transform.position;
        targetPos = transform.position + dashDirection3D;
        targetPos.y = transform.position.y;
        dashTime = 0;

        rb.velocity = Vector3.zero;
        rb.gravityScale = 0;


        // ANIMATION
        playerAnimations.TriggerAttack(dashDirection);
    }

    // Hits with a phantom collider to apply the attack's damage during active frames
    void ApplyAttackHitbox()
    {
        bool enemyDead = false;


        Collider2D[] hitsCol = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (transform.localScale.x * (- actualAttackRange + attackRangeDisjoint) / 2), transform.position.y), new Vector2(actualAttackRange + attackRangeDisjoint, 1), 0);
        List<GameObject> hits = new List<GameObject>();


        foreach (Collider2D c in hitsCol)
        {
            if (c.CompareTag("Player"))
            {
                if (!hits.Contains(c.transform.parent.gameObject))
                {
                    hits.Add(c.transform.parent.gameObject);
                    //Debug.Log(c.transform.parent.gameObject);
                }
            }
        }


        foreach (GameObject g in hits)
        {
            if (g != gameObject)
            {
                enemyDead = g.GetComponent<Player>().TakeDamage(gameObject, chargeLevel);


                // FX
                attackRangeFX.gameObject.SetActive(false);
                attackRangeFX.gameObject.SetActive(true);


                if (enemyDead)
                {
                    SwitchState(STATE.enemyKilled);
                }
            }   
        }
    }

    // Draw the attack range when the player is selected
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * ( - actualAttackRange + attackRangeDisjoint) / 2), transform.position.y, transform.position.z), new Vector3(actualAttackRange + attackRangeDisjoint, 1, 1));
        Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * - kickRange / 2), transform.position.y, transform.position.z), new Vector3(kickRange, 1, 1));
    }

    // Draw the attack range is the attack is in active frames in the scene viewer
    private void OnDrawGizmos()
    {
        if (activeFrame)
            Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * (-actualAttackRange + attackRangeDisjoint) / 2), transform.position.y, transform.position.z), new Vector3(actualAttackRange + attackRangeDisjoint, 1, 1));

        if (kickFrame)
            Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * - kickRange / 2), transform.position.y, transform.position.z), new Vector3(kickRange, 1, 1));
    }
    # endregion









    # region PARRY
    // PARRY
    // Detect parry inputs
    void ManageParryInput()
    {
        if (inputManager.playerInputs[playerNum].parry)
        {
            currentParryFramesPressed++;
            TriggerParry();
            currentParryFramesPressed = 0;
        }
    }

    // Parry coroutine
    void TriggerParry()
    {
        SwitchState(STATE.parrying);


        // Cancel charge
        /*
        if (charging)
        {
            // PARRYING
            playerAnimations.CancelCharge();
        }
        */


        // ANIMATION
        playerAnimations.TriggerParry();
    }
    # endregion








    # region POMMEL
    // POMMEL
    // Detect pommel inputs
    void ManagePommel()
    {
        if (!inputManager.playerInputs[playerNum].kick)
        {
            canPommel = true;
        }
        
   
        if (inputManager.playerInputs[playerNum].kick && canPommel)
        {
            canPommel = false;

            TriggerPommel();
        }     
    }

    // Kick coroutine
    void TriggerPommel()
    {
        SwitchState(STATE.pommeling);


        // ANIMATION
        playerAnimations.TriggerPommel();
    }

    // Apply pommel hitbox depending on kick frames
    void ApplyPommelHitbox()
    {
        Collider2D[] hitsCol = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (transform.localScale.x * - kickRange / 2), transform.position.y), new Vector2(kickRange, 1), 0);
        List<GameObject> hits = new List<GameObject>();


        foreach (Collider2D c in hitsCol)
        {
            if (c.CompareTag("Player"))
            {
                if (!hits.Contains(c.transform.parent.gameObject))
                {
                    hits.Add(c.transform.parent.gameObject);
                }
            }
        }


        foreach (GameObject g in hits)
        {
            if (g != gameObject)
            {
                //g.GetComponent<PlayerStats>().StaminaCost(kickedStaminaLoss);
                if (g.GetComponent<Player>().playerState != Player.STATE.clashed)
                {
                    g.GetComponent<Player>().Pommeled();
                }  
            }
        }
    }
    # endregion








    # region POMMELED
    // POMMELED
    // The player have been kicked
    public void Pommeled()
    {
        if (!kickFrame)
        {
            StopAllCoroutines();
            SwitchState(STATE.clashed);


            // Stamina
            if (playerState == STATE.parrying || playerState == STATE.attacking)
                StaminaCost(staminaCostForMoves);
                

            canCharge = false;
            chargeLevel = 1;


            // Dash knockback
            isDashing = true;
            dashDirection = transform.localScale.x;
            actualUsedDashDistance = kickKnockbackDistance;
            initPos = transform.position;
            targetPos = transform.position + new Vector3(actualUsedDashDistance * dashDirection, 0, 0);
            dashTime = 0;


            // FX
            kickKanasFX.Play();
            kickedFX.Play();


            // ANIMATIONs
            playerAnimations.CancelCharge(true);
            playerAnimations.TriggerClashed(true);


            // SOUND
            audioManager.TriggerClashAudio();
            audioManager.BattleEventIncreaseIntensity();
        }
    }
    # endregion








    # region CLASHED
    //CLASHED

    // The player have been clashed / parried
    void TriggerClash()
    {
        SwitchState(STATE.clashed);


        StopAllCoroutines();
        gameManager.TriggerSlowMoCoroutine(gameManager.clashSlowMoDuration, gameManager.clashSlowMoTimeScale, gameManager.clashTimeScaleFadeSpeed);
        

        temporaryDashDirectionForCalculation = transform.localScale.x;
        actualUsedDashDistance = clashKnockback;
        initPos = transform.position;
        targetPos = transform.position + new Vector3(actualUsedDashDistance * temporaryDashDirectionForCalculation, 0, 0);
        dashTime = 0;


        StartCoroutine(ClashDurationAndEndCoroutine());


        // ANIMATION
        playerAnimations.CancelCharge(true);
        playerAnimations.TriggerClashed(true);


        // SOUND
        audioManager.TriggerClashAudio();
        audioManager.BattleEventIncreaseIntensity();


        // FX
        if (gameManager.playersList.Count > 1 && !gameManager.playersList[otherPlayerNum].GetComponent<Player>().clashKanasFX.isPlaying)
            clashKanasFX.Play();
    }

    IEnumerator ClashDurationAndEndCoroutine()
    {
        yield return new WaitForSeconds(0f);
    }
    # endregion








    # region DASH
    //DASH
    // Functions to detect the dash input etc
    void ManageDashInput()
    {
        // Detects dash with basic input rather than double tap, shortcut
        if (Mathf.Abs(inputManager.playerInputs[playerNum].dash) < shortcutDashDeadZone && currentShortcutDashStep == DASHSTEP.invalidated)
        {
            //inputManager.playerInputs[playerStats.playerNum - 1].horizontal;
            currentShortcutDashStep = DASHSTEP.rest;
        }
        

        if (Mathf.Abs(inputManager.playerInputs[playerNum].dash) > shortcutDashDeadZone && currentShortcutDashStep == DASHSTEP.rest)
        {
            dashDirection = Mathf.Sign(inputManager.playerInputs[playerNum].dash);
            

            TriggerBasicDash();
        }


        // Resets the dash input if too much time has passed
        if (currentDashStep == DASHSTEP.firstInput || currentDashStep == DASHSTEP.firstRelease)
        {
            if (Time.time - dashInitializationStartTime > allowanceDurationForDoubleTapDash)
            {
                currentDashStep = DASHSTEP.invalidated;
            }
        }


        // The player needs to let go the direction before pressing it again to dash
        if (Mathf.Abs(inputManager.playerInputs[playerNum].horizontal) < dashDeadZone)
        {
            if (currentDashStep == DASHSTEP.firstInput)
            {
                currentDashStep = DASHSTEP.firstRelease;
            }
            // To make the first dash input he must have not been pressing it before, we need a double tap
            else if (currentDashStep == DASHSTEP.invalidated)
            {
                currentDashStep = DASHSTEP.rest;
            }
        }


        // When the player presses the direction
        // Presses the
        if (Mathf.Abs(inputManager.playerInputs[playerNum].horizontal) > dashDeadZone)
        {
            temporaryDashDirectionForCalculation = Mathf.Sign(inputManager.playerInputs[playerNum].horizontal);

            if (currentDashStep == DASHSTEP.rest)
            {
                currentDashStep = DASHSTEP.firstInput;
                dashDirection = temporaryDashDirectionForCalculation;
                dashInitializationStartTime = Time.time;

            }
            // Dash is validated, the player is gonna dash
            else if (currentDashStep == DASHSTEP.firstRelease && dashDirection == temporaryDashDirectionForCalculation && stamina >= staminaCostForMoves)
            {
                currentDashStep = DASHSTEP.invalidated;
                TriggerBasicDash();
            }
        }
    }

    // If the player collides with a wall
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //EndDash();
            targetPos = transform.position;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //EndDash();
            targetPos = transform.position;
        }
    }

    // Triggers the dash (Not the clash or attack dash) for it to run
    void TriggerBasicDash()
    {
        // CHANGE STATE
        SwitchState(STATE.dashing);


        StopAllCoroutines();
        dashTime = 0;


        if (dashDirection == - transform.localScale.x)
        {
            actualUsedDashDistance = forwardDashDistance;
            dashFXFront.Play();
        }
        else
        {
            actualUsedDashDistance = backwardsDashDistance;
            dashFXBack.Play();
        }


        // ANIMATION
        playerAnimations.TriggerDash(dashDirection * transform.localScale.x);

 
        initPos = transform.position;
        targetPos = transform.position + new Vector3(actualUsedDashDistance * dashDirection, 0, 0);
    }

    // Runs the dash, to use in FixedUpdate
    void RunDash()
    {
        if (isDashing)
        {
            // Sets the dash speed
            if (playerState == STATE.clashed)
                dashTime += Time.deltaTime * clashKnockbackSpeed;
            else
                dashTime += Time.fixedUnscaledDeltaTime * baseDashSpeed;


            transform.position = Vector3.Lerp(initPos, targetPos, dashTime);


            if (dashTime >= 1.0f)
            {
                EndDash();
            }
        }
    } 
    
    // End currently running dash
    void EndDash()
    {
        // CHANGE STATE
        if (playerState != STATE.attacking && playerState != STATE.recovering)
        {
            SwitchState(STATE.normal);
        }


        isDashing = false;


        // ANIMATION
        playerAnimations.TriggerClashed(false);


        // FX
        dashFXFront.Stop();
        dashFXBack.Stop();
        attackDashFXFront.Stop();
        attackDashFXBack.Stop();
    }
    # endregion







    # region ORIENTATION
    // ORIENTATION CALLED IN UPDATE
    void ManageOrientation()
    {
        // Orient towards the enemy if player can in their current state
        if (canOrientTowardsEnemy)
        {
            GameObject p1 = null, p2 = null, self = null, other = null;
            Player[] stats = FindObjectsOfType<Player>();


            foreach (Player stat in stats)
            {
                switch (stat.playerNum)
                {
                    case 0:
                        p1 = stat.gameObject;
                        break;

                    case 1:
                        p2 = stat.gameObject;
                        break;

                    default:
                        break;
                }
            }


            if (p1 == gameObject)
            {
                self = p1;
                other = p2;
            }
            else if (p2 == gameObject)
            {
                self = p2;
                other = p1;
            }


            float sign = Mathf.Sign(self.transform.position.x - other.transform.position.x);


            if (orientationCooldownFinished)
                ApplyOrientation(sign);
        }


        if (Time.time >= orientationCooldown + orientationCooldownStartTime)
        {
            orientationCooldownFinished = true;
        }
    }

    // Immediatly rotates the player
    void ApplyOrientation(float sign)
    {
        if (sign > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }


        orientationCooldownStartTime = Time.time;
        orientationCooldownFinished = false;
        

        // FX
        Vector3 deathBloodFXRotation = deathBloodFX.gameObject.transform.localEulerAngles;


        if (transform.localScale.x <= 0)
        {
            
            deathBloodFX.gameObject.transform.localEulerAngles = new Vector3(deathBloodFXRotation.x, deathBloodFXRotation.y, deathBloodFXRotationForDirectionChange);


            // Changes the draw text indication's scale so that it's, well, readable for a human being
            drawText.transform.localScale = new Vector3(- drawTextBaseScale.x, drawTextBaseScale.y, drawTextBaseScale.z);
        }
        else
        {
            deathBloodFX.gameObject.transform.localEulerAngles = deathBloodFXBaseRotation;


            // Changes the draw text indication's scale so that it's, well, readable for a human being
            drawText.transform.localScale = new Vector3(drawTextBaseScale.x, drawTextBaseScale.y, drawTextBaseScale.z);
        }
    }
    # endregion







    # region CHEATS
    // CHEATS
    void CheatsInputs()
    {
        if (Input.GetKeyDown(clashCheatKey))
        {
            TriggerClash();
        }


        if (Input.GetKeyDown(deathCheatKey))
        {
            TakeDamage(gameObject, 1);
        }


        if (Input.GetKeyDown(staminaCheatKey))
        {
            stamina = maxStamina;
        }
    }
    #endregion
    # endregion
}
