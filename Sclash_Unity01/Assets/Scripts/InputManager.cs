using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputManager : MonoBehaviour
{
    public struct PlayerInputs
    {
        public bool pauseUp;
        public bool anyKey;
        public float horizontal;
        public float dash;
        public bool attack;
        public bool kick;
        public bool parry;
        public bool jump;
    }

    [SerializeField] float
        axisDeadZone = 0.1f,
        dashDeadZone = 0.5f;

    [HideInInspector] public PlayerInputs[] playerInputs = new PlayerInputs[2];

    [HideInInspector] public bool scoreInput = false;

    

    










    # region BASE FUNCTIONS
    // BASE FUNCTIONS   
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per graphic frame
    void Update()
    {
        scoreInput = Input.GetButton("Score");
        ManageHorizontalInput();
        ManageDashInput();
        ManageKickInput();
        ManageAnyKeyInput();
        ManagePauseInput();
        ManageJumpInput();
        ManageParryInput();
        ManageAttackInput();
        
    }

    // FixedUpdate is called 50 times per second
    private void FixedUpdate()
    {
        
    }
    # endregion









    #region ANY KEY
    // ANY KEY
    void ManageAnyKeyInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].anyKey = (playerInputs[i].attack || playerInputs[i].jump || playerInputs[i].kick || Mathf.Abs(playerInputs[i].horizontal) > axisDeadZone);
        }
    }
    # endregion








    # region PAUSE
    // PAUSE
    void ManagePauseInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].pauseUp = Input.GetButtonUp("Pause" + i);
        }
    }
    # endregion









    # region HORIZONTAL
    // HORIZONTAL
    void ManageHorizontalInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].horizontal = Input.GetAxis("Horizontal" + i);
        }
    }
    # endregion






    # region DASH
    // DASH
    void ManageDashInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].dash = Input.GetAxis("Dash" + i);
        }
    }
    # endregion








    #region ATTACK
    // KICK
    void ManageAttackInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].attack = Input.GetButton("Fire" + i) || Input.GetAxis("Fire" + i) > axisDeadZone;
        }
    }
    # endregion








    # region KICK
    // KICK
    void ManageKickInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].kick = Input.GetButton("Kick" + i);
        }
    }
    # endregion






    # region PARRY
    // KICK
    void ManageParryInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].parry = Input.GetButton("Parry" + i) || Input.GetAxis("Parry" + i) < - axisDeadZone;
        }
    }
    # endregion








    #region JUMP
    // JUMP
    void ManageJumpInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].jump = Input.GetButtonDown("Jump" + i);
        }
    }
    # endregion
}
