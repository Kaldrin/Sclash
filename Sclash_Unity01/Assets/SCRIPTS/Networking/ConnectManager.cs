using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class ConnectManager : MonoBehaviourPunCallbacks, IConnectionCallbacks
{

    public static ConnectManager Instance;

    AudioManager audioManager;

    string gameVersion = "2.7";
    public bool connectedToMaster = false;
    public bool enableMultiplayer;

    public GameObject localPlayer;
    public int localPlayerNum;

    GameObject[] spawners;

    [SerializeField] GameObject[] spawnlist;

    [SerializeField] TMP_InputField[] parametersInputs;

    string[] newRoomParameters = { "rn", "pc", "rc" };
    ExitGames.Client.Photon.Hashtable customRoomProperties;

    #region Base methods

    void Awake()
    {
        Instance = this;
        audioManager = FindObjectOfType<AudioManager>();
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NetworkingClient.EnableLobbyStatistics = true;
    }

    void Start()
    {
        if (enableMultiplayer)
            Connect();

        spawners = GameObject.FindGameObjectsWithTag("PlayerSpawn");
    }
    
    #endregion

    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void JoinRoom()
    {
        if (!connectedToMaster)
        {
            if (!enableMultiplayer)
            {
                GameManager.Instance.StartMatch();
            }
            else
            {
                Debug.LogWarning("Can't join room if you're not connected");
            }
            return;
        }

        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinSelectedRoom(TextMeshProUGUI roomName)
    {
        PhotonNetwork.JoinRoom(roomName.text);
    }
    

    #region Init room

    public void InitMultiplayerMatch()
    {
        // DELETE EXISTING PLAYERS IN SCENE //
        Player[] oldPlayers = FindObjectsOfType<Player>();
        foreach (Player p in oldPlayers)
        {
            Destroy(p.gameObject);
        }

        GameManager.Instance.playersList.Clear();

        //  JOIN OR CREATE ROOM  //
        JoinRoom();

    }

    IEnumerator JoinRoomCoroutine()
    {
        // INSTANTIATE PLAYER //
        ///Get player spawns
        Vector3 spawnPos = spawners[PhotonNetwork.CurrentRoom.PlayerCount - 1].transform.position;

        GameObject newPlayer = PhotonNetwork.Instantiate("PlayerNetwork", spawnPos, Quaternion.identity);
        newPlayer.name = "Player" + PhotonNetwork.CurrentRoom.PlayerCount;

        CameraManager.Instance.FindPlayers();

        Debug.LogFormat("Spawning on spawner {0} : {1}", PhotonNetwork.CurrentRoom.PlayerCount - 1, spawnPos);

        Player stats = newPlayer.GetComponent<Player>();
        PlayerAnimations playerAnimations = newPlayer.GetComponent<PlayerAnimations>();


        stats.playerNum = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        stats.ResetAllPlayerValuesForNextMatch();

        ///Input setup
        InputManager.Instance.playerInputs = new InputManager.PlayerInputs[1];

        ///Animation setup
        playerAnimations.spriteRenderer.color = GameManager.Instance.playersColors[stats.playerNum];
        playerAnimations.legsSpriteRenderer.color = GameManager.Instance.playersColors[stats.playerNum];

        playerAnimations.spriteRenderer.sortingOrder = 10 * stats.playerNum;
        playerAnimations.legsSpriteRenderer.sortingOrder = 10 * stats.playerNum;
        playerAnimations.legsMask.GetComponent<SpriteMask>().frontSortingOrder = 10 * stats.playerNum + 2;
        playerAnimations.legsMask.GetComponent<SpriteMask>().backSortingOrder = 10 * stats.playerNum - 2;

        ///FX Setup
        ParticleSystem attackSignParticles = stats.attackRangeFX.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule attackSignParticlesMain = attackSignParticles.main;
        attackSignParticlesMain.startColor = GameManager.Instance.attackSignColors[stats.playerNum];

        stats.playerLight.color = GameManager.Instance.playerLightsColors[stats.playerNum];


        if (!GameManager.Instance.playersList.Contains(newPlayer))
            GameManager.Instance.playersList.Add(newPlayer);


        yield return new WaitForEndOfFrame();

        // INIT MATCH //
        stats.SwitchState(Player.STATE.sneathed);

        /// Wait 0.1 seconds
        yield return new WaitForSeconds(0.1f);

        GameManager.Instance.SwitchState(GameManager.GAMESTATE.game);
        CameraManager.Instance.SwitchState(CameraManager.CAMERASTATE.battle);

        audioManager.ActivateWind();

        /// Wait 0.5 seconds
        yield return new WaitForSeconds(0.5f);

        audioManager.matchBeginsRandomSoundSource.Play();

        Debug.Log("PUN : OnJoinedRoom() called by PUN. Player is now in a room");
        Debug.LogFormat("Players in room : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

        CameraManager.Instance.actualXSmoothMovementsMultiplier = CameraManager.Instance.battleXSmoothMovementsMultiplier;
        CameraManager.Instance.actualZoomSpeed = CameraManager.Instance.battleZoomSpeed;
        CameraManager.Instance.actualZoomSmoothDuration = CameraManager.Instance.battleZoomSmoothDuration;
    }

    #endregion


    #region Monobehaviour Callbacks

    public override void OnConnectedToMaster()
    {
        connectedToMaster = true;
        Debug.Log("PUN : OnConnectedToMaster() was called by PUN");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("PUN : OnJoinedLobby() was called by PUN");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarningFormat("PUN : Failed to join a room");
        Debug.Log("PUN : CreateRoom called");

        string randRoomName = Time.time.ToString();

        RoomOptions options = new RoomOptions();
        options.CustomRoomPropertiesForLobby = newRoomParameters;

        customRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {"rn", randRoomName },
            {"pc", "2" },
            {"rc", "10"}
        };

        options.CustomRoomProperties = customRoomProperties;

        if (parametersInputs.Length > 0)
        {
            if (parametersInputs[1].text != "")
                options.MaxPlayers = byte.Parse(parametersInputs[1].text);
            else
                options.MaxPlayers = 2;
        }

        PhotonNetwork.CreateRoom(randRoomName, options);

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning(cause);
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(JoinRoomCoroutine());
    }

    [PunRPC]
    void GetAllPlayers()
    {
        Debug.Log("Get all players in room");
        if (PhotonNetwork.CurrentRoom.PlayerCount != GameManager.Instance.playersList.Count)
        {
            GameManager.Instance.playersList.Clear();

            Debug.Log("Update playerlists");

            Player[] playersStats = FindObjectsOfType<Player>();
            for(int i = 0; i < playersStats.Length; i++)
            {
                GameManager.Instance.playersList.Add(playersStats[i].gameObject);

                //Instantiate 2nd player
                Player p2 = playersStats[i];
                PlayerAnimations p2Anim = playersStats[i].gameObject.GetComponent<PlayerAnimations>();

                p2Anim.spriteRenderer.color = GameManager.Instance.playersColors[p2.playerNum];
                p2Anim.legsSpriteRenderer.color = GameManager.Instance.playersColors[p2.playerNum];

                p2Anim.spriteRenderer.sortingOrder = 10 * p2.playerNum;
                p2Anim.legsSpriteRenderer.sortingOrder = 10 * p2.playerNum;
                p2Anim.legsMask.GetComponent<SpriteMask>().frontSortingOrder = 10 * p2.playerNum + 2;
                p2Anim.legsMask.GetComponent<SpriteMask>().backSortingOrder = 10 * p2.playerNum - 2;

                ///FX Setup
                ParticleSystem attackSignParticles = p2.attackRangeFX.GetComponent<ParticleSystem>();
                ParticleSystem.MainModule attackSignParticlesMain = attackSignParticles.main;
                attackSignParticlesMain.startColor = GameManager.Instance.attackSignColors[p2.playerNum];

                p2.playerLight.color = GameManager.Instance.playerLightsColors[p2.playerNum];
            }


            if (PhotonNetwork.CurrentRoom.PlayerCount != GameManager.Instance.playersList.Count)
            {
                StartCoroutine(WaitForInstantiation());
            }
            else
            {
                for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
                {
                    Debug.LogFormat("Player list n°{0} : {1}",
                        i,
                        GameManager.Instance.playersList[i]
                    );

                    if (GameManager.Instance.playersList[i] == null)
                    {
                        StartCoroutine(WaitForInstantiation());
                        return;
                    }
                }

                foreach (Player p in playersStats)
                {
                    p.ManageOrientation();
                }

                Debug.Log("All players are here");
                CameraManager.Instance.FindPlayers();
            }
        }
    }

    IEnumerator WaitForInstantiation()
    {
        yield return new WaitForSeconds(0.5f);
        photonView.RPC("GetAllPlayers", RpcTarget.All);
    }

    public override void OnLeftRoom()
    {

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
    {
        Debug.LogFormat("{0} joined the room", other.NickName);
        Debug.LogFormat("Players in room : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

        GameObject otherPlayer = GameManager.Instance.GetOtherPlayer(localPlayer);
        if (otherPlayer == null)
        {
            photonView.RPC("GetAllPlayers", RpcTarget.All);
            return;
        }

        GameManager.Instance.playersList.Add(otherPlayer);
    }

    public void CreateCustomRoom()
    {
        RoomOptions options = new RoomOptions();
        options.CustomRoomPropertiesForLobby = newRoomParameters;

        customRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {"rn", parametersInputs[0].text },
            {"pc", parametersInputs[1].text },
            {"rc", parametersInputs[2].text }
        };

        options.CustomRoomProperties = customRoomProperties;

        if (parametersInputs[1].text != "")
            options.MaxPlayers = byte.Parse(parametersInputs[1].text);
        else
            options.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(parametersInputs[0].text, options);
    }

    public void WaitSceneLoading()
    {
        StartCoroutine("WaitSceneLoadingCoroutine");
    }

    IEnumerator WaitSceneLoadingCoroutine()
    {
        yield return new WaitForSeconds(2f);

        GameObject.Find("RightPanel").GetComponent<Animator>().Play("SlideOut");
        GameObject.Find("LeftPanel").GetComponent<Animator>().Play("SlideOut");
        GameObject.Find("MenuLayout").SetActive(false);
        GameObject.Find("BackIndicator").SetActive(false);

        yield return new WaitForSeconds(0.3f);

        GameObject.Find("MultiplayerMenu").SetActive(false);

    }

    #endregion
}