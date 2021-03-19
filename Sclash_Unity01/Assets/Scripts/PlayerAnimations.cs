using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

using Photon.Pun;
using Photon.Realtime;




// HEADER
// For Sclash
// COULD BE MORE OPTIMIZED PROBABLY ?

// REQUIREMENTS
// Photon Unity package
// Player script

/// <summary>
/// Manages all animations of the player
/// </summary>

// VERSION
// Made for Unity 2019.1.1f1
public class PlayerAnimations : MonoBehaviourPunCallbacks
{
    #region VARIABLES
    #region PLAYER'S COMPONENTS
    [Header("PLAYER'S COMPONENTS")]
    [Tooltip("The reference to the player's Rigidbody component")]
    [SerializeField] Rigidbody2D rigid = null;

    [Tooltip("The reference to the player's Animators components for the character and their legs")]
    [SerializeField] public Animator animator = null;
    [SerializeField] Animator legsAnimator2 = null;
    [Tooltip("The reference to the animator component of the game object containing the text telling the player to draw")]
    [SerializeField] public Animator nameDisplayAnimator = null;

    [Tooltip("The reference to the player's SpriteRenderers components for the character and their legs")]
    [SerializeField] public SpriteRenderer spriteRenderer = null;
    [SerializeField] public SpriteRenderer legsSpriteRenderer = null;

    [SerializeField] Player playerScript = null;
    # endregion



    [Header("DEFAULT")]
    [SerializeField] RuntimeAnimatorController defaultAnimator = null;
    [SerializeField] RuntimeAnimatorController defaultLegsAnimator = null;
    [SerializeField] Sprite defaultMask = null;
    [HideInInspector] public float animatorBaseSpeed = 0;
    [HideInInspector] public float legsAnimatorBaseSpeed = 0;
    [HideInInspector] public float nextAttackState = 0;
    [Tooltip("The minimum speed required for the walk anim to trigger")]
    float minSpeedForWalkAnim = 0.05f;



    #region ANIMATOR PARAMETERS
    [Header("PLAYER ANIMATOR PARAMETERS")]
    //[SerializeField] string Walk = "Walk";
    [SerializeField] string
        playerWalkDirection = "WalkDirection";
    [SerializeField] string moving = "Moving",
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
    private void Start()                                                                                    // START
    {
        animatorBaseSpeed = animator.speed;

        // Default look
        if (animator != null && defaultAnimator != null)
            animator.runtimeAnimatorController = defaultAnimator;
        if (legsAnimator2 != null && defaultLegsAnimator != null)
            legsAnimator2.runtimeAnimatorController = defaultLegsAnimator;
        if (playerScript != null && playerScript.maskSpriteRenderer != null && defaultMask != null)
            playerScript.maskSpriteRenderer.sprite = defaultMask;
    }

    void FixedUpdate()                                                                                      // FIXED UPDATE
    {
        if (enabled && isActiveAndEnabled)
        {
            if (photonView != null)
                if (!photonView.IsMine)
                    return;


            UpdateAnims();
            UpdateWalkDirection();
            UpdateIdleStateDependingOnStamina(playerScript.stamina);
            animator.SetFloat(verticalSpeed, rigid.velocity.y);
        }
    }
    #endregion









    #region UPDATE VISUALS
    // Update the animator's parameters in Update
    void UpdateAnims()
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
            if (ConnectManager.Instance != null && ConnectManager.Instance.enableMultiplayer)
                animator.SetFloat(moving, Mathf.Abs(Mathf.Sign(InputManager.Instance.playerInputs[0].horizontal)));
            else
            {
                animator.SetFloat(moving, Mathf.Abs(Mathf.Sign(InputManager.Instance.playerInputs[playerScript.playerNum].horizontal)));


                // Walk anim speed depending on speed
                if (playerScript.playerState == Player.STATE.normal && !playerScript.playerIsAI)
                    animator.speed = Mathf.Abs(InputManager.Instance.playerInputs[playerScript.playerNum].horizontal);
                else
                    animator.speed = animatorBaseSpeed;

                if (playerScript.playerState == Player.STATE.charging && !playerScript.playerIsAI)
                    legsAnimator2.speed = Mathf.Abs(InputManager.Instance.playerInputs[playerScript.playerNum].horizontal);
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
        catch {}
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
        catch {}
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
    // DRAW
    // Triggers the draw animation
    public void TriggerDraw()
    {
        animator.SetTrigger(draw);
    }
    // SNEATH
    // Triggers the sneath animation
    public void TriggerSneath()
    {
        animator.SetTrigger(sneath);
    }
    # endregion




    #region BATTLE SNEATH / DRAW ANIMATIONS
    // SNEATH
    public void TriggerBattleSneath()
    {
        animator.SetTrigger(battleSneath);
        animator.ResetTrigger(battleDraw);
    }
    // DRAW
    public void TriggerBattleDraw()
    {
        animator.SetTrigger(battleDraw);
        animator.ResetTrigger(battleSneath);
    }
    // RESET
    public void ResetBattleSneath()
    {
        animator.ResetTrigger(battleSneath);
        animator.ResetTrigger(battleDraw);
    }
    # endregion




    # region PARRY ANIMATION
    //PARRY
    // Triggers on / off parry animation
    public void TriggerParry()
    {
        animator.SetTrigger(parryOn);
    }
    // RESET
    // Resets the parry animation triggers
    public void ResetParry()
    {
        animator.ResetTrigger(parryOn);
    }
    // PERFECT
    public void TriggerPerfectParry()
    {
        animator.SetTrigger(perfectParry);
    }
    // PERFECT RESET
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
    // POMMEL
    // Trigger on pommel animation
    public void TriggerPommel()
    {
        animator.SetTrigger(pommelOn);
    }
    // RESET
    public void ResetPommelTrigger()
    {
        animator.ResetTrigger(pommelOn);
    }
    // POMMELED
    // Trigger on pommeled animation
    public void TriggerPommeled()
    {
        animator.SetTrigger(pommeledOn);
    }
    // RESET
    public void ResetPommeledTrigger()
    {
        animator.ResetTrigger(pommeledOn);
    }
    #endregion POMMEL ANIMATION





    # region CHARGE ANIMATIONS
    // CHARGE
    // Trigger charge animation
    public void TriggerCharge(bool state)
    {
        animator.SetFloat(attackState, nextAttackState);


        if (state)
            animator.SetTrigger(chargeOn);
        else
            animator.ResetTrigger(chargeOn);
    }
    // MAX
    // Trigger max charge reached animation
    public void TriggerMaxCharge()
    {
        animator.SetTrigger(maxCharge);
    }
    // CANCEL
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
    // CLASHED
    public void TriggerClashed(bool state)
    {
        if (state)
            animator.SetTrigger(clashedOn);
        else
            animator.SetTrigger(clashedOff);
    }
    // RESET
    public void ResetClashedTrigger()
    {
        animator.ResetTrigger(clashedOn);
        animator.ResetTrigger(clashedOff);
    }
    # endregion





    # region DEATH ANIMATION
    // DEATH
    // Trigger death animation
    public void TriggerDeath()
    {
        animator.SetTrigger(deathOn);
    }
    // DEAD
    public void DeathActivated(bool state)
    {
        animator.SetBool(dead, state);
    }
    // REALLY DEAD
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
