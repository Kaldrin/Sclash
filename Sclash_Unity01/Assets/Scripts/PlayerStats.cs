using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{

    // AUDIO
    [SerializeField] string audioManagerName = "GlobalManager";
    AudioManager audioManager;





    // COMPONENTS
    PlayerAttack playerAttack;
    PlayerAnimations playerAnimation;
    Rigidbody2D rigid;





    // HEALTH
    float currentHealth;
    [SerializeField] float maxHealth;
    [HideInInspector] public bool dead = false;





    //STAMINA
    [SerializeField] float maxStamina = 3f;
    [HideInInspector] public float stamina;
    [SerializeField] Slider staminaSlider;
    [SerializeField] float staminaGainOverTimeMultiplier = 0.1f;
    [SerializeField] float idleStaminaGainOverTimeMultiplier = 0.5f;
    [SerializeField] public float staminaCostForMoves = 1;
    bool canRegenStamina = true;
    float currentTimeBeforeStaminaRegen = 0;
    [SerializeField] float durationBeforeStaminaRegen = 0.2f;




    public int playerNum;
    public bool parryBroke = false;







    void Awake()
    {
        // Getting sound 
        audioManager = GameObject.Find(audioManagerName).GetComponent<AudioManager>();

        // Getting components
        rigid = GetComponent<Rigidbody2D>();
        playerAttack = GetComponent<PlayerAttack>();
        playerAnimation = GetComponent<PlayerAnimations>();

        // Set the stamina slider's max value to the stamina max value
        staminaSlider.maxValue = maxStamina;


        ResetValues();
    }

    void FixedUpdate()
    {
        // PARRY
        if (Input.GetButtonDown("Parry" + playerNum) && !parryBroke && !playerAttack.parrying)
        {
            if (stamina >= staminaCostForMoves)
            {
                //stamina -= Time.deltaTime * 2;
                playerAttack.Parry();
                // STAMINA COST
                StaminaCost(staminaCostForMoves);
            }
        }
        // If the player is not parrying
        else
        {
            StaminaRegen();
        }
    }

    void LateUpdate()
    {
        staminaSlider.value = stamina;
    }





    // STAMINA
    void StaminaRegen()
    {
        if (stamina < maxStamina && canRegenStamina)
        {
            if (Mathf.Abs(rigid.velocity.x) <= 0.5f)
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
        dead = false;
        playerAttack.enemyDead = false;
        playerAnimation.ResetAnims();
        playerAttack.playerCollider.isTrigger = false;
        rigid.gravityScale = 1;
    }









    // When receiving an attack
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
            if (playerAttack.activeFrame)
            {
                instigator.GetComponent<PlayerAttack>().Clash();
                playerAttack.Clash();
                hit = false;
            }
            // DEATH
            else if (!playerAttack.parrying)
            {
                currentHealth -= 1;
                hit = true;

                // Sound
                audioManager.SuccessfulAttack();

                playerAttack.playerCollider.isTrigger = true;
            }
            // PARRY
            else
            {
                //stamina -= hitStrength;
                stamina += staminaCostForMoves;
                instigator.GetComponent<PlayerAttack>().Clash();

                if (stamina > 0)
                {
                    Debug.Log("Player " + playerNum + " : Clang");
                }
                else
                {
                    //PARRY BREAK
                    parryBroke = true;
                }
                hit = false;



                // Sound
                audioManager.Parried();
            }


            // IS DEAD ?
            if (currentHealth <= 0 && !dead)
            {
                Debug.Log("Player" + playerNum + " : Dead");
                FindObjectOfType<GameManager>().Death(instigator.GetComponent<PlayerStats>().playerNum);
                playerAnimation.Dead();
                dead = true;
            }
        }

        return hit;
    }
}
