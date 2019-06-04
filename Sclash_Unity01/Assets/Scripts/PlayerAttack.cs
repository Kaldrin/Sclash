using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{ 
    // AUDIO
    [SerializeField] string audioManagerName = "GlobalManager";
    AudioManager audioManager;


    // COMPONENTS
    Rigidbody2D rb;
    PlayerStats playerStats;
    PlayerAnimations playerAnimations;
    PlayerMovement playerMovement;



    // TAP
    float lastTap;
    float tapTime = 0.25f;
    bool tapping;



    // ATTACK RECOVERY
    [SerializeField] float minRecoveryDuration = 0.4f;
    [SerializeField] float maxRecoveryDuration = 1;
    bool attackRecovery = false;
    float attackRecoveryStartTime;
    float attackRecoveryDuration;



    // CHARGE & ATTACK
    [SerializeField] float durationToNextChargeLevel = 1.5f;
    [SerializeField] float maxHoldDurationAtMaxCharge = 3;
    float maxChargeLevelStartTime;
    bool maxChargeLevelReached = false;
    float chargeStartTime;
    [HideInInspector] public int chargeLevel = 1;
    [SerializeField] int maxChargeLevel = 2;

    [HideInInspector] public bool charging = false;
    bool triggerAttack = false;
    [SerializeField] public bool activeFrame;

    

    // PARRY
    [HideInInspector] public bool parrying;
    [SerializeField] float parryDuration = 0.4f;


    // CLASH
    bool clashed = true;
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
    [Range(1.0f, 10.0f)] public float dashDistance;
    [SerializeField] float attackDashDistance = 3;
    float actualDashDistance;
    [HideInInspector] public bool isDashing;
    Vector3 initPos;
    Vector3 targetPos;
    float time;
    [SerializeField] float dashDeadZone = 0.5f;


    void Start()
    {
        // Get audio
        audioManager = GameObject.Find(audioManagerName).GetComponent<AudioManager>();

        // Get components
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();
        playerAnimations = GetComponent<PlayerAnimations>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (playerStats.stamina > 0)
        {
            // ATTACK
            ManageCharge();

            // DASH
            ManageDash();
        }

        // RECOVERY
        ManageRecoveries();
    }

    void FixedUpdate()
    {
        if (activeFrame)
        {
            ApplyDamage();
        }


        if (triggerAttack)
        {
            Vector3 dir = new Vector3(0, 0, 0);
            if (Mathf.Abs(Input.GetAxis("Horizontal" + playerStats.playerNum)) > 0.2)
                dir = new Vector3(dashDirection, 0, 0);

            dir *= dashDistance;

            initPos = transform.position;
            targetPos = transform.position + dir;
            targetPos.y = transform.position.y;

            time = 0.0f;
            rb.velocity = Vector3.zero;

            isDashing = true;
            rb.gravityScale = 0f;

            //rb.AddForce((Vector2)dir * dashMultiplier, ForceMode2D.Impulse);

            triggerAttack = false;
        }




        if (isDashing)
        {
            time += Time.deltaTime * dashSpeed;
            transform.position = Vector3.Lerp(initPos, targetPos, time);
            if (time >= 1.0f)
            {
                time = 0;
                isDashing = false;


                // If the player was clashed / countered and has finished their knockback
                if (clashed)
                {
                    clashed = false;
                    playerAnimations.Clashed(clashed);
                }
            }
        }
    }







    // RECOVERIES
    void ManageRecoveries()
    {
        if (attackRecovery)
        {
            if (Time.time - attackRecoveryStartTime >= attackRecoveryDuration)
            {
                attackRecovery = false;
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
                    //playerAnimations.ChargeChange(chargeLevel);
                    Debug.Log(chargeLevel);
                }
                else if (chargeLevel >= maxChargeLevel)
                {
                    chargeLevel = maxChargeLevel;
                    maxChargeLevelStartTime = Time.time;
                    maxChargeLevelReached = true;

                    playerAnimations.TriggerMaxCharge();
                    Debug.Log(charging);
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


        // Check if player has enough remaining stamina
        if (playerStats.stamina >= playerStats.staminaCostForMoves)
        {
            actualDashDistance = attackDashDistance;
            triggerAttack = true;
            playerAnimations.TriggerAttack();
            //ApplyDamage();

            playerStats.stamina -= playerStats.staminaCostForMoves;



            // Activate recovery
            attackRecovery = true;
            attackRecoveryStartTime = Time.time;
            attackRecoveryDuration = minRecoveryDuration + (maxRecoveryDuration - minRecoveryDuration) * ((float)chargeLevel - 1) / (float)maxChargeLevel;



            // Sound
            audioManager.Attack(chargeLevel, maxChargeLevel);
        }
        else
        {
            playerAnimations.CancelCharge();
        }
        

       
        chargeLevel = 1;
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


        actualDashDistance = attackDashDistance;

        
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
                actualDashDistance = dashDistance;
                isDashing = true;

                // Animation
                playerAnimations.Dash(tempDashDirection * transform.localScale.x);

                playerStats.stamina -= playerStats.staminaCostForMoves;

                

                initPos = transform.position;
                targetPos = transform.position + new Vector3(dashDistance * tempDashDirection, 0, 0);
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
        triggerAttack = false;
        activeFrame = false;

        parrying = false;

        tempDashDirection = transform.localScale.x;
        actualDashDistance = clashKnockback;
        isDashing = true;
        initPos = transform.position;
        targetPos = transform.position + new Vector3(dashDistance * tempDashDirection, 0, 0);


        clashed = true;
        playerAnimations.CancelCharge();
        playerAnimations.Clashed(clashed);

        // Sound
        audioManager.Clash();
    }





    // Hits with a phantom collider
    void ApplyDamage()
    {
        Collider2D[] hitsCol = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (transform.localScale.x * -1), transform.position.y), new Vector2(1, 1), 0);
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
            g.GetComponent<PlayerStats>().TakeDamage(gameObject, chargeLevel);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * -1), transform.position.y, transform.position.z), new Vector3(1, 1, 1));
    }

    private void OnDrawGizmos()
    {
        if (activeFrame)
            Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * -1), transform.position.y, transform.position.z), new Vector3(1, 1, 1));
    }
}
