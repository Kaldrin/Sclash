using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    [HideInInspector] public bool scoreInput = false;

    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreInput = Input.GetButton("Score");
    }
}
