using System.Collections;
using System.Collections.Generic;
using UnityEngine;





// Bastien BERNAND
// Reusable asset

/// <summary>
/// This script disables the attached game object if the version of the game isn't a beta
/// </summary>

// UNITY 2020.3
public class DisableIfNotBeta : MonoBehaviour
{
    private void Awake()
    {
        CheckVersion();
    }

    private void Start()
    {
        CheckVersion();
    }

    private void OnEnable()
    {
        CheckVersion();
    }

    void CheckVersion()
    {
        if (!Application.version.ToLower().Contains("beta"))
            gameObject.SetActive(false);
    }
}
