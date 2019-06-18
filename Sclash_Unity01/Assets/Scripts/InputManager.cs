using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputManager : MonoBehaviour
{
    [HideInInspector] public bool score = false;
    [SerializeField] TextMeshPro[] controllersText = null;

    public struct PlayerInputs
    {
        public float horizontal;
    }

    PlayerInputs[] playerInputs = new PlayerInputs[2];


    // BASE FUNCTIONS   
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            Debug.Log(Input.GetJoystickNames()[i]);
            controllersText[i].text = Input.GetJoystickNames()[i];
        }
    }

    // Update is called once per graphic frame
    void Update()
    {
        score = Input.GetButton("Score");


        Debug.Log("Horizontal 1 = " + Input.GetAxis("Horizontal1"));
        Debug.Log("Horizontal 2 = " + Input.GetAxis("Horizontal2"));


        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].horizontal = Input.GetAxis("Horizontal" + i + 1);
        }
    }
}
