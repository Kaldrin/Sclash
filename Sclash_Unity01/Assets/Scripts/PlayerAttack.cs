using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    # region MANAGERS
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
    [SerializeField] PlayerStats playerStats = null;
    [SerializeField] PlayerAnimations playerAnimations = null;
    [SerializeField] PlayerMovement playerMovement = null;
    #endregion






    # region ATTACK
    // ATTACK
    [Header("ATTACK")]
    [Tooltip("Attack range parameters")]
    [SerializeField] public float lightAttackRange = 1.8f;
    [Tooltip("Attack range parameters")]
    [SerializeField] public float
        heavyAttackRange = 3.2f,
        attackRangeDisjoint = 0.2f;
    [HideInInspector] public float actualAttackRange = 0;

    [Tooltip("Frame parameters for the attack")]
    [SerializeField] public bool
        activeFrame = false,
        clashFrames = false;
    [HideInInspector] public bool isAttacking = false;
    # endregion






    # region ATTACK RECOVERY
    // ATTACK RECOVERY
    [Header("ATTACK RECOVERY")]
    [Tooltip("The attack lagg duration at the lowest charge level")]
    [SerializeField] float minRecoveryDuration = 0.1f;
    [Tooltip("The attack lagg duration at the max charge level")]
    [SerializeField] float maxRecoveryDuration = 0.6f;
    float
        attackRecoveryStartTime = 0,
        attackRecoveryDuration = 0;

    [Tooltip("Has the attack lagg started for this player ?")]
    [SerializeField] public bool attackRecoveryStart = false;
    [HideInInspector] public bool isAttackRecovering = false;
    # endregion







    # region CHARGE
    // CHARGE
    [Header("CHARGE")]
    [Tooltip("The number of charge levels for the attack, so the number of range subdivisions")]
    [SerializeField] public int maxChargeLevel = 4;
    [HideInInspector] public int chargeLevel = 1;
    //[SerializeField] int numberOfFramesToDetectChargeInput = 3;
    int currentChargeFramesPressed = 0;

    [Tooltip("Charge duration parameters")]
    [SerializeField] float
        durationToNextChargeLevel = 0.7f,
        maxHoldDurationAtMaxCharge = 2;
    float
        maxChargeLevelStartTime = 0,
        chargeStartTime = 0;

    [HideInInspector] public bool charging = false;
    bool maxChargeLevelReached = false;
    [HideInInspector] public bool canCharge = true;
    # endregion







    # region DRAW
    // DRAW
    [Header("DRAW")]
    [Tooltip("The duration the draw animation takes to switch to drawn state")]
    [SerializeField] public float drawDuration = 2f;

    [Tooltip("Is the player currently drawing during these frames ?")]
    [SerializeField] bool isDrawing = false;
    [Tooltip("Has the player drawn their saber ?")]
    [SerializeField] public bool hasDrawn = false;

    [Tooltip("The reference to the game object containing the text component telling the players to draw their sabers")]
    [SerializeField] public GameObject drawText = null;
    Vector3 drawTextScale = new Vector3(0, 0, 0);
    # endregion








    # region OTHER PLAYER
    // OTHER PLAYER
    [Header("OTHER PLAYER")]
    [HideInInspector] public bool enemyDead = false;
    # endregion









    # region KICK
    // KICK
    [Header("KICK")]
    [Tooltip("Is currently applying the pommel effect to what they touches ?")]
    [SerializeField] public bool kickFrame = false;
    [HideInInspector] public bool
        kicking = false,
        canKick = true;

    [SerializeField] float
        kickRange = 0.88f,
        kickDuration = 0.5f;
    # endregion







    # region KICKED
    // KICKED
    [Header("KICKED")]
    [Tooltip("The distance the player will be pushed on when pommeled")]
    [SerializeField] float kickKnockbackDistance = 1f;
    [Tooltip("The amount of stamina the player will lose when pommeled during an attack or parry")]
    [SerializeField] float kickedStaminaLoss = 1;

    [HideInInspector] public bool kicked = false;
    # endregion









    # region PARRY
    // PARRY
    [Header("PARRY")]
    [Tooltip("Is currently parrying any attack that will touch them ?")]
    [SerializeField] public bool parrying = false;
    [HideInInspector] public bool canParry = true;

    [Tooltip("How much time will last the parry state")]
    [SerializeField] float parryDuration = 0.3f;
    
    //[SerializeField] int numberOfFramesToDetectParryInput = 3;
    int currentParryFramesPressed = 0;
    # endregion







    # region CLASH
    // CLASH
    [Header("CLASH")]
    [Tooltip("The distance the player will be pushed on when clashed")]
    [SerializeField] float clashKnockback = 2;
    [Tooltip("The speed at which the knockback distance will be covered")]
    [SerializeField] public float clashKnockbackSpeed = 2;
    bool clashed = false;
    # endregion







    #region DASH
    // DASH
    [Header("DASH")]
    [Tooltip("The basic collider of the player")]
    [SerializeField] public Collider2D playerCollider;
    [Tooltip("All of the player's 2D colliders")]
    [SerializeField] public Collider2D[] playerColliders = null;
    [HideInInspector] public bool isDashing;

    int dashStep = -1;
        //The current step of the dash input
        // - 1 = Player was pressing the direction for too long / nothing
        // 0 = Player wasn't pressing the direction
        // 1 = Player pressed the direction 1 time
        // 2 = Player released the direction after pressing it
        // 3 = Player pressed again the same direction after releasing it, executes dash
    int shortcutDashStep = - 1;
        //The current step of the shortcut dash input
        // -1 = Player was already pressing the direction
        // 0 = Player wasn't pressing the direction
        // 1 = Player pressed the direction after releasing it, executes dash

    float
        dashDirection,
        tempDashDirection,
        dashInitStartTime = 0,
        actualDashDistance,
        time;
    [SerializeField] public float
        dashSpeed = 3,
        forwardDashDistance = 3,
        backwardsDashDistance = 2.5f;
    [SerializeField] float
        dashAllowanceDuration = 0.3f,
        forwardAttackDashDistance = 2.5f,
        backwardsAttackDashDistance = 1.5f,
        dashDeadZone = 0.5f,
        shortcutDashDeadZone = 0.5f;

    Vector3 initPos;
    Vector3 targetPos;
    # endregion





    # region FX
    // FX
    [Header("FX")]
    [Tooltip("The attack sign FX object reference, the one that spawns at the range distance before the attack hits")]
    [SerializeField] public GameObject attackSign = null;
    [SerializeField] public GameObject
        clash = null,
        clashKana = null,
        kickKanas = null,
        kickedFX = null,
        chargeFullFX = null;
    [Tooltip("The katana charging FX game object reference")]
    [SerializeField] public GameObject chargeFX = null;

    [Tooltip("The slider component reference to move the charging FX on the katana")]
    [SerializeField] Slider chargeSlider = null;

    [SerializeField] ParticleSystem
        dashLeavesFront = null,
        dashLeavesBack = null,
        attackDashLeavesFront = null,
        attackDashLeavesBack = null,
        attackNeutralLeaves = null;

    [SerializeField] float attackSignDisjoint = 0.4f;
    # endregion





    # region CHEATS FOR DEVELOPMENT PURPOSES
    // CHEATS FOR DEVELOPMENT PURPOSES
    [Header("CHEATS")]
    [Tooltip("The cheat key to trigger a clash for the player")]
    [SerializeField] KeyCode clashCheatKey = KeyCode.Alpha1;
    [Tooltip("The other cheat keys for other effects")]
    [SerializeField] KeyCode
        deathCheatKey = KeyCode.Alpha2,
        drawCheatKey = KeyCode.Alpha3,
        staminaCheatKey = KeyCode.Alpha4;
    # endregion




















    // BASE FUNCTIONS
    void Start()
    {
        // Get audio manager to use in the script
        audioManager = GameObject.Find(audioManagerName).GetComponent<AudioManager>();

        // Get game manager to use in the script
        gameManager = GameObject.Find(gameManagerName).GetComponent<GameManager>();

        // Get input manager
        inputManager = GameObject.Find(inputManagerName).GetComponent<InputManager>();



        drawTextScale = drawText.transform.localScale;
    }

    // Update is called once per graphic frame
    void Update()
    {
        // The player can only use actions when the game has started
        if (gameManager.gameStarted)
        {
            if (!gameManager.paused)
            {
                // The player cna only use actions if they are not dead
                if (!playerStats.dead && !enemyDead && hasDrawn)
                {
                    // KICK
                    ManageKick();


                    if (playerStats.stamina >= playerStats.staminaCostForMoves)
                    {
                        // CHARGE & ATTAQUE
                        if (!clashed)
                            ManageCharge();


                        // DASH INPUTS
                        if (!clashed)
                            ManageDash();


                        // PARRY INPUT
                        ManageParry();
                    }
                }
                else if (!hasDrawn && gameManager.gameStarted && !enemyDead)
                {
                    ManageDraw();
                }
            }


            /*
            if (!hasDrawn && !isDrawing)
                drawText.SetActive(true);
            else
                drawText.SetActive(false);
            */

            // Changes the draw text indication's scale so that it's, well, readable for a human being
            drawText.transform.localScale = new Vector3(drawTextScale.x * transform.localScale.x, drawTextScale.y, drawTextScale.z);


            // RECOVERY TIME MANAGING
            ManageRecoveries();
        }


        // Cheatcodes to use for development purposes
        if (gameManager.cheatCodes)
            Cheats();
    }

    // FixedUpdate is called 30 times per second
    void FixedUpdate()
    {
        // Apply damages if the current attack animation has entered active frame, thus activating the bool in the animation
        if (activeFrame)
        {
            ApplyDamage();
        }
        else if (kickFrame)
        {
            ApplyKick();
        }


        // DASH FUNCTIONS
        if (isDashing && !gameManager.paused)
        {
            RunDash();
        }
    }







    // CHEATS
    void Cheats()
    {
        if (Input.GetKeyDown(clashCheatKey))
        {
            Clash();
        }


        if (Input.GetKeyDown(deathCheatKey))
        {
            playerStats.TakeDamage(gameObject, 1);
        }


        if (Input.GetKeyDown(drawCheatKey))
        {
            hasDrawn = true;
        }

        if (Input.GetKeyDown(staminaCheatKey))
        {
            playerStats.stamina = playerStats.maxStamina;
        }
    }







    // RECOVERIES
    void ManageRecoveries()
    {
        // If the animation of the attack has reached the recovery starting point, it turns on the the attackRecoveryStart bool for a frame, which triggers the recovery time calculation and run
        if (attackRecoveryStart)
        {
            isAttackRecovering = true;
            attackRecoveryStart = false;
            isAttacking = false;
            attackRecoveryStartTime = Time.time;
            attackRecoveryDuration = minRecoveryDuration + (maxRecoveryDuration - minRecoveryDuration) * ((float)chargeLevel - 1) / (float)maxChargeLevel;
            chargeLevel = 1;
        }
        

        if (isAttackRecovering)
        {
            if (Time.time - attackRecoveryStartTime >= attackRecoveryDuration)
            {
                isAttackRecovering = false;
                playerAnimations.EndAttack();
            }
        }
    }








    // DRAW
    // Detects draw input
    void ManageDraw()
    {
        if (!hasDrawn)
        {
            if (inputManager.playerInputs[playerStats.playerNum - 1].anyKeyDown)
            {   
                if (!isDrawing)
                {
                    StartCoroutine(Draw());
                }   
            }
        }
    }

    // Triggers saber draw and informs the game manager
    IEnumerator Draw()
    {
        isDrawing = true;
        playerAnimations.TriggerDraw();
        gameManager.SaberDrawn(playerStats.playerNum);
        yield return new WaitForSecondsRealtime(drawDuration);
        hasDrawn = true;
        isDrawing = false;
    }








    //CHARGE
    // Manages the detection of attack charge inputs
    void ManageCharge()
    {
        //Player presses attack button
        if (Input.GetButtonDown("Fire" + playerStats.playerNum) || Input.GetAxis("Fire" + playerStats.playerNum) > 0.1)
        {
            currentChargeFramesPressed++;

            
            //Debug.Log(isAttacking);
            if (canCharge && !isAttackRecovering && !isAttacking && !charging && playerStats.stamina >= playerStats.staminaCostForMoves && !kicking && !parrying && !clashed && !isDrawing)
            {
                charging = true;
                chargeFX.SetActive(true);
                canCharge = false;
                isAttackRecovering = false;
                currentChargeFramesPressed = 0;

                chargeStartTime = Time.time;
                playerAnimations.TriggerCharge();
                playerMovement.Charging(true);
            }
            else
            {
            }
        }


        //Player releases attack button
        if (!Input.GetButton("Fire" + playerStats.playerNum) && Input.GetAxis("Fire" + playerStats.playerNum) < 0.1)
        {
            canCharge = true;

            
            if (charging)
            {
                ReleaseAttack();
            }
        }

        
        //When the player is charging the attack
        if (charging)
        {
            
            
            // Charge slider
            
            /*
            if (chargeSlider.value > 0)
                chargeSlider.value -= chargeFXFillingSpeed;
                */
            


            // If the player has wiated too long charging
            if (maxChargeLevelReached)
            {
                if (Time.time - maxChargeLevelStartTime >= maxHoldDurationAtMaxCharge)
                {
                    ReleaseAttack();
                }
            }
            else if (Time.time - chargeStartTime >= durationToNextChargeLevel)
            {
                chargeStartTime = Time.time;

                if (chargeLevel < maxChargeLevel)
                {
                    chargeLevel++;
                    chargeSlider.value = chargeSlider.maxValue - (chargeSlider.maxValue / maxChargeLevel) * chargeLevel;
                    chargeFullFX.SetActive(false);
                    chargeFullFX.SetActive(true);
                }
                else if (chargeLevel >= maxChargeLevel)
                {
                    // FX
                    chargeFullFX.SetActive(false);
                    chargeFullFX.SetActive(true);
                    chargeFX.SetActive(false);

                    chargeSlider.value = 0;
                    chargeLevel = maxChargeLevel;
                    maxChargeLevelStartTime = Time.time;
                    maxChargeLevelReached = true;
                    playerAnimations.TriggerMaxCharge();
                }
            }
        }
        else
        {
            chargeFX.SetActive(false);
            chargeSlider.value = 1;
        }
    }






    // ATTACK
    // Triggers the attck
    void ReleaseAttack()
    {
        StopAllCoroutines();


        // Charge
        playerMovement.Charging(false);
        charging = false;
        isAttacking = true;
        maxChargeLevelReached = false;


        // Calculate attack range depending on level of charge
        if (chargeLevel == 1)
            actualAttackRange = lightAttackRange;
        else if (chargeLevel == maxChargeLevel)
            actualAttackRange = heavyAttackRange;
        else
            actualAttackRange = lightAttackRange + (heavyAttackRange - lightAttackRange) * ((float)chargeLevel - 1) / (float)maxChargeLevel;

        Vector3 attackSignPos = attackSign.transform.localPosition;
        attackSign.transform.localPosition = new Vector3(- (actualAttackRange + attackSignDisjoint), attackSignPos.y, attackSignPos.z);



        // Dash direction and distance
        if (Mathf.Sign(Input.GetAxis("Horizontal" + playerStats.playerNum)) == -Mathf.Sign(transform.localScale.x))
        {
            actualDashDistance = forwardAttackDashDistance;


            // FX
            if (Mathf.Abs(inputManager.playerInputs[playerStats.playerNum - 1].horizontal) > 0.2f)
                attackDashLeavesFront.Play();
            else
                attackNeutralLeaves.Play();
        }
        else
        {
            actualDashDistance = backwardsAttackDashDistance;


            // FX
            if (Mathf.Abs(inputManager.playerInputs[playerStats.playerNum - 1].horizontal) > 0.2f)
                attackDashLeavesBack.Play();
            else
                attackNeutralLeaves.Play();
        }


        float direction = 0;


        if (Mathf.Abs(inputManager.playerInputs[playerStats.playerNum - 1].horizontal) > 0.2f)
            direction = Mathf.Sign(inputManager.playerInputs[playerStats.playerNum - 1].horizontal) * transform.localScale.x;
        

        Vector3 dir = new Vector3(0, 0, 0);


        if (Mathf.Abs(inputManager.playerInputs[playerStats.playerNum - 1].horizontal) > 0.2)
            dir = new Vector3(Mathf.Sign(Input.GetAxis("Horizontal" + playerStats.playerNum)), 0, 0);


        dir *= actualDashDistance;
        initPos = transform.position;
        targetPos = transform.position + dir;
        targetPos.y = transform.position.y;
        time = 0.0f;
        rb.velocity = Vector3.zero;
        isDashing = true;
        rb.gravityScale = 0f;


        if (Mathf.Abs(Input.GetAxis("Horizontal" + playerStats.playerNum)) > 0.1f)
        {
            playerAnimations.TriggerAttack(direction);
        }
        else
        {
            playerAnimations.TriggerAttack(0);
        }


        // STAMINA COST
        playerStats.StaminaCost(playerStats.staminaCostForMoves);

        StartCoroutine(AttackFailSafe());

    }

    IEnumerator AttackFailSafe()
    {
        yield return new WaitForSeconds(1); //Adapter la durée en fonction de celle de l'animation d'attaque
        isAttacking = false;
    }

    // Hits with a phantom collider to apply the attack's damage during active frames
    void ApplyDamage()
    {
        Collider2D[] hitsCol = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (transform.localScale.x * (- actualAttackRange + attackRangeDisjoint) / 2), transform.position.y), new Vector2(actualAttackRange + attackRangeDisjoint, 1), 0);
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
                enemyDead = g.GetComponent<PlayerStats>().TakeDamage(gameObject, chargeLevel);
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









    // PARRY
    // Detect parry inputs
    void ManageParry()
    {
        if ((!Input.GetButtonDown("Parry" + playerStats.playerNum) && Input.GetAxis("Parry" + playerStats.playerNum) > -0.1f))
        {
            canParry = true;
            currentParryFramesPressed = 0;
        }


        if ((Input.GetButtonDown("Parry" + playerStats.playerNum) || Input.GetAxis("Parry" + playerStats.playerNum) < -0.1))
        {
            currentParryFramesPressed++;


            if (!parrying && gameManager.gameStarted && !isAttacking && !isAttackRecovering && canParry && !kicking && !kicked && !clashed)
            {
                Parry();
                currentParryFramesPressed = 0;
            }
        }
    }
        
    // Start the parry coroutine
    public void Parry()
    {
        //StopAllCoroutines();
        StartCoroutine(ParryCoroutine());
    }

    // Parry coroutine
    IEnumerator ParryCoroutine()
    {
        // Cancel charge
        if (charging)
        {
            playerMovement.Charging(false);
            charging = false;
            chargeLevel = 1;
            maxChargeLevelReached = false;
            playerAnimations.CancelCharge();
        }


        isAttacking = false;
        canParry = false;


        // Stamina cost
        playerStats.StaminaCost(playerStats.staminaCostForMoves);
        playerAnimations.TriggerParry(true);


        yield return new WaitForSeconds(parryDuration);


        playerAnimations.TriggerParry(false);
    }









    // KICK
    // Detect kick inputs
    void ManageKick()
    {
        if (!inputManager.playerInputs[playerStats.playerNum - 1].kick)
        {
            canKick = true;
        }
        
   
        if (inputManager.playerInputs[playerStats.playerNum - 1].kickDown)
        {
            if (canKick && !kicked && !isAttacking && !activeFrame && !kicking && !parrying && !clashed)
            {
                Kick();
            }
        }     
    }

    // Start the kick coroutine
    void Kick()
    {
        StopAllCoroutines();
        StartCoroutine(KickCoroutine());
    }

    // Kick coroutine
    IEnumerator KickCoroutine()
    {
        if (charging)
        {
            playerMovement.Charging(false);
            charging = false;
            chargeLevel = 1;
            maxChargeLevelReached = false;
            playerAnimations.CancelCharge();
        }


        kicking = true;
        parrying = false;
        isAttacking = false;
        playerStats.PauseStaminaRegen();

        //activeKickInput = 0;
        playerAnimations.TriggerKick(true);
        yield return new WaitForSeconds(kickDuration);

        playerAnimations.TriggerKick(false);
        kicking = false;
    }

    // Apply kick hitbox depending on kick frames
    void ApplyKick()
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
                if (!g.GetComponent<PlayerAttack>().kicked)
                {
                    g.GetComponent<PlayerAttack>().Kicked();
                }  
            }
        }
    }









    // KICKED
    // The player have been kicked
    public void Kicked()
    {
        if (!kickFrame)
        {
            


            // Stamina
            if (parrying || isAttacking)
                playerStats.StaminaCost(kickedStaminaLoss);



            // FX
            kickKanas.SetActive(false);
            kickedFX.SetActive(false);
            kickedFX.SetActive(true);
            kickKanas.SetActive(true);


            // States
            charging = false;
            isAttackRecovering = false;
            isAttacking = false;
            canCharge = false;
            activeFrame = false;
            parrying = false;
            kicking = false;
            isDashing = true;
            clashed = true;
            kicked = true;

            chargeLevel = 1;

            StopAllCoroutines();


            // Dash knockback
            dashDirection = transform.localScale.x;
            actualDashDistance = kickKnockbackDistance;
            initPos = transform.position;
            targetPos = transform.position + new Vector3(actualDashDistance * dashDirection, 0, 0);


            // Animation
            playerAnimations.CancelCharge();
            playerAnimations.Clashed(clashed);

            

            // Sound
            audioManager.Clash();
            audioManager.BattleEventIncreaseIntensity();
        }
    }






    //CLASHED
    // Start the clash coroutine
    public void Clash()
    {
        StartCoroutine(ClashCoroutine());
    }

    // The player have been clashed / parried
    IEnumerator ClashCoroutine()
    {
        clashKana.SetActive(false);
        clashKana.SetActive(true);
        gameManager.SlowMo(gameManager.clashSlowMoDuration, gameManager.clashSlowMoTimeScale, gameManager.clashTimeScaleFadeSpeed);


        charging = false;
        isAttackRecovering = false;
        isAttacking = false;
        canCharge = true;
        activeFrame = false;
        parrying = false;
        kicking = false;
        isDashing = true;
        clashed = true;

        chargeLevel = 1;

        tempDashDirection = transform.localScale.x;
        actualDashDistance = clashKnockback;

        StopAllCoroutines();
        initPos = transform.position;
        targetPos = transform.position + new Vector3(actualDashDistance * tempDashDirection, 0, 0);

        playerAnimations.CancelCharge();
        playerAnimations.Clashed(clashed);


        // Sound
        audioManager.Clash();
        audioManager.BattleEventIncreaseIntensity();


        yield return new WaitForSeconds(0f);
    }

    IEnumerator EndClashed()
    {
        yield return new WaitForSeconds(0.2f);

        clashed = false;
        playerAnimations.Clashed(clashed);
    }








    //DASH
    // Functions to detect the dash input etc
    void ManageDash()
    {
        // Detects dash with basic input rather than double tap, shortcut
        if (Mathf.Abs(Input.GetAxisRaw("Dash" + playerStats.playerNum)) < shortcutDashDeadZone && shortcutDashStep == -1)
        {
            shortcutDashStep = 0;
        }
        

        if (Mathf.Abs(Input.GetAxisRaw("Dash" + playerStats.playerNum)) > shortcutDashDeadZone && shortcutDashStep == 0 && !isAttacking && !activeFrame && !parrying)
        {
            dashDirection = Mathf.Sign(Input.GetAxisRaw("Dash" + playerStats.playerNum));

            TriggerBasicDash();
        }


        // Resets the dash input if too much time has passed
        if (dashStep == 1 || dashStep == 2)
        {
            if (Time.time - dashInitStartTime > dashAllowanceDuration)
            {
                dashStep = -1;
            }
        }


        //The player needs to let go the input before pressing it again to dash
        if (Mathf.Abs(Input.GetAxis("Horizontal" + playerStats.playerNum)) < dashDeadZone)
        {
            if (dashStep == 1)
            {
                dashStep = 2;
            }
            else if (dashStep == 3)
            {
                dashStep = -1;
            }
            // To make the first dash input he must have not been pressing it before, we need a double tap
            else if (dashStep == -1)
            {
                dashStep = 0;
            }

        }


        //When the player presses the directiosn
        if (Mathf.Abs(Input.GetAxis("Horizontal" + playerStats.playerNum)) > dashDeadZone)
        {
            tempDashDirection = Mathf.Sign(Input.GetAxis("Horizontal" + playerStats.playerNum));

            if (dashStep == 0)
            {
                dashStep = 1;
                dashDirection = tempDashDirection;
                dashInitStartTime = Time.time;

            }
            // Dash is validated, the player is gonna dash
            else if (dashStep == 2 && dashDirection == tempDashDirection && playerStats.stamina >= playerStats.staminaCostForMoves && !isAttacking && !activeFrame)
            {
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
        StopAllCoroutines();


        dashStep = 3;
        shortcutDashStep = -1;
        chargeLevel = 1;
        time = 0;

        charging = false;
        isDashing = true;
        parrying = false;
        canKick = true;
        kicking = false;
        isAttackRecovering = false;
        maxChargeLevelReached = false;


        if (dashDirection == - transform.localScale.x)
        {
            actualDashDistance = forwardDashDistance;
            dashLeavesFront.Play();
        }
        else
        {
            actualDashDistance = backwardsDashDistance;
            dashLeavesBack.Play();
        }


        playerAnimations.ResetParryTriggers();
        playerAnimations.Dash(dashDirection * transform.localScale.x);
        playerStats.StaminaCost(playerStats.staminaCostForMoves);
        playerMovement.Charging(false);
        

        // If the player was clashed / countered and has finished their knockback
        if (clashed || kicked)
        {
            clashed = false;
            kicked = false;
            playerAnimations.Clashed(clashed);
            time = 0;


            playerCollider.isTrigger = false;


            for (int i = 0; i < playerColliders.Length; i++)
            {
                playerColliders[i].isTrigger = false;
            }
        }

 
        initPos = transform.position;
        targetPos = transform.position + new Vector3(actualDashDistance * dashDirection, 0, 0);
    }

    // Runs the dash, to use in FixedUpdate
    void RunDash()
    {
        playerCollider.isTrigger = true;


        for (int i = 0; i < playerColliders.Length; i++)
        {
            playerColliders[i].isTrigger = true;
        }


        if (clashed)
            time += Time.deltaTime * clashKnockbackSpeed;
        else
            time += Time.fixedUnscaledDeltaTime * dashSpeed;


        transform.position = Vector3.Lerp(initPos, targetPos, time);


        if (time >= 1.0f)
        {
            EndDash();
        }
    } 
    
    // End currently running dash
    void EndDash()
    {
        time = 0;
        isDashing = false;

        // Player can cross up, collider is deactivated during dash but it can still be hit
        for (int i = 0; i < playerColliders.Length; i++)
        {
            playerColliders[i].isTrigger = false;
        }


        playerCollider.isTrigger = false;


        // If the player was clashed / countered and has finished their knockback
        if (clashed)
        {
            StartCoroutine(EndClashed());
        }

        if (kicked)
        {
            kicked = false;
            playerAnimations.TriggerKicked(false);
        }


        // FX
        dashLeavesFront.Stop();
        dashLeavesBack.Stop();
        attackDashLeavesFront.Stop();
        attackDashLeavesBack.Stop();
    }
}
