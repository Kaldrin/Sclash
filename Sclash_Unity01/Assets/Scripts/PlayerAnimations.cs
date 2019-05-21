using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D rigid;
    [SerializeField]
    GameObject colliderChild;


    [SerializeField]
    Animator animator;
    [SerializeField]
    string walkBool = "Walk";
    [SerializeField]
    float speedForWalking = 0.5f;

    PlayerStats stats;
    bool canAttack;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnims();
    }


    void UpdateAnims()
    {
        animator.SetFloat("Move", Mathf.Abs(Input.GetAxis("Horizontal" + stats.playerNum)));

        /* if (Mathf.Abs(rigid.velocity.x) > speedForWalking)
		{
			animator.SetBool(walkBool, true);
		}
		else
		{
			animator.SetBool(walkBool, false);
		}
		
		if (colliderUpdate)
        {
            Destroy(colliderChild.GetComponent<PolygonCollider2D>());
            colliderChild.AddComponent<PolygonCollider2D>();
		}*/

    }

    public void TriggerAttack()
    {
        if (canAttack)
        {
            canAttack = false;
            animator.SetBool("Attack", true);
            animator.SetBool("Charging", false);
            StartCoroutine(AttackTime());
        }
    }

    public void TriggerCharge()
    {
        animator.SetBool("Charging", true);
    }

    public void ChargeChange(float chargeLevel)
    {
        animator.SetFloat("Level", chargeLevel);
    }

    IEnumerator AttackTime()
    {
        yield return new WaitForSeconds(0.15f);
        animator.SetBool("Attack", false);
        canAttack = true;
    }


}
