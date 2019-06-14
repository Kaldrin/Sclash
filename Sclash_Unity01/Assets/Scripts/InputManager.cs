using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [HideInInspector] public bool score = false;


    // BASE FUNCTIONS   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per graphic frame
    void Update()
    {
        score = Input.GetButton("Score");
    }
}
