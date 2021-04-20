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
        public bool anyKeyUp;
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
    [HideInInspector] public float axisDeadZone = 0.1f; 
    [HideInInspector] public float dashDeadZone = 0.5f;
    [SerializeField] public PlayerInputs[] playerInputs = new PlayerInputs[2];
    [HideInInspector] public bool scoreInput = false;
    [HideInInspector] public bool submitInput = false;
    [HideInInspector] public bool submitInputUp = false;
    [HideInInspector] public bool skip = false;



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

    [Header("MESSAGES")]
    [SerializeField] GameObject controllerP1InMessageRef = null;
    [SerializeField] GameObject controllerP2InMessageRef = null;
    #endregion












    protected PlayerControls controls;

    [SerializeField]
    protected List<Gamepad> gamepads = new List<Gamepad>();
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
    protected int _gamepadCount;

    protected List<PlayerInput> inputs = new List<PlayerInput>();










    #region FUNCTIONS
    #region BASE FUNCTIONS
    protected void Awake()                                                                                                                                                        // AWAKE
    {
        Instance = this;

        if (GameManager.Instance && GameManager.Instance.Controls != null)
            controls = GameManager.Instance.Controls;
    }


    protected virtual void Start()                                                                                                                                                 // START
    {
        gamepads = new List<Gamepad>();
        GamepadCount = Gamepad.all.Count;

        if (GamepadCount >= 1)
        {
            inputs.Add(PlayerInputManager.instance.JoinPlayer(inputs.Count, -1, "Gamepad Scheme", gamepads[0]));
            if (GamepadCount >= 2)
                inputs.Add(PlayerInputManager.instance.JoinPlayer(inputs.Count, -1, "Gamepad Scheme", gamepads[1]));
            else
                inputs.Add(PlayerInputManager.instance.JoinPlayer(inputs.Count, -1, "ArrowScheme", Keyboard.current));
        }
        else if (PlayerInputManager.instance)
        {
            inputs.Add(PlayerInputManager.instance.JoinPlayer(inputs.Count, -1, "WASDScheme", Keyboard.current));
            inputs.Add(PlayerInputManager.instance.JoinPlayer(inputs.Count, -1, "ArrowScheme", Keyboard.current));
        }
    }


    private void OnEnable()                                                                                                                                                       // ON ENABLE
    {
        ConnectManager.PlayerDisconnected += DisconnectedPlayer;
        ConnectManager.PlayerConnected += OnConnectedPlayer;

        InputSystem.onDeviceChange += (device, change) =>
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    Gamepad g = null;
                    g = (Gamepad)device;
                    if (g != null)
                        foreach (PlayerInput pI in inputs)
                            if (pI.devices[0] as Keyboard != null)
                            {
                                pI.SwitchCurrentControlScheme("Gamepad Scheme", g);
                                break;
                            }
                    break;

                default:
                    break;
            }
        };
    }


    private void OnDisable()                                                                                                                                                    // ON DISABLE
    {
        ConnectManager.PlayerDisconnected -= DisconnectedPlayer;
        ConnectManager.PlayerConnected -= OnConnectedPlayer;
    }


    public virtual void Update()                                                                                                                                                // UPDATE
    {
        GamepadCount = Gamepad.all.Count;
    }
    #endregion





    private void OnConnectedPlayer()                                                                                                                                            // ON CONNECTED PLAYER
    {
        Destroy(inputs[1].gameObject);
        inputs.RemoveAt(1);
    }


    private void DisconnectedPlayer()                                                                                                                                            // DISCONNECTED PLAYER
    {
        Debug.Log("Player Disconnected, rebuilding input for P2");
        if (gamepads.Count == 2)
            inputs.Add(PlayerInputManager.instance.JoinPlayer(inputs.Count, -1, "Gamepad Scheme", gamepads[1]));
        else
            inputs.Add(PlayerInputManager.instance.JoinPlayer(inputs.Count, -1, "ArrowScheme", Keyboard.current));
    }


    private void NewController()                                                                                                                                                // NEW CONTROLLER
    {
        var Gamepads = Gamepad.all;
        foreach (Gamepad g in Gamepads)
            if (!gamepads.Contains(g))
            {
                gamepads.Add(g);
                Debug.Log("Controller plugged in");
                /*
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                */

                // MESSAGE
                if (gamepads.Count == 1)
                    if (controllerP1InMessageRef != null)
                        controllerP1InMessageRef.SetActive(true);
                if (gamepads.Count == 2)
                    if (controllerP2InMessageRef != null)
                        controllerP2InMessageRef.SetActive(true);
            }
    }


    public void LostDevice(PlayerInput input)                                                                                                                                           // LOST DEVICE
    {
        switch (input.playerIndex)
        {
            case 0:
                input.SwitchCurrentControlScheme("WASDScheme", Keyboard.current);
                break;

            case 1:
                input.SwitchCurrentControlScheme("ArrowScheme", Keyboard.current);
                break;

            default:
                break;
        }
    }


    public void RegainedDevice(PlayerInput input)                                                                                                                                       // REGAINED DEVICE
    {
        Debug.Log("Device regained !");
    }
    #endregion
}
