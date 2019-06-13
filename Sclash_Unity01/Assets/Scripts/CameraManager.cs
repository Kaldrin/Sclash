using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    // CAMERA
    Camera cam;





    // CAMERA STATE
    [HideInInspector] public string cameraState = "Inactive";





    // ZOOM
    [SerializeField] Vector2 cameraZLimits = new Vector2(-9, -19);
    [SerializeField] Vector2 playersDistanceForCameraZoomedUnzoomedLimits = new Vector2(5, 25);

    [SerializeField]
    float
        roundEndSlowMoZoom = -6f,
        zoomSpeed = 1;
    [SerializeField] public float 
        cinematicZoomSpeed = 5,
        battleZoomSpeed = 20;
    float
        givenDistanceBetweenPlayers = 0,
        distanceBetweenPlayers = 0;
    [HideInInspector] public float actualZoomSpeed = 0;
    Vector3 currentZoomSpeed = new Vector3(0, 0, 0);
    
    



    // CAMERA MOVEMENTS
    [SerializeField] public float battleSmoothMovementsMultiplier = 0.5f;
    [SerializeField] public float cinematicSmoothMovementsMultiplier = 0.05f;
    [HideInInspector] public float actualSmoothMovementsMultiplier = 0.5f;
    [SerializeField] Vector2 maxSidesZoomedUnzoomed = new Vector2(-10, -5);
    GameObject[] players;
    float time = 0;
    [SerializeField] float
        movementsSpeed = 25,
        maxMovementsSpeed = 15;
    Vector3 currentMovementSpeed;











    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        if (players == null)
        {
            players = FindPlayers();
        }

        
        actualSmoothMovementsMultiplier = cinematicSmoothMovementsMultiplier;
        actualZoomSpeed = cinematicZoomSpeed;

        StartCoroutine(BehaviourDependingOnState());
    }

    // Update is called once per frame
    void Update()
    {
    }

    // FixedUpdate is called 30 times per second
    private void FixedUpdate()
    {
        //BehaviourDependingOnState();
    }






    // PLAYERS
    // Find the players to use in the script
    public GameObject[] FindPlayers()
    {
        PlayerStats[] stats = FindObjectsOfType<PlayerStats>();
        players = new GameObject[stats.Length];

        for (int i = 0; i < stats.Length; i++)
        {
            players[i] = stats[i].gameObject;
        }

        return players;
    }





    // BEHAVIOUR MANAGEMENT
    // Camera uses the associated behaviour
    IEnumerator BehaviourDependingOnState()
    {
        while(true)
        {
            if (cameraState == "Inactive")
            {

            }
            else if (cameraState == "Battle")
            {
                givenDistanceBetweenPlayers = distanceBetweenPlayers;


                MoveCameraWithPlayers();
                ZoomCameraWithPlayers();
            }
            else if (cameraState == "Event")
            {
                MoveCameraWithPlayers();
                ZoomCameraCustom(roundEndSlowMoZoom);
            }
            else if (cameraState == "Clash")
            {
                MoveCameraWithPlayers();
                ZoomCameraCustom(roundEndSlowMoZoom);
            }


            yield return new WaitForEndOfFrame();
        }
    }





    // MOVEMENTS
    // Camera follows players positions
    void MoveCameraWithPlayers()
    {
        Vector3 tempPos = transform.position;

        if (players.Length > 1)
        {
            tempPos.x = 0;

            for (int i = 0; i < players.Length; i++)
            {
                tempPos.x += players[i].transform.position.x;
            }

            tempPos.x = tempPos.x / players.Length;
        }
        else if (players.Length == 1)
        {
            tempPos.x = players[0].transform.position.x;
        }


        float cameraLimitsDifference = Mathf.Abs(cameraZLimits.y) - Mathf.Abs(cameraZLimits.x);
        float maxSidesZoomUnzoomeDifference = maxSidesZoomedUnzoomed.y - maxSidesZoomedUnzoomed.x;
        float maxSide = 0;

        maxSide = maxSidesZoomedUnzoomed.x - maxSidesZoomUnzoomeDifference * ((cam.transform.localPosition.z - cameraZLimits.y) / cameraLimitsDifference);

        if (tempPos.x > maxSide)
        {
            tempPos.x = maxSide;
        }
        else if (tempPos.x < -maxSide)
        {
            tempPos.x = -maxSide;
        }


        transform.position = transform.position + (tempPos - transform.position) * actualSmoothMovementsMultiplier;

        //transform.position = Vector3.Lerp(transform.position, tempPos, Time.unscaledDeltaTime * movementsSpeed);
        //transform.position = Vector3.SmoothDamp(transform.position, tempPos, ref currentMovementSpeed, movementsSpeed, maxMovementsSpeed, Time.unscaledDeltaTime);
    }

    // Camera zooms with players distance
    void ZoomCameraWithPlayers()
    {
        Vector3 tempPos = cam.transform.localPosition;
        float newCamZ = 0;

        distanceBetweenPlayers = 0;


        if (players.Length > 1)
        {
            distanceBetweenPlayers = Mathf.Abs(Vector3.Distance(players[0].transform.position, players[1].transform.position));
        }
        else if (players.Length == 1)
        {
        }


        float cameraZLimitsDifference = Mathf.Abs(cameraZLimits.y) - Mathf.Abs(cameraZLimits.x);
        float playersDistanceForCameraZoomLimitsDifference = Mathf.Abs(playersDistanceForCameraZoomedUnzoomedLimits.y) - Mathf.Abs(playersDistanceForCameraZoomedUnzoomedLimits.x);


        if (givenDistanceBetweenPlayers <= playersDistanceForCameraZoomedUnzoomedLimits.x)
            newCamZ = cameraZLimits.x;
        else if (givenDistanceBetweenPlayers >= playersDistanceForCameraZoomedUnzoomedLimits.y)
            newCamZ = cameraZLimits.y;
        else
            newCamZ = cameraZLimits.x - cameraZLimitsDifference * ((givenDistanceBetweenPlayers - playersDistanceForCameraZoomedUnzoomedLimits.x) / playersDistanceForCameraZoomLimitsDifference);


        Vector3 newPos = new Vector3(tempPos.x, tempPos.y, newCamZ);
        //cam.transform.localPosition = cam.transform.localPosition + (newPos - cam.transform.localPosition) * actualSmoothMovementsMultiplier;

        //cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, newPos, Time.unscaledDeltaTime * zoomSpeed);
        cam.transform.localPosition = Vector3.SmoothDamp(cam.transform.localPosition, newPos, ref currentZoomSpeed, zoomSpeed, actualZoomSpeed, Time.unscaledDeltaTime);
    }

    void ZoomCameraCustom(float customZoom)
    {
        Vector3 tempPos = cam.transform.localPosition;
        float newCamZ = customZoom;

        Vector3 newPos = new Vector3(tempPos.x, tempPos.y, newCamZ);
        //cam.transform.localPosition = cam.transform.localPosition + (newPos - cam.transform.localPosition) * actualSmoothMovementsMultiplier;

        //cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, newPos, Time.unscaledDeltaTime * zoomSpeed);
        cam.transform.localPosition = Vector3.SmoothDamp(cam.transform.localPosition, newPos, ref currentZoomSpeed, zoomSpeed, actualZoomSpeed, Time.unscaledDeltaTime);
    }
}
