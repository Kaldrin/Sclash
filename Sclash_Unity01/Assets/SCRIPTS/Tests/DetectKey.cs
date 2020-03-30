// Detects keys pressed and prints their keycode

using UnityEngine;
using System.Collections;

public class DetectKey : MonoBehaviour
{
    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey)
        {
            Debug.Log("Detected key code: " + e.keyCode);
        }
    }
}