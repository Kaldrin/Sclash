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
        //animator.SetBool("Parry", Input.GetButton("Parry" + stats.playerNum) & !stats.parryBroke);
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

    public void TriggerAttack()
    {
        if (canAttack)
        {
            canAttack = false;
            animator.SetBool("Attack", true);
            animator.SetTrigger("Attacking");
            animator.SetBool("Charging", false);
            StartCoroutine(AttackTime());
        }
    }

    public void TriggerCharge()
    {
        animator.SetBool("Charging", true);
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
    }


    IEnumerator AttackTime()
    {
        yield return new WaitForSeconds(0.15f);
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
        animator.ResetTrigger("Attacking");
        animator.SetBool("Parry", false);
    }

}
