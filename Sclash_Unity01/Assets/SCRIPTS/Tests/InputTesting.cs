using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTesting : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetButton("P2Joystick"))
            Debug.Log("P2");
        if (Input.GetButton("P1Joystick"))
            Debug.Log("P1");

    }
}
