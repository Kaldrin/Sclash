using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    # region PLAYER'S COMPONENTS
    // PLAYER'S COMPONENTS
    [Header("PLAYER'S COMPONENTS")]
    [Tooltip("The reference to the player's Rigidbody component")]
    [SerializeField] Rigidbody2D rigid = null;
    [Tooltip("The reference to the player's Animators components for the character and their legs")]
    [SerializeField] Animator
        animator = null,
        legsAnimator = null;

    [Tooltip("The reference to the player's SpriteRenderers components for the character and their legs")]
    [SerializeField] public SpriteRenderer
        spriteRenderer = null,
        legsSpriteRenderer = null;

    PlayerAttack playerAttack = null;
    PlayerStats playerStats = null;
    PlayerMovement playerMovements = null;
    # endregion






    #region ANIMATION VALUES
    // ANIMATION VALUES
    [Header("ANIMATION VALUES")]
    [Tooltip("The minimum speed required for the walk anim to trigger")]
    [SerializeField] float minSpeedForWalkAnim = 0.05f;
    # endregion





    # region FX
    // FX
    [Header("FX")]
    [Tooltip("The reference to the TrailRenderer component of the saber")]
    [SerializeField] public TrailRenderer swordTrail = null;
    [Tooltip("The width of the attack trail depending on the range of the attack")]
    [SerializeField] float
        lightAttackSwordTrailWidth = 20f,
        heavyAttackSwordTrailWidth = 65f;

    [Tooltip("The colors of the attack trail depending on the range of the attack")]
    [SerializeField] Color
        lightAttackColor = Color.yellow,
        heavyAttackColor = Color.red;

    [SerializeField] public ParticleSystem slashFX = null;
    [Tooltip("The references to the particle systems components used for the walk leaves FX")]
    [SerializeField] ParticleSystem
        walkLeavesFront = null,
        walkLeavesBack = null;

    [Tooltip("The reference to the animator component of the game object containing the text telling the player to draw")]
    [SerializeField] public Animator drawTextAnimator = null;
    # endregion





    # region COLORS
    // COLORS
    [Header("COLORS")]
    [Tooltip("The opacity amount of the player's sprite when in untouchable frames")]
    [SerializeField] float untouchableFrameOpacity = 0.3f;

    [Tooltip("The reference to the light component which lits the player with their color")]
    [SerializeField] public Light playerLight = null;
    # endregion





    # region ANIMATION OBJECTS
    // ANIMATION OBJECTS
    [Header("ANIMATION OBJECTS")]
    [Tooltip("The reference to the game object containing the sprite mask of the player's legs")]
    [SerializeField] public GameObject legsMask = null;
    # endregion



















    // BASE FUNCTIONS
    private void Start()
    {
        //walkLeavesFront.Play();
    }

    void Awake()
    {
        // Get player's components
        playerAttack = GetComponent<PlayerAttack>();
        playerStats = GetComponent<PlayerStats>();
        playerMovements = GetComponent<PlayerMovement>();



        //canAttack = true;
    }

    // Update is called once per graphic frame
    void Update()
    {
        if (GameManager.Instance.gameStarted)
        {
            UpdateAnims();
        }
        UpdateFX();
        UpdateAnimStamina(playerStats.stamina);
        UpdateChargeWalk();
    }

    // FixedUpdate is called 30 times per second
    private void FixedUpdate()
    {
        ManageUntouchableFramesVisual();
    }






    // UPDATE VISUALS
    // Update the animator's parameters in Update
    void UpdateAnims()
    {

        animator.SetFloat("Level", playerAttack.chargeLevel - 1);
        animator.SetBool("Charging", playerAttack.charging);
        animator.SetBool("Dashing", playerAttack.isDashing);


        if ((rigid.velocity.x * -transform.localScale.x) < 0)
        {
            animator.SetFloat("WalkDirection", 0);
            legsAnimator.SetFloat("WalkDirection", 0);
        }
        else
        {
            animator.SetFloat("WalkDirection", 1);
            legsAnimator.SetFloat("WalkDirection", 1);
        }


        // If the player is in fact moving fast enough
        if (Mathf.Abs(rigid.velocity.x) > minSpeedForWalkAnim && GameManager.Instance.gameStarted)
        {
            animator.SetFloat("Move", Mathf.Abs(Mathf.Sign((Input.GetAxis("Horizontal" + playerStats.playerNum)))));


            if ((rigid.velocity.x * -transform.localScale.x) < 0)
            {
                walkLeavesFront.Stop();


                if (!walkLeavesBack.isPlaying)
                    walkLeavesBack.Play();
            }
            else
            {
                //Debug.Log("Leaves");
                if (!walkLeavesFront.isPlaying)
                    walkLeavesFront.Play();

                walkLeavesBack.Stop();
            }
        }
        // If the player isn't really moving
        else
        {
            animator.SetFloat("Move", 0);
            walkLeavesBack.Stop();
            walkLeavesFront.Stop();
        }
    }

    // Update FX's parameters in Update
    void UpdateFX()
    {
        //Debug.Log(playerAttack.actualAttackRange);
        swordTrail.startWidth = playerAttack.actualAttackRange;


        // Trail color and width depending on attack range
        if (playerAttack.chargeLevel == 1)
        {
            swordTrail.startColor = lightAttackColor;
            swordTrail.startWidth = lightAttackSwordTrailWidth;
        }
        else if (playerAttack.chargeLevel == playerAttack.maxChargeLevel)
        {
            swordTrail.startColor = heavyAttackColor;
            swordTrail.startWidth = heavyAttackSwordTrailWidth;
        }
        else
        {
            swordTrail.startColor = new Color(
                lightAttackColor.r + (heavyAttackColor.r - lightAttackColor.r) * ((float)playerAttack.actualAttackRange - playerAttack.lightAttackRange) / (float)playerAttack.heavyAttackRange,
                lightAttackColor.g + (heavyAttackColor.g - lightAttackColor.g) * ((float)playerAttack.actualAttackRange - playerAttack.lightAttackRange) / (float)playerAttack.heavyAttackRange,
                lightAttackColor.b + (heavyAttackColor.b - lightAttackColor.b) * ((float)playerAttack.actualAttackRange - playerAttack.lightAttackRange) / (float)playerAttack.heavyAttackRange);


            swordTrail.startWidth = lightAttackSwordTrailWidth + (heavyAttackSwordTrailWidth - lightAttackSwordTrailWidth) * ((float)playerAttack.actualAttackRange - playerAttack.lightAttackRange) / (float)playerAttack.heavyAttackRange;
        }
    }








    // STAMINA ANIMATIONS STATES
    // Updates the idle animation state of the player depending on its current stamina
    void UpdateAnimStamina(float stamina)
    {
        float blendTreeStamina = 0;


        if (stamina < playerStats.staminaCostForMoves)
            blendTreeStamina = 0;
        else
            blendTreeStamina = 1;


        animator.SetFloat("Stamina", blendTreeStamina);
    }





    // DRAW ANIMATION
    // Triggers the draw animation
    public void TriggerDraw()
    {
        animator.SetTrigger("Draw");
        drawTextAnimator.SetTrigger("Draw");
    }

    // Triggers the sneath animation
    public void TriggerSneath(bool reactivateDrawText)
    {
        animator.SetTrigger("Sneath");


        if (reactivateDrawText)
            drawTextAnimator.SetTrigger("ResetDraw");
    }



    // PARRY ANIMATION
    // Triggers on / off parry animation
    public void TriggerParry(bool state)
    {
        if (state)
        {
            animator.SetTrigger("ParryOn");
        }
        else
            animator.SetTrigger("ParryOff");
    }

    // Resets the parry animation triggers
    public void ResetParryTriggers()
    {
        animator.ResetTrigger("ParryOn");
        animator.ResetTrigger("ParryOff");
    }







    // KICK ANIMATION
    // Trigger on / off kick animation
    public void TriggerKick(bool state)
    {
        if (state)
            animator.SetTrigger("KickOn");
        else
            animator.SetTrigger("KickOff");
    }

    // Trigger on / off kicked animation
    public void TriggerKicked(bool state)
    {
        if (state)
            animator.SetTrigger("KickedOn");
        else
            animator.SetTrigger("KickedOff");
    }







    // CHARGE ANIMATIONS
    // Trigger charge animation
    public void TriggerCharge()
    {
        animator.SetTrigger("ChargeOn");
    }

    // Trigger max charge reached animation
    public void TriggerMaxCharge()
    {
        animator.SetTrigger("MaxCharge");
    }

    // Cancel charge animation
    public void CancelCharge()
    {
        animator.SetBool("Charging", false);
    }

    // Update charge walking anim
    void UpdateChargeWalk()
    {
        if (playerAttack.charging && Mathf.Abs(rigid.velocity.x) > minSpeedForWalkAnim && !playerStats.dead)
        {
            legsMask.SetActive(true);
        }
        else
        {
            legsMask.SetActive(false);
        }
    }










    // CLASHED ANIMATION
    // Trigger on / off clashed animation
    public void Clashed(bool state)
    {
        animator.SetBool("Clash", state);


        if (state)
            animator.SetTrigger("Clashed");
    }







    // DEATH ANIMATION
    // Trigger death animation
    public void Dead()
    {
        //animator.SetTrigger("Dead");
        animator.SetBool("Dead", true);
        animator.SetTrigger("DeathOn");
        slashFX.gameObject.SetActive(false);
        slashFX.gameObject.SetActive(true);
    }








    // ATTACK ANIMATIONS
    // Trigger attack depending on the intended direction
    public void TriggerAttack(float attackDir)
    {

        float attackDirection;


        if (attackDir == 1)
            attackDirection = 0f;
        else if (attackDir == -1)
            attackDirection = 1f;
        else
            attackDirection = 0.5f;


        //canAttack = false;
        animator.SetBool("Attack", true);
        animator.SetTrigger("AttackOn");
        animator.SetBool("Charging", false);

        animator.SetFloat("AttackDirection", attackDirection);
    }

    // Trigger attack's end animation
    public void EndAttack()
    {
        StartCoroutine(EndAttackCoroutine());
    }

    // Attack's end coroutine
    IEnumerator EndAttackCoroutine()
    {
        yield return new WaitForSeconds(0.05f);
        animator.SetBool("Attack", false);
        //canAttack = true;
    }










    // DASH ANIMATIONS
    // Trigger dash animation depending on dash intended direction
    public void Dash(float dashDirection)
    {
        float blendTreeValue = 0;


        if (dashDirection < 0)
            blendTreeValue = 0;
        else
            blendTreeValue = 1;


        animator.SetTrigger("DashOn");
        animator.SetFloat("DashDirection", blendTreeValue);
    }








    // UNTOUCHABLE FRAMES
    void ManageUntouchableFramesVisual()
    {
        if (playerStats.untouchable)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, untouchableFrameOpacity);
        }
        else
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        }
    }










    // RESET ANIMATION STATES
    public void ResetAnims()
    {
        animator.SetBool("Attack", false);
        //animator.ResetTrigger("Dead");
        animator.SetBool("Dead", false);
        animator.ResetTrigger("Clashed");
        animator.SetBool("Clash", false);
        animator.SetBool("Charging", false);
        animator.ResetTrigger("AttackOn");
        animator.SetBool("Parry", false);
    }
}
