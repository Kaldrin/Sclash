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


    /*[Header("INPUT AXIS NAMES")]
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
        selectcharAxis = "SelectChar";*/
    #endregion












    protected PlayerControls controls;
    bool WASDjoined = false;
    bool Arrowjoined = false;

    [SerializeField]
    List<Gamepad> gamepads;
    public int GamepadCount
    {
        get { return _gamepadCount; }
        set
        {
            if (_gamepadCount != value)
            {
                _gamepadCount = value;
                NewController();
            }
        }
    }
    private int _gamepadCount;

    List<PlayerInput> inputs = new List<PlayerInput>();


    #region FUNCTIONS
    #region BASE FUNCTIONS

    void Start()
    {
        gamepads = new List<Gamepad>();
        NewController();
    }

    protected void Awake()
    {
        Instance = this;

        controls = GameManager.Instance.Controls;
    }

    // Update is called once per graphic frame
    public virtual void Update()
    {
        GamepadCount = Gamepad.all.Count;
        ManageControllers();

        if (!enabled || !isActiveAndEnabled)
            return;

        // Players inputs
        for (int i = 0; i < playerInputs.Length; i++)
        {
            if (GameManager.Instance.playersList.Count > 0)
            {
                if (GameManager.Instance.playersList[i] != null)
                {
                    if (GameManager.Instance.playersList[i].GetComponent<Player>().playerIsAI)
                        return;
                }
            }
        }
    }
    #endregion

    //TODO finish new controller 
    private void NewController()
    {
        var Gamepads = Gamepad.all;
        foreach (Gamepad g in Gamepads)
        {
            if (!gamepads.Contains(g))
            {
                Debug.Log("Controller plugged in");
                gamepads.Add(g);
            }
        }
    }

    private void ManageControllers()
    {
        PlayerInput p = null;

        switch (inputs.Count)
        {
            case 0:
                if (Keyboard.current.fKey.wasPressedThisFrame && !WASDjoined)
                {
                    p = PlayerInputManager.instance.JoinPlayer(inputs.Count, -1, "WASDScheme", Keyboard.current);
                    WASDjoined = true;
                }

                if (Keyboard.current.numpad1Key.wasPressedThisFrame && !Arrowjoined)
                {
                    p = PlayerInputManager.instance.JoinPlayer(inputs.Count, -1, "ArrowScheme", Keyboard.current);
                    Arrowjoined = true;
                }
                break;

            case 1:
                if (Arrowjoined)
                    break;
                else if (Keyboard.current.numpad1Key.wasPressedThisFrame)
                {
                    p = PlayerInputManager.instance.JoinPlayer(inputs.Count, -1, "ArrowScheme", Keyboard.current);
                    Arrowjoined = true;
                }
                break;

            case 2:
                for (int i = 0; i < inputs.Count; i++)
                {
                    if (inputs[i].currentControlScheme == "WASDScheme" || inputs[i].currentControlScheme == "ArrowScheme")
                    {
                        foreach (Gamepad g in gamepads)
                        {
                            if (g.startButton.wasPressedThisFrame)
                            {
                                inputs[i].SwitchCurrentControlScheme(g);
                                gamepads.Remove(g);
                                Debug.Log("Controller connected for J" + (i + 1));
                                return;
                            }
                        }
                    }
                }
                return;
        }

        foreach (Gamepad g in gamepads)
        {
            if (g.startButton.wasPressedThisFrame)
            {
                p = PlayerInputManager.instance.JoinPlayer(inputs.Count, -1, "Gamepad Scheme", g);
                gamepads.Remove(g);
                break;
            }
        }

        if (p != null)
            inputs.Add(p);
    }

    public void LostDevice(PlayerInput input)
    {
        int index = -1;
        for (int i = 0; i < inputs.Count; i++)
        {
            if (inputs[i] == input)
            {
                index = i;
                break;
            }
        }

        switch (index)
        {
            case 0:
                input.SwitchCurrentControlScheme("WASDScheme", Keyboard.current);
                break;

            case 1:
                input.SwitchCurrentControlScheme("ArrowScheme", Keyboard.current);
                break;

            case -1:
                return;
        }
    }

    public void RegainedDevice(PlayerInput input)
    {
        Debug.Log("Device regained !");
    }

    #endregion
}
