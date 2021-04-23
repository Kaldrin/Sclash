using System.Collections;
using System.Collections.Generic;
using UnityEngine;




// Reusable asset
// Bastien BERNAND

/// <summary>
/// This script is to put on an object and will activate / deactivate it on Awake depending on which device the game is running.
/// </summary>

// UNITY 2020.3
public class ActivateOnDevice : MonoBehaviour
{
    [SerializeField] DeviceType deviceOnWhichThisObjectIsActivated = DeviceType.Desktop;




    private void Awake()                                                                                                                                // AWAKE
    {
        if (SystemInfo.deviceType != deviceOnWhichThisObjectIsActivated)
            gameObject.SetActive(false);
    }
}
