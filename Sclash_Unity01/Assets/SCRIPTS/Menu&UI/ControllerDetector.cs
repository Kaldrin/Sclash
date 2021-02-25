using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This script allows for detecting non mouse inputs in the menu and disables the mouse if so
// OPTIMIZED
public class ControllerDetector : MonoBehaviour
{
    [SerializeField] List<string> controllerAxisToCheck = new List<string>();
    // [SerializeField] float controllerDeadZone = 0.3f;
    [SerializeField] List<string> mouseAxisToCheck = new List<string>();
    // [SerializeField] float mouseDeadZone = 0.3f;
    [SerializeField] bool doDetectController = true;






    void Update()
    {/*
        if (enabled && isActiveAndEnabled && doDetectController)
        {
            if (Cursor.visible)
                for (int i = 0; i < controllerAxisToCheck.Count; i++)
                {
                    // If axis used (superior to dead zone value), disables the mouse
                    if (Mathf.Abs(Input.GetAxis(controllerAxisToCheck[i])) > controllerDeadZone)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                }
            else if (!Cursor.visible)
                for(int i = 0; i < mouseAxisToCheck.Count; i++)
                    // If axis used (superior to dead zone value), enables the mouse
                    if (Mathf.Abs(Input.GetAxis(mouseAxisToCheck[i])) > mouseDeadZone)
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
        }*/
    }
}
