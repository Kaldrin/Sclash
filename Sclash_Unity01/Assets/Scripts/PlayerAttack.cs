using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{ 
    // AUDIO
    [SerializeField] string audioManagerName = "GlobalManager";
    AudioManager audioManager;



    // MANAGER
    GameManager gameManager;
    [SerializeField] string gameManagerName = "GlobalManager";




    // COMPONENTS
    Rigidbody2D rb = null;
    PlayerStats playerStats = null;
    PlayerAnimations playerAnimations = null;
    PlayerMovement playerMovement = null;



    // TAP
    // float lastTap = 0;
    // float tapTime = 0.25f;
    // bool tapping = false;





    // ATTACK RECOVERY
    [SerializeField] float minRecoveryDuration = 0.4f;
    [SerializeField] float maxRecoveryDuration = 1;
    [SerializeField] public bool attackRecoveryStart = false;
    [HideInInspector] public bool attackRecovery = false;
    float attackRecoveryStartTime = 0;
    float attackRecoveryDuration = 0;




    
    // CHARGE
    [SerializeField] float durationToNextChargeLevel = 1.5f;
    [SerializeField] float maxHoldDurationAtMaxCharge = 3;
    float maxChargeLevelStartTime = 0;
    bool maxChargeLevelReached = false;
    float chargeStartTime = 0;
    [HideInInspector] public int chargeLevel = 1;
    [SerializeField] int maxChargeLevel = 2;
    [HideInInspector] public bool charging = false;





    // ATTACK
    //bool triggerAttack = false;
    [SerializeField] public bool
        activeFrame = false,
        clashFrames = false;
    //[SerializeField] bool attackDone;
    [SerializeField] float lightAttackRange = 1.5f;





    // OTHER PLAYER
    [HideInInspector] public bool enemyDead = false;

    



    // PARRY
    [HideInInspector] public bool parrying = false;
    [SerializeField] float parryDuration = 0.4f;






    // CLASH
    bool clashed = false;
    [SerializeField] float clashKnockback = 2;






    // DASH
    int dashStep = 0;
    //The current step to the dash
    // 0 = Nothing done
    // 1 = Player pressed the direction 1 time
    // 2 = Player released the direction after pressing it
    // 3 = Player pressed again the same direction after releasing it, dash
    float dashDirection;
    float tempDashDirection;
    float dashInitStartTime = 0;
    [SerializeField] float dashAllowanceDuration = 0.3f;
    [Range(1.0f, 10.0f)] public float dashSpeed;
    [Range(1.0f, 10.0f)] public float forwardDashDistance;
    [Range(1.0f, 10.0f)] public float backwardsDashDistance;
    [SerializeField] float forwardAttackDashDistance = 3;
    [SerializeField] float backwardsAttackDashDistance = 3;
    float actualDashDistance;
    [HideInInspector] public bool isDashing;
    Vector3 initPos;
    Vector3 targetPos;
    float time;
    [SerializeField] float dashDeadZone = 0.5f;
    [SerializeField] public Collider2D playerCollider;
    /*
    [SerializeField] int normalPlayerLayer = 8;
    [SerializeField] int dashPhantomPlayerLayer = 10;
    */


    // CHEAT
    [SerializeField] bool cheatCodes = false;
    [SerializeField] KeyCode clashCheatKey = KeyCode.Alpha1;
    [SerializeField] KeyCode deathCheatKey = KeyCode.Alpha2;





    void Start()
    {
        // Get audio
        audioManager = GameObject.Find(audioManagerName).GetComponent<AudioManager>();


        // Get manager
        gameManager = GameObject.Find(gameManagerName).GetComponent<GameManager>();


        // Get components
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();
        playerAnimations = GetComponent<PlayerAnimations>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (gameManager.gameStarted)
        {
            
            if (!playerStats.dead)
            {
                if (playerStats.stamina > 0)
                {
                    // ATTACK
                    if (!clashed)
                        ManageCharge();

                    // DASH
                    if (!clashed)
                        ManageDash();
                }
            }

            // RECOVERY
            ManageRecoveries();
        }
        
        

        

        if (cheatCodes)
            cheats();
    }

    void FixedUpdate()
    {
        if (activeFrame)
        {
            ApplyDamage();
        }



        // DASH
        if (isDashing)
        {
            //gameObject.layer = dashPhantomPlayerLayer;

            // Player can cross up, collider is deactivated during dash but it can still be hit
            playerCollider.isTrigger = true;

            time += Time.deltaTime * dashSpeed;
            transform.position = Vector3.Lerp(initPos, targetPos, time);
            if (time >= 1.0f)
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
    }






    // CHEATS
    void cheats()
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
        if (attackRecoveryStart)
        {
            attackRecovery = true;
            attackRecoveryStart = false;
            // Activate recovery
            //attackRecovery = true;
            attackRecoveryStartTime = Time.time;
            attackRecoveryDuration = minRecoveryDuration + (maxRecoveryDuration - minRecoveryDuration) * ((float)chargeLevel - 1) / (float)maxChargeLevel;
            chargeLevel = 1;
        }
        

        if (attackRecovery)
        {
            if (Time.time - attackRecoveryStartTime >= attackRecoveryDuration)
            {
                attackRecovery = false;
                playerAnimations.EndAttack();
            }
        }
    }








    //CHARGE & ATTACK
    void ManageCharge()
    {

        //Player presses parry button while charging
        if (Input.GetButtonDown("Parry" + playerStats.playerNum) && charging)
        {
            Parry();
        }


        //Player presses attack button
        if (Input.GetButtonDown("Fire" + playerStats.playerNum) && !attackRecovery)
        {
            charging = true;
            chargeStartTime = Time.time;
            playerAnimations.TriggerCharge();
            playerMovement.Charging(true);
        }


        //Player releases attack button
        if (Input.GetButtonUp("Fire" + playerStats.playerNum) && charging)
        {
            ReleaseAttack();
        }


        //When the player is charging the attack
        if (charging)
        {
            // If the player runs out of stamina while charging
            if (playerStats.stamina < playerStats.staminaCostForMoves)
            {
                ReleaseAttack();
            }


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

    void ReleaseAttack()
    {
        // Charge
        playerMovement.Charging(false);
        charging = false;
        
        maxChargeLevelReached = false;
        //playerAnimations.TriggerCharge();

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




            /*
            // Sound
            audioManager.Attack(chargeLevel, maxChargeLevel);
            */
        }
        else
        {
            playerAnimations.CancelCharge();
            
        }
    }








    // PARRY
    // Starts the parry coroutine
    public void Parry()
    {
        StartCoroutine(ParryC());
    }

    IEnumerator ParryC()
    {
        parrying = true;
        
        playerMovement.Charging(false);
        charging = false;
        chargeLevel = 1;
        maxChargeLevelReached = false;



        playerAnimations.CancelCharge();
        //playerAnimations.Parry(true);
        playerAnimations.TriggerParry();


        actualDashDistance = forwardAttackDashDistance;

        
        // Sound
        audioManager.ParryOn();
        


        yield return new WaitForSeconds(parryDuration);

        parrying = false;
        //playerAnimations.Parry(parrying);
    }










    //DASH
    void ManageDash()
    {
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
            else if (dashStep == 2 && dashDirection == tempDashDirection && playerStats.stamina >= playerStats.staminaCostForMoves)
            {
                dashStep = 3;

                if (Mathf.Sign(dashDirection) == -Mathf.Sign(transform.localScale.x))
                    actualDashDistance = forwardDashDistance;
                else
                    actualDashDistance = backwardsDashDistance;

                //actualDashDistance = dashDistance;
                isDashing = true;

                // Animation
                playerAnimations.Dash(tempDashDirection * transform.localScale.x);

                // STAMINA COST
                playerStats.StaminaCost(playerStats.staminaCostForMoves);

                // CANCEL ATTACK
                charging = false;
                chargeLevel = 1;
                maxChargeLevelReached = false;
                playerMovement.Charging(false);
                //triggerAttack = false;
                attackRecovery = false;

                

                // If the player was clashed / countered and has finished their knockback
                if (clashed)
                {
                    clashed = false;
                    playerAnimations.Clashed(clashed);
                    time = 0;
                    //gameObject.layer = normalPlayerLayer;
                    playerCollider.isTrigger = false;
                }



                initPos = transform.position;
                targetPos = transform.position + new Vector3(actualDashDistance * tempDashDirection, 0, 0);
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

    /*
    IEnumerator SingleTap()
    {
        yield return new WaitForSeconds(tapTime);
        if (tapping)
        {
            Debug.Log("Single Tap");
            tapping = false;
            dashDirection = 0;
        }
    }
    */








    //CLASH

    public void Clash()
    {
        charging = false;
        chargeLevel = 1;
        playerAnimations.CancelCharge();

        attackRecovery = false;
        //triggerAttack = false;
        activeFrame = false;

        parrying = false;

        tempDashDirection = transform.localScale.x;
        actualDashDistance = clashKnockback;
        isDashing = true;
        initPos = transform.position;
        targetPos = transform.position + new Vector3(actualDashDistance * tempDashDirection, 0, 0);


        clashed = true;
        playerAnimations.CancelCharge();
        playerAnimations.Clashed(clashed);

        // Sound
        audioManager.Clash();
    }





    // Hits with a phantom collider
    void ApplyDamage()
    {
        Collider2D[] hitsCol = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (transform.localScale.x * -lightAttackRange / 2), transform.position.y), new Vector2(lightAttackRange, 1), 0);
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

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * -lightAttackRange / 2), transform.position.y, transform.position.z), new Vector3(lightAttackRange, 1, 1));
    }

    private void OnDrawGizmos()
    {
        if (activeFrame)
            Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * - lightAttackRange / 2), transform.position.y, transform.position.z), new Vector3(lightAttackRange, 1, 1));
    }
}
