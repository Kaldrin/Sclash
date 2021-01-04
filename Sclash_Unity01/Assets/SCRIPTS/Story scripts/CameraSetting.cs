using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Camera setting", menuName = "Scriptable objects/Camera setting")]
public class CameraSetting : ScriptableObject
{
    [Header("ZOOM")]
    public Vector2 CameraZoomZLimits;
    public Vector2 PlayersDistanceLimitsForCameraZoomedUnzoomed;
    public float CustomEventZoom;
    public float BattleZoomSmoothDuration;
    public float CinematicZoomSmoothDuration;
    public float BattleZoomSpeed;
    public float EventZoomSpeed;

    [Header("CAMERA MOVEMENTS")]
    public float BattleXSmoothMovementsMultiplier;
    public float CinematicXSmoothMovementsMultiplier;
    public Vector2 cameraArmXLimitsZoomedAndUnzoomed;
}
