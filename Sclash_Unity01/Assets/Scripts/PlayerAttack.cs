using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Range(1.0f, 10.0f)]
    public float dashSpeed;
    [Range(1.0f, 10.0f)]
    public float dashDistance;

    bool triggerAttack = false;
    public bool isDashing;

    Vector3 initPos;
    Vector3 targetPos;
    float time;

    Rigidbody2D rb;
    PlayerStats stats;

    bool tapping;
    float lastTap;
    float tapTime = 0.25f;
    float dashDirection;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {

        // ATTACK
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Slash");
            dashDistance = 3.0f;
            //triggerAttack = true;
            GetComponent<PlayerAnimations>().TriggerAttack();
            ApplyDamage();
        }

        // DEFENSE
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Clang");
        }

        //DASH
        if (Input.GetButtonDown("Left" + stats.playerNum) || Input.GetButtonDown("Right" + stats.playerNum))
        {
            float tempDashDirection = Mathf.Sign(Input.GetAxis("Horizontal" + stats.playerNum));
            if (tempDashDirection == dashDirection)
            {
                dashDirection = Mathf.Sign(Input.GetAxis("Horizontal" + stats.playerNum));

                if (!tapping)
                {
                    tapping = true;
                    StartCoroutine(SingleTap());
                }

                if ((Time.time - lastTap) < tapTime)
                {
                    Debug.Log("Double tap");
                    tapping = false;
                    dashDistance = 5.0f;
                    triggerAttack = true;
                }
                lastTap = Time.time;
            }

            dashDirection = tempDashDirection;
        }
    }

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

    void ApplyDamage()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (transform.localScale.x * -1), transform.position.y), new Vector2(1, 1), 0);
        foreach (Collider2D c in hits)
        {
            if (c.CompareTag("Player"))
            {
                c.transform.parent.GetComponent<PlayerStats>().TakeDamage();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(new Vector3(transform.position.x + (transform.localScale.x * -1), transform.position.y, transform.position.z), new Vector3(1, 1, 1));
    }

    void FixedUpdate()
    {
        if (triggerAttack)
        {
            Vector3 dir = new Vector3(dashDirection, 0, 0);
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
                time = 1.0f;
                isDashing = false;
            }
        }
    }


}
