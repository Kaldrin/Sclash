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
        public bool kick;
        public bool kickDown;
    }

    [HideInInspector] public PlayerInputs[] playerInputs = new PlayerInputs[2];


    // BASE FUNCTIONS   
    // Start is called before the first frame update
    void Start()
    {
        if (Input.GetJoystickNames().Length > 1)
        {
            for (int i = 0; i < Input.GetJoystickNames().Length; i++)
            {
                Debug.Log(Input.GetJoystickNames()[i]);
                controllersText[i].text = Input.GetJoystickNames()[i];
            }
        }
    }

    // Update is called once per graphic frame
    void Update()
    {
        
    }

    // FixedUpdate is called 30 times per second
    private void FixedUpdate()
    {
        score = Input.GetButton("Score");
        ManageHorizontalInput();
        ManageKick();
    }







    // HORIZONTAL
    void ManageHorizontalInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].horizontal = Input.GetAxis("Horizontal" + (i + 1));
        }
    }



    // KICK
    void ManageKick()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].kick = Input.GetButton("Kick" + (i + 1));
            playerInputs[i].kickDown = Input.GetButtonDown("Kick" + (i + 1));
                
        }
    }
}
