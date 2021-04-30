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
    private void Awake()                                                                                                        // AWAKE
    {
        CheckVersion();
    }

    private void Start()                                                                                                            // START
    {
        CheckVersion();
    }

    private void OnEnable()                                                                                                     // ON ENABLE
    {
        CheckVersion();
    }






    void CheckVersion()                                                                                                             // CHECK VERSION
    {
        Debug.Log(Application.version.ToLower());
        if (!Application.version.ToLower().Contains("beta"))
            gameObject.SetActive(false);
    }
}
