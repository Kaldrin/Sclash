using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // AUDIO
    [SerializeField] string audioManagerName = "GlobalManager";
    AudioManager audioManager;



    // MENU
    [SerializeField] GameObject mainMenu;
    [SerializeField] Material blur;
    [SerializeField] GameObject blurPanel;
    [SerializeField] Material unblur;



    // ROUND
    [SerializeField] float timeBeforeNextRound = 3;
    [SerializeField] ParticleSystem roundLeaves;



    // PLAYERS
    [SerializeField] GameObject player;
    List<GameObject> playersList = new List<GameObject>();
    [SerializeField] Color[] playersColors;



    [SerializeField] Text scoreText;
    [SerializeField] CameraManager cameraManager;

    public Vector2 score;

    float menuPanelBlur = 1.7f;

    // Start is called before the first frame update
    void Start()
    {
        // Get audio
        audioManager = GameObject.Find(audioManagerName).GetComponent<AudioManager>();

        // Reset score
        score = new Vector2(0, 0);


        // Start game
        StartCoroutine(SetupGame());
    }


    IEnumerator SetupGame()
    {
        yield return new WaitForSeconds(0);


        // SOUND
        // Set on the menu music
        audioManager.MenuMusicOn();

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
        StartCoroutine(NextRoundCoroutine());
    }

    IEnumerator NextRoundCoroutine()
    {
        StartCoroutine(ShowScore());
        roundLeaves.gameObject.SetActive(true);
        roundLeaves.Play();

        yield return new WaitForSeconds(1.5f);
        ResetPlayers();
    }

    public void Death(int playerNum)
    {
        StartCoroutine(Score(playerNum));
    }

    IEnumerator Score(int playerNum)
    {
        score[playerNum - 1] += 1;

        yield return new WaitForSeconds(timeBeforeNextRound);

        scoreText.text = ScoreBuilder(); 
        NextRound();
    }

    public void Play()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        // SOUND
        audioManager.FindPlayers();
        yield return new WaitForSeconds(0.1f);
        audioManager.BattleMusicOn();
        



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
            PlayerAnimations playerAnimations;

            playersList.Add(Instantiate(player, playerSpawns[i].transform.position, playerSpawns[i].transform.rotation));
            playerStats = playersList[i].GetComponent<PlayerStats>();
            playerAnimations = playersList[i].GetComponent<PlayerAnimations>();
            playerStats.playerNum = i + 1;
            playerStats.ResetValues();

            playerAnimations.spriteRenderer.color = playersColors[i];
            Debug.Log("Changed color");


        }
    }

    IEnumerator ShowScore()
    {
        scoreText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        scoreText.gameObject.SetActive(false);
    }


    
    IEnumerator UpdateBlurPanel()
    {
        yield return new WaitForSeconds(0);
        blur.Lerp(blur, unblur, 3);
    }
    





    void ResetPlayers()
    {
        GameObject[] playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");

        for (int i = 0; i < playersList.Count; i++)
        {
            GameObject p = playersList[i];

            p.transform.position = playerSpawns[i].transform.position;
            p.transform.rotation = playerSpawns[i].transform.rotation;
            p.GetComponent<PlayerStats>().ResetValues();
            p.GetComponent<PlayerAnimations>().ResetAnims();
        }
    }
}
