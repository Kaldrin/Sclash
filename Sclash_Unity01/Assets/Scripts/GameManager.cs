using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Menu
    [SerializeField]
    GameObject mainMenu;
    [SerializeField]
    Material blur;
    [SerializeField]
    GameObject blurPanel;
    [SerializeField]
    Material unblur;


    [SerializeField]
    GameObject player;
    [SerializeField]
    CameraManager cameraManager;
    List<GameObject> playersList = new List<GameObject>();
    [SerializeField]
    Text scoreText;

    public Vector2 score;

    float menuPanelBlur = 1.7f;

    // Start is called before the first frame update
    void Start()
    {
        score = new Vector2(0, 0);
        StartCoroutine(SetupGame());
    }


    IEnumerator SetupGame()
    {
        SpawnPlayers();
        yield return new WaitForSeconds(0.5f);
        cameraManager.FindPlayers();
        /*
        yield return new WaitForSeconds(2);
        Play();
        */
    }

    string ScoreBuilder()
    {
        string scoreString = "<color=#FF0000>" + score[0].ToString() + "</color> / <color=#0000FF>" + score[1].ToString() + "</color>";
        return scoreString;
    }

    public void NextRound()
    {
        StartCoroutine(ShowScore());
        GameObject[] playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");
        for (int i = 0; i < playersList.Count; i++)
        {
            GameObject p = playersList[i];

            p.transform.position = playerSpawns[i].transform.position;
            p.transform.rotation = playerSpawns[i].transform.rotation;
            p.GetComponent<PlayerStats>().ResetHealth();
        }
    }

    public void Score(int playerNum)
    {
        score[playerNum - 1] += 1;
        Debug.Log(score);
        scoreText.text = ScoreBuilder();
        NextRound();
    }

    public void Play()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        mainMenu.SetActive(false);
        blurPanel.SetActive(false);
        cameraManager.cameraState = "Battle";
        //StartCoroutine(UpdateBlurPanel());
        yield return new WaitForSeconds(0.5f);
        cameraManager.FindPlayers();
        yield return new WaitForSeconds(1);
        cameraManager.smoothMovementsMultiplier = 0.5f;
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
            playerStats.ResetHealth();
        }
    }

    IEnumerator ShowScore()
    {
        Debug.Log("Show score");
        scoreText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        scoreText.gameObject.SetActive(false);
    }

    IEnumerator UpdateBlurPanel()
    {
        yield return new WaitForSeconds(0);
        blur.Lerp(blur, unblur, 3);
    }
}
