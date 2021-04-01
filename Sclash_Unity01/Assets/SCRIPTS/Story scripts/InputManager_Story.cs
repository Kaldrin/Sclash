using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager_Story : InputManager
{
    public static InputManager_Story Instance_Solo;

    public InputManager_Story()
    {
        playerInputs = new PlayerInputs[1];
    }

    new void Awake()
    {
        base.Awake();

        Instance = Instance_Solo = this;
    }

    public override void Update()
    {
        /*
        scoreInput = Input.GetButton("Score");
        submitInputUp = (submitInput && !Input.GetButton("Submit"));
        submitInput = Input.GetButton("Submit");

        base.ManageHorizontalInput(0);
        base.ManageVerticalInput(0);
        base.ManageDashInput(0);
        base.ManageKickInput(0);
        base.ManageJumpInput(0);
        base.ManageParryInput(0);
        base.ManageAttackInput(0);
        base.ManageAnyKeyInput(0);
        base.ManageReallyAnyKeyInput(0);
        base.ManageScoreInput(0);
        base.ManagePauseInput(0);*/
    }

    protected override void Start()
    {
        gamepads = new List<Gamepad>();
        GamepadCount = Gamepad.all.Count;


        if (GamepadCount >= 1)
            inputs.Add(PlayerInputManager.instance.JoinPlayer(0, -1, "Gamepad Scheme", gamepads[0]));
        else
            inputs.Add(PlayerInputManager.instance.JoinPlayer(0, -1, "WASDSCheme", Keyboard.current));
    }

    public void AddInputs(int newAmount)
    {
        if (newAmount < playerInputs.Length)
            return;

        playerInputs = new PlayerInputs[newAmount];
        for (int i = 0; i < newAmount; i++) { playerInputs[i] = new PlayerInputs(); }
    }
}
