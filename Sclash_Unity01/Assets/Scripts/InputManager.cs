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





    [Header("INPUTS")]
    [SerializeField] string pauseAxis = "Pause";
    [SerializeField]
    string
        horizontalAxis = "Horizontal",
        dashAxis = "Dash",
        attackAxis = "Fire",
        pommelAxis = "Kick",
        parryAxis = "Parry",
        jumpAxis = "Jump";
    
    

    










    # region BASE FUNCTIONS
    // BASE FUNCTIONS   

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
            playerInputs[i].pauseUp = Input.GetButtonUp(pauseAxis + i);
        }
    }
    # endregion







    # region HORIZONTAL
    // HORIZONTAL
    void ManageHorizontalInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].horizontal = Input.GetAxis(horizontalAxis + i);
        }
    }
    # endregion







    # region DASH
    // DASH
    void ManageDashInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].dash = Input.GetAxis(dashAxis + i);
        }
    }
    # endregion








    #region ATTACK
    // KICK
    void ManageAttackInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].attack = Input.GetButton(attackAxis + i) || Input.GetAxis(attackAxis + i) > axisDeadZone;
        }
    }
    # endregion








    # region KICK
    // KICK
    void ManageKickInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].kick = Input.GetButton(pommelAxis + i);
        }
    }
    # endregion







    # region PARRY
    // KICK
    void ManageParryInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].parry = Input.GetButton(parryAxis + i) || Input.GetAxis(parryAxis + i) > axisDeadZone;
        }
    }
    # endregion







    #region JUMP
    // JUMP
    void ManageJumpInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].jump = Input.GetButtonDown(jumpAxis + i);
        }
    }
    # endregion
}
