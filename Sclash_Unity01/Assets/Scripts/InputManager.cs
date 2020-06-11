using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputManager : MonoBehaviour
{
    #region Singleton
    public static InputManager Instance;
    #endregion  

    #region Event
    public delegate void OnPlayerInput();
    public event OnPlayerInput P2Input;
    public event OnPlayerInput SwitchChar;
    #endregion


    [System.Serializable]
    public struct PlayerInputs
    {
        public bool pauseUp;
        public bool anyKey;
        public bool anyKeyDown;
        public bool reallyanykey;
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
        public bool switchChar;
        public float vertical;
    }

    [SerializeField] public float
        axisDeadZone = 0.1f,
        dashDeadZone = 0.5f;

    [SerializeField] public PlayerInputs[] playerInputs = new PlayerInputs[2];

    [HideInInspector] public bool scoreInput = false;
    [HideInInspector] public bool submitInput = false;
    [HideInInspector] public bool submitInputUp = false;




    [Header("INPUTS")]
    [SerializeField] string pauseAxis = "Pause";
    [SerializeField] string
        horizontalAxis = "Horizontal",
        verticalAxis = "Vertical",
        dashAxis = "Dash",
        attackAxis = "Fire",
        pommelAxis = "Kick",
        parryAxis = "Parry",
        jumpAxis = "Jump",
        scoreAxis = "Score",
        selectcharAxis = "SelectChar";















    #region BASE FUNCTIONS
    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per graphic frame
    void Update()
    {
        scoreInput = Input.GetButton("Score");
        submitInputUp = (submitInput && !Input.GetButton("Submit"));
        submitInput = Input.GetButton("Submit");

        for (int i = 0; i < playerInputs.Length; i++)
        {
            ManageCharacSelectInput(i);

            if (GameManager.Instance.playersList.Count > 0)
            {
                if (GameManager.Instance.playersList[i] != null)
                {
                    if (GameManager.Instance.playersList[i].GetComponent<Player>().playerIsAI)
                        return;
                }
            }


            ManageHorizontalInput(i);
            ManageVerticalInput(i);
            ManageDashInput(i);
            ManageKickInput(i);
            ManageJumpInput(i);
            ManageParryInput(i);
            ManageAttackInput(i);

            ManageAnyKeyInput(i);
            ManageReallyAnyKeyInput(i); 

            ManageScoreInput(i);
            ManagePauseInput(i);
        }
    }

    # endregion



    #region REALLY ANY KEY
    void ManageReallyAnyKeyInput(int i)
    {
        playerInputs[i].reallyanykey = (playerInputs[i].attack || playerInputs[i].parry || playerInputs[i].jump || playerInputs[i].kick || Mathf.Abs(playerInputs[i].horizontal) > axisDeadZone || playerInputs[i].dash > axisDeadZone);
    }
    # endregion



    #region ANY KEY
    void ManageAnyKeyInput(int i)
    {
        playerInputs[i].anyKeyDown = (!playerInputs[i].anyKey && (playerInputs[i].attack || playerInputs[i].parry || playerInputs[i].kick));

        //playerInputs[i].anyKey = (playerInputs[i].attack || playerInputs[i].parry || playerInputs[i].jump || playerInputs[i].kick || Mathf.Abs(playerInputs[i].horizontal) > axisDeadZone || playerInputs[i].dash > axisDeadZone);
        playerInputs[i].anyKey = (playerInputs[i].attack || playerInputs[i].parry || playerInputs[i].kick);
    }
    # endregion




    # region SCORE
    void ManageScoreInput(int i)
    {
        playerInputs[i].scoreUp = playerInputs[i].score && !Input.GetButton(scoreAxis + i);
        playerInputs[i].score = Input.GetButton(scoreAxis + i);
    }
    # endregion


    void ManageCharacSelectInput(int i)
    {
        playerInputs[i].switchChar = Input.GetButtonDown(selectcharAxis + i);

        if (i == 1 && playerInputs[i].switchChar)
            if (P2Input != null)
                P2Input();
    }


    # region PAUSE
    // PAUSE
    void ManagePauseInput(int i)
    {
        playerInputs[i].pauseUp = Input.GetButtonUp(pauseAxis + i);
    }
    # endregion






    # region VERTICAL
    void ManageVerticalInput(int i)
    {
        playerInputs[i].vertical = Input.GetAxis(verticalAxis + i);
    }
    # endregion





    # region HORIZONTAL
    void ManageHorizontalInput(int i)
    {
        playerInputs[i].horizontal = Input.GetAxis(horizontalAxis + i);
    }
    # endregion







    # region DASH
    // DASH
    void ManageDashInput(int i)
    {
        playerInputs[i].dash = Input.GetAxis(dashAxis + i);
    }
    # endregion








    #region ATTACK
    // KICK
    void ManageAttackInput(int i)
    {
        playerInputs[i].attackDown = (!playerInputs[i].attack && (Input.GetButton(attackAxis + i) || Input.GetAxis(attackAxis + i) > axisDeadZone));
        playerInputs[i].attack = Input.GetButton(attackAxis + i) || Input.GetAxis(attackAxis + i) > axisDeadZone;


    }
    # endregion








    # region KICK
    // KICK
    void ManageKickInput(int i)
    {
        playerInputs[i].kick = Input.GetButton(pommelAxis + i);
    }
    # endregion







    # region PARRY
    // KICK
    void ManageParryInput(int i)
    {
        playerInputs[i].parryDown = (!playerInputs[i].parry && (Input.GetButton(parryAxis + i) || Input.GetAxis(parryAxis + i) > axisDeadZone));
        playerInputs[i].parry = Input.GetButton(parryAxis + i) || Input.GetAxis(parryAxis + i) > axisDeadZone;
    }
    # endregion







    #region JUMP
    void ManageJumpInput(int i)
    {
        playerInputs[i].jump = Input.GetButtonDown(jumpAxis + i);
        playerInputs[i].jump = (Input.GetAxis(jumpAxis + i) >= axisDeadZone);
    }
    # endregion
}
