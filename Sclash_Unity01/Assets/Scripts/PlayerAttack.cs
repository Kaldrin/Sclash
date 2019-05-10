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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        // ATTACK
        if (Input.GetButtonDown("Fire1"))
        {
            triggerAttack = true;
            Debug.Log("Slash");
        }

        // DEFENSE
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Clang");
        }
    }

    void FixedUpdate()
    {

        if (triggerAttack)
        {
            Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            dir = Vector3.Normalize(dir) * dashDistance;
            dir.z = 0.0f;

            initPos = transform.position;
            targetPos = transform.position + dir;
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
