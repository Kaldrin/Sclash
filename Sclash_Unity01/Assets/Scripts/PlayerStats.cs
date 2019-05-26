using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    //Components
    PlayerAttack playerAttack;

    // Health
    float currentHealth;
    [SerializeField] float maxHealth;

    //Stamina
    [SerializeField] float maxStamina = 3f;
    [HideInInspector] public float stamina;
    [SerializeField] Slider staminaSlider;
    [SerializeField] float staminaGainOverTimeMultiplier = 0.5f;
    [SerializeField] public float staminaCostForMoves = 1;

    public int playerNum;

    public bool parryBroke = false;

    void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();

        // Set the stamina slider's max value to the stamina max value
        staminaSlider.maxValue = maxStamina;


        ResetValues();
    }

    void FixedUpdate()
    {
        // If the player is parrying
        if (Input.GetButtonDown("Parry" + playerNum) && !parryBroke && !playerAttack.parrying)
        {
            if (stamina >= staminaCostForMoves)
            {
                //stamina -= Time.deltaTime * 2;
                playerAttack.Parry();
                stamina -= staminaCostForMoves;
            }
        }
        // If the player is not parrying
        else
        {
            if (stamina < maxStamina)
            {
                stamina += Time.deltaTime * staminaGainOverTimeMultiplier;
            }

            // If the player recovered at least half of his stamina he can parry again
            if (stamina >= maxStamina / 2)
            {
                parryBroke = false;
            }
        }
    }

    void LateUpdate()
    {
        staminaSlider.value = stamina;
    }

    public void ResetValues()
    {
        currentHealth = maxHealth;
        stamina = maxStamina;
    }

    public bool TakeDamage(GameObject instigator, int hitStrength = 1)
    {
        bool hit;
        /*
        if (!Input.GetButton("Parry" + playerNum) || parryBroke)
        {
            currentHealth -= 1;
            hit = true;
        }
        */
        if (!playerAttack.parrying)
        {
            currentHealth -= 1;
            hit = true;
        }
        else
        {
            //stamina -= hitStrength;
            stamina += staminaCostForMoves;
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
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Player" + playerNum + " : Dead");
            FindObjectOfType<GameManager>().Score(instigator.GetComponent<PlayerStats>().playerNum);
        }

        return hit;
    }
}
