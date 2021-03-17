using System.Collections;
using System.Collections.Generic;
using UnityEngine;





// HEADER
// For Sclash

/// <summary>
/// Script of the camera arm for the duel mode, manages all camera movements
/// </summary>

// VERSION
// Originally made for Unity 2019.1.1f1
public class CameraManager : MonoBehaviour
{
    #region VARIABLES
    // Singleton
    // Camera instance for quicker reference
    public static CameraManager Instance;


    [Header("CAMERA MAIN COMPONENTS")]
    [SerializeField] public Camera cameraComponent;



    [Header("CAMERA STATE")]
    [SerializeField] public CAMERASTATE camState = CAMERASTATE.inactive;
    [HideInInspector]
    public enum CAMERASTATE
    {
        inactive,
        battle,
        eventcam,
        solo
    }



    [Header("CAMERA SETTING")]
    public CameraSetting camSetting;




    # region ZOOM
    [Header("ZOOM")]
    [SerializeField] Vector2 cameraZoomZLimits = new Vector2(-6, -25);
    [SerializeField] Vector2 playersDistanceLimitsForCameraZoomedUnzoomedLimits = new Vector2(5, 25);

    [SerializeField] float customEventZoom = -5f;
    [SerializeField]
    public float
        actualZoomSmoothDuration = 1f,
        battleZoomSmoothDuration = 0.25f,
        cinematicZoomSmoothDuration = 1f,
        cinematicZoomSpeed = 5,
        battleZoomSpeed = 20,
        eventZoomSpeed = 10;
    [HideInInspector] public float actualDistanceBetweenPlayers = 0;
    [Tooltip("The distance between players value communicated to the algorithm. Can be faked for effects")]
    float forCalculationDistanceBetweenPlayers = 0;
    [HideInInspector] public float actualZoomSpeed = 0;

    Vector3 currentZoomSpeed = new Vector3(0, 0, 0);
    [HideInInspector] public Vector3 cameraBasePos = new Vector3(0, 0, 0);
    # endregion





    [Header("CAMERA MOVEMENTS")]
    public float battleXSmoothMovementsMultiplier = 0.5f;
    public float cinematicXSmoothMovementsMultiplier = 0.05f;
    [HideInInspector] public float actualXSmoothMovementsMultiplier = 0.5f;
    float playersBaseYPos = 0;

    [SerializeField] Vector2 cameraArmXLimitsZoomedAndUnzoomed = new Vector2(10, 5);
    Vector3 currentMovementSpeed = new Vector3(0, 0, 0);
    [HideInInspector] public Vector3 cameraArmBasePos = new Vector3(0, 0, 0);
    [SerializeField] GameObject[] playersList = new GameObject[2];





    [Header("FX")]
    [SerializeField] public float deathCameraShakeDuration = 0.4f;
    #endregion

















    # region FUNCTIONS
    #region BASE FUNCTIONS
    private void Awake()                                                                                                // AWAKE
    {
        Instance = this;
        if (camSetting != null)
            UseCameraSetting();
    }


    void UseCameraSetting()                                                                                                                     // USE CAMERA SETTINGS
    {
        cameraZoomZLimits = camSetting.CameraZoomZLimits;
        playersDistanceLimitsForCameraZoomedUnzoomedLimits = camSetting.PlayersDistanceLimitsForCameraZoomedUnzoomed;
        customEventZoom = camSetting.CustomEventZoom;
        battleZoomSmoothDuration = camSetting.BattleZoomSmoothDuration;
        cinematicZoomSmoothDuration = camSetting.CinematicZoomSmoothDuration;
        battleZoomSpeed = camSetting.BattleZoomSpeed;
        eventZoomSpeed = camSetting.EventZoomSpeed;
        battleXSmoothMovementsMultiplier = camSetting.BattleXSmoothMovementsMultiplier;
        cinematicXSmoothMovementsMultiplier = camSetting.CinematicXSmoothMovementsMultiplier;
        cameraArmXLimitsZoomedAndUnzoomed = camSetting.cameraArmXLimitsZoomedAndUnzoomed;
    }

    
    protected virtual void Start()                                                                                                                  // START
    {
        playersList = FindPlayers();
        /*
        if (playersList == null)
        {
            playersList = FindPlayers();
        }
        else if (playersList.Length <= 0)
        {
            playersList = FindPlayers();
        }*/


        actualXSmoothMovementsMultiplier = cinematicXSmoothMovementsMultiplier;
        actualZoomSpeed = cinematicZoomSpeed;
        actualZoomSmoothDuration = cinematicZoomSmoothDuration;
        cameraBasePos = cameraComponent.transform.position;
        cameraArmBasePos = transform.position;


        StartCoroutine(BehaviourDependingOnState());
    }


    void Update()                                                                                                                   // UPDATE
    {
        switch (camState)
        {
            case CAMERASTATE.battle:
                break;

            case CAMERASTATE.eventcam:
                break;

            case CAMERASTATE.inactive:
                break;

            case CAMERASTATE.solo:
                break;
        }
    }


    IEnumerator BehaviourDependingOnState()                                                                                                 // BEHAVIOUR DEPENDING ON STATE
    {
        while (true)
        {
            switch (camState)
            {
                case CAMERASTATE.battle:
                    forCalculationDistanceBetweenPlayers = actualDistanceBetweenPlayers;
                    MoveCameraWithPlayers();
                    ZoomCameraWithPlayers();
                    break;

                case CAMERASTATE.eventcam:
                    MoveCameraWithPlayers();
                    ZoomCameraCustom(customEventZoom);
                    break;

                case CAMERASTATE.inactive:
                    break;

                case CAMERASTATE.solo:
                    MoveCameraWithPlayers();
                    ZoomCameraWithPlayers();
                    break;
            }


            yield return new WaitForSeconds(Time.fixedDeltaTime / 10);
        }
    }
    #endregion








    #region CAMERA STATE
    public void SwitchState(CAMERASTATE newState)                                                                                                               // SWITCH STATE
    {
        camState = newState;

        switch (camState)
        {
            case CAMERASTATE.battle:
                actualZoomSpeed = battleZoomSpeed;
                break;

            case CAMERASTATE.eventcam:
                actualZoomSpeed = eventZoomSpeed;
                break;

            case CAMERASTATE.inactive:
                break;

            case CAMERASTATE.solo:
                actualZoomSpeed = battleZoomSpeed;
                break;
        }
    }
    #endregion









    // Find the players to use in the script
    public GameObject[] FindPlayers()                                                                                                               // FIND PLAYERS
    {
        Player[] playersScripts = FindObjectsOfType<Player>();
        playersList = new GameObject[playersScripts.Length];

        for (int i = 0; i < playersScripts.Length; i++)
            playersList[i] = playersScripts[i].gameObject;


        if (playersList != null)
        {
            for (int i = 0; i < playersList.Length; i++)
                playersBaseYPos += playersList[i].transform.position.y;
            playersBaseYPos = playersBaseYPos / 2/*playersList.Length*/;

            return playersList;
        }
        else
        {
            Debug.LogError("The camera couldn't find the players");
            return null;
        }
    }









    # region X MOVEMENTS
    // Camera follows players positions
    protected virtual void MoveCameraWithPlayers()                                                                                                                  // MOVE CAMERA WITH PLAYERS
    {
        // Only calculates camera movements with players if there is at least 1 player in the scene
        if (playersList.Length > 0 && playersList != null)
        {
            Vector3 temporaryCalculationPosition = transform.position;


            // Calculates the position the camera arm should aim to reach depending on the number of players and their respective distance
            if (playersList.Length > 1)
            {
                temporaryCalculationPosition.x = 0;

                for (int i = 0; i < playersList.Length; i++)
                {
                    if (playersList[i] == null)
                        return;

                    temporaryCalculationPosition.x += playersList[i].transform.position.x;
                }

                temporaryCalculationPosition.x = temporaryCalculationPosition.x / playersList.Length;
                //temporaryCalculationPosition.y = playersList[0].transform.position.y;
                temporaryCalculationPosition.y = playersBaseYPos;
            }
            else if (playersList != null && playersList.Length == 1)
                if (playersList[0] != null)
                {
                    temporaryCalculationPosition.x = playersList[0].transform.position.x;
                    temporaryCalculationPosition.y = playersList[0].transform.position.y;
                }


            float cameraZoomZLimitsDifference = Mathf.Abs(cameraZoomZLimits.y) - Mathf.Abs(cameraZoomZLimits.x);
            float camerArmXLimitsZoomedAndUnzoomedDifference = cameraArmXLimitsZoomedAndUnzoomed.y - cameraArmXLimitsZoomedAndUnzoomed.x;
            float camerArmXLimitsForCurrentZoomLevel = 0;


            camerArmXLimitsForCurrentZoomLevel = cameraArmXLimitsZoomedAndUnzoomed.x - camerArmXLimitsZoomedAndUnzoomedDifference * ((cameraComponent.transform.localPosition.z - cameraZoomZLimits.y) / cameraZoomZLimitsDifference);


            // Applies limits to the camera arm X position so it doesn't go out of the game scene
            if (temporaryCalculationPosition.x > camerArmXLimitsForCurrentZoomLevel)
                temporaryCalculationPosition.x = camerArmXLimitsForCurrentZoomLevel;
            else if (temporaryCalculationPosition.x < -camerArmXLimitsForCurrentZoomLevel)
                temporaryCalculationPosition.x = -camerArmXLimitsForCurrentZoomLevel;


            // Translates the camera arm towards the new X position if there is at least one player in the scene
            transform.position += (temporaryCalculationPosition - transform.position) * actualXSmoothMovementsMultiplier;
        }
    }
    # endregion









    # region Z MOVEMENTS
    // Camera zooms with players distance to keep them in view
    void ZoomCameraWithPlayers()                                                                                                                                                 // ZOOM CAMERA WITH PLAYERS
    {
        // Only calculates the zoom level depending on the players if there is at least one players in the scene
        if (playersList.Length > 0 && playersList != null)
        {
            Vector3 temporaryCalculationPosition = cameraComponent.transform.localPosition;
            float newCameraZPosition = 0;


            if (playersList.Length > 1)
            {
                if (playersList[0] == null || playersList[1] == null)
                    return;

                actualDistanceBetweenPlayers = Mathf.Abs(Vector3.Distance(playersList[0].transform.position, playersList[1].transform.position));
            }
            else if (playersList.Length == 1)
                actualDistanceBetweenPlayers = 0;


            float cameraZoomZLimitsDifference = Mathf.Abs(cameraZoomZLimits.y) - Mathf.Abs(cameraZoomZLimits.x);
            float playersDistanceForCameraZoomLimitsDifference = Mathf.Abs(playersDistanceLimitsForCameraZoomedUnzoomedLimits.y) - Mathf.Abs(playersDistanceLimitsForCameraZoomedUnzoomedLimits.x);


            if (forCalculationDistanceBetweenPlayers <= playersDistanceLimitsForCameraZoomedUnzoomedLimits.x)
                newCameraZPosition = cameraZoomZLimits.x;
            else if (forCalculationDistanceBetweenPlayers >= playersDistanceLimitsForCameraZoomedUnzoomedLimits.y)
                newCameraZPosition = cameraZoomZLimits.y;
            else
                newCameraZPosition = cameraZoomZLimits.x - cameraZoomZLimitsDifference * ((forCalculationDistanceBetweenPlayers - playersDistanceLimitsForCameraZoomedUnzoomedLimits.x) / playersDistanceForCameraZoomLimitsDifference);


            Vector3 newCameraPosition = new Vector3(temporaryCalculationPosition.x, temporaryCalculationPosition.y, newCameraZPosition);


            cameraComponent.transform.localPosition = Vector3.SmoothDamp(cameraComponent.transform.localPosition, newCameraPosition, ref currentZoomSpeed, actualZoomSmoothDuration, actualZoomSpeed, Time.unscaledDeltaTime);
        }
    }

    void ZoomCameraCustom(float customZoom)                                                                                                                                           // ZOOM CAMERA CUSTOM
    {
        Vector3 temporaryCalculationPosition = cameraComponent.transform.localPosition;
        float newCameraZPosition = customZoom;


        // Calculates the new position the camera component object should go to
        Vector3 newCameraPosition = new Vector3(temporaryCalculationPosition.x, temporaryCalculationPosition.y, newCameraZPosition);
        //cam.transform.localPosition = cam.transform.localPosition + (newPos - cam.transform.localPosition) * actualSmoothMovementsMultiplier;


        // Appplies the movement towards the new position with a smooth translation
        cameraComponent.transform.localPosition = Vector3.Lerp(cameraComponent.transform.localPosition, newCameraPosition, Time.deltaTime * actualZoomSpeed);
        //cameraComponent.transform.localPosition = Vector3.SmoothDamp(cameraComponent.transform.localPosition, newCameraPosition, ref currentZoomSpeed, actualZoomSmoothDuration, actualZoomSpeed, Time.deltaTime);
    }
    #endregion
    #endregion
}
