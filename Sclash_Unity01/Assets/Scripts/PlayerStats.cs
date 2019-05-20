using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    float currentHealth;
    [SerializeField]
    float maxHealth;

    public int playerNum;

    void Awake()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public bool TakeDamage(GameObject instigator)
    {
        bool hit;
        if (!Input.GetButton("Fire2"))
        {
            currentHealth -= 1;
            hit = true;
        }
        else
        {
            Debug.Log("Player " + playerNum + " : Clang");
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
