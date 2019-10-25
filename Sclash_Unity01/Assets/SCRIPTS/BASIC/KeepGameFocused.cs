using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepGameFocused : MonoBehaviour
{
    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
