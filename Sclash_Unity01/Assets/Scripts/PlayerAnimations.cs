using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    // COMPONENTS
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] Animator animator;
    PlayerAttack playerAttack;
    PlayerStats playerStats;

    [SerializeField] GameObject colliderChild;


    
    [SerializeField] string walkBool = "Walk";
    [SerializeField] float speedForWalking = 0.5f;

    
    bool canAttack;

    void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerStats = GetComponent<PlayerStats>();


        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnims();
    }


    void UpdateAnims()
    {
        animator.SetFloat("Move", Mathf.Abs(Input.GetAxis("Horizontal" + playerStats.playerNum)));
        animator.SetBool("Parry", playerAttack.parrying);
        animator.SetFloat("Level", playerAttack.chargeLevel - 1);
        animator.SetBool("Charging", playerAttack.charging);
        animator.SetBool("Dashing", playerAttack.isDashing);
        //animator.SetBool("Parry", Input.GetButton("Parry" + stats.playerNum) & !stats.parryBroke);
        UpdateAnimStamina(playerStats.stamina);
    }

    void UpdateAnimStamina(float stamina)
    {
        float blendTreeStamina = 0;


        if (stamina < playerStats.staminaCostForMoves)
            blendTreeStamina = 0;
        else
            blendTreeStamina = 1;


        animator.SetFloat("Stamina", blendTreeStamina);
    }

    // Triggers or deactivates parry
    /*
    public void Parry(bool state)
    {
        if (state)
        {
            animator.SetBool("Parry", true);
        }
        else
        {
            animator.SetBool("Parry", false);
        }
    }
    */

    
    public void TriggerParry()
    {
        animator.SetTrigger("ParryOn");
    }

    public void TriggerAttack()
    {
        if (canAttack)
        {
            canAttack = false;
            animator.SetBool("Attack", true);
            animator.SetTrigger("AttackOn");
            animator.SetBool("Charging", false);
            //StartCoroutine(AttackTime());
        }
    }

    public void TriggerCharge()
    {
        animator.SetTrigger("ChargeOn");
    }

    public void TriggerMaxCharge()
    {
        animator.SetTrigger("MaxCharge");
    }

    public void CancelCharge()
    {
        animator.SetBool("Charging", false);
    }

    /*
    public void ChargeChange(float chargeLevel)
    {
        animator.SetFloat("Level", chargeLevel - 1);
    }
    */

    public void Clashed(bool state)
    {
        animator.SetBool("Clash", state);

        if (state)
            animator.SetTrigger("Clashed");
    }

    public void Dead()
    {
        //animator.SetTrigger("Dead");
        animator.SetBool("Dead", true);
        animator.SetTrigger("DeathOn");
    }


    public void EndAttack()
    {
        StartCoroutine(AttackTime());
    }
    IEnumerator AttackTime()
    {
        yield return new WaitForSeconds(0.05f);
        animator.SetBool("Attack", false);
        canAttack = true;
    }



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
}
