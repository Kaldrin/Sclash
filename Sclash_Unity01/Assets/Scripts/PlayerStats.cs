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
    float maxStamina = 3f;
    public float stamina;
    [SerializeField] Slider staminaSlider;

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
        if (Input.GetButton("Parry" + playerNum) && !parryBroke)
        {
            if (stamina > 0)
            {
                //stamina -= Time.deltaTime * 2;
                playerAttack.Parry();
            }
        }
        // If the player is not parrying
        else
        {
            if (stamina < maxStamina)
            {
                stamina += Time.deltaTime;
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
            stamina++;
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
