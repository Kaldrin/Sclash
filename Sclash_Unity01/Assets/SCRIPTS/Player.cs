using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

using Photon.Pun;
using Photon.Realtime;

// Main player script for duel mode
// A LITTLE MESSY ?
public class Player : MonoBehaviourPunCallbacks
{
    #region Events
    public delegate void OnDrawnEvent();
    public event OnDrawnEvent DrawnEvent;
    #endregion


    #region VARIABLES
    #region MANAGERS
    [Header("MANAGERS")]
    AudioManager audioManager;
    [HideInInspector] public GameManager gameManager;

    // Input manager
    [Tooltip("The name of the object in the scene containing the InputManager script component, to find its reference")]

    protected InputManager inputManager = null;

    protected StatsManager statsManager = null;
    #endregion



    #region PLAYER'S COMPONENTS
    [Header("PLAYER'S COMPONENTS")]
    public Rigidbody2D rb = null;
    public PlayerAnimations playerAnimations = null;
    public CharacterChanger characterChanger = null;
    public IAScript iaScript = null;
    [Tooltip("All of the player's 2D colliders")]
    public Collider2D[] playerColliders = null;
    public SpriteRenderer spriteRenderer = null;
    [SerializeField] SpriteRenderer maskSpriteRenderer = null;
    [SerializeField] SpriteRenderer weaponSpriteRenderer = null;
    [SerializeField] SpriteRenderer sheathSpriteRenderer = null;
    [SerializeField] Renderer scarfRenderer = null;
    [Tooltip("The reference to the light component which lits the player with their color")]
    [SerializeField] public Light playerLight = null;
    [Tooltip("The animator controller that will be put on the sprite object of the player to enable nice looking character change animations")]
    [SerializeField] public RuntimeAnimatorController characterChangerAnimatorController = null;
    #endregion




    #region PLAYER STATES
    [Header("PLAYER STATES")]
    [SerializeField] public STATE playerState = STATE.normal;
    [HideInInspector] public STATE oldState = STATE.normal;


    // Enum for the player's state definition
    public enum STATE
    {
        frozen,
        onlinefrozen,
        sneathing,
        sneathed,
        drawing,
        battleSneathing,
        battleSneathedNormal,
        battleDrawing,
        normal,
        charging,
        attacking,
        canAttackAfterAttack,
        pommeling,
        parrying,
        maintainParrying,
        preparingToJump,
        jumping,
        dashing,
        recovering,
        clashed,
        enemyKilled,
        enemyKilledEndMatch,
        dead,
    }


    [SerializeField] protected bool hasFinishedAnim = false;
    [SerializeField] bool waitingForNextAttack = false;
    [SerializeField] bool hasAttackRecoveryAnimFinished = false;

    public enum CharacterType
    {
        duel,
        campaign,
    }

    [SerializeField] public CharacterType characterType = CharacterType.duel;
    #endregion




    #region PLAYERS IDENTIFICATION
    [Header("PLAYERS IDENTIFICATION")]
    [SerializeField] public Text characterNameDisplay = null;
    [SerializeField] public Image characterIdentificationArrow = null;
    [SerializeField] bool usePlayerColorsDifferenciation = false;
    [SerializeField] Color secondPlayerMaskColor = new Color(0, 0, 0, 1);
    [SerializeField] Color secondPlayerSaberColor = new Color(0, 0, 0, 1);
    [HideInInspector] public int characterIndex = 0;
    [HideInInspector] public int networkPlayerNum = 0;
    [HideInInspector] public bool playerIsAI;
    public int playerNum = 0;
    public int otherPlayerNum = 0;
    Player opponent;
    #endregion




    #region HEALTH
    [Header("HEALTH")]
    [Tooltip("The maximum health of the player")]
    [SerializeField] float maxHealth = 1;
    protected float currentHealth;
    #endregion




    #region STAMINA STUFF
    [Header("STAMINA")]
    [Tooltip("The reference to the base stamina slider attached to the player to create the other sliders")]
    [SerializeField] public Slider staminaSlider = null;
    protected List<Slider> staminaSliders = new List<Slider>();

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
        quickStaminaRegenGap = 1,
        lowStaminaGap = 1,
        idleQuickStaminaGainOverTimeMultiplier = 1.2f,
        backWalkingQuickStaminaGainOverTime = 1.2f,
        frontWalkingQuickStaminaGainOverTime = 0.8f,
        staminaBarBaseOpacity = 0.8f,
        staminaRecupTriggerDelay = 0.35f,
        staminaRecupAnimRegenSpeed = 0.025f;
    [HideInInspector] public float stamina = 0;
    float currentTimeBeforeStaminaRegen = 0;
    protected float staminaBarsOpacity = 1;
    float oldStaminaValue = 0;


    [HideInInspector] public bool canRegenStamina = true;
    bool hasReachedLowStamina = false;
    bool staminaRecupAnimOn = false;
    bool staminaBreakAnimOn = false;

    [Header("STAMINA COLORS")]
    [Tooltip("Stamina colors depending on how much there is left")]
    [SerializeField] Color staminaBaseColor = Color.green;
    [SerializeField]
    Color
        staminaLowColor = Color.yellow,
        staminaDeadColor = Color.red,
        staminaRecupColor = Color.blue,
        staminaBreakColor = Color.red;
    #endregion




    #region MOVEMENTS SETTING
    [Header("MOVEMENTS")]
    [Tooltip("The default movement speed of the player")]
    [SerializeField] protected float baseMovementsSpeed = 2.5f;
    [SerializeField] float chargeMovementsSpeed = 1.2f;
    [SerializeField] float sneathedMovementsSpeed = 1.8f;
    [SerializeField] float attackingMovementsSpeed = 2.2f;
    [HideInInspector] public float actualMovementsSpeed = 1;

    Vector3 oldPos = Vector3.zero;
    protected Vector2 netTargetPos = Vector2.zero;
    //float lerpValue = 0f;
    //bool lerpToTarget = false;
    #endregion



    #region ORIENTATION
    [Header("ORIENTATION")]
    [Tooltip("The duration before the player can orient again towards the enemy if they need to once they applied the orientation")]
    [SerializeField] protected float orientationCooldown = 0.1f;
    protected float
         orientationCooldownStartTime = 0;

    protected bool
        orientationCooldownFinished = true,
        canOrientTowardsEnemy = true;
    #endregion




    #region ACTIONS & RULES
    [Header("ACTIONS & RULES")]
    [SerializeField] protected bool canJump = true;
    [SerializeField] protected bool canMaintainParry = true;
    [SerializeField] protected bool canBriefParry = false;
    [SerializeField] protected bool quickRegen = false;
    [SerializeField] protected bool quickRegenOnlyWhenReachedLowStaminaGap = true;
    [SerializeField] protected bool canBattleSneath = false;
    #endregion



    #region FRAMES STATE
    [Header("FRAMES STATE")]
    [Tooltip("Only editable by animator, is currently in parry frames state")]
    [SerializeField] public bool parryFrame = false;
    [SerializeField] public bool perfectParryFrame = false;
    [SerializeField] public bool activeFrame = false;
    [SerializeField] public bool clashFrames = false;
    [Tooltip("Can the player be hit in the current frames ?")]
    [SerializeField] public bool untouchableFrame = false;
    [Tooltip("The opacity amount of the player's sprite when in untouchable frames")]
    [SerializeField] float untouchableFrameOpacity = 0.3f;
    #endregion



    [Header("JUMP")]
    [SerializeField] float jumpPower = 10f;



    #region CHARGE STUFF
    [Header("CHARGE")]
    [Tooltip("The number of charge levels for the attack, so the number of range subdivisions")]
    [SerializeField] public int maxChargeLevel = 4;
    [HideInInspector] public int chargeLevel = 1;

    [Tooltip("Charge duration parameters")]
    [SerializeField] protected float durationToNextChargeLevel = 0.7f;
    [SerializeField] protected float maxHoldDurationAtMaxCharge = 2f;
    [SerializeField] float attackReleaseAxisInputDeadZoneForDashAttack = 0.1f;
    protected float
        maxChargeLevelStartTime = 0,
        chargeStartTime = 0;

    [HideInInspector] public bool canCharge = true;
    #endregion



    #region ATTACK STUFF
    [Header("ATTACK")]
    [Tooltip("Attack range parameters")]
    [SerializeField] public float lightAttackRange = 1.8f;
    [Tooltip("Attack range parameters")]
    [SerializeField] public float heavyAttackRange = 3.2f;
    [SerializeField] public float baseBackAttackRangeDisjoint = 0f;
    [SerializeField] public float forwardAttackBackrangeDisjoint = 2.5f;
    //[SerializeField] float axisDeadZoneForAttackDash = 0.2f;

    [HideInInspector] public float actualAttackRange = 0;
    protected float actualBackAttackRangeDisjoint = 0f;

    [Tooltip("Frame parameters for the attack")]
    [HideInInspector] public bool isAttacking = false;
    protected List<GameObject> targetsHit = new List<GameObject>();
    #endregion



    #region DASH
    [Header("DASH")]
    [SerializeField] public float baseDashSpeed = 3;
    [SerializeField]
    public float forwardDashDistance = 3,
        backwardsDashDistance = 2.5f;
    [SerializeField]
    protected float
        allowanceDurationForDoubleTapDash = 0.3f,
        forwardAttackDashDistance = 2.5f,
        backwardsAttackDashDistance = 1.5f,
        dashDeadZone = 0.5f,
        shortcutDashDeadZone = 0.5f;
    [SerializeField]
    protected float
   dashDirection = 0,
   temporaryDashDirectionForCalculation = 0,
   dashInitializationStartTime = 0,
   actualUsedDashDistance = 0,
   dashTime = 0;

    protected enum DASHSTEP
    {
        rest,
        firstInput,
        firstRelease,
        invalidated,
    }

    protected DASHSTEP currentDashStep = DASHSTEP.invalidated;
    protected DASHSTEP currentShortcutDashStep = DASHSTEP.invalidated;

    protected Vector3 initPos;
    protected Vector3 targetPos;

    protected bool isDashing = false;
    #endregion



    #region POMMEL
    [Header("POMMEL")]
    [Tooltip("Is currently applying the pommel effect to what they touches ?")]
    [SerializeField] public bool kickFrame = false;
    [SerializeField] protected float kickRange = 0.88f;
    [HideInInspector] public bool canPommel = true;

    #endregion



    [Header("POMMELED")]
    [Tooltip("The distance the player will be pushed on when pommeled")]
    [SerializeField] protected float kickKnockbackDistance = 1f;



    [Header("PARRY")]
    public bool canParry = true;
    protected int currentParryFramesPressed = 0;


    [Header("MAINTAIN PARRY")]
    [SerializeField] float maintainParryStaminaCostOverTime = 0.1f;




    #region CLASHED
    [Header("CLASHED")]
    [Tooltip("The distance the player will be pushed on when clashed")]
    [SerializeField] float clashKnockback = 2;
    //[SerializeField] float clashDuration = 2f;
    [Tooltip("The speed at which the knockback distance will be covered")]
    [SerializeField] public float clashKnockbackSpeed = 2;
    #endregion



    #region FX
    [Header("FX")]
    [Tooltip("The references to the game objects holding the different FXs")]
    [SerializeField] protected GameObject clashFXPrefabRef = null;
    [SerializeField] protected GameObject deathBloodFX = null;

    [Tooltip("The attack sign FX object reference, the one that spawns at the range distance before the attack hits")]
    [SerializeField] public ParticleSystem attackRangeFX = null;
    [SerializeField]
    protected ParticleSystem
        clashKanasFX = null,
        kickKanasFX = null,
        kickedFX = null,
        clashFX = null,
        slashFX = null;


    [SerializeField] float attackSignDisjoint = 0.4f;
    [Tooltip("The amount to rotate the death blood FX's object because for some reason it takes another rotation when it plays :/")]
    [SerializeField] float deathBloodFXRotationForDirectionChange = 240;
    [SerializeField] GameObject attackSlashFXParent = null;
    [SerializeField]
    float
        //lightAttackSwordTrailWidth = 20f,
        //heavyAttackSwordTrailWidth = 65f,
        lightAttackSwordTrailScale = 1,
        heavyAttackSwordTrailScale = 3;
    [Tooltip("The minimum speed required for the walk fx to trigger")]
    [SerializeField] protected float minSpeedForWalkFX = 0.05f;

    Vector3 deathFXbaseAngles = new Vector3(0, 0, 0);
    Vector3 deathBloodFXBaseRotation = Vector3.zero;



    [Header("CHARGE FX")]
    [Tooltip("The slider component reference to move the charging FX on the katana")]
    [SerializeField] protected Slider chargeSlider = null;
    [SerializeField] protected ParticleSystem chargeFlareFX = null;
    [SerializeField] protected ParticleSystem chargeFX = null;
    [SerializeField] protected ParticleSystem chargeFullFX = null;
    [SerializeField] GameObject rangeIndicatorShadow = null;
    [SerializeField] SpriteRenderer rangeIndicatorShadowSprite = null;



    [Header("STAMINA FX")]
    [SerializeField] ParticleSystem staminaLossFX = null;
    [SerializeField]
    ParticleSystem staminaGainFX = null,
        staminaRecupFX = null,
        staminaRecupFinishedFX = null,
        staminaBreakFX = null;
    #endregion



    #region STAGE DEPENDENT FX
    [Header("STAGE DEPENDENT FX")]
    [SerializeField]
    ParticleSystem
        dashFXFront = null;
    [SerializeField]
    ParticleSystem
        dashFXBack = null,
        attackDashFXFront = null,
        attackDashFXBack = null,
        attackNeutralFX = null;
    [SerializeField]
    protected ParticleSystem
        walkFXFront = null,
        walkFXBack = null;
    #endregion


    [System.Serializable]
    public struct ParticleSet
    {
        public List<GameObject> particleSystems;
    }
    [Tooltip("Different lists of particle effects for the player's steps, for the different stages")]
    [SerializeField] public List<ParticleSet> particlesSets = new List<ParticleSet>();



    #region AUDIO
    [Header("AUDIO")]
    [Tooltip("The reference to the stamina charged audio FX AudioSource")]
    [SerializeField] AudioSource staminaBarChargedAudioEffectSource = null;
    [SerializeField] AudioSource staminaBreakAudioFX = null;
    [SerializeField] AudioSource finalDeathAudioFX = null;
    [SerializeField] PlayRandomSoundInList notEnoughStaminaSFX = null;
    [SerializeField] PlayRandomSoundInList staminaEndSFX = null;
    [SerializeField] PlayRandomSoundInList staminaUseSFX = null;
    #endregion



    // NETWORK
    protected bool enemyDead = false;



    [Header("CHEATS")]
    [SerializeField] PlayerCheatsParameters cheatSettings = null;
    #endregion




















    #region FUNCTIONS
    #region BASE FUNCTIONS
    protected void Awake()
    {

        // GET PLAYER CHARACTER CHANGE ANIMATOR (Because I always forget to add it back while editing the animations (Because I have to remove it, it conflicts with the main animator))
        if (spriteRenderer.gameObject.GetComponent<Animator>() == null)
            characterChanger.characterChangeAnimator = spriteRenderer.gameObject.AddComponent<Animator>();
        if (characterChanger.characterChangeAnimator != null && characterChangerAnimatorController != null && characterChanger.characterChangeAnimator.runtimeAnimatorController != characterChangerAnimatorController)
            characterChanger.characterChangeAnimator.runtimeAnimatorController = characterChangerAnimatorController;
    }

    public virtual void Start()                                                                // START
    {
        // GET MANAGERS
        //audioManager = GameObject.Find(audioManagerName).GetComponent<AudioManager>();
        audioManager = AudioManager.Instance;
        //gameManager = GameObject.Find(gameManagerName).GetComponent<GameManager>();
        gameManager = GameManager.Instance;
        //inputManager = GameObject.Find(inputManagerName).GetComponent<InputManager>();
        inputManager = InputManager.Instance;
        //statsManager = GameObject.Find(statsManagerName).GetComponent<StatsManager>();
        statsManager = StatsManager.Instance;

        // The forward attack touches a little behind the character for cool effects
        actualBackAttackRangeDisjoint = baseBackAttackRangeDisjoint;


        // STAMINA SET UP
        oldStaminaValue = maxStamina;


        // Reset all the player's values and variable to start fresh
        Invoke("GetOtherPlayerNum", 0.2f);
        SetUpStaminaBars();
        deathFXbaseAngles = deathBloodFX.transform.localEulerAngles;
        ResetAllPlayerValuesForNextMatch();


        // Find the UI elements for the character changer
        characterChanger.FindElements();


        // ELEMENTS COLOR
        if (usePlayerColorsDifferenciation && playerNum == 1)
        {
            maskSpriteRenderer.color = secondPlayerMaskColor;
            weaponSpriteRenderer.color = secondPlayerSaberColor;
        }
    }

    // Update is called once per graphic frame
    public virtual void Update()
    {
        if (enabled && isActiveAndEnabled)
        {
            // ONLINE
            if (photonView != null && !photonView.IsMine)
            {
                UpdateStaminaSlidersValue();
                UpdateStaminaColor();


                SetStaminaBarsOpacity(staminaBarsOpacity);

                return;
            }

            if (opponent == null)
                FindOpponent();


            // Action depending on state
            switch (playerState)
            {
                case STATE.onlinefrozen:                    // ONLINE FROZEN
                    UpdateStaminaSlidersValue();
                    UpdateStaminaColor();

                    UpdateChargeShadowSize();
                    ManageOrientation();
                    RunDash();
                    break;

                case STATE.frozen:                          // FROZEN                         
                    break;

                case STATE.sneathing:                       // SNEATHING
                    break;

                case STATE.sneathed:                        // SNEATHED
                    ManageOrientation();
                    ManageIA();
                    ManageDraw();
                    break;

                case STATE.drawing:                                // DRAWING
                    break;

                case STATE.battleDrawing:                              // BATTLE DRAWING
                    break;

                case STATE.battleSneathing:                                 // BATTLE SNEATHING
                    break;

                case STATE.battleSneathedNormal:                            // BATTLE SNEATHED NORMAL
                    ManageBattleDraw();
                    break;

                case STATE.normal:                                              // NORMAL
                    ManageJumpInput();
                    ManageChargeInput();
                    ManageDashInput();
                    ManagePommel();
                    ManageParryInput();
                    ManageMaintainParryInput();
                    ManageBattleSneath();

                    UpdateStaminaSlidersValue();
                    UpdateStaminaColor();

                    UpdateChargeShadowSize();
                    break;

                case STATE.charging:                                    // CHARGING
                    ManageDashInput();
                    ManagePommel();
                    ManageParryInput();
                    ManageMaintainParryInput();
                    ManageCharging();

                    UpdateChargeShadowSize();
                    break;

                case STATE.attacking:                                           // ATTACKING
                    UpdateChargeShadowSize();
                    break;

                case STATE.canAttackAfterAttack:                                        // CAN ATTACK AFTER ATTACK
                    ManageJumpInput();
                    ManageChargeInput();
                    ManageDashInput();
                    ManagePommel();
                    ManageParryInput();
                    ManageMaintainParryInput();

                    UpdateStaminaSlidersValue();
                    UpdateStaminaColor();

                    UpdateChargeShadowSize();
                    break;

                case STATE.recovering:                                      // RECOVERING
                    UpdateChargeShadowSize();
                    break;

                case STATE.pommeling:                                           // POMMELING
                    ManageDashInput();
                    break;

                case STATE.parrying:                                            // PARRYING
                    UpdateStaminaSlidersValue();
                    UpdateStaminaColor();
                    break;

                case STATE.maintainParrying:                                    // MAINTING PARYYING
                    ManageMaintainParryInput();
                    break;

                case STATE.preparingToJump:                                             // PREPARING TO JUMP
                    break;

                case STATE.jumping:                                                 // JUMPING
                    break;

                case STATE.dashing:                                                     // DASHING
                    ManageDashInput();
                    ManageChargeInput();
                    ManagePommel();
                    ManageParryInput();
                    ManageMaintainParryInput();
                    break;

                case STATE.clashed:                                             // CLASHED
                    RunDash();
                    break;

                case STATE.enemyKilled:                                                     // ENEMY KILLED
                    break;

                case STATE.enemyKilledEndMatch:                                             // ENEMY KILLED END MATCH
                    break;

                case STATE.dead:                                                // DEAD
                    break;
            }


            // Cheatcodes to use for development purposes
            if (gameManager.cheatCodes)
                CheatsInputs();
        }


        // Cheatcodes to use for development purposes
        if (GameManager.Instance.cheatCodes)
            CheatsInputs();
    }

    // FixedUpdate is called 50 times per second
    public virtual void FixedUpdate()
    {
        if (enabled && isActiveAndEnabled)
        {
            // KICK FRAMES
            if (kickFrame)
                ApplyPommelHitbox();



            // Transparency on dodge frames
            if (cheatSettings.useTransparencyForDodgeFrames && untouchableFrame)
            {
                if (spriteRenderer != null)
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, untouchableFrameOpacity);
                if (maskSpriteRenderer != null)
                    maskSpriteRenderer.color = new Color(maskSpriteRenderer.color.r, maskSpriteRenderer.color.g, maskSpriteRenderer.color.b, untouchableFrameOpacity);
                if (weaponSpriteRenderer != null)
                    weaponSpriteRenderer.color = new Color(weaponSpriteRenderer.color.r, weaponSpriteRenderer.color.g, weaponSpriteRenderer.color.b, untouchableFrameOpacity);
                if (sheathSpriteRenderer != null)
                    sheathSpriteRenderer.color = new Color(sheathSpriteRenderer.color.r, sheathSpriteRenderer.color.g, sheathSpriteRenderer.color.b, untouchableFrameOpacity);
                if (scarfRenderer != null)
                    scarfRenderer.material.color = new Color(scarfRenderer.material.color.r, scarfRenderer.material.color.g, scarfRenderer.material.color.b, untouchableFrameOpacity);
            }
            else
            {
                if (spriteRenderer != null)
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
                if (maskSpriteRenderer != null)
                    maskSpriteRenderer.color = new Color(maskSpriteRenderer.color.r, maskSpriteRenderer.color.g, maskSpriteRenderer.color.b, 1);
                if (weaponSpriteRenderer != null)
                    weaponSpriteRenderer.color = new Color(weaponSpriteRenderer.color.r, weaponSpriteRenderer.color.g, weaponSpriteRenderer.color.b, 1);
                if (sheathSpriteRenderer != null)
                    sheathSpriteRenderer.color = new Color(sheathSpriteRenderer.color.r, sheathSpriteRenderer.color.g, sheathSpriteRenderer.color.b, 1);
                if (scarfRenderer != null)
                    scarfRenderer.material.color = new Color(scarfRenderer.material.color.r, scarfRenderer.material.color.g, scarfRenderer.material.color.b, 1);
            }



            // Behaviour depending on state
            switch (playerState)
            {
                case STATE.onlinefrozen:                                                      // FROZEN
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    rb.velocity = Vector2.zero;
                    UpdateStaminaSlidersValue();
                    ManageStaminaRegen();
                    playerAnimations.UpdateIdleStateDependingOnStamina(stamina);
                    break;

                case STATE.frozen:                                                      // FROZEN
                    SetStaminaBarsOpacity(0);
                    rb.velocity = Vector2.zero;
                    break;

                case STATE.sneathing:                                                       // SNEATHING
                    UpdateStaminaSlidersValue();
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    UpdateStaminaColor();
                    if (hasFinishedAnim)
                    {
                        hasFinishedAnim = false;
                        SwitchState(STATE.sneathed);
                    }
                    break;

                case STATE.sneathed:                                                    // SNEATHED
                    ManageStaminaRegen();
                    UpdateStaminaSlidersValue();
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    UpdateStaminaColor();
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    break;

                case STATE.drawing:                                                     // DRAWING
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    UpdateStaminaColor();
                    if (DrawnEvent != null)
                        DrawnEvent();
                    if (hasFinishedAnim)
                    {
                        hasFinishedAnim = false;
                        SwitchState(STATE.normal);
                        gameManager.SaberDrawn(playerNum);
                    }
                    break;

                case STATE.battleDrawing:                                               // BATTLE DRAWING
                    UpdateStaminaSlidersValue();
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    UpdateStaminaColor();
                    rb.velocity = Vector2.zero;
                    if (hasFinishedAnim)
                        SwitchState(STATE.normal);
                    break;

                case STATE.battleSneathing:                                                     // BATTLE SNEATHING
                    if (hasFinishedAnim)
                        SwitchState(STATE.battleSneathedNormal);
                    UpdateStaminaSlidersValue();
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    UpdateStaminaColor();
                    rb.velocity = Vector2.zero;
                    break;

                case STATE.battleSneathedNormal:                                            // BATTLE SNEATHED NORMAL
                    UpdateStaminaSlidersValue();
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    UpdateStaminaColor();
                    ManageMovementsInputs();
                    ManageOrientation();
                    break;

                case STATE.normal:                                                      // NORMAL
                    ManageMovementsInputs();
                    ManageOrientation();
                    ManageStaminaRegen();
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    playerAnimations.UpdateIdleStateDependingOnStamina(stamina);
                    break;

                case STATE.charging:                                                // CHARGING
                    ManageMovementsInputs();
                    ManageStaminaRegen();
                    UpdateStaminaSlidersValue();
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    UpdateStaminaColor();
                    break;

                case STATE.attacking:                                               // ATTACKING
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
                        ApplyAttackHitbox();
                    break;

                case STATE.canAttackAfterAttack:                                    // CAN ATTACK AFTER ATTACK
                    ManageStaminaRegen();
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    if (hasAttackRecoveryAnimFinished)
                    {
                        hasFinishedAnim = false;
                        SwitchState(STATE.normal);
                    }
                    break;

                case STATE.recovering:                                              // RECOVERING
                    if (waitingForNextAttack)
                        SwitchState(STATE.canAttackAfterAttack);
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

                case STATE.pommeling:                                               // POMMELING
                    RunDash();
                    if (hasFinishedAnim)
                    {
                        hasFinishedAnim = false;
                        SwitchState(STATE.normal);
                    }
                    UpdateStaminaSlidersValue();
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    UpdateStaminaColor();
                    rb.velocity = Vector3.zero;
                    break;

                case STATE.parrying:                                                // PARRYING
                    RunDash();
                    if (hasFinishedAnim)
                    {
                        hasFinishedAnim = false;
                        SwitchState(STATE.normal);
                    }
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    rb.velocity = Vector3.zero;
                    break;

                case STATE.maintainParrying:                                        // MAINTAIN PARRY
                    RunDash();
                    if (hasFinishedAnim)
                        SwitchState(STATE.normal);
                    StaminaCost(maintainParryStaminaCostOverTime, false);
                    UpdateStaminaSlidersValue();
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    UpdateStaminaColor();
                    break;

                case STATE.preparingToJump:                                         // PREPARING TO JUMP
                    if (hasFinishedAnim)
                        ActuallyJump();
                    UpdateStaminaSlidersValue();
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    UpdateStaminaColor();
                    break;

                case STATE.jumping:                                                 // JUMPING
                    if (hasAttackRecoveryAnimFinished)
                        SwitchState(STATE.normal);
                    UpdateStaminaSlidersValue();
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    UpdateStaminaColor();
                    break;

                case STATE.dashing:                                                 // DASHING
                    UpdateStaminaSlidersValue();
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    UpdateStaminaColor();
                    RunDash();
                    break;

                case STATE.clashed:                                                     // CLASHED
                    ManageStaminaRegen();
                    UpdateStaminaSlidersValue();
                    SetStaminaBarsOpacity(staminaBarsOpacity);
                    UpdateStaminaColor();

                    rb.velocity = Vector3.zero;
                    break;

                case STATE.enemyKilled:                                     // ENEMY KILLED
                    ManageMovementsInputs();
                    SetStaminaBarsOpacity(0);
                    playerAnimations.UpdateIdleStateDependingOnStamina(stamina);
                    break;

                case STATE.enemyKilledEndMatch:                                     // ENEMY KILLED END MATCH
                    ManageMovementsInputs();
                    SetStaminaBarsOpacity(0);
                    break;

                case STATE.dead:                                        // DEAD
                    break;
            }
        }
    }
    #endregion






    #region STATE SWITCH
    public virtual void SwitchState(STATE newState)
    {
        if (playerState != STATE.frozen)
            oldState = playerState;

        playerState = newState;

        switch (newState)
        {
            case STATE.onlinefrozen:                                                                    // ONLINE FROZEN
                SetStaminaBarsOpacity(0);

                // ONLINE
                if (ConnectManager.Instance != null)
                {
                    if (GetComponent<PhotonView>() && GetComponent<PhotonView>().IsMine)
                    {
                        characterChanger.enabled = false;
                        characterChanger.EnableVisuals(false);
                    }
                }
                else
                {
                    characterChanger.enabled = false;
                    characterChanger.EnableVisuals(false);
                }
                break;

            case STATE.frozen:                                                                            // FROZEN
                SetStaminaBarsOpacity(0);
                attackDashFXFront.Stop();
                attackDashFXBack.Stop();
                dashFXBack.Stop();
                dashFXFront.Stop();
                if (characterChanger != null)
                {
                    characterChanger.enabled = false;
                    characterChanger.EnableVisuals(false);
                }
                break;

            case STATE.sneathing:                                                                                 // SNEATHING
                rb.velocity = Vector3.zero;
                playerAnimations.TriggerSneath();
                break;

            case STATE.sneathed:                                                                                  // SNEATHED
                staminaBarsOpacity = 0;
                actualMovementsSpeed = sneathedMovementsSpeed;
                rb.simulated = true;

                if (characterType == CharacterType.duel)
                {
                    // ONLINE
                    if (ConnectManager.Instance != null && ConnectManager.Instance.enableMultiplayer)
                    {
                        if (GetComponent<PhotonView>() && GetComponent<PhotonView>().IsMine)
                            characterChanger.enabled = true;
                    }
                    else
                        characterChanger.enabled = true;
                }
                break;

            case STATE.drawing:                                                                                          // DRAWING
                rb.velocity = Vector3.zero;
                // ONLINE
                if (ConnectManager.Instance != null && ConnectManager.Instance.enableMultiplayer)
                {
                    if (GetComponent<PhotonView>() && GetComponent<PhotonView>().IsMine)
                        characterChanger.EnableVisuals(false);
                    characterChanger.enabled = false;
                }
                else
                {
                    characterChanger.EnableVisuals(false);
                    characterChanger.enabled = false;
                }

                if (playerNum == 0)
                    if (!ConnectManager.Instance.connectedToMaster)
                        if (GameManager.Instance.playersList.Count > 1)
                            GameManager.Instance.playersList[1].GetComponent<IAChanger>().enabled = false;
                break;

            case STATE.battleDrawing:                                                                                          // BATTLE DRAWING
                break;

            case STATE.battleSneathing:                                                                                  // BATTLE SNEATHING
                break;

            case STATE.battleSneathedNormal:                                                                              // BATTLE SNEATHED NORMAL
                break;

            case STATE.normal:                                                                                         // NORMAL
                actualMovementsSpeed = baseMovementsSpeed;
                dashTime = 0;
                isDashing = false;
                foreach (Collider2D col in playerColliders)
                    col.isTrigger = false;
                attackDashFXFront.Stop();
                attackDashFXBack.Stop();
                dashFXBack.Stop();
                dashFXFront.Stop();
                break;

            case STATE.charging:                                                    // CHARGING
                isDashing = false;
                chargeLevel = 1;
                chargeStartTime = Time.time;
                actualMovementsSpeed = chargeMovementsSpeed;
                dashFXBack.Stop();
                dashFXFront.Stop();
                chargeFlareFX.gameObject.SetActive(true);
                foreach (Collider2D col in playerColliders)
                    col.isTrigger = false;
                break;

            case STATE.attacking:                                                       // ATTACKING
                isDashing = true;
                canCharge = false;
                chargeLevel = 1;
                chargeSlider.value = 1;
                actualMovementsSpeed = attackingMovementsSpeed;
                foreach (Collider2D col in playerColliders)
                    col.isTrigger = true;
                PauseStaminaRegen();

                chargeFlareFX.gameObject.SetActive(false);
                chargeFlareFX.gameObject.SetActive(true);
                break;

            case STATE.canAttackAfterAttack:                                                // CAN ATTACK AFTER ATTACK
                actualMovementsSpeed = baseMovementsSpeed;
                dashTime = 0;
                isDashing = false;

                foreach (Collider2D col in playerColliders)
                    col.isTrigger = false;
                attackDashFXFront.Stop();
                attackDashFXBack.Stop();
                dashFXBack.Stop();
                dashFXFront.Stop();
                break;

            case STATE.pommeling:                                                       // POMMELING
                chargeLevel = 1;
                rb.velocity = Vector3.zero;
                PauseStaminaRegen();
                chargeFlareFX.gameObject.SetActive(false);
                chargeFlareFX.gameObject.SetActive(true);
                break;

            case STATE.parrying:                                                                // PARRYING
                chargeLevel = 1;
                canParry = false;
                PauseStaminaRegen();
                rb.velocity = Vector3.zero;
                dashFXBack.Stop();
                dashFXFront.Stop();
                chargeFlareFX.gameObject.SetActive(false);
                chargeFlareFX.gameObject.SetActive(true);
                break;

            case STATE.maintainParrying:                                                                // MAINTAIN PARRYING
                chargeLevel = 1;
                rb.velocity = Vector3.zero;
                PauseStaminaRegen();
                dashFXBack.Stop();
                dashFXFront.Stop();
                chargeFlareFX.gameObject.SetActive(false);
                chargeFlareFX.gameObject.SetActive(true);
                break;

            case STATE.preparingToJump:                                                                     // PREPARING TO JUMP
                rb.velocity = new Vector2(0, rb.velocity.y);
                walkFXBack.Stop();
                walkFXFront.Stop();
                break;

            case STATE.jumping:                                                                             // JUMPING
                PauseStaminaRegen();
                rb.velocity = new Vector2(0, rb.velocity.y);
                break;

            case STATE.dashing:                                                                                // DASHING
                canCharge = false;
                currentDashStep = DASHSTEP.invalidated;
                currentShortcutDashStep = DASHSTEP.invalidated;
                chargeLevel = 1;
                isDashing = true;
                foreach (Collider2D col in playerColliders)
                    col.isTrigger = true;
                PauseStaminaRegen();
                chargeFlareFX.gameObject.SetActive(false);
                break;

            case STATE.recovering:                                                                          // RECOVERING
                rb.velocity = Vector3.zero;
                break;

            case STATE.clashed:                                                                             // CLASHED
                chargeLevel = 1;
                foreach (Collider2D col in playerColliders)
                    col.isTrigger = true;

                PauseStaminaRegen();
                attackDashFXFront.Stop();
                attackDashFXBack.Stop();
                dashFXBack.Stop();
                dashFXFront.Stop();
                chargeFlareFX.gameObject.SetActive(false);

                attackRangeFX.gameObject.SetActive(false);
                attackRangeFX.gameObject.SetActive(true);
                break;

            case STATE.enemyKilled:                                                                             // ENEMY KILLED
                SetStaminaBarsOpacity(0);
                stamina = maxStamina;
                attackDashFXFront.Stop();
                attackDashFXBack.Stop();
                dashFXBack.Stop();
                dashFXFront.Stop();
                break;

            case STATE.dead:                                                                            // DEAD
                rb.velocity = new Vector2(0, rb.velocity.y);
                SetStaminaBarsOpacity(0);
                foreach (Collider2D col in playerColliders)
                    col.isTrigger = true;

                chargeFlareFX.gameObject.SetActive(false);
                chargeFlareFX.gameObject.SetActive(true);
                attackDashFXFront.Stop();
                attackDashFXBack.Stop();
                walkFXBack.Stop();
                walkFXFront.Stop();
                dashFXBack.Stop();
                dashFXFront.Stop();
                characterChanger.enabled = false;
                characterChanger.EnableVisuals(false);
                break;
        }
    }
    #endregion







    #region PLAYERS IDENTIFICATION
    // ENEMY NUM
    void GetOtherPlayerNum()
    {
        for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
            if (i != playerNum)
                otherPlayerNum = i;
    }

    protected virtual void FindOpponent()
    {
        foreach (Player p in FindObjectsOfType<Player>())
            if (p != this)
            {
                opponent = p;
                break;
            }
    }
    #endregion








    #region RESET ALL VALUES
    public void ResetAllPlayerValuesForNextMatch()
    {
        // Set up the character depending on the game's part
        if (characterType == CharacterType.duel)
            SwitchState(Player.STATE.frozen);
        else if (characterType == CharacterType.campaign)
            SwitchState(Player.STATE.sneathed);


        currentHealth = maxHealth;
        stamina = maxStamina;
        staminaSlider.gameObject.SetActive(true);
        canRegenStamina = true;
        chargeLevel = 1;


        rb.simulated = true;


        for (int i = 0; i < playerColliders.Length; i++)
            playerColliders[i].isTrigger = false;


        // ANIMATIONS
        playerAnimations.CancelCharge(true);
        playerAnimations.ResetAnimsForNextMatch();
    }


    [PunRPC]
    public virtual void ResetAllPlayerValuesForNextRound()
    {
        SwitchState(STATE.normal);


        currentHealth = maxHealth;
        stamina = maxStamina;
        staminaSlider.gameObject.SetActive(true);
        canRegenStamina = true;
        chargeLevel = 1;


        rb.simulated = true;


        // Restablishes colliders


        for (int i = 0; i < playerColliders.Length; i++)
            playerColliders[i].isTrigger = false;


        // ANIMATIONS
        playerAnimations.CancelCharge(true);
        playerAnimations.ResetAnimsForNextRound();
    }
    #endregion








    #region RECEIVE AN ATTACK
    public virtual bool TakeDamage(GameObject instigator, int hitStrength = 1)
    {
        bool hit = false;


        if (playerState != STATE.dead)
        {
            if (Mathf.Sign(instigator.transform.localScale.x) == Mathf.Sign(transform.localScale.x))
            {
                hit = true;
                if (ConnectManager.Instance.connectedToMaster)
                    photonView.RPC("TriggerHit", RpcTarget.AllViaServer);
                else
                    TriggerHit();

            }
            // CLASH
            else if (clashFrames)
            {
                foreach (GameObject p in GameManager.Instance.playersList)
                    if (ConnectManager.Instance.connectedToMaster)
                        p.GetComponent<PhotonView>().RPC("TriggerClash", RpcTarget.AllViaServer);
                    else
                        p.GetComponent<Player>().TriggerClash();

                // FX
                Vector3 fxPos = new Vector3((GameManager.Instance.playersList[0].transform.position.x + GameManager.Instance.playersList[1].transform.position.x) / 2, clashFX.transform.position.y, clashFX.transform.position.z);
                Instantiate(clashFXPrefabRef, fxPos, clashFX.transform.rotation, null).GetComponent<ParticleSystem>().Play();



                // AUDIO
                AudioManager.Instance.TriggerClashAudioCoroutine();



                // STATS
                if (statsManager)
                {
                    statsManager.AddAction(ACTION.clash, playerNum, 0);
                    statsManager.AddAction(ACTION.clash, otherPlayerNum, 0);
                }
                else
                    Debug.Log("Couldn't access statsManager to record action, ignoring");
            }
            // PARRY
            else if (parryFrame)
            {
                // STAMINA
                //stamina += staminaCostForMoves;
                if (ConnectManager.Instance.connectedToMaster)
                    photonView.RPC("N_TriggerStaminaRecupAnim", RpcTarget.All);
                else
                    StartCoroutine(TriggerStaminaRecupAnim());


                // CLASH
                if (ConnectManager.Instance.connectedToMaster)
                    instigator.GetComponent<PhotonView>().RPC("TriggerClash", RpcTarget.AllViaServer);
                else
                    instigator.GetComponent<Player>().TriggerClash();


                // ANIMATION
                playerAnimations.TriggerPerfectParry();


                // FX
                clashFX.Play();

                // SOUND
                AudioManager.Instance.TriggerParriedAudio();


                // STATS
                if (characterType == CharacterType.duel)
                {
                    if (statsManager)
                        statsManager.AddAction(ACTION.successfulParry, playerNum, 0);
                    else
                        Debug.Log("Couldn't access statsManager to record action, ignoring");
                }
            }
            // UNTOUCHABLE FRAMES
            else if (untouchableFrame)
            {
                GameManager.Instance.TriggerSlowMoCoroutine(GameManager.Instance.dodgeSlowMoDuration, GameManager.Instance.dodgeSlowMoTimeScale, GameManager.Instance.dodgeTimeScaleFadeSpeed);


                // SOUND
                AudioManager.Instance.BattleEventIncreaseIntensity();


                // STATS
                if (characterType == CharacterType.duel)
                {
                    if (statsManager)
                        statsManager.AddAction(ACTION.dodge, playerNum, 0);
                    else
                        Debug.Log("Couldn't access statsManager to record action, ignoring");
                }
            }
            // TOUCHED
            else
            {
                hit = true;


                // SOUND
                if (ConnectManager.Instance.connectedToMaster)
                    photonView.RPC("TriggerHit", RpcTarget.AllViaServer);
                else
                    TriggerHit();
            }

            if (ConnectManager.Instance.connectedToMaster)
                photonView.RPC("CheckDeath", RpcTarget.AllViaServer, instigator.GetComponent<Player>().playerNum);
            else
                CheckDeath(instigator.GetComponent<Player>().playerNum);
        }


        // FX
        attackRangeFX.gameObject.SetActive(false);
        attackRangeFX.gameObject.SetActive(true);


        return hit;
    }

    [PunRPC]
    public virtual void CheckDeath(int instigatorNum)
    {
        // IS DEAD ?
        if (currentHealth <= 0 && playerState != STATE.dead)
        {
            // Place correctly the players so it looks good
            // PLACE OPPONENT
            float howCloseTheOpponentIs = (GameManager.Instance.playersList[otherPlayerNum].transform.position.x - transform.position.x) * Mathf.Sign(GameManager.Instance.playersList[otherPlayerNum].transform.localScale.x);

            if (howCloseTheOpponentIs > -1f && howCloseTheOpponentIs < 0)
                GameManager.Instance.playersList[otherPlayerNum].transform.position = GameManager.Instance.playersList[otherPlayerNum].transform.position + new Vector3(-Mathf.Sign(GameManager.Instance.playersList[otherPlayerNum].transform.localScale.x) * 1.2f, 0, 0);
            else if (howCloseTheOpponentIs < 1f && howCloseTheOpponentIs > 0)
                GameManager.Instance.playersList[otherPlayerNum].transform.position = GameManager.Instance.playersList[otherPlayerNum].transform.position + new Vector3(Mathf.Sign(GameManager.Instance.playersList[otherPlayerNum].transform.localScale.x) * 1.4f, 0, 0);

            // PLACE SELF
            howCloseTheOpponentIs = (GameManager.Instance.playersList[otherPlayerNum].transform.position.x - transform.position.x) * Mathf.Sign(GameManager.Instance.playersList[otherPlayerNum].transform.localScale.x);
            if (howCloseTheOpponentIs < 0)
                transform.localScale = new Vector3(-Mathf.Sign(GameManager.Instance.playersList[otherPlayerNum].transform.localScale.x) * 1, transform.localScale.y, transform.localScale.z);




            // FX
            slashFX.Play();
            UpdateFXOrientation();




            // SNEATH STUFF
            bool wasSneathed = false;



            // ASKS TO START MATCH IF SNEATHED
            if (playerState == STATE.sneathed || playerState == STATE.drawing)
                wasSneathed = true;


            // STATE
            SwitchState(STATE.dead);


            // STARTS MATCH IF PLAYER WAS SNEATHED
            if (wasSneathed)
                GameManager.Instance.SaberDrawn(playerNum);


            // HAS WON ?
            if (GameManager.Instance.score[instigatorNum] + 1 >= GameManager.Instance.scoreToWin)
            {
                GameManager.Instance.TriggerMatchEndFilterEffect(true);
                GameManager.Instance.finalCameraShake.shakeDuration = GameManager.Instance.finalCameraShakeDuration;
            }


            // ANIMATIONS
            playerAnimations.TriggerDeath();
            playerAnimations.DeathActivated(true);


            // FX
            chargeFX.Stop();


            // CAMERA FX
            GameManager.Instance.APlayerIsDead(instigatorNum);


            // STATS
            if (characterType == CharacterType.duel)
            {
                if (statsManager)
                    statsManager.AddAction(ACTION.death, playerNum, 0);
                else
                    Debug.Log("Couldn't access statsManager to record action, ignoring");
            }
        }
    }

    // Hit
    [PunRPC]
    protected virtual void TriggerHit()
    {
        currentHealth -= 1;



        // SOUND
        if (GameManager.Instance.score[otherPlayerNum] >= GameManager.Instance.scoreToWin - 1)
        {
            if (finalDeathAudioFX != null)
                finalDeathAudioFX.Play();
            else
            {
                Debug.Log("Couldn't find final death audio source, ignoring");

                if (AudioManager.Instance != null)
                    AudioManager.Instance.TriggerSuccessfulAttackAudio();
                else
                    Debug.Log("Couldn't find audio manager, ignoring");
            }
        }
        else
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.TriggerSuccessfulAttackAudio();
            else
                Debug.Log("Couldn't find audio manager, ignoring");
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.BattleEventIncreaseIntensity();
        else
            Debug.Log("Couldn't find audio manager, ignoring");



        // CAMERA FX
        GameManager.Instance.deathCameraShake.shakeDuration = GameManager.Instance.deathCameraShakeDuration;
        GameManager.Instance.TriggerSlowMoCoroutine(GameManager.Instance.roundEndSlowMoDuration, GameManager.Instance.roundEndSlowMoTimeScale, GameManager.Instance.roundEndTimeScaleFadeSpeed);
    }
    #endregion





    void ManageIA()
    {
        if (ConnectManager.Instance != null && ConnectManager.Instance.connectedToMaster)
            return;

        if (InputManager.Instance.playerInputs[playerNum].switchChar && opponent.playerIsAI)
        {
            IAScript enemyIA = opponent.GetComponent<IAScript>();


            if (enemyIA != null)
                switch (enemyIA.IADifficulty)
                {
                    case IAScript.Difficulty.Easy:
                        enemyIA.SetDifficulty(IAScript.Difficulty.Medium);
                        break;

                    case IAScript.Difficulty.Medium:
                        enemyIA.SetDifficulty(IAScript.Difficulty.Hard);
                        break;

                    case IAScript.Difficulty.Hard:
                        enemyIA.SetDifficulty(IAScript.Difficulty.Easy);
                        break;
                }
        }
    }





    #region STAMINA STUFF
    // Set up stamina bar system
    public void SetUpStaminaBars()
    {
        staminaSliders.Add(staminaSlider);


        for (int i = 0; i < maxStamina - 1; i++)
            staminaSliders.Add(Instantiate(staminaSlider.gameObject, staminaSlider.transform.parent).GetComponent<Slider>());
    }

    // Manage stamina regeneration, executed in FixedUpdate
    void ManageStaminaRegen()
    {
        if (canRegenStamina && !staminaRecupAnimOn)
        {
            // Quick regen gap mode
            if (stamina < quickStaminaRegenGap && quickRegen && (!quickRegenOnlyWhenReachedLowStaminaGap || hasReachedLowStamina))
            {
                // If back walking
                if (rb.velocity.x * -transform.localScale.x < 0)
                    stamina += Time.deltaTime * backWalkingQuickStaminaGainOverTime * staminaGlobalGainOverTimeMultiplier;
                // If idle walking
                else if (Mathf.Abs(rb.velocity.x) <= 0.5f)
                    stamina += Time.deltaTime * idleQuickStaminaGainOverTimeMultiplier * staminaGlobalGainOverTimeMultiplier;
                // If front walking
                else
                    stamina += Time.deltaTime * frontWalkingQuickStaminaGainOverTime * staminaGlobalGainOverTimeMultiplier;
            }
            else if (stamina < maxStamina)
            {
                if (hasReachedLowStamina)
                    hasReachedLowStamina = false;

                // If back walking
                if (rb.velocity.x * -transform.localScale.x < 0)
                    stamina += Time.deltaTime * backWalkingStaminaGainOverTime * staminaGlobalGainOverTimeMultiplier;
                // If idle walking
                else if (Mathf.Abs(rb.velocity.x) <= 0.5f)
                    stamina += Time.deltaTime * idleStaminaGainOverTimeMultiplier * staminaGlobalGainOverTimeMultiplier;
                // If front walking
                else
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
            currentTimeBeforeStaminaRegen -= Time.deltaTime;
    }

    // Trigger the stamina regen pause duration
    public void PauseStaminaRegen()
    {
        canRegenStamina = false;
        currentTimeBeforeStaminaRegen = durationBeforeStaminaRegen;
    }

    // Function to decrement to stamina
    public void StaminaCost(float cost, bool playFX)
    {
        if (!cheatSettings.infiniteStamina)
        {
            stamina -= cost;


            if (stamina < lowStaminaGap)
                hasReachedLowStamina = true;

            if (stamina <= 0)
                stamina = 0;


            // SOUND
            // Used all stamina sfx
            if (stamina < staminaCostForMoves)
            {
                if (staminaEndSFX != null)
                    staminaEndSFX.Play();
                else
                    Debug.Log("Couldn't fine stamina end audio source, ignoring");
            }

            // Used stamina sfx
            if (staminaUseSFX != null)
                staminaUseSFX.Play();
            else
                Debug.Log("Couldn't fine stamina use audio source, ignoring");


            // FX
            if (cheatSettings.useExtraDiegeticFX && playFX)
                staminaLossFX.Play();
        }
    }

    // Update stamina slider value
    protected void UpdateStaminaSlidersValue()
    {
        // DETECT STAMINA CHARGE UP
        if (Mathf.FloorToInt(oldStaminaValue) < Mathf.FloorToInt(stamina) && (characterType == CharacterType.campaign || (characterType == CharacterType.duel && !gameManager.playerDead && gameManager.gameState == GameManager.GAMESTATE.game)))
        {
            if (!staminaRecupAnimOn && !staminaBreakAnimOn)
            {
                if (!staminaRecupAnimOn && !staminaBreakAnimOn)
                {
                    staminaBarChargedAudioEffectSource.Play();

                    if (cheatSettings.useExtraDiegeticFX)
                    {
                        staminaGainFX.Play();
                        staminaGainFX.GetComponent<ParticleSystem>().Play();
                    }
                }
            }
        }

        oldStaminaValue = stamina;
        staminaSliders[0].value = Mathf.Clamp(stamina, 0, 1);


        // FX pos
        staminaLossFX.gameObject.transform.position = staminaSliders[(int)Mathf.Clamp((int)(stamina + 0.5f), 0, maxStamina - 1)].transform.position;
        staminaGainFX.gameObject.transform.position = staminaSliders[(int)Mathf.Clamp((int)(stamina - 0.5f), 0, maxStamina - 1)].transform.position + new Vector3(0.1f, 0, 0) * Mathf.Sign(transform.localScale.x);

        // Stamina recup anim FX pox
        staminaRecupFX.gameObject.transform.position = staminaSliders[(int)Mathf.Clamp((int)(stamina - 0f), 0, maxStamina - 1)].transform.position + new Vector3(0.1f, 0.1f * Mathf.Sign(transform.localScale.x), 0) * Mathf.Sign(transform.localScale.x);
        staminaRecupFinishedFX.gameObject.transform.position = staminaSliders[(int)Mathf.Clamp((int)(stamina - 0f), 0, maxStamina - 1)].transform.position + new Vector3(0.1f, 0.1f * Mathf.Sign(transform.localScale.x), 0) * Mathf.Sign(transform.localScale.x);


        // Break FX pos
        staminaBreakFX.gameObject.transform.position = staminaSliders[(int)Mathf.Clamp((int)(stamina + 0.5f), 0, maxStamina - 1)].transform.position + new Vector3(0.2f, 0.1f * Mathf.Sign(transform.localScale.x), 0) * Mathf.Sign(transform.localScale.x);


        for (int i = 1; i < staminaSliders.Count; i++)
        {
            staminaSliders[i].value = Mathf.Clamp(stamina, i, i + 1) - i;
        }


        if (stamina >= maxStamina)
        {
            if (staminaBarsOpacity > 0)
            {
                staminaBarsOpacity -= 0.05f;
            }
        }
        else if (staminaBarsOpacity != staminaBarBaseOpacity)
        {
            staminaBarsOpacity = staminaBarBaseOpacity;
        }
    }

    // Manages stamina bars opacity
    protected void SetStaminaBarsOpacity(float opacity)
    {
        if (!staminaRecupAnimOn)
        {
            for (int i = 0; i < staminaSliders.Count; i++)
            {
                Color fillColor = staminaSliders[i].fillRect.GetComponent<Image>().color;
                Color backgroundColor = staminaSliders[i].GetComponent<StaminaSlider>().fillArea.color;

                staminaSliders[i].fillRect.GetComponent<Image>().color = new Color(fillColor.r, fillColor.g, fillColor.b, opacity);
                staminaSliders[i].GetComponent<StaminaSlider>().fillArea.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, opacity);
            }
        }
        //Debug.Log();
    }

    protected void UpdateStaminaColor()
    {
        if (!staminaRecupAnimOn && !staminaBreakAnimOn)
        {
            if (stamina < staminaCostForMoves)
                SetStaminaColor(staminaDeadColor);
            else if (stamina < staminaCostForMoves * 2)
                SetStaminaColor(staminaLowColor);
            else
                SetStaminaColor(staminaBaseColor);
        }
    }

    void SetStaminaColor(Color color)
    {
        for (int i = 0; i < staminaSliders.Count; i++)
            staminaSliders[i].fillRect.gameObject.GetComponent<Image>().color = Color.Lerp(staminaSliders[i].fillRect.gameObject.GetComponent<Image>().color, color, Time.deltaTime * 10);
    }
    #endregion




    #region STAMINA ANIMS
    // Not enough stamina anim
    protected void TriggerNotEnoughStaminaAnim(bool state)
    {
        for (int i = 0; i < staminaSliders.Count; i++)
        {
            if (state)
                staminaSliders[i].GetComponent<Animator>().SetTrigger("NotEnoughStamina");
            else
                staminaSliders[i].GetComponent<Animator>().ResetTrigger("NotEnoughStamina");
        }


        // AUDIO
        if (notEnoughStaminaSFX != null)
            notEnoughStaminaSFX.Play();
    }

    [PunRPC]
    protected virtual void N_TriggerStaminaRecupAnim()
    {
        StartCoroutine("TriggerStaminaRecupAnim");
    }

    // Stamina recup anim
    protected IEnumerator TriggerStaminaRecupAnim()
    {
        // COLOR
        for (int i = 0; i < staminaSliders.Count; i++)
            staminaSliders[i].fillRect.gameObject.GetComponent<Image>().color = staminaRecupColor;


        staminaRecupAnimOn = true;


        yield return new WaitForSecondsRealtime(staminaRecupTriggerDelay);


        float regeneratedAmount = 0;


        // FX
        if (cheatSettings.useExtraDiegeticFX)
            staminaRecupFX.Play();


        while (regeneratedAmount < 1)
        {
            stamina += staminaRecupAnimRegenSpeed;
            regeneratedAmount += staminaRecupAnimRegenSpeed;


            if (stamina >= maxStamina)
                stamina = maxStamina;


            yield return new WaitForSecondsRealtime(0.01f);
        }


        if (cheatSettings.useExtraDiegeticFX)
            staminaRecupFinishedFX.Play();
        staminaRecupAnimOn = false;


        // AUDIO
        staminaBarChargedAudioEffectSource.Play();


        // FX
        if (cheatSettings.useExtraDiegeticFX)
        {
            staminaGainFX.Play();
            staminaRecupFX.Stop();
        }
    }

    // Stamina break 
    [PunRPC]
    protected virtual void InitStaminaBreak()
    {
        for (int i = 0; i < staminaSliders.Count; i++)
            staminaSliders[i].fillRect.gameObject.GetComponent<Image>().color = staminaBreakColor;

        staminaBreakAnimOn = true;
        Invoke("TriggerStaminaBreak", 0.4f);
    }

    void TriggerStaminaBreak()
    {
        TriggerNotEnoughStaminaAnim(false);
        TriggerNotEnoughStaminaAnim(true);
        StaminaCost(staminaCostForMoves, false);
        staminaBreakAudioFX.Play();


        // FX
        if (cheatSettings.useExtraDiegeticFX)
            staminaBreakFX.Play();

        Invoke("StopStaminaBreak", 0.6f);
    }

    void StopStaminaBreak()
    {
        staminaBreakAnimOn = false;
    }

    #endregion








    #region MOVEMENTS

    public virtual void ManageMovementsInputs()
    {
        /*  if (rb.simulated == false)
              rb.simulated = true;
  */
        if (!playerIsAI)
        {
            if (ConnectManager.Instance != null && ConnectManager.Instance.connectedToMaster)
            {
                if (photonView.IsMine)
                    rb.velocity = new Vector2(InputManager.Instance.playerInputs[0].horizontal * actualMovementsSpeed, rb.velocity.y);
            }
            else
                rb.velocity = new Vector2(InputManager.Instance.playerInputs[playerNum].horizontal * actualMovementsSpeed, rb.velocity.y);
        }



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

    // MOVEMENTS
    /*public virtual void ManageMovementsInputs(InputAction.CallbackContext ctx)
    {
        float velocity = ctx.ReadValue<float>();


        // The player move if they can in their state
        if (rb.simulated == false)
            rb.simulated = true;


        if (!playerIsAI)
        {
            if (ConnectManager.Instance != null && ConnectManager.Instance.connectedToMaster)
            {
                if (photonView.IsMine)
                {
                    rb.velocity = new Vector2(velocity * actualMovementsSpeed, rb.velocity.y);
                }
            }
            else
            {
                rb.velocity = new Vector2(velocity * actualMovementsSpeed, rb.velocity.y);
            }
        }


        // FX
        if (Mathf.Abs(rb.velocity.x) > minSpeedForWalkFX && playerState == Player.STATE.normal && (characterType == CharacterType.campaign || (characterType == CharacterType.duel && GameManager.Instance.gameState == GameManager.GAMESTATE.game)))
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
    }*/
    #endregion








    #region DRAW
    // Detects draw input
    protected virtual void ManageDraw()
    {
        if (ConnectManager.Instance != null && ConnectManager.Instance.connectedToMaster)
        {
            if (InputManager.Instance.playerInputs[0].anyKey)
                photonView.RPC("TriggerDraw", RpcTarget.AllBufferedViaServer);
        }
        else
        {
            if (InputManager.Instance.playerInputs[playerNum].anyKey && !characterChanger.charactersDatabase.charactersList[characterChanger.currentCharacterIndex].locked)
            {
                TriggerDraw();
                Debug.Log("Draw");
            }
        }
    }

    // Triggers saber draw and informs the game manager
    [PunRPC]
    public virtual void TriggerDraw()
    {
        if (DrawnEvent != null)
        {
            DrawnEvent();
            Debug.Log("Drawn");
        }
        SwitchState(STATE.drawing);


        // RANGE
        // Get range of the character
        lightAttackRange = characterChanger.charactersDatabase.charactersList[characterChanger.currentCharacterIndex].character.attack01RangeRange[0];
        heavyAttackRange = characterChanger.charactersDatabase.charactersList[characterChanger.currentCharacterIndex].character.attack01RangeRange[1];



        // ANIMATION
        playerAnimations.TriggerDraw();
    }
    #endregion


    #region BATTLE SNEATH / DRAW
    void ManageBattleSneath()
    {
        if (canBattleSneath)
        {
            // ONLINE
            if (ConnectManager.Instance != null && ConnectManager.Instance.enableMultiplayer)
            {
                if (InputManager.Instance.playerInputs != null && InputManager.Instance.playerInputs.Length > 0 && InputManager.Instance.playerInputs[0].battleSneathDraw)
                    TriggerBattleSneath();
            }
            else if (InputManager.Instance.playerInputs.Length > playerNum && InputManager.Instance.playerInputs[playerNum].battleSneathDraw)
                TriggerBattleSneath();
        }
    }

    void ManageBattleDraw()
    {
        if (InputManager.Instance.playerInputs[playerNum].battleSneathDraw)
            TriggerBattleDraw();
    }

    void TriggerBattleSneath()
    {
        Debug.Log("Battle sneath");

        // If players haven't all drawn, go back to chara selec state
        if (!GameManager.Instance.allPlayersHaveDrawn)
            // STATE
            SwitchState(STATE.sneathing);
        /*
           
        else
        {
            // ANIMATION
            playerAnimations.TriggerBattleSneath();


            // STATE
            SwitchState(STATE.battleSneathing);
        }
        */
    }

    void TriggerBattleDraw()
    {
        // ANIMATION
        playerAnimations.TriggerBattleDraw();


        // STATE
        SwitchState(STATE.battleDrawing);
    }
    #endregion





    #region JUMP
    void ManageJumpInput()
    {
        if (canJump)
        {
            if (ConnectManager.Instance.enableMultiplayer)
            {
                if (!InputManager.Instance.playerInputs[0].jump)
                {
                    //StartCoroutine(JumpCoroutine());
                    TriggerJumpBeginning();
                }
            }
            else
            {
                if (InputManager.Instance.playerInputs[playerNum].jump)
                    TriggerJumpBeginning();
            }
        }
    }

    void TriggerJumpBeginning()
    {
        SwitchState(STATE.preparingToJump);
        playerAnimations.TriggerJump();
    }

    void ActuallyJump()
    {
        SwitchState(STATE.jumping);
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
    }
    #endregion








    #region CHARGE
    // Manages the detection of attack charge inputs
    internal virtual void ManageChargeInput()
    {
        // ONLINE
        if (ConnectManager.Instance != null && ConnectManager.Instance.enableMultiplayer)
        {
            // Player presses attack button
            if (InputManager.Instance.playerInputs[0].attack && canCharge)
            {
                if (stamina >= staminaCostForMoves)
                {
                    SwitchState(STATE.charging);
                    canCharge = false;
                    chargeStartTime = Time.time;

                    // FX
                    chargeFlareFX.Play();
                    chargeSlider.value = 1;


                    // ANIMATION
                    playerAnimations.CancelCharge(false);
                    playerAnimations.TriggerCharge(true);


                    // STATS
                    if (characterType == CharacterType.duel)
                    {
                        if (statsManager)
                            statsManager.AddAction(ACTION.charge, playerNum, 0);
                        else
                            Debug.Log("Couldn't access statsManager to record action, ignoring");
                    }


                    // FX
                    chargeFlareFX.Play();


                    // ANIMATION
                    playerAnimations.CancelCharge(false);
                    playerAnimations.TriggerCharge(true);
                }
            }

            // Player releases attack button
            if (!InputManager.Instance.playerInputs[0].attack)
                canCharge = true;
        }
        else
        {
            // Player presses attack button
            //OLD_INPUT
            //if (InputManager.Instance.playerInputs[playerNum].attack && canCharge)
            if (InputManager.Instance.playerInputs[playerNum].attack && canCharge)
            {
                // ANIMATION STAMINA
                if (stamina <= staminaCostForMoves)
                    TriggerNotEnoughStaminaAnim(true);

                if (stamina >= staminaCostForMoves)
                {
                    canCharge = false;
                    SwitchState(STATE.charging);


                    // ANIMATION
                    playerAnimations.CancelCharge(false);
                    playerAnimations.TriggerCharge(true);


                    chargeStartTime = Time.time;


                    // STATS
                    if (characterType == CharacterType.duel)
                    {
                        if (statsManager)
                            statsManager.AddAction(ACTION.charge, playerNum, 0);
                        else
                            Debug.Log("Couldn't access statsManager to record action, ignoring");
                    }


                    // FX
                    chargeFlareFX.Play();
                }
            }

            

            if (!InputManager.Instance.playerInputs[playerNum].attack)
                canCharge = true;
        }
    }

    protected virtual void ManageCharging()
    {
        // Online
        if (ConnectManager.Instance != null && ConnectManager.Instance.connectedToMaster && photonView.IsMine)
        {
            //Player releases attack button
            if (!InputManager.Instance.playerInputs[0].attack)
            {
                photonView.RPC("ReleaseAttack", RpcTarget.AllViaServer);
                return;
            }
        }
        else
        {
            //Player releases attack button
            if (!InputManager.Instance.playerInputs[playerNum].attack)
            {
                ReleaseAttack();
                return;
            }
        }


        // If the player has waited too long charging
        if (chargeLevel >= maxChargeLevel)
        {
            if (Time.time - maxChargeLevelStartTime >= maxHoldDurationAtMaxCharge)
            {
                if (ConnectManager.Instance != null && ConnectManager.Instance.enableMultiplayer)
                    photonView.RPC("ReleaseAttack", RpcTarget.All);
                else
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

    void UpdateChargeShadowSize()
    {
        if (cheatSettings.useRangeShadow)
        {
            float X_ScaleObjective = 0;
            float opacityObjective = 0.3f;
            Color shadowColor = rangeIndicatorShadowSprite.color;


            if (playerState == STATE.charging)
            {
                opacityObjective = 1;
                X_ScaleObjective = lightAttackRange + (heavyAttackRange - 0.4f - lightAttackRange) * ((float)chargeLevel - 1) / (float)maxChargeLevel;
            }
            else
                X_ScaleObjective = 1f;



            float newX_Scale = Mathf.Lerp(rangeIndicatorShadow.transform.localScale.x, X_ScaleObjective, 0.05f);
            rangeIndicatorShadow.transform.localScale = new Vector3(newX_Scale, rangeIndicatorShadow.transform.localScale.y, rangeIndicatorShadow.transform.localScale.z);

            opacityObjective = Mathf.Lerp(shadowColor.a, opacityObjective, 0.05f);
            Color newShadowColor = new Color(shadowColor.r, shadowColor.g, shadowColor.b, opacityObjective);
            rangeIndicatorShadowSprite.color = newShadowColor;
        }
    }
    #endregion









    #region ATTACK
    // Triggers the attack
    [PunRPC]
    protected virtual void ReleaseAttack()
    {
        // STATS
        int saveChargeLevelForStats = chargeLevel;




        // Get range of the character
        lightAttackRange = characterChanger.charactersDatabase.charactersList[characterChanger.currentCharacterIndex].character.attack01RangeRange[0];
        heavyAttackRange = characterChanger.charactersDatabase.charactersList[characterChanger.currentCharacterIndex].character.attack01RangeRange[1];



        // Calculates attack range
        if (chargeLevel == 1)
            actualAttackRange = lightAttackRange;
        else if (chargeLevel == maxChargeLevel)
            actualAttackRange = heavyAttackRange;
        else
            actualAttackRange = lightAttackRange + (heavyAttackRange - lightAttackRange) * ((float)chargeLevel - 1) / (float)maxChargeLevel;

        actualBackAttackRangeDisjoint = baseBackAttackRangeDisjoint;



        // Get graphic range
        lightAttackSwordTrailScale = characterChanger.charactersDatabase.charactersList[characterChanger.currentCharacterIndex].character.lightAttackSwordTrailScale;
        heavyAttackSwordTrailScale = characterChanger.charactersDatabase.charactersList[characterChanger.currentCharacterIndex].character.heavyAttackSwordTrailScale;


        // FX
        // Slash FX width depending on range
        if (chargeLevel == 1)
            attackSlashFXParent.transform.localScale = new Vector3(lightAttackSwordTrailScale, attackSlashFXParent.transform.localScale.y, attackSlashFXParent.transform.localScale.z);
        else if (chargeLevel == maxChargeLevel)
            attackSlashFXParent.transform.localScale = new Vector3(heavyAttackSwordTrailScale, attackSlashFXParent.transform.localScale.y, attackSlashFXParent.transform.localScale.z);
        else
        {
            attackSlashFXParent.transform.localScale = new Vector3(
                lightAttackSwordTrailScale + (heavyAttackSwordTrailScale - lightAttackSwordTrailScale) * (actualAttackRange - lightAttackRange) / (heavyAttackRange - lightAttackRange),
                attackSlashFXParent.transform.localScale.y,
                attackSlashFXParent.transform.localScale.z
            );
        }






        // STAMINA
        StaminaCost(staminaCostForMoves, true);


        targetsHit.Clear();


        // FX
        Vector3 attackSignPos = attackRangeFX.transform.localPosition;
        attackRangeFX.transform.localPosition = new Vector3(-(actualAttackRange + attackSignDisjoint), attackSignPos.y, attackSignPos.z);
        if (cheatSettings.useExtraDiegeticFX && cheatSettings.useRangeFlareFX)
            attackRangeFX.Play();
        chargeFlareFX.gameObject.SetActive(false);
        chargeFlareFX.gameObject.SetActive(true);


        // Dash direction & distance
        Vector3 dashDirection3D = new Vector3(0, 0, 0);
        float dashDirection = 0;

        int inputNum;
        if (ConnectManager.Instance != null && ConnectManager.Instance.connectedToMaster)
            inputNum = 0;
        else
            inputNum = playerNum;

        if (Mathf.Abs(InputManager.Instance.playerInputs[inputNum].horizontal) > attackReleaseAxisInputDeadZoneForDashAttack)
        {
            dashDirection = Mathf.Sign(InputManager.Instance.playerInputs[inputNum].horizontal) * transform.localScale.x;
            dashDirection3D = new Vector3(Mathf.Sign(InputManager.Instance.playerInputs[inputNum].horizontal), 0, 0);


            // Dash distance
            if (Mathf.Sign(InputManager.Instance.playerInputs[inputNum].horizontal) == -Mathf.Sign(transform.localScale.x))
            {
                actualUsedDashDistance = forwardAttackDashDistance;
                actualBackAttackRangeDisjoint = forwardAttackBackrangeDisjoint;


                // FX
                attackDashFXFront.Play();


                // STATS
                if (characterType == CharacterType.duel)
                {
                    if (statsManager)
                        statsManager.AddAction(ACTION.forwardAttack, inputNum, saveChargeLevelForStats);
                    else
                        Debug.Log("Couldn't access statsManager to record action, ignoring");
                }
            }
            else
            {
                actualUsedDashDistance = backwardsAttackDashDistance;


                // FX
                attackDashFXBack.Play();


                // STATS
                if (characterType == CharacterType.duel)
                {
                    if (statsManager)
                        statsManager.AddAction(ACTION.backwardsAttack, inputNum, saveChargeLevelForStats);
                    else
                        Debug.Log("Couldn't access statsManager to record action, ignoring");
                }
            }
        }
        else
        {
            // FX
            attackNeutralFX.Play();


            // STATS
            if (characterType == CharacterType.duel)
            {
                if (statsManager)
                    statsManager.AddAction(ACTION.neutralAttack, inputNum, saveChargeLevelForStats);
                else
                    Debug.Log("Couldn't access statsManager to record action, ignoring");
            }
        }


        dashDirection3D *= actualUsedDashDistance;
        initPos = transform.position;
        targetPos = transform.position + dashDirection3D;
        targetPos.y = transform.position.y;
        dashTime = 0;

        rb.velocity = Vector3.zero;



        // ANIMATION
        playerAnimations.TriggerAttack(dashDirection);



        // STATE SWITCH
        //NE PAS SUPPRIMER
        /*StopAllCoroutines();
        Debug.Log("Stop coroutines");*/
        SwitchState(STATE.attacking);
    }

    // Hits with a phantom collider to apply the attack's damage during active frames
    protected virtual void ApplyAttackHitbox()
    {
        enemyDead = false;

        Collider2D[] hitsCol = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (transform.localScale.x * (-actualAttackRange + actualBackAttackRangeDisjoint) / 2), transform.position.y), new Vector2(actualAttackRange + actualBackAttackRangeDisjoint, 0.2f), 0);
        List<GameObject> hits = new List<GameObject>();


        foreach (Collider2D c in hitsCol)
        {
            if (c.CompareTag("Player") && !hits.Contains(c.transform.parent.gameObject))
            {

                hits.Add(c.transform.parent.gameObject);
            }
            else if (c.CompareTag("Destructible") && !hits.Contains(c.transform.parent.gameObject))
            {
                hits.Add(c.gameObject);
            }
        }


        foreach (GameObject g in hits)
            if (g != gameObject && !targetsHit.Contains(g) && g.CompareTag("Player"))
            {
                targetsHit.Add(g);

                enemyDead = g.GetComponent<Player>().TakeDamage(gameObject, chargeLevel);

                // FX
                attackRangeFX.gameObject.SetActive(false);
                attackRangeFX.gameObject.SetActive(true);


                if (enemyDead)
                    SwitchState(STATE.enemyKilled);
            }
            else if (g != gameObject && !targetsHit.Contains(g) && g.CompareTag("Destructible"))
            {
                targetsHit.Add(g);

                if (g.GetComponent<Destructible>())
                    g.GetComponent<Destructible>().Destroy();
                else if (g.transform.parent.gameObject.GetComponent<Destructible>())
                    g.transform.parent.gameObject.GetComponent<Destructible>().Destroy();
            }
    }
    #endregion








    #region MAINTAIN PARRY
    // Detect parry inputs
    void ManageMaintainParryInput()
    {
        if (canMaintainParry)
        {
            if (InputManager.Instance.playerInputs[playerNum].parry && canParry)
            {
                currentParryFramesPressed++;
                canParry = false;


                if (stamina >= maintainParryStaminaCostOverTime)
                    TriggerMaintainParry();


                currentParryFramesPressed = 0;
            }


            if (stamina <= maintainParryStaminaCostOverTime)
                ReleaseMaintainParry();

            if (!InputManager.Instance.playerInputs[playerNum].parry)
            {
                ReleaseMaintainParry();
                canParry = true;
            }
        }
    }

    // Maintain parry coroutine
    void TriggerMaintainParry()
    {
        // ANIMATION
        playerAnimations.ResetMaintainParry();
        playerAnimations.TriggerMaintainParry();


        // STATE
        SwitchState(STATE.maintainParrying);


        // STATS
        statsManager.AddAction(ACTION.parry, playerNum, chargeLevel);
    }

    void ReleaseMaintainParry()
    {
        // ANIMATION
        playerAnimations.EndMaintainParry();
    }
    #endregion







    #region PARRY
    // Detect parry inputs
    public virtual void ManageParryInput()
    {
        // If online, only take inputs from player 1
        if (canBriefParry)
        {

            // Stamina animation
            if (InputManager.Instance.playerInputs[playerNum].parryDown && stamina <= staminaCostForMoves && canParry)
                TriggerNotEnoughStaminaAnim(true);


            if (InputManager.Instance.playerInputs[playerNum].parry && canParry)
            {
                canParry = false;


                if (stamina >= staminaCostForMoves)
                    TriggerParry();
            }



            // Can input again if released the input
            if (!InputManager.Instance.playerInputs[playerNum].parry)
                canParry = true;
        }
    }

    // Parry coroutine
    protected virtual void TriggerParry()
    {
        // ANIMATION
        playerAnimations.TriggerParry();

        SwitchState(STATE.parrying);
        StaminaCost(staminaCostForMoves, true);

        // STATS
        if (characterType == CharacterType.duel)
        {
            if (statsManager)
                statsManager.AddAction(ACTION.parry, playerNum, chargeLevel);
            else
                Debug.Log("Couldn't access statsManager to record action, ignoring");
        }
    }
    #endregion








    #region POMMEL
    // Detect pommel inputs
    public virtual void ManagePommel(
        )
    {
        // ONLINE
        if (ConnectManager.Instance != null && ConnectManager.Instance.connectedToMaster)
        {
            if (!InputManager.Instance.playerInputs[0].kick)
                canPommel = true;


            if (InputManager.Instance.playerInputs[0].kick && canPommel)
            {
                canPommel = false;

                photonView.RPC("TriggerPommel", RpcTarget.All);
            }
        }
        else
        {
            if (!InputManager.Instance.playerInputs[playerNum].kick)
                canPommel = true;


            if (InputManager.Instance.playerInputs[playerNum].kick && canPommel)
            {
                canPommel = false;
                TriggerPommel();
            }
        }
    }

    // Pommel coroutine
    [PunRPC]
    protected virtual void TriggerPommel()
    {
        // ANIMATION
        playerAnimations.TriggerPommel();


        // STATE
        SwitchState(STATE.pommeling);



        targetsHit.Clear();


        // STATS
        if (characterType == CharacterType.duel)
        {
            if (statsManager)
                statsManager.AddAction(ACTION.pommel, playerNum, chargeLevel);
            else
                Debug.Log("Couldn't access statsManager to record action, ignoring");
        }
    }

    // Apply pommel hitbox depending on kick frames
    protected virtual void ApplyPommelHitbox()
    {
        float pommelRange = characterChanger.charactersDatabase.charactersList[characterIndex].character.pommelRange;

        Collider2D[] hitsCol = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (transform.localScale.x * -pommelRange / 2), transform.position.y), new Vector2(pommelRange, 0.2f), 0);
        List<GameObject> hits = new List<GameObject>();


        foreach (Collider2D c in hitsCol)
            if (c.CompareTag("Player") && !hits.Contains(c.transform.parent.gameObject))
                hits.Add(c.transform.parent.gameObject);


        foreach (GameObject g in hits)
            if (g != gameObject && !targetsHit.Contains(g))
            {
                targetsHit.Add(g);


                if (g.GetComponent<Player>().playerState != Player.STATE.clashed)
                {
                    if (ConnectManager.Instance.connectedToMaster)
                        g.GetComponent<PhotonView>().RPC("Pommeled", RpcTarget.All);
                    else
                        g.GetComponent<Player>().Pommeled();
                }
            }
    }
    #endregion








    #region POMMELED
    // The player have been kicked
    [PunRPC]
    public virtual void Pommeled()
    {
        if (!kickFrame)
        {
            bool wasSneathed = false;


            // ASKS TO START MATCH IF SNEATHED
            if (playerState == STATE.sneathed || playerState == STATE.drawing)
                wasSneathed = true;


            // ANIMATIONs
            playerAnimations.CancelCharge(true);
            playerAnimations.ResetPommeledTrigger();
            playerAnimations.TriggerPommeled();


            // Stamina
            if (playerState == STATE.parrying || playerState == STATE.attacking)
            {
                if (ConnectManager.Instance.connectedToMaster)
                    photonView.RPC("InitStaminaBreak", RpcTarget.All);
                else
                    InitStaminaBreak();
            }


            //NE PAS SUPPRIMER
            //StopAllCoroutines();
            SwitchState(STATE.clashed);
            ApplyOrientation(-GameManager.Instance.playersList[otherPlayerNum].transform.localScale.x);


            // STARTS MATCH IF PLAYER WAS SNEATHED
            if (wasSneathed)
                GameManager.Instance.SaberDrawn(playerNum);


            canCharge = false;
            chargeLevel = 1;




            // If is behind opponent when parried / clashed adds additional distance to evade the position and not look weird like they're fused together
            if (((transform.position.x - GameManager.Instance.playersList[otherPlayerNum].transform.position.x) * Mathf.Sign(transform.localScale.x)) <= 0.7f)
                transform.position = new Vector3(GameManager.Instance.playersList[otherPlayerNum].transform.position.x + -Mathf.Sign(GameManager.Instance.playersList[otherPlayerNum].transform.localScale.x) * 0.7f, transform.position.y, transform.position.z);




            // Dash knockback
            dashDirection = transform.localScale.x;
            actualUsedDashDistance = kickKnockbackDistance;
            initPos = transform.position;
            targetPos = transform.position + new Vector3(actualUsedDashDistance * dashDirection, 0, 0);
            dashTime = 0;
            isDashing = true;


            // FX
            kickKanasFX.Play();
            kickedFX.Play();
            GameManager.Instance.pommelCameraShake.shakeDuration = GameManager.Instance.pommelCameraShakeDuration;



            // AUDIO
            //audioManager.TriggerClashAudioCoroutine();
            AudioManager.Instance.BattleEventIncreaseIntensity();


            // STATS
            if (characterType == CharacterType.duel)
            {
                if (statsManager)
                    statsManager.AddAction(ACTION.successfulPommel, otherPlayerNum, chargeLevel);
                else
                    Debug.Log("Couldn't access statsManager to record action, ignoring");
            }
        }
    }
    #endregion








    #region CLASHED
    // The player have been clashed / parried
    [PunRPC]
    protected virtual void TriggerClash()
    {
        // STATE
        SwitchState(STATE.clashed);

        //NE PAS SUPPRIMER
        /*StopAllCoroutines();
        Debug.Log("Stop coroutines");*/
        GameManager.Instance.clashCameraShake.shakeDuration = GameManager.Instance.clashCameraShakeDuration;
        GameManager.Instance.TriggerSlowMoCoroutine(GameManager.Instance.clashSlowMoDuration, GameManager.Instance.clashSlowMoTimeScale, GameManager.Instance.clashTimeScaleFadeSpeed);


        // If is behind opponent when parried / clashed adds additional distance to evade the position and not look weird like they're fused together
        if (((transform.position.x - GameManager.Instance.playersList[otherPlayerNum].transform.position.x) * Mathf.Sign(transform.localScale.x)) <= 0.9f)
            transform.position = new Vector3(GameManager.Instance.playersList[otherPlayerNum].transform.position.x + -Mathf.Sign(GameManager.Instance.playersList[otherPlayerNum].transform.localScale.x) * 1.5f, transform.position.y, transform.position.z);



        // DASH CALCULATION
        temporaryDashDirectionForCalculation = transform.localScale.x;
        actualUsedDashDistance = clashKnockback;
        initPos = transform.position;

        targetPos = transform.position + new Vector3(actualUsedDashDistance * temporaryDashDirectionForCalculation, 0, 0);
        dashTime = 0;
        isDashing = true;


        // ANIMATION
        playerAnimations.ResetClashedTrigger();
        playerAnimations.CancelCharge(true);
        playerAnimations.TriggerClashed(true);


        // SOUND
        AudioManager.Instance.BattleEventIncreaseIntensity();


        // FX
        if (GameManager.Instance.playersList.Count > 1 && !GameManager.Instance.playersList[otherPlayerNum].GetComponent<Player>().clashKanasFX.isPlaying)
            clashKanasFX.Play();
    }
    #endregion








    #region DASH
    internal virtual void DashInput(float inDirection, bool quickDash)
    {
        switch (playerState)
        {
            case STATE.normal:
                break;
            case STATE.charging:
                break;
            case STATE.canAttackAfterAttack:
                break;
            case STATE.pommeling:
                break;
            case STATE.dashing:
                break;

            default:
                return;
        }

        if (quickDash)
        {
            if (Mathf.Abs(inDirection) < shortcutDashDeadZone && currentShortcutDashStep == DASHSTEP.invalidated)
                currentShortcutDashStep = DASHSTEP.rest;

            if (Mathf.Abs(inDirection) > shortcutDashDeadZone && currentShortcutDashStep == DASHSTEP.rest)
            {
                dashDirection = Mathf.Sign(inDirection);

                TriggerBasicDash();
                currentShortcutDashStep = DASHSTEP.invalidated;
            }
        }
        else
        {
            switch (currentDashStep)
            {
                case DASHSTEP.rest:
                    Debug.Log(inDirection);
                    temporaryDashDirectionForCalculation = inDirection;
                    dashInitializationStartTime = Time.time;
                    currentDashStep = DASHSTEP.firstInput;
                    break;

                case DASHSTEP.firstInput:
                    if (inDirection == 0f)
                    {
                        currentDashStep = DASHSTEP.firstRelease;
                        break;
                    }

                    if (Mathf.Sign(inDirection) != temporaryDashDirectionForCalculation)
                    {
                        currentDashStep = DASHSTEP.invalidated;
                        break;
                    }
                    break;

                case DASHSTEP.firstRelease:
                    if (temporaryDashDirectionForCalculation == Mathf.Sign(inDirection))
                    {
                        dashDirection = temporaryDashDirectionForCalculation;
                        currentDashStep = DASHSTEP.invalidated;
                        TriggerBasicDash();
                    }
                    else
                    {
                        currentDashStep = DASHSTEP.invalidated;
                    }
                    break;


                case DASHSTEP.invalidated:
                    if (inDirection == 0f)
                    {
                        Debug.Log("Resetting values");
                        currentDashStep = DASHSTEP.rest;
                    }
                    break;
            }
        }
    }

    // Functions to detect the dash input etc
    internal virtual void ManageDashInput()
    {
        if (currentDashStep == DASHSTEP.firstInput || currentDashStep == DASHSTEP.firstRelease)
            if (Time.time - dashInitializationStartTime > allowanceDurationForDoubleTapDash)
                currentDashStep = DASHSTEP.rest;

        if (InputManager.Instance.playerInputs[playerNum].dash == 0f)
        {
            if (currentDashStep == DASHSTEP.invalidated)
                currentDashStep = DASHSTEP.rest;

            if (currentShortcutDashStep == DASHSTEP.invalidated)
                currentShortcutDashStep = DASHSTEP.rest;
        }


        return;
    }

    // If the player collides with a wall
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
            targetPos = transform.position;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
            targetPos = transform.position;
    }

    // Triggers the dash (Not the clash or attack dash) for it to run
    [PunRPC]
    protected virtual void TriggerBasicDash()
    {
        // Triggers dash if enough stamina
        if (stamina >= staminaCostForMoves)
        {
            // CHANGE STATE
            SwitchState(STATE.dashing);


            // STAMINA
            StaminaCost(staminaCostForMoves, true);

            //NE PAS SUPPRIMER
            /* StopAllCoroutines();
             Debug.Log("Stop coroutines");*/
            dashTime = 0;


            if (dashDirection == -transform.localScale.x)
            {
                actualUsedDashDistance = forwardDashDistance;
                dashFXFront.Play();


                // STATS
                if (characterType == CharacterType.duel)
                {
                    if (statsManager)
                        statsManager.AddAction(ACTION.forwardDash, otherPlayerNum, chargeLevel);
                    else
                        Debug.Log("Couldn't access statsManager to record action, ignoring");
                }
            }
            else
            {
                actualUsedDashDistance = backwardsDashDistance;
                dashFXBack.Play();


                // STATS
                if (characterType == CharacterType.duel)
                {
                    if (statsManager)
                        statsManager.AddAction(ACTION.backwardsDash, otherPlayerNum, chargeLevel);
                    else
                        Debug.Log("Couldn't access statsManager to record action, ignoring");
                }
            }


            // ANIMATION
            playerAnimations.TriggerDash(dashDirection * transform.localScale.x);


            initPos = transform.position;
            targetPos = transform.position + new Vector3(actualUsedDashDistance * dashDirection, 0, 0);
            dashDirection = 0f;
        }


        // Stamina animation
        else
            TriggerNotEnoughStaminaAnim(true);
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
                dashTime += Time.deltaTime * baseDashSpeed;


            transform.position = Vector3.Lerp(initPos, targetPos, dashTime);


            if (dashTime >= 1.0f)
                EndDash();
        }
    }

    // End currently running dash
    void EndDash()
    {
        // CHANGE STATE
        if (playerState != STATE.attacking && playerState != STATE.recovering && playerState != STATE.onlinefrozen)
        {
            // ONLINE
            if (ConnectManager.Instance != null && ConnectManager.Instance.enableMultiplayer)
            {
                if (playerState == STATE.clashed && oldState == STATE.onlinefrozen)
                    SwitchState(STATE.onlinefrozen);
                else
                    SwitchState(STATE.normal);
            }
            else
                SwitchState(STATE.normal);
        }


        isDashing = false;


        // ANIMATION
        playerAnimations.TriggerClashed(false);
        //Debug.Log("End dash");


        // FX
        dashFXFront.Stop();
        dashFXBack.Stop();
        attackDashFXFront.Stop();
        attackDashFXBack.Stop();
    }
    #endregion







    #region ORIENTATION
    // ORIENTATION CALLED IN UPDATE
    public virtual void ManageOrientation()
    {
        if (photonView != null)
            if (!photonView.IsMine)
                return;


        // Orient towards the enemy if player can in their current state
        if (canOrientTowardsEnemy)
        {
            GameObject z = null, p1 = null, p2 = null;
            Vector3 self = Vector3.zero, other = Vector3.zero;
            Player[] stats = new Player[2];


            for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
            {
                if (GameManager.Instance.playersList[i] == null)
                    return;

                stats[i] = GameManager.Instance.playersList[i].GetComponent<Player>();
            }


            foreach (Player stat in stats)
            {
                if (stat == null)
                    return;


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


            if (p1 == null)
            {
                Debug.LogWarning("Player 1 not found");
                z = new GameObject();
                z.transform.position = Vector3.zero;
                p1 = z;
            }
            if (p2 == null)
            {
                Debug.LogWarning("Player 2 not found");
                z = new GameObject();
                z.transform.position = Vector3.zero;
                p2 = z;
            }


            if (p1 == gameObject)
            {
                self = p1.transform.position;
                other = p2.transform.position;
            }
            else if (p2 == gameObject)
            {
                self = p2.transform.position;
                other = p1.transform.position;
            }


            float sign = Mathf.Sign(self.x - other.x);
            Destroy(z);


            if (orientationCooldownFinished)
                ApplyOrientation(sign);
        }


        if (Time.time >= orientationCooldown + orientationCooldownStartTime)
            orientationCooldownFinished = true;
    }


    // Immediatly rotates the player
    protected void ApplyOrientation(float sign)
    {
        if (sign > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            UpdateNameScale(Mathf.Abs(characterNameDisplay.rectTransform.localScale.x));
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
            UpdateNameScale(-Mathf.Abs(characterNameDisplay.rectTransform.localScale.x));
        }


        orientationCooldownStartTime = Time.time;
        orientationCooldownFinished = false;
    }


    protected virtual void UpdateFXOrientation()
    {
        // FX
        Vector3 deathBloodFXRotation = deathBloodFX.gameObject.transform.localEulerAngles;

        /*if (GameManager.Instance.playersList[otherPlayerNum].transform.localScale.x >= 0)
        {
            deathBloodFX.gameObject.transform.localEulerAngles = new Vector3(deathBloodFXRotation.x, deathBloodFXRotation.y, -deathBloodFXRotationForDirectionChange * transform.localScale.x);


            // Changes the draw text indication's scale so that it's, well, readable for a human being
            drawText.transform.localScale = new Vector3(-drawTextBaseScale.x, drawTextBaseScale.y, drawTextBaseScale.z);

        }
        else
        {
            deathBloodFX.gameObject.transform.localEulerAngles = deathBloodFXBaseRotation;



            // Changes the draw text indication's scale so that it's, well, readable for a human being
            drawText.transform.localScale = new Vector3(drawTextBaseScale.x, drawTextBaseScale.y, drawTextBaseScale.z);
        }*/

        if (Mathf.Sign(GameManager.Instance.playersList[otherPlayerNum].transform.localScale.x) == Mathf.Sign(transform.localScale.x))
            deathBloodFX.gameObject.transform.localEulerAngles = new Vector3(deathBloodFXRotation.x, deathBloodFXRotation.y, -deathBloodFXRotationForDirectionChange * transform.localScale.x);
    }

    void UpdateNameScale(float newXScale)
    {
        characterNameDisplay.rectTransform.localScale = new Vector3(newXScale, characterNameDisplay.rectTransform.localScale.y, characterNameDisplay.rectTransform.localScale.z);
    }
    #endregion






    #region DRAW RANGE
    // Draw the attack range when the player is selected
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * (-lightAttackRange + baseBackAttackRangeDisjoint) / 2), transform.position.y, transform.position.z), new Vector3(lightAttackRange + baseBackAttackRangeDisjoint, 0.2f, 1));
        Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * -kickRange / 2), transform.position.y, transform.position.z), new Vector3(kickRange, 0.2f, 1));
    }


    // Draw the attack range is the attack is in active frames in the scene viewer
    private void OnDrawGizmos()
    {
        if (activeFrame)
            Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * (-actualAttackRange + baseBackAttackRangeDisjoint) / 2), transform.position.y, transform.position.z), new Vector3(actualAttackRange + baseBackAttackRangeDisjoint, 0.2f, 1));


        if (kickFrame)
            Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * -kickRange / 2), transform.position.y, transform.position.z), new Vector3(kickRange, 0.2f, 1));
    }
    #endregion





    // To set which particles are active depending of the terrain
    public void SetParticleSets(int index)
    {
        bool state = false;


        for (int y = 0; y < particlesSets.Count; y++)
        {
            if (y == index)
                state = true;
            else
                state = false;


            for (int o = 0; o < particlesSets[y].particleSystems.Count; o++)
                particlesSets[y].particleSystems[o].SetActive(state);
        }
    }








    #region CHEATS
    void CheatsInputs()
    {
        // CLASH
        if (Input.GetKeyDown(cheatSettings.clashCheatKey))
            TriggerClash();


        // DEATH
        if (Input.GetKeyDown(cheatSettings.deathCheatKey))
            TakeDamage(gameObject, 1);


        // STAMINA MAX
        if (Input.GetKeyDown(cheatSettings.staminaCheatKey))
            stamina = maxStamina;


        // REGEN STAMINA
        if (Input.GetKeyDown(cheatSettings.stopStaminaRegenCheatKey))
        {
            if (canRegenStamina)
                canRegenStamina = false;
            else
                canRegenStamina = true;
        }


        // RECUP STAMINA
        if (Input.GetKeyDown(cheatSettings.triggerStaminaRecupAnim))
            StartCoroutine(TriggerStaminaRecupAnim());
    }

    #endregion
    #endregion







    #region Network
    [PunRPC]
    public virtual void ResetPos()
    {
        netTargetPos = rb.position;
    }
    #endregion

    public void GetColliders()
    {
        Collider2D[] test = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < playerColliders.Length; i++)
        {
            if (test[i].CompareTag("Player"))
                playerColliders[i] = test[i];
        }
    }
}
