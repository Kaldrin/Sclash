using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

// This script, unique in the scene on the manager, manages the inputs of the player, it is a useful bridge between the input system and the player script. Heavily used by a lot of scripts
// OPTIMIZED ?
public class InputManager : MonoBehaviour
{
    #region Singleton
    [HideInInspector] public static InputManager Instance;
    #endregion  


    #region Event
    [HideInInspector] public delegate void OnPlayerInput();
    [HideInInspector] public event OnPlayerInput P2Input;
    //[HideInInspector] public event OnPlayerInput SwitchChar;
    #endregion


    #region VARIABLES
    // Struct for each player's inputs
    [System.Serializable]
    public struct PlayerInputs
    {
        public bool pauseUp;
        public bool anyKey;
        public bool anyKeyDown;
        public bool reallyanykey;
        public bool battleSneathDraw;
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


    [Header("INPUT SETTINGS")]
    [SerializeField]
    public float
        axisDeadZone = 0.1f,
        dashDeadZone = 0.5f;
    [SerializeField] public PlayerInputs[] playerInputs = new PlayerInputs[2];
    [HideInInspector] public bool scoreInput = false;
    [HideInInspector] public bool submitInput = false;
    [HideInInspector] public bool submitInputUp = false;


    [Header("INPUT AXIS NAMES")]
    [SerializeField] string pauseAxis = "Pause";
    [SerializeField]
    string
        battleSneathDrawAxis = "SneathDraw",
        horizontalAxis = "Horizontal",
        verticalAxis = "Vertical",
        dashAxis = "Dash",
        attackAxis = "Fire",
        pommelAxis = "Kick",
        parryAxis = "Parry",
        jumpAxis = "Jump",
        scoreAxis = "Score",
        selectcharAxis = "SelectChar";
    #endregion












    protected PlayerControls controls;

    [SerializeField]
    PlayerInput[] inputs = new PlayerInput[2];

    #region FUNCTIONS
    #region BASE FUNCTIONS

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        //Assign controller to player
        if (inputs[0] == null)
            inputs[0] = playerInput;
        else
            inputs[1] = playerInput;
    }

    protected void Awake()
    {
        Instance = this;

        controls = GameManager.Instance.Controls;
    }

    // Update is called once per graphic frame
    public virtual void Update()
    {
        var Gamepads = Gamepad.all;
        foreach (Gamepad g in Gamepads)
        {
            if (g.buttonWest.isPressed)
            {
                Debug.Log("Button pressed, assign to player !");
            }
        }


        if (enabled && isActiveAndEnabled)
        {
            // In game info display input
            //scoreInput = Input.GetButton("Score");

            // Menu input
            submitInputUp = (submitInput && !controls.Menu.Submit.triggered);
            submitInput = controls.Menu.Submit.triggered;


            // Players inputs
            for (int i = 0; i < playerInputs.Length; i++)
            {
                // ManageCharacSelectInput(i);

                if (GameManager.Instance.playersList.Count > 0)
                    if (GameManager.Instance.playersList[i] != null)
                    {
                        if (GameManager.Instance.playersList[i].GetComponent<Player>().playerIsAI)
                            return;
                    }


                //ManageHorizontalInput(i);
                //ManageVerticalInput(i);
                //ManageDashInput(i);
                //ManageKickInput(i);
                //ManageJumpInput(i);
                //ManageParryInput(i);
                //ManageBattleSneathDrawInput(i);
                //ManageAttackInput(i);

                //ManageAnyKeyInput(i);
                //ManageReallyAnyKeyInput(i);

                //ManageScoreInput(i);
                //ManagePauseInput(i);
            }
        }
    }
    #endregion



    #region REALLY ANY KEY
    protected void ManageReallyAnyKeyInput(int i)
    {
        playerInputs[i].reallyanykey = Input.anyKey;
        //playerInputs[i].reallyanykey = (playerInputs[i].attack || playerInputs[i].parry || playerInputs[i].jump || playerInputs[i].kick || Mathf.Abs(playerInputs[i].horizontal) > axisDeadZone || playerInputs[i].dash > axisDeadZone);
    }
    #endregion


    #region ANY KEY
    protected void ManageAnyKeyInput(int i)
    {
        playerInputs[i].anyKeyDown = (!playerInputs[i].anyKey && (playerInputs[i].attack || playerInputs[i].parry || playerInputs[i].kick));

        //playerInputs[i].anyKey = (playerInputs[i].attack || playerInputs[i].parry || playerInputs[i].jump || playerInputs[i].kick || Mathf.Abs(playerInputs[i].horizontal) > axisDeadZone || playerInputs[i].dash > axisDeadZone);
        playerInputs[i].anyKey = (playerInputs[i].attack || playerInputs[i].parry || playerInputs[i].kick);
    }
    #endregion



    #region SCORE
    protected void ManageScoreInput(int i)
    {
        playerInputs[i].scoreUp = playerInputs[i].score && !Input.GetButton(scoreAxis + i);
        playerInputs[i].score = Input.GetButton(scoreAxis + i);
    }


    protected void ManageCharacSelectInput(int i)
    {
        playerInputs[i].switchChar = Input.GetButtonDown(selectcharAxis + i);

        if (i == 1 && playerInputs[i].switchChar)
            if (P2Input != null)
                P2Input();
    }
    #endregion


    #region PAUSE
    // PAUSE
    protected void ManagePauseInput(int i)
    {
        //playerInputs[i].pauseUp = Input.GetButtonUp(pauseAxis + i);
    }
    #endregion





    #region VERTICAL
    protected void ManageVerticalInput(int i)
    {
        // playerInputs[i].vertical = Input.GetAxis(verticalAxis + i);
    }
    #endregion





    #region HORIZONTAL
    protected void ManageHorizontalInput(int i)
    {
        //playerInputs[i].horizontal = Input.GetAxis(horizontalAxis + i);
    }


    // DASH
    protected void ManageDashInput(int i)
    {
        //playerInputs[i].dash = Input.GetAxis(dashAxis + i);
    }


    void ManageBattleSneathDrawInput(int i)
    {
        //playerInputs[i].battleSneathDraw = Input.GetButton(battleSneathDrawAxis + i);
    }
    #endregion



    #region ATTACK
    // KICK
    protected void ManageAttackInput(int i)
    {
        // playerInputs[i].attackDown = (!playerInputs[i].attack && (Input.GetButton(attackAxis + i) || Input.GetAxis(attackAxis + i) > axisDeadZone));
        // playerInputs[i].attack = Input.GetButton(attackAxis + i) || Input.GetAxis(attackAxis + i) > axisDeadZone;
    }
    #endregion




    #region KICK
    // KICK
    protected void ManageKickInput(int i)
    {
        //playerInputs[i].kick = Input.GetButton(pommelAxis + i);
    }
    #endregion


    #region PARRY
    // KICK
    protected void ManageParryInput(int i)
    {
        //playerInputs[i].parryDown = (!playerInputs[i].parry && (Input.GetButton(parryAxis + i) || Input.GetAxis(parryAxis + i) > axisDeadZone));
        //playerInputs[i].parry = Input.GetButton(parryAxis + i) || Input.GetAxis(parryAxis + i) > axisDeadZone;
    }
    #endregion


    #region JUMP
    protected void ManageJumpInput(int i)
    {
        //playerInputs[i].jump = Input.GetButtonDown(jumpAxis + i);
        //playerInputs[i].jump = (Input.GetAxis(jumpAxis + i) >= axisDeadZone);
    }
    #endregion

    #endregion
}
