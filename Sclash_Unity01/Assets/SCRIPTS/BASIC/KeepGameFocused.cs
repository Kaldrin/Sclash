using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// OPTIMIZED
public class KeepGameFocused : MonoBehaviour
{
    void Start()                                                                        // START
    {
        Application.runInBackground = true;
    }
}
