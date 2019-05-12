using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Camera cam;
    [SerializeField]
    float maxLeft = -4f;
    [SerializeField]
    float maxRight = 5f;

    GameObject[] players;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
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
        Vector3 tempPos = cam.transform.position;
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

        

        
        if (tempPos.x > maxRight)
        {
            tempPos.x = maxRight;
        }
        else if (tempPos.x < maxLeft)
        {
            tempPos.x = maxLeft;
        }
        
        cam.transform.position = tempPos;

    }
}
