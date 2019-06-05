using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // AUDIO
    [SerializeField] string audioManagerName = "GlobalManager";
    AudioManager audioManager;



    // COMPONENTS
    PlayerStats playerStats;
    PlayerAttack playerAttack;
    Rigidbody2D rb;



    // ORIENTATION
    float initialXScale;
    bool orientTowardsEnemy = true;



    // MOVEMENTS
    [SerializeField] [Range(1f, 20f)] float baseMovementsSpeed = 10f;
    [SerializeField] [Range(1f, 20f)] float chargeMovementsSpeed = 5f;
    float movementsMultiplier;
    [SerializeField] float minSpeedForWalkAnim = 0.1f;
    float baseY = 0;
    


    // JUMP
    bool jumpRequest;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    [Range(1f, 10f)] public float jumpHeight = 10f;



    // Start is called before the first frame update
    void Awake()
    {
        // Get audio
        audioManager = GameObject.Find(audioManagerName).GetComponent<AudioManager>();


        // Get components
        playerAttack = GetComponent<PlayerAttack>();
        playerStats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();

        // Initialize variables
        movementsMultiplier = baseMovementsSpeed;
        initialXScale = transform.localScale.x;
        GetBaseY();



        StartCoroutine(ExecOnAwake());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            //jumpRequest = true;
        }

        if (!playerStats.dead && !playerAttack.charging && !playerAttack.activeFrame && !playerAttack.attackRecovery && !playerAttack.enemyDead)
            OrientTowardsEnemy();
    }

    void FixedUpdate()
    {
        // MOVEMENTS
        if (!playerStats.dead && !playerAttack.attackRecovery && !playerAttack.activeFrame)
            rb.velocity = new Vector2(Input.GetAxis("Horizontal" + playerStats.playerNum) * movementsMultiplier, rb.velocity.y);
        else if (playerStats.dead)
        {
            rb.velocity = new Vector2(0, 0);
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            rb.gravityScale = 0;
        }
            

        // SOUND
        if (rb.velocity.x > minSpeedForWalkAnim)
        {
            audioManager.Walk(true);
        }
        else
        {
            audioManager.Walk(false);
        }


        // JUMP
        if (jumpRequest)
        {
            rb.AddForce(Vector2.up * jumpHeight * 50, ForceMode2D.Impulse);
            jumpRequest = false;
        }

        // DASH
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



    IEnumerator GetBaseY()
    {
        yield return new WaitForSeconds(0.5f);
        baseY = transform.position.y;
    }




    // CHARGING
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



    // ORIENTATION
    void OrientTowardsEnemy()
    {

        if (orientTowardsEnemy)
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
}
