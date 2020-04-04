using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputManager : MonoBehaviour
{
    #region Singleton
    public static InputManager Instance;
    #endregion  


    [System.Serializable]
    public struct PlayerInputs
    {
        public bool pauseUp;
        public bool anyKey;
        public float horizontal;
        public float dash;
        public bool attack;
        public bool attackDown;
        public bool kick;
        public bool parry;
        public bool parryDown;
        public bool jump;
        public bool score;
        public bool scoreUp;
    }

    [SerializeField] float
        axisDeadZone = 0.1f,
        dashDeadZone = 0.5f;

    [SerializeField] public PlayerInputs[] playerInputs = new PlayerInputs[2];

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
        jumpAxis = "Jump",
        scoreAxis = "Score";
    
    














    #region BASE FUNCTIONS
    // BASE FUNCTIONS   
    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per graphic frame
    void Update()
    {
        scoreInput = Input.GetButton("Score");
        ManageHorizontalInput();
        ManageDashInput();
        ManageKickInput();
        ManageAnyKeyInput();
        
        ManageJumpInput();
        ManageParryInput();
        ManageAttackInput();

        ManageScoreInput();
        ManagePauseInput();
    }

    # endregion



    #region ANY KEY
    void ManageAnyKeyInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].anyKey = (playerInputs[i].attack || playerInputs[i].jump || playerInputs[i].kick || Mathf.Abs(playerInputs[i].horizontal) > axisDeadZone || playerInputs[i].dash > axisDeadZone);
        }
    }
    # endregion




    # region SCORE
    void ManageScoreInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].scoreUp = playerInputs[i].score && !Input.GetButton(scoreAxis + i);
            playerInputs[i].score = Input.GetButton(scoreAxis + i);
            

            Debug.Log(playerInputs[i].scoreUp);
        }
    }
    # endregion





    # region PAUSE

    void ManagePauseInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].pauseUp = Input.GetButtonUp(pauseAxis + i);
        }
    }
    # endregion







    # region HORIZONTAL
    void ManageHorizontalInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].horizontal = Input.GetAxis(horizontalAxis + i);
        }
    }
    # endregion







    # region DASH
    void ManageDashInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].dash = Input.GetAxis(dashAxis + i);
        }
    }
    # endregion








    #region ATTACK
    void ManageAttackInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].attackDown = (!playerInputs[i].attack && (Input.GetButton(attackAxis + i) || Input.GetAxis(attackAxis + i) > axisDeadZone));
            playerInputs[i].attack = Input.GetButton(attackAxis + i) || Input.GetAxis(attackAxis + i) > axisDeadZone;
        }
    }
    # endregion








    # region KICK
    void ManageKickInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].kick = Input.GetButton(pommelAxis + i);
        }
    }
    # endregion







    # region PARRY
    void ManageParryInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].parryDown = (!playerInputs[i].parry && (Input.GetButton(parryAxis + i) || Input.GetAxis(parryAxis + i) > axisDeadZone));
            playerInputs[i].parry = Input.GetButton(parryAxis + i) || Input.GetAxis(parryAxis + i) > axisDeadZone;
        }
    }
    # endregion







    #region JUMP
    void ManageJumpInput()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].jump = Input.GetButtonDown(jumpAxis + i);
        }
    }
    # endregion
}
