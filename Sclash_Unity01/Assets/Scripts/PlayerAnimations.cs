using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour

{
    // COMPONENTS
    [SerializeField] Rigidbody2D rigid = null;
    [SerializeField] Animator animator = null;
    PlayerAttack playerAttack = null;
    PlayerStats playerStats = null;
    //[SerializeField] GameObject colliderChild = null;
    [SerializeField] public SpriteRenderer spriteRenderer = null;


    
    //[SerializeField] string walkBool = "Walk";
    //[SerializeField] float speedForWalking = 0.5f;

    
    bool canAttack = true;





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
        animator.SetFloat("WalkDirection", rigid.velocity.x * - transform.localScale.x);
    }


    // STAMINA
    void UpdateAnimStamina(float stamina)
    {
        float blendTreeStamina = 0;


        if (stamina < playerStats.staminaCostForMoves)
            blendTreeStamina = 0;
        else
            blendTreeStamina = 1;


        animator.SetFloat("Stamina", blendTreeStamina);
    }


    
    public void TriggerParry()
    {
        animator.SetTrigger("ParryOn");
    }

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
        Debug.Log("Attack off");
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
