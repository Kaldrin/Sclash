using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // AUDIO
    [SerializeField] string audioManagerName = "GlobalManager";
    AudioManager audioManager;





    // MANAGER
    [SerializeField] string gameManagerName = "GlobalManager";
    GameManager gameManager;







    // PLAYER'S COMPONENTS
    Rigidbody2D rb = null;
    PlayerStats playerStats = null;
    PlayerAnimations playerAnimations = null;
    PlayerMovement playerMovement = null;






    // ATTACK RECOVERY
    [SerializeField] float
        minRecoveryDuration = 0.4f,
        maxRecoveryDuration = 1;
    float
        attackRecoveryStartTime = 0,
        attackRecoveryDuration = 0;

    bool canCharge = true;
    [SerializeField] public bool attackRecoveryStart = false;
    [HideInInspector] public bool isAttackRecovering = false;







    // CHARGE
    [SerializeField] public int maxChargeLevel = 2;
    [HideInInspector] public int chargeLevel = 1;

    [SerializeField] float
        durationToNextChargeLevel = 1.5f,
        maxHoldDurationAtMaxCharge = 3;
    float
        maxChargeLevelStartTime = 0,
        chargeStartTime = 0;

    [HideInInspector] public bool charging = false;
    bool maxChargeLevelReached = false;






    // ATTACK
    [SerializeField] public float
        lightAttackRange = 1.5f,
        heavyAttackRange = 2.5f;
    [HideInInspector] public float actualAttackRange = 0;

    [SerializeField] public bool
        activeFrame = false,
        clashFrames = false;
    [HideInInspector] public bool isAttacking = false;









    // OTHER PLAYER
    [HideInInspector] public bool enemyDead = false;









    // KICK
    [SerializeField] bool kickFrame = false;
    bool
        kicking = false,
        canKick = true;

    [SerializeField] float
        kickRange = 1.3f,
        kickDuration = 0.2f;






    // KICKED
    [SerializeField] float
        kickKnockbackDistance = 1f,
        kickedStaminaLoss = 1;

    bool kicked = false;









    // PARRY
    [SerializeField] public bool parrying = false;
    bool canParry = true;

    [SerializeField] float parryDuration = 0.4f;
    





    // CLASH
    bool clashed = false;
    [SerializeField] float clashKnockback = 2;






    // DASH
    [HideInInspector] public bool isDashing;
    [SerializeField] public Collider2D playerCollider;

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
        dashSpeed,
        forwardDashDistance,
        backwardsDashDistance;
    [SerializeField] float
        dashAllowanceDuration = 0.3f,
        forwardAttackDashDistance = 3,
        backwardsAttackDashDistance = 3,
        dashDeadZone = 0.5f,
        shortcutDashDeadZone = 0.5f;

    Vector3 initPos;
    Vector3 targetPos;






    // CHEATS FOR DEVELOPMENT PURPOSES
    [SerializeField] bool cheatCodes = false;
    [SerializeField] KeyCode
        clashCheatKey = KeyCode.Alpha1,
        deathCheatKey = KeyCode.Alpha2;











    // BASE FUNCTIONS
    void Start()
    {
        // Get audio manager to use in the script
        audioManager = GameObject.Find(audioManagerName).GetComponent<AudioManager>();


        // Get game manager to use in the script
        gameManager = GameObject.Find(gameManagerName).GetComponent<GameManager>();


        // Get player's components to use in the script
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();
        playerAnimations = GetComponent<PlayerAnimations>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // The player can only use actions when the game has started
        if (gameManager.gameStarted)
        {
            // The player cna only use actions if they are not dead
            if (!playerStats.dead)
            {
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


                // KICK
                ManageKick();
            }


            // RECOVERY TIME MANAGING
            ManageRecoveries();
        }


        // Cheatcodes to use for development purposes
        if (cheatCodes)
            Cheats();
    }

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
        if (isDashing)
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
            // Activate recovery
            //attackRecovery = true;
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







    //CHARGE
    void ManageCharge()
    {
        //Player presses attack button
        if (Input.GetButtonDown("Fire" + playerStats.playerNum) || Input.GetAxis("Fire" + playerStats.playerNum) > 0.1)
        {
            if (canCharge && !isAttackRecovering && !isAttacking && !charging && playerStats.stamina >= playerStats.staminaCostForMoves && !kicking && !parrying)
            {
                charging = true;
                canCharge = false;

                chargeStartTime = Time.time;
                playerAnimations.TriggerCharge();
                playerMovement.Charging(true);
                
            }
        }


        //Player releases attack button
        if (!Input.GetButton("Fire" + playerStats.playerNum) && Input.GetAxis("Fire" + playerStats.playerNum) < 0.1)
        {
            canCharge = true;

            if (charging)
                ReleaseAttack();
        }

        
        //When the player is charging the attack
        if (charging)
        {
            /*
            // If the player runs out of stamina while charging
            if (playerStats.stamina < playerStats.staminaCostForMoves)
            {
                ReleaseAttack();
            }
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
                    Debug.Log("Charge up");
                    //playerAnimations.ChargeChange(chargeLevel);
                }
                else if (chargeLevel >= maxChargeLevel)
                {
                    chargeLevel = maxChargeLevel;
                    maxChargeLevelStartTime = Time.time;
                    maxChargeLevelReached = true;

                    playerAnimations.TriggerMaxCharge();
                }
            }



        }
    }







    // ATTACK
    // Triggers the attck
    void ReleaseAttack()
    {
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


        // Check if player has enough remaining stamina and attack if so
        if (playerStats.stamina >= playerStats.staminaCostForMoves)
        {
            if (Mathf.Sign(Input.GetAxis("Horizontal" + playerStats.playerNum)) == -Mathf.Sign(transform.localScale.x))
                actualDashDistance = forwardAttackDashDistance;
            else
                actualDashDistance = backwardsAttackDashDistance;



            //actualDashDistance = forwardAttackDashDistance;
            float direction = Mathf.Sign(Input.GetAxis("Horizontal" + playerStats.playerNum)) * transform.localScale.x;



            Vector3 dir = new Vector3(0, 0, 0);
            if (Mathf.Abs(Input.GetAxis("Horizontal" + playerStats.playerNum)) > 0.2)
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
        }
        else
        {
            playerAnimations.CancelCharge();
        }
    }

    // Hits with a phantom collider to apply the attack's damage during active frames
    void ApplyDamage()
    {
        Collider2D[] hitsCol = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (transform.localScale.x * - actualAttackRange / 2), transform.position.y), new Vector2(lightAttackRange, 1), 0);
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
                enemyDead = g.GetComponent<PlayerStats>().TakeDamage(gameObject, chargeLevel);
        }
    }

    // Draw the attack range when the player is selected
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * - actualAttackRange / 2), transform.position.y, transform.position.z), new Vector3(actualAttackRange, 1, 1));
        Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * - kickRange / 2), transform.position.y, transform.position.z), new Vector3(kickRange, 1, 1));
    }

    // Draw the attack range is the attack is in active frames in the scene viewer
    private void OnDrawGizmos()
    {
        if (activeFrame)
            Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * -actualAttackRange / 2), transform.position.y, transform.position.z), new Vector3(actualAttackRange, 1, 1));

        if (kickFrame)
            Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * - kickRange / 2), transform.position.y, transform.position.z), new Vector3(kickRange, 1, 1));
    }









    // PARRY
    // Detect parry inputs
    void ManageParry()
    {
        if ((!Input.GetButton("Parry" + playerStats.playerNum) && Input.GetAxis("Parry" + playerStats.playerNum) > -0.1f))
        {
            canParry = true;
        }

        if ((Input.GetButtonDown("Parry" + playerStats.playerNum) || Input.GetAxis("Parry" + playerStats.playerNum) < -0.1) && !parrying && gameManager.gameStarted && !isAttacking && !isAttackRecovering && canParry && !kicking)
        {
            Parry();
        }
    }
        
    // Start the parry coroutine
    public void Parry()
    {
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
        if ((Input.GetButtonDown("Fire" + playerStats.playerNum) && Input.GetButtonDown("Parry" + playerStats.playerNum)) || Input.GetButtonDown("Kick" + playerStats.playerNum))
        {
            if (canKick && !kicked && !isAttacking && !activeFrame && !kicking && !parrying)
                Kick();
        }
    }

    // Start the kick coroutine
    void Kick()
    {
        StartCoroutine(KickCoroutine());
    }

    // Kick coroutine
    IEnumerator KickCoroutine()
    {
        playerAnimations.TriggerKick(true);
        kicking = true;
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
                    Debug.Log("Kick");
                }
                    
            }
        }
    }









    // KICKED
    public void Kicked()
    {
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

        dashDirection = transform.localScale.x;
        actualDashDistance = kickKnockbackDistance;

        initPos = transform.position;
        targetPos = transform.position + new Vector3(actualDashDistance * dashDirection, 0, 0);

        playerAnimations.CancelCharge();
        playerAnimations.Clashed(clashed);

        playerStats.StaminaCost(kickedStaminaLoss);

        // Sound
        audioManager.Clash();
    }






    //CLASH
    public void Clash()
    {
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

        initPos = transform.position;
        targetPos = transform.position + new Vector3(actualDashDistance * tempDashDirection, 0, 0);

        playerAnimations.CancelCharge();
        playerAnimations.Clashed(clashed);

        // Sound
        audioManager.Clash();
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
            else if (dashStep == 2 && dashDirection == tempDashDirection && playerStats.stamina >= playerStats.staminaCostForMoves && !isAttacking && !activeFrame && !parrying)
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
            time = 0;
            isDashing = false;
            //gameObject.layer = normalPlayerLayer;
            playerCollider.isTrigger = false;


            // If the player was clashed / countered and has finished their knockback
            if (clashed)
            {
                clashed = false;
                playerAnimations.Clashed(clashed);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            time = 0;
            isDashing = false;
            //gameObject.layer = normalPlayerLayer;
            playerCollider.isTrigger = false;


            // If the player was clashed / countered and has finished their knockback
            if (clashed)
            {
                clashed = false;
                playerAnimations.Clashed(clashed);
            }
        }
    }

    // Triggers the dash (Not the clash or attack dash) for it to run
    void TriggerBasicDash()
    {
        dashStep = 3;
        shortcutDashStep = -1;
        chargeLevel = 1;
        time = 0;

        charging = false;
        isDashing = true;
        isAttackRecovering = false;
        maxChargeLevelReached = false;


        if (dashDirection == - transform.localScale.x)
        {
            actualDashDistance = forwardDashDistance;
        }
        else
        {
            actualDashDistance = backwardsDashDistance;
        }
           

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
        }

 
        initPos = transform.position;
        targetPos = transform.position + new Vector3(actualDashDistance * dashDirection, 0, 0);
    }

    // Runs the dash, to use in FixedUpdate
    void RunDash()
    {
        playerCollider.isTrigger = true;

        time += Time.deltaTime * dashSpeed;
        transform.position = Vector3.Lerp(initPos, targetPos, time);
        if (time >= 1.0f)
        {
            time = 0;
            isDashing = false;

            // Player can cross up, collider is deactivated during dash but it can still be hit
            playerCollider.isTrigger = false;


            // If the player was clashed / countered and has finished their knockback
            if (clashed)
            {
                clashed = false;
                playerAnimations.Clashed(clashed);
            }

            if (kicked)
            {
                kicked = false;
                playerAnimations.TriggerKicked(false);
            }
        }
    }  
}
