using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager_Story : InputManager
{
    public InputManager_Story()
    {
        playerInputs = new PlayerInputs[1];
    }

    void Awake()
    {
        Instance = this;
    }

    public override void Update()
    {
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
        base.ManagePauseInput(0);
    }
}
