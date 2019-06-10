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

    [SerializeField] float roundEndSlowMoZoom = -6f;
    float givenDistanceBetweenPlayers = 0;
    float distanceBetweenPlayers = 0;
    
    



    // CAMERA MOVEMENTS
    [SerializeField] public float battleSmoothMovementsMultiplier = 0.5f;
    [SerializeField] public float cinematicSmoothMovementsMultiplier = 0.05f;
    [HideInInspector] public float actualSmoothMovementsMultiplier = 0.5f;
    [SerializeField] Vector2 maxSidesZoomedUnzoomed = new Vector2(-10, -5);
    GameObject[] players;











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
    }

    // Update is called once per frame
    void Update()
    {
    }

    // FixedUpdate is called 30 times per second
    private void FixedUpdate()
    {
        BehaviourDependingOnState();
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
    void BehaviourDependingOnState()
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
        cam.transform.localPosition = cam.transform.localPosition + (newPos - cam.transform.localPosition) * actualSmoothMovementsMultiplier;
    }

    void ZoomCameraCustom(float customZoom)
    {
        Vector3 tempPos = cam.transform.localPosition;
        float newCamZ = customZoom;

        /*
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
        */

        Vector3 newPos = new Vector3(tempPos.x, tempPos.y, newCamZ);
        cam.transform.localPosition = cam.transform.localPosition + (newPos - cam.transform.localPosition) * actualSmoothMovementsMultiplier;
    }
}
