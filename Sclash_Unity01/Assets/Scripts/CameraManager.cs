using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //State
    [HideInInspector]
    public string cameraState = "Inactive";


    //Zoom
    [SerializeField]
    float zoomMultiplier = 0.5f;
    float distanceBetweenPlayers = 0;
    [SerializeField]
    float baseCameraZ;

    Camera cam;

    //Camera movements
    [SerializeField]
    public float maxLeft = -4f;
    [SerializeField]
    public float maxRight = 5f;

    GameObject[] players;



    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        //baseCameraZ = cam.transform.localPosition.z;
        FindPlayers();
    }

    public GameObject[] FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        return players;
    }

    // Update is called once per frame
    void Update()
    {
        BehaviourDependingOnState();
    }

    void BehaviourDependingOnState()
    {
        if (cameraState == "Inactive")
        {

        }
        else if (cameraState == "Battle")
        {
            MoveCameraWithPlayers();
            ZoomCameraWithPlayers();
        }

    }

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


        if (tempPos.x > maxRight - distanceBetweenPlayers * zoomMultiplier)
        {
            tempPos.x = maxRight - distanceBetweenPlayers * zoomMultiplier;
        }
        else if (tempPos.x < maxLeft + distanceBetweenPlayers * zoomMultiplier)
        {
            tempPos.x = maxLeft + distanceBetweenPlayers * zoomMultiplier;
        }

        transform.position = tempPos;
    }


    void ZoomCameraWithPlayers()
    {
        Vector3 tempPos = cam.transform.localPosition;
        distanceBetweenPlayers = 0;

        if (players.Length > 1)
        {
            distanceBetweenPlayers = Mathf.Abs(Vector3.Distance(players[0].transform.position, players[1].transform.position));
        }
        else if (players.Length == 1)
        {
        }

        cam.transform.localPosition = new Vector3(tempPos.x, tempPos.y, baseCameraZ - distanceBetweenPlayers * zoomMultiplier);
    }
}
