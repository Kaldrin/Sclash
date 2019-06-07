using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    // PLAYER'S COMPONENTS
    [SerializeField] Rigidbody2D rigid = null;
    [SerializeField] Animator animator = null;
    [SerializeField] public SpriteRenderer spriteRenderer = null;
    PlayerAttack playerAttack = null;
    PlayerStats playerStats = null;
    


    
    bool canAttack = true;





    void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerStats = GetComponent<PlayerStats>();


        canAttack = true;
    }

    // Update is called once per graphic frame
    void Update()
    {
        UpdateAnims();
    }

    // Update the animator's parameters in Update
    void UpdateAnims()
    {
        animator.SetFloat("Move", Mathf.Abs(Input.GetAxis("Horizontal" + playerStats.playerNum)));
        
        //animator.SetBool("Parry", playerAttack.parrying);
        animator.SetFloat("Level", playerAttack.chargeLevel - 1);
        animator.SetBool("Charging", playerAttack.charging);
        animator.SetBool("Dashing", playerAttack.isDashing);
        //animator.SetBool("Parry", Input.GetButton("Parry" + stats.playerNum) & !stats.parryBroke);
        UpdateAnimStamina(playerStats.stamina);
        animator.SetFloat("WalkDirection", rigid.velocity.x * - transform.localScale.x);
    }








    // STAMINA ANIMATIONS STATES
    void UpdateAnimStamina(float stamina)
    {
        float blendTreeStamina = 0;


        if (stamina < playerStats.staminaCostForMoves)
            blendTreeStamina = 0;
        else
            blendTreeStamina = 1;


        animator.SetFloat("Stamina", blendTreeStamina);
    }








    // PARRY ANIMATION
    // Triggers on / off parry animation
    public void TriggerParry(bool state)
    {
        if (state)
            animator.SetTrigger("ParryOn");
        else
            animator.SetTrigger("ParryOff");
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
    }








    // ATTACK ANIMATIONS
    // Trigger attack depending on the intended direction
    public void TriggerAttack(float attackDir)
    {
        if (canAttack)
        {
            float attackDirection;


            if (attackDir == 1)
                attackDirection = 0f;
            else if (attackDir == -1)
                attackDirection = 0.5f;
            else
                attackDirection = 0.5f;

            canAttack = false;
            animator.SetBool("Attack", true);
            animator.SetTrigger("AttackOn");
            animator.SetBool("Charging", false);

            animator.SetFloat("AttackDirection", attackDirection);
        }
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
        canAttack = true;
        Debug.Log("Attack off");
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
