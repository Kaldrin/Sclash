using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // AUDIO
    [SerializeField] string audioManagerName = "GlobalManager";
    AudioManager audioManager = null;



    // CAMERA
    [SerializeField] CameraManager cameraManager = null;



    // MENU
    [SerializeField] GameObject mainMenu = null;
    [SerializeField] GameObject blurPanel = null;





    // ROUND
    [SerializeField] float timeBeforeNextRound = 3;
    [SerializeField] ParticleSystem roundLeaves = null;
    public bool gameStarted = false;





    // PLAYERS
    [SerializeField] GameObject player = null;
    List<GameObject> playersList = new List<GameObject>();
    [SerializeField] Color[] playersColors = null;




    // EFFECTS
    [SerializeField] float roundEndSlowMoTimeScale = 0.3f;
    [SerializeField] float rounEndSlowMoDuration = 2f;




    // SCORE
    [SerializeField] Text scoreText = null;
    public Vector2 score = new Vector2(0, 0);
    [SerializeField] float scoreShowDuration = 4f;





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


    // BEGIN GAME

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



        gameStarted = true;


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
        }
    }

    /*
    IEnumerator UpdateBlurPanel()
    {
        yield return new WaitForSeconds(0);
    }
    */




    // END & NEXT ROUND

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






    // SCORE
    IEnumerator ShowScore()
    {
        scoreText.gameObject.SetActive(true);
        yield return new WaitForSeconds(scoreShowDuration);
        scoreText.gameObject.SetActive(false);
    }

    IEnumerator Score(int playerNum)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        score[playerNum - 1] += 1;
        scoreText.text = ScoreBuilder();

        yield return new WaitForSecondsRealtime(timeBeforeNextRound);

        
        NextRound();
    }

    string ScoreBuilder()
    {
        string scoreString = "<color=#FF0000>" + score[0].ToString() + "</color> / <color=#0000FF>" + score[1].ToString() + "</color>";
        return scoreString;
    }

    

    // EFFECTS

    public void EndRoundSlowMo()
    {
        StartCoroutine(EndRoundSlowMoCoroutine());
    }

    IEnumerator EndRoundSlowMoCoroutine()
    {
        
        Time.timeScale = roundEndSlowMoTimeScale;
        yield return new WaitForSecondsRealtime(rounEndSlowMoDuration);
        Time.timeScale = 1;
    }





    
}
