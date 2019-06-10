using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // AUDIO MANAGER
    [SerializeField] string audioManagerName = "GlobalManager";
    AudioManager audioManager = null;





    // PLAYER'S COMPONENTS
    PlayerStats playerStats = null;
    PlayerAttack playerAttack = null;
    Rigidbody2D rb = null;





    // ORIENTATION
    float initialXScale = 0;
    bool canOrientTowardsEnemy = true;






    // MOVEMENTS
    [SerializeField]
    float
        baseMovementsSpeed = 10f,
        chargeMovementsSpeed = 5f;
        
    float movementsMultiplier = 0;
    





    // JUMP
    //bool jumpRequest = false;

    /*
    [SerializeField] float
        fallMultiplier = 2.5f,
        lowJumpMultiplier = 2f,
        jumpHeight = 10f;
    */







    // BASIC FUNCTIONS
    // Start is called before the first frame update
    void Awake()
    {
        // Get the audio manager to use in the script
        audioManager = GameObject.Find(audioManagerName).GetComponent<AudioManager>();


        // Get player's components to use in the script
        playerAttack = GetComponent<PlayerAttack>();
        playerStats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();

        // Initialize variables
        movementsMultiplier = baseMovementsSpeed;
        initialXScale = transform.localScale.x;


        StartCoroutine(ExecOnAwake());
    }

    // Update is called once per graphic frame
    void Update()
    {
        // JUMP INPUT
        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            //jumpRequest = true;
        }

        // ORIENTATION IF PLAYER CAN ORIENT
        if (!playerStats.dead && !playerAttack.charging && !playerAttack.activeFrame && !playerAttack.isAttackRecovering && !playerAttack.enemyDead && !playerAttack.isAttacking && !playerAttack.isDashing)
            OrientTowardsEnemy();
    }

    // Fixed update is called 30 times per second
    void FixedUpdate()
    {
        // MOVEMENTS INPUTS
        ManageMovements();


        /*
        // JUMP
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
        */
    }






    // TO EXECUTE A FEW FRAMES AFTER THE AWAKE FUNCTION
    IEnumerator ExecOnAwake()
    {
        yield return new WaitForSeconds(0.2f);
        OrientTowardsEnemy();
    }








    // MOVEMENTS
    void ManageMovements()
    {
        // The player move if they can in their state
        if (!playerStats.dead && !playerAttack.isAttackRecovering && !playerAttack.activeFrame)
            rb.velocity = new Vector2(Input.GetAxis("Horizontal" + playerStats.playerNum) * movementsMultiplier, rb.velocity.y);
        // If they are dead they can't move and are then stuck in place
        else if (playerStats.dead)
        {
            // rb.velocity = new Vector2(0, 0);
            // transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            // rb.gravityScale = 0;
            rb.simulated = false;
        }

        /*
        // WALK SOUND
        if (rb.velocity.x > minSpeedForWalkAnim)
        {
            audioManager.Walk(true);
        }
        else
        {
            audioManager.Walk(false);
        }
        */
    }







    // CHANGE SPEED IF CHARGING/ NOT CHARGING
    public void Charging(bool on)
    {
        if (on)
            movementsMultiplier = chargeMovementsSpeed;
        else
            movementsMultiplier = baseMovementsSpeed;
    }








    // ORIENTATION CALLED IN UPDATE
    void OrientTowardsEnemy()
    {
        // Orient towards the enemy if player can in their current state
        if (canOrientTowardsEnemy)
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
