using System.Collections;
using System.Collections.Generic;
using UnityEngine;




// Bastien BERNAND
// For Sclash

/// <summary>
/// Main script for the training dummies
/// </summary>

// UNITY 2020.3
public class DummyMain : MonoBehaviour
{
    [SerializeField] DummyRequirement dummyRequirement = DummyRequirement.attack;
    enum DummyRequirement
    {
        attack,
        pommel,
        parry,
        dodge,
    }



    public void Hit()
    {
        if (dummyRequirement == DummyRequirement.attack)
            Debug.Log("Hit");
        else
            Debug.Log("You need to attack it");
    }


    public void Kicked()
    {
        if (dummyRequirement == DummyRequirement.pommel)
            Debug.Log("Kicked");
        else
            Debug.Log("No");
    }
}
