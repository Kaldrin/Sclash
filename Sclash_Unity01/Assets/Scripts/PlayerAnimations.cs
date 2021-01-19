using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;


// MANAGES ALL ANIMATIONS OF THE PLAYER
// COULD BE MORE OPTIMIZED PROBABLY ?
public class PlayerAnimations : MonoBehaviourPunCallbacks
{
    #region VARIABLES
    #region PLAYER'S COMPONENTS
    [Header("PLAYER'S COMPONENTS")]
    [Tooltip("The reference to the player's Rigidbody component")]
    [SerializeField] Rigidbody2D rigid = null;

    [Tooltip("The reference to the player's Animators components for the character and their legs")]
    [SerializeField] public Animator animator = null;
    [SerializeField]
    Animator legsAnimator2 = null;
    //legsAnimator = null,
    [Tooltip("The reference to the animator component of the game object containing the text telling the player to draw")]
    [SerializeField] public Animator nameDisplayAnimator = null;

    [Tooltip("The reference to the player's SpriteRenderers components for the character and their legs")]
    [SerializeField]
    public SpriteRenderer
        spriteRenderer,
        legsSpriteRenderer = null;

    [SerializeField] Player playerScript = null;
    # endregion



    #region ANIMATION VALUES
    [Header("ANIMATION VALUES")]
    [Tooltip("The minimum speed required for the walk anim to trigger")]
    [SerializeField] float minSpeedForWalkAnim = 0.05f;
    [HideInInspector]
    public float
        animatorBaseSpeed,
        legsAnimatorBaseSpeed = 0;

    float nextAttackState = 0;
    #endregion



    #region ANIMATOR PARAMETERS
    [Header("PLAYER ANIMATOR PARAMETERS")]
    //[SerializeField] string Walk = "Walk";
    [SerializeField]
    string
        playerWalkDirection = "WalkDirection";
    [SerializeField]
    string moving = "Moving",
        stamina = "Stamina",
        attackOn = "AttackOn",
        maxCharge = "MaxCharge",
        chargeOn = "ChargeOn",
        chargeOff = "ChargeOff",
        attackDirection = "AttackDirection",
        parryOn = "ParryOn",
        perfectParry = "PerfectParry",
        //maintainParry = "MaintainParry",
        maintainParryOn = "MaintainParryOn",
        maintainParryOff = "MaintainParryOff",
        clashedOn = "ClashedOn",
        clashedOff = "ClashedOff",
        deathOn = "DeathOn",
        reallyDead = "ReallyDead",
        dead = "Dead",
        dashOn = "DashOn",
        dashing = "Dashing",
        dashDirection = "DashDirection",
        pommelOn = "PommelOn",
        pommeledOn = "PommeledOn",
        draw = "Draw",
        sneath = "Sneath",
        battleSneath = "BattleSneath",
        battleDraw = "BattleDraw",
        jump = "Jump",
        land = "Land",
        verticalSpeed = "VerticalSpeed",
        attackState = "AttackState";

    [Header("LEG ANIMATOR PARAMETERS")]
    [SerializeField] string legsWalkDirection = "WalkDirection";
    #endregion
    #endregion





















    #region FUNCTIONS
    #region BASE FUNCTIONS
    private void Start()
    {
        animatorBaseSpeed = animator.speed;
    }

    void FixedUpdate()
    {
        if (photonView != null)
            if (!photonView.IsMine)
                return;


        if (InputManager.Instance.playerInputs.Length > playerScript.playerNum)
            UpdateAnims(InputManager.Instance.playerInputs[playerScript.playerNum].horizontal);

        UpdateWalkDirection();
        UpdateIdleStateDependingOnStamina(playerScript.stamina);
        animator.SetFloat(verticalSpeed, rigid.velocity.y);
    }
    #endregion






    #region UPDATE VISUALS
    // Update the animator's parameters in Update
    void UpdateAnims(float horizontal)
    {
        if (playerScript.playerState == Player.STATE.charging && Mathf.Abs(rigid.velocity.x) > minSpeedForWalkAnim)
        {
            legsAnimator2.gameObject.SetActive(true);
            legsAnimator2.SetFloat("AttackState", nextAttackState);
        }
        else
            legsAnimator2.gameObject.SetActive(false);



        // DASHING STATE
        animator.SetBool(dashing, playerScript.playerState == Player.STATE.dashing);


        // WALK ANIM
        // If the player is in fact moving fast enough
        if (Mathf.Abs(rigid.velocity.x) > minSpeedForWalkAnim)
        {
            if (ConnectManager.Instance.enableMultiplayer)
                animator.SetFloat(moving, Mathf.Abs(Mathf.Sign(InputManager.Instance.playerInputs[0].horizontal)));
            else
            {
                animator.SetFloat(moving, Mathf.Abs(Mathf.Sign(horizontal)));


                // Walk anim speed depending on speed
                if (playerScript.playerState == Player.STATE.normal && !playerScript.playerIsAI)
                    animator.speed = Mathf.Abs(horizontal);
                else
                    animator.speed = animatorBaseSpeed;

                if (playerScript.playerState == Player.STATE.charging && !playerScript.playerIsAI)
                    legsAnimator2.speed = Mathf.Abs(horizontal);
                else
                    legsAnimator2.speed = 1;
            }
        }
        // If the player isn't really moving
        else
        {
            animator.SetFloat(moving, 0);

            if (!playerScript.playerIsAI)
                animator.speed = 1;
        }
    }
    # endregion





    #region RESET ANIMATION STATES
    public void ResetAnimsForNextRound()
    {
        ResetWalkDirecton();


        animator.SetFloat(moving, 0);
        animator.SetFloat(stamina, 1f);
        animator.SetFloat(dashDirection, 0f);


        animator.SetFloat(attackDirection, 0f);
        animator.ResetTrigger(parryOn);
        animator.ResetTrigger(clashedOn);
        animator.ResetTrigger(clashedOff);
        animator.ResetTrigger(deathOn);
        animator.ResetTrigger(attackOn);
        animator.ResetTrigger(maxCharge);
        animator.ResetTrigger(chargeOn);
        animator.ResetTrigger(chargeOff);
        animator.ResetTrigger(pommelOn);
        animator.ResetTrigger(pommeledOn);
        animator.ResetTrigger(draw);
        animator.ResetTrigger(sneath);
        animator.ResetTrigger(dashOn);
        ResetBattleSneath();
        ResetAllJumpParameters();
        ResetMaintainParry();


        animator.SetBool(dead, false);
        animator.SetBool(dashing, false);
    }

    public void ResetAnimsForNextMatch()
    {
        ResetWalkDirecton();


        animator.SetFloat(moving, 0);
        animator.SetFloat(stamina, 1f);
        animator.SetFloat(dashDirection, 0f);


        animator.SetFloat(attackDirection, 0f);
        animator.ResetTrigger(parryOn);
        animator.ResetTrigger(clashedOn);
        animator.ResetTrigger(clashedOff);
        animator.ResetTrigger(deathOn);
        animator.ResetTrigger(attackOn);
        animator.ResetTrigger(maxCharge);
        animator.ResetTrigger(chargeOn);
        animator.ResetTrigger(chargeOff);
        animator.ResetTrigger(pommelOn);
        animator.ResetTrigger(pommeledOn);
        animator.ResetTrigger(draw);
        animator.ResetTrigger(sneath);
        animator.ResetTrigger(dashOn);
        ResetBattleSneath();
        ResetAllJumpParameters();
        ResetMaintainParry();


        animator.SetBool(dead, false);
        animator.SetBool(dashing, false);


        animator.speed = 1;
        TriggerSneath();
    }
    #endregion



    # region WALK DIRECTION
    void UpdateWalkDirection()
    {
        try
        {
            if ((rigid.velocity.x * -transform.localScale.x) < 0)
            {
                animator.SetFloat(playerWalkDirection, 0);

                if (legsAnimator2.isActiveAndEnabled)
                    legsAnimator2.SetFloat(legsWalkDirection, 0);
            }
            else
            {
                animator.SetFloat(playerWalkDirection, 1);

                if (legsAnimator2.isActiveAndEnabled && playerScript.playerState == Player.STATE.charging)
                    legsAnimator2.SetFloat(legsWalkDirection, 1);
            }
        }
        catch { }
    }


    // RESET
    void ResetWalkDirecton()
    {
        try
        {
            if (legsAnimator2.enabled && legsAnimator2.gameObject.activeInHierarchy)
                legsAnimator2.SetFloat(legsWalkDirection, 0);


            animator.SetFloat(playerWalkDirection, 0f);
        }
        catch { }
    }
    #endregion









    #region IDLE STAMINA STATE / EXHAUSTED
    // Updates the idle animation state of the player depending on its current stamina
    public void UpdateIdleStateDependingOnStamina(float playerStamina)
    {
        float blendTreeStamina = 0;


        if (playerStamina < playerScript.staminaCostForMoves)
            blendTreeStamina = 0;
        else
            blendTreeStamina = 1;


        animator.SetFloat(stamina, blendTreeStamina);
    }
    # endregion




    #region DRAW ANIMATION
    // Triggers the draw animation
    public void TriggerDraw()
    {
        animator.SetTrigger(draw);
    }

    // Triggers the sneath animation
    public void TriggerSneath()
    {
        animator.SetTrigger(sneath);
    }
    # endregion




    #region BATTLE SNEATH / DRAW ANIMATIONS
    public void TriggerBattleSneath()
    {
        animator.SetTrigger(battleSneath);
        animator.ResetTrigger(battleDraw);
    }

    public void TriggerBattleDraw()
    {
        animator.SetTrigger(battleDraw);
        animator.ResetTrigger(battleSneath);
    }

    public void ResetBattleSneath()
    {
        animator.ResetTrigger(battleSneath);
        animator.ResetTrigger(battleDraw);
    }
    # endregion




    # region PARRY ANIMATION
    // Triggers on / off parry animation
    public void TriggerParry()
    {
        animator.SetTrigger(parryOn);
    }

    // Resets the parry animation triggers
    public void ResetParry()
    {
        animator.ResetTrigger(parryOn);
    }

    public void TriggerPerfectParry()
    {
        animator.SetTrigger(perfectParry);
    }

    // Resets the parry animation triggers
    public void ResetPerfectParry()
    {
        animator.ResetTrigger(perfectParry);
    }
    #endregion




    # region MAINTAIN PARRY ANIMATION
    // Triggers on / off parry animation
    public void TriggerMaintainParry()
    {
        animator.SetTrigger(maintainParryOn);
    }

    // Resets the parry animation triggers
    public void EndMaintainParry()
    {
        //animator.SetBool(maintainParry, false);
        animator.SetTrigger(maintainParryOff);
    }

    public void ResetMaintainParry()
    {
        animator.ResetTrigger(maintainParryOn);
        animator.ResetTrigger(maintainParryOff);
    }
    #endregion




    # region POMMEL ANIMATION
    // Trigger on pommel animation
    public void TriggerPommel()
    {
        animator.SetTrigger(pommelOn);
    }

    public void ResetPommelTrigger()
    {
        animator.ResetTrigger(pommelOn);
    }

    // Trigger on pommeled animation
    public void TriggerPommeled()
    {
        animator.SetTrigger(pommeledOn);
    }

    public void ResetPommeledTrigger()
    {
        animator.ResetTrigger(pommeledOn);
    }
    #endregion POMMEL ANIMATION





    # region CHARGE ANIMATIONS
    // Trigger charge animation
    public void TriggerCharge(bool state)
    {
        animator.SetFloat(attackState, nextAttackState);


        if (state)
            animator.SetTrigger(chargeOn);
        else
            animator.ResetTrigger(chargeOn);
    }

    // Trigger max charge reached animation
    public void TriggerMaxCharge()
    {
        animator.SetTrigger(maxCharge);
    }

    // Cancel charge animation
    public void CancelCharge(bool state)
    {
        if (state)
            animator.SetTrigger(chargeOff);
        else
            animator.ResetTrigger(chargeOff);
    }
    # endregion





    # region  CLASHED ANIMATIONS
    // Trigger on clashed animation
    public void TriggerClashed(bool state)
    {
        if (state)
            animator.SetTrigger(clashedOn);
        else
            animator.SetTrigger(clashedOff);
    }

    public void ResetClashedTrigger()
    {
        animator.ResetTrigger(clashedOn);
        animator.ResetTrigger(clashedOff);
    }
    # endregion





    # region DEATH ANIMATION
    // Trigger death animation
    public void TriggerDeath()
    {
        animator.SetTrigger(deathOn);
    }

    public void DeathActivated(bool state)
    {
        animator.SetBool(dead, state);
    }

    public void TriggerRealDeath()
    {
        animator.SetTrigger(reallyDead);
    }
    # endregion





    # region ATTACK ANIMATIONS
    // Trigger attack depending on the intended direction
    public void TriggerAttack(float attackDir)
    {
        if (nextAttackState == 0)
            nextAttackState = 1;
        else
            nextAttackState = 0;


        float tempAttackDirectionStock;


        if (attackDir == 1)
            tempAttackDirectionStock = 0f;
        else if (attackDir == -1)
            tempAttackDirectionStock = 1f;
        else
            tempAttackDirectionStock = 0.5f;


        animator.SetTrigger(attackOn);
        animator.SetFloat(attackDirection, tempAttackDirectionStock);
    }
    # endregion





    # region DASH ANIMATIONS
    // Trigger dash animation depending on dash intended direction
    public void TriggerDash(float playerDashDirection)
    {
        float blendTreeValue = 0;


        if (playerDashDirection < 0)
            blendTreeValue = 0;
        else
            blendTreeValue = 1;


        animator.SetTrigger(dashOn);
        animator.SetFloat(dashDirection, blendTreeValue);
    }
    #endregion





    #region JUMP ANIMATIONS
    public void TriggerJump()
    {
        animator.SetTrigger(jump);
    }

    public void ResetJump()
    {
        animator.ResetTrigger(jump);
    }

    public void TriggerLand()
    {
        animator.SetTrigger(land);
    }

    public void ResetLand()
    {
        animator.ResetTrigger(land);
    }

    public void ResetAllJumpParameters()
    {
        ResetJump();
        ResetLand();
    }
    #endregion
    #endregion
}
