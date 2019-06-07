using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{

    // AUDIO MANAGER
    [SerializeField] string audioManagerName = "GlobalManager";
    AudioManager audioManager;





    // GAME MANAGER
    [SerializeField] string gameManagerName = "GlobalManager";
    GameManager gameManager;
    





    // PLAYER'S COMPONENTS
    PlayerAttack playerAttack;
    PlayerAnimations playerAnimation;
    PlayerMovement playerMovements;
    Rigidbody2D rigid;





    // HEALTH
    [SerializeField] float maxHealth = 0;
    float currentHealth;

    [SerializeField] bool untouchable = false;
    [HideInInspector] public bool dead = false;





    //STAMINA
    [SerializeField] public Slider staminaSlider = null;

    [SerializeField] public float staminaCostForMoves = 1;
    [SerializeField] float
        maxStamina = 3f,
        durationBeforeStaminaRegen = 0.2f,
        staminaGainOverTimeMultiplier = 0.1f,
        idleStaminaGainOverTimeMultiplier = 0.5f,
        backWalkingStaminaGainOverTime = 0.5f;
    [HideInInspector] public float stamina = 0;  
    float currentTimeBeforeStaminaRegen = 0;

    bool canRegenStamina = true;

    




    // PLAYER IDENTIFICATION
    public int playerNum;
    //public bool parryBroke = false;







    void Awake()
    {
        // Get audio manager to use in the script
        audioManager = GameObject.Find(audioManagerName).GetComponent<AudioManager>();



        // Get game manager to use in the script
        gameManager = GameObject.Find(gameManagerName).GetComponent<GameManager>();




        // Get player's components to use in the script
        rigid = GetComponent<Rigidbody2D>();
        playerAttack = GetComponent<PlayerAttack>();
        playerAnimation = GetComponent<PlayerAnimations>();
        playerMovements = GetComponent<PlayerMovement>();



        // Set the stamina slider's max value to the stamina max value
        staminaSlider.maxValue = maxStamina;



        // Begin by reseting all the player's values and variable to start fresh
        ResetValues();
    }

    void FixedUpdate()
    {
        /*
        // If the player is not parrying
        else
        {
            
        }
        */


        StaminaRegen();
    }

    void LateUpdate()
    {
        staminaSlider.value = stamina;
    }






    // PARRY
    void Parry()
    {
        if (stamina >= staminaCostForMoves)
        {
            //stamina -= Time.deltaTime * 2;
            playerAttack.Parry();
            // STAMINA COST
            StaminaCost(staminaCostForMoves);
        }
    }




    // STAMINA
    void StaminaRegen()
    {
        if (stamina < maxStamina && canRegenStamina)
        {
            // If back walking
            if (rigid.velocity.x * - transform.localScale.x < 0 )
            {
                stamina += Time.deltaTime * backWalkingStaminaGainOverTime;
            }
            else if (Mathf.Abs(rigid.velocity.x) <= 0.5f)
            {
                stamina += Time.deltaTime * idleStaminaGainOverTimeMultiplier;
            }
            else
            {
                stamina += Time.deltaTime * staminaGainOverTimeMultiplier;
            }
        }

        // Small duration before the player can regen stamina again after a move
        if (currentTimeBeforeStaminaRegen <= 0 && !canRegenStamina)
        {
            currentTimeBeforeStaminaRegen = 0;
            canRegenStamina = true;
        }
        else if (!canRegenStamina)
        {
            currentTimeBeforeStaminaRegen -= Time.deltaTime;
        }

        /*
        // If the player recovered at least half of his stamina he can parry again
        if (stamina >= maxStamina / 2)
        {
            parryBroke = false;
        }
        */
    }

    public void PauseStaminaRegen()
    {
        canRegenStamina = false;
        currentTimeBeforeStaminaRegen = durationBeforeStaminaRegen;
    }

    public void StaminaCost(float cost)
    {
        stamina -= cost;
        PauseStaminaRegen();
    }








    // RESET VALUES
    public void ResetValues()
    {
        currentHealth = maxHealth;
        stamina = maxStamina;
        staminaSlider.gameObject.SetActive(true);
        dead = false;
        playerAttack.enemyDead = false;
        playerAnimation.ResetAnims();
        playerAttack.playerCollider.isTrigger = false;
        rigid.gravityScale = 1;
        rigid.simulated = true;
        playerAnimation.CancelCharge();
        playerMovements.Charging(false);
    }









    // RECEIVE AN ATTACK
    public bool TakeDamage(GameObject instigator, int hitStrength = 1)
    {
        bool hit = false;


        if (!dead)
        { 
            /*
            if (!Input.GetButton("Parry" + playerNum) || parryBroke)
            {
                currentHealth -= 1;
                hit = true;
            }
            */


            // CLASH
            if (playerAttack.activeFrame || playerAttack.clashFrames)
            {
                instigator.GetComponent<PlayerAttack>().Clash();
                playerAttack.Clash();
                hit = false;
            }
            // PARRY
            else if (playerAttack.parrying)
            {
                stamina += staminaCostForMoves;
                instigator.GetComponent<PlayerAttack>().Clash();


                // Sound
                audioManager.Parried();
            }
            // UNTOUCHABLE FRAMES
            else if (untouchable)
            {

            }
            // TOUCHED
            else
            {
                hit = true;
                Touched();
            }


            // IS DEAD ?
            if (currentHealth <= 0 && !dead)
            {
                gameManager.Death(instigator.GetComponent<PlayerStats>().playerNum);
                playerAnimation.Dead();
                dead = true;
            }
        }

        return hit;
    }





    // TOUCHED
    void Touched()
    {
        currentHealth -= 1;

        // Sound
        audioManager.SuccessfulAttack();


        playerAttack.playerCollider.isTrigger = true;
        gameManager.EndRoundSlowMo();
    }
}




