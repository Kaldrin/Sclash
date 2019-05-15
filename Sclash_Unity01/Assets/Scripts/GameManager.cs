using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Menu
    [SerializeField]
    GameObject mainMenu;


    [SerializeField]
    GameObject player;
    [SerializeField]
    CameraManager cameraManager;
    List<GameObject> playersList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetupGame());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator SetupGame()
    {
        SpawnPlayers();
        yield return new WaitForSeconds(0.5f);
        cameraManager.FindPlayers();
    }

    public void Play()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        mainMenu.SetActive(false);
        cameraManager.cameraState = "Battle";
        yield return new WaitForSeconds(0.5f);
        cameraManager.FindPlayers();
    }

    void SpawnPlayers()
    {
        GameObject[] playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");

        for (int i = 0; i < playerSpawns.Length; i++)
        {
            PlayerStats playerStats;

            playersList.Add(Instantiate(player, playerSpawns[i].transform.position, playerSpawns[i].transform.rotation))
                ;
            playerStats = playersList[i].GetComponent<PlayerStats>();
            playerStats.playerNum = i + 1;
        }
    }
}
