using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Created for Unity 2019.1.1f1
public class PlayerMovement : MonoBehaviour
{
    // MANAGERS
    [Header("MANAGERS")]
    // Game manager
    [Tooltip("The name of the game manager's object the find in the scene")]
    [SerializeField] string gameManagerName = "GlobalManager";
    GameManager gameManager = null;

    // Audio manager
    [Tooltip("The name of the audio manager's object the find in the scene")]
    [SerializeField] string audioManagerName = "GlobalManager";
    AudioManager audioManager = null;

    // Input manager
    [Tooltip("The name of the input manager's object the find in the scene")]
    [SerializeField] string inputManagerName = "GlobalManager";
    InputManager inputManager = null;







    // PLAYER'S COMPONENTS
    [Header("PLAYER'S COMPONENTS")]
    PlayerStats playerStats = null;
    PlayerAttack playerAttack = null;
    Rigidbody2D rb = null;







    // ORIENTATION
    [Header("ORIENTATION")]
    [Tooltip("The duration before the player can orient again towards the enemy if they need to once they applied the orientation")]
    [SerializeField] float orientationCooldown = 0.1f;
    float orientationCooldownStartTime = 0;
    float initialXScale = 0;

    bool orientationCooldownFinished = true;
    bool canOrientTowardsEnemy = true;






    // MOVEMENTS
    [Header("MOVEMENTS")]
    [Tooltip("The default movement speed of the player")]
    [SerializeField] float baseMovementsSpeed = 10f;
    [SerializeField] float
        chargeMovementsSpeed = 5f,
        clampY = 2f;
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
        // Get game manager to use in the script
        gameManager = GameObject.Find(gameManagerName).GetComponent<GameManager>();

        // Get the audio manager to use in the script
        audioManager = GameObject.Find(audioManagerName).GetComponent<AudioManager>();

        // Get the input manager
        inputManager = GameObject.Find(inputManagerName).GetComponent<InputManager>();



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
            ManageOrientation();
    }

    // Fixed update is called 30 times per second
    void FixedUpdate()
    {
        // MOVEMENTS INPUTS
        ManageMovements();


        // CLAMP
        ClampY();


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
        ManageOrientation();
    }







    // MOVEMENTS
    void ManageMovements()
    {
        // The player move if they can in their state
        if (!playerStats.dead && !playerAttack.isAttackRecovering && !playerAttack.activeFrame && !gameManager.paused && gameManager.gameStarted && playerAttack.hasDrawn && !playerAttack.kicking)
        {
            if (rb.simulated == false)
                rb.simulated = true;


            //rb.velocity = new Vector2(Input.GetAxis("Horizontal" + playerStats.playerNum) * movementsMultiplier, rb.velocity.y);
            rb.velocity = new Vector2(inputManager.playerInputs[playerStats.playerNum - 1].horizontal * movementsMultiplier, rb.velocity.y);
        }
        // If they are dead they can't move and are then stuck in place
        else if (playerStats.dead)
        {
            rb.simulated = false;
        }
        else if (gameManager.paused || !gameManager.gameStarted)
        {
            rb.velocity = new Vector2(0, 0);
            rb.simulated = false;
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }
    }

    void ClampY()
    {
        Vector3 playerPos = transform.position;


        if (transform.position.y >= clampY)
        {
            transform.position = new Vector3(playerPos.x, clampY, playerPos.z);
        }
    }







    // CHANGE SPEED IF CHARGING / NOT CHARGING
    public void Charging(bool on)
    {
        if (on)
            movementsMultiplier = chargeMovementsSpeed;
        else
            movementsMultiplier = baseMovementsSpeed;
    }








    // ORIENTATION CALLED IN UPDATE
    void ManageOrientation()
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


            


            if (orientationCooldownFinished)
                ApplyOrientation(sign);
        }



        if (Time.time >= orientationCooldown + orientationCooldownStartTime)
        {
            orientationCooldownFinished = true;
        }
    }


    void ApplyOrientation(float sign)
    {
        if (sign > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }


        orientationCooldownStartTime = Time.time;
        orientationCooldownFinished = false;
    }
}
