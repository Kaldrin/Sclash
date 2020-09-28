using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager_Solo : CameraManager
{
    protected override void Start()
    {
        base.Start();
        SwitchState(CAMERASTATE.battle);
    }
    protected override void MoveCameraWithPlayers()
    {
        base.MoveCameraWithPlayers();
    }

}
