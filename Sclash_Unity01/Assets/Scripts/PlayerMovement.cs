using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Stats
    PlayerStats playerStats;
    PlayerAttack playerAttack;

    //Orientation
    float initialXScale;

    //Movements
    [SerializeField] [Range(1f, 20f)] float baseMovementsSpeed = 10f;
    [SerializeField] [Range(1f, 20f)] float chargeMovementsSpeed = 5f;
    float movementsMultiplier;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Range(1f, 10f)] public float jumpHeight = 10f;

    bool jumpRequest;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerStats = GetComponent<PlayerStats>();

        movementsMultiplier = baseMovementsSpeed;
        initialXScale = transform.localScale.x;
        rb = GetComponent<Rigidbody2D>();


        StartCoroutine(ExecOnAwake());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            //jumpRequest = true;
        }

        OrientTowardsEnemy();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(Input.GetAxis("Horizontal" + playerStats.playerNum) * movementsMultiplier, rb.velocity.y);


        if (jumpRequest)
        {
            rb.AddForce(Vector2.up * jumpHeight * 50, ForceMode2D.Impulse);
            jumpRequest = false;
        }


        if (!playerAttack.isDashing)
        {
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = lowJumpMultiplier;
            }
            else
            {
                rb.gravityScale = 1f;
            }
        }
    }


    public void Charging(bool on)
    {
        if (on)
            movementsMultiplier = chargeMovementsSpeed;
        else
            movementsMultiplier = baseMovementsSpeed;
    }

    IEnumerator ExecOnAwake()
    {
        yield return new WaitForSeconds(0.2f);
        OrientTowardsEnemy();
    }

    // Orients the player towards the other player
    void OrientTowardsEnemy()
    {
        GameObject p1 = null, p2 = null, self = null, other = null;
        PlayerStats[] stats = FindObjectsOfType<PlayerStats>();

        foreach (PlayerStats stat in stats)
        {
            switch (stat.playerNum)
            {
                case 1:
                    p1 = stat.gameObject;
                    break;

                case 2:
                    p2 = stat.gameObject;
                    break;

                default:
                    break;
            }
        }

        if (p1 == gameObject)
        {
            self = p1;
            other = p2;
        }
        else if (p2 == gameObject)
        {
            self = p2;
            other = p1;
        }

        float sign = Mathf.Sign(self.transform.position.x - other.transform.position.x);

        if (sign > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
