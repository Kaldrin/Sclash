using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    float currentHealth;
    [SerializeField]
    float maxHealth;

    float maxStamina = 10f;
    public float stamina;
    [SerializeField]
    Slider staminaSlider;

    public int playerNum;

    public bool parryBroke = false;


    void Awake()
    {
        stamina = maxStamina;
        ResetHealth();
    }

    void FixedUpdate()
    {
        if (Input.GetButton("Parry" + playerNum) && !parryBroke)
        {
            if (stamina > 0)
            {
                stamina -= Time.deltaTime * 2;
            }
        }
        else
        {
            if (stamina < maxStamina)
            {
                stamina += Time.deltaTime;
            }

            if (stamina >= 5)
            {
                parryBroke = false;
            }
        }
    }

    void LateUpdate()
    {
        staminaSlider.value = stamina;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        stamina = maxStamina;
    }

    public bool TakeDamage(GameObject instigator, int hitStrength = 1)
    {
        bool hit;
        if (!Input.GetButton("Parry" + playerNum) || parryBroke)
        {
            currentHealth -= 1;
            hit = true;
        }
        else
        {
            stamina -= hitStrength;
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
