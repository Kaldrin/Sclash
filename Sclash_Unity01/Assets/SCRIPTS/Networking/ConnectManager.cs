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

    string gameVersion = "2.7";
    public bool connectedToMaster = false;
    public bool enableMultiplayer;

    public GameObject localPlayer;
    public int localPlayerNum;

    [SerializeField] GameObject[] spawnlist;

    [SerializeField] TMP_InputField[] parametersInputs;

    string[] newRoomParameters = { "rn", "pc", "rc" };
    ExitGames.Client.Photon.Hashtable customRoomProperties;

    void Awake()
    {
        Instance = this;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NetworkingClient.EnableLobbyStatistics = true;
    }

    void Start()
    {
        if (enableMultiplayer)
            Connect();
    }

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

        if (parametersInputs[1].text != "")
            options.MaxPlayers = byte.Parse(parametersInputs[1].text);
        else
            options.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(randRoomName, options);

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning(cause);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name);

        localPlayerNum = PhotonNetwork.CurrentRoom.PlayerCount;

        GameObject[] spawners = GameManager.Instance.playerSpawns;

        Vector3 spawnPos = spawners[PhotonNetwork.CurrentRoom.PlayerCount - 1].transform.position;
        Debug.LogFormat("Spawning on spawner {0} : {1}", PhotonNetwork.CurrentRoom.PlayerCount - 1, spawnPos);

        PlayerStats playerStats;
        PlayerAnimations playerAnimations;
        PlayerAttack playerAttack;

        GameObject newPlayer = PhotonNetwork.Instantiate("PlayerNetwork", spawnPos, Quaternion.identity);
        localPlayer = newPlayer;


        if (!GameManager.Instance.playersList.Contains(newPlayer))
            GameManager.Instance.playersList.Add(newPlayer);

        playerStats = newPlayer.GetComponent<PlayerStats>();
        playerAnimations = newPlayer.GetComponent<PlayerAnimations>();
        playerAttack = newPlayer.GetComponent<PlayerAttack>();

        //playerStats.networkPlayerNum = PhotonNetwork.CurrentRoom.PlayerCount;
        //newPlayer.name = "Player" + playerStats.networkPlayerNum;

        if (PhotonNetwork.CurrentRoom.PlayerCount > 0)
            playerStats.playerNum = 1;
        else
            Debug.LogError("Player count error ");

        //playerStats.ResetValues();

        /* playerAnimations.spriteRenderer.color = GameManager.Instance.playersColors[PhotonNetwork.CurrentRoom.PlayerCount - 1];
         playerAnimations.legsSpriteRenderer.color = GameManager.Instance.playersColors[PhotonNetwork.CurrentRoom.PlayerCount - 1];
         playerAnimations.playerLight.color = GameManager.Instance.playersColors[PhotonNetwork.CurrentRoom.PlayerCount - 1];*/

        playerAnimations.spriteRenderer.sortingOrder = 10 * PhotonNetwork.CurrentRoom.PlayerCount - 1;
        playerAnimations.legsSpriteRenderer.sortingOrder = 10 * PhotonNetwork.CurrentRoom.PlayerCount - 1;
        playerAnimations.legsMask.GetComponent<SpriteMask>().frontSortingOrder = 10 * (PhotonNetwork.CurrentRoom.PlayerCount + 2);
        playerAnimations.legsMask.GetComponent<SpriteMask>().backSortingOrder = 10 * (PhotonNetwork.CurrentRoom.PlayerCount - 2);



        ParticleSystem attackSignParticles = playerAttack.attackSign.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule attackSignParticlesMain = attackSignParticles.main;
        // attackSignParticlesMain.startColor = GameManager.Instance.attackSignColors[PhotonNetwork.CurrentRoom.PlayerCount - 1];

        Debug.Log("PUN : OnJoinedRoom() called by PUN. Player is now in a room");
        Debug.LogFormat("Players in room : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

        GetAllPlayers();

        GameManager.Instance.StartMatch();
    }

    void GetAllPlayers()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount != GameManager.Instance.playersList.Count)
        {
            GameManager.Instance.playersList.Clear();

            Debug.Log("Update playerlists");

            PlayerStats[] playersStats = FindObjectsOfType<PlayerStats>();
            foreach (PlayerStats p in playersStats)
            {
                GameManager.Instance.playersList.Add(p.gameObject);
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

                Debug.Log("All players are here");
                CameraManager.Instance.FindPlayers();
            }
        }
    }

    IEnumerator WaitForInstantiation()
    {
        yield return new WaitForSeconds(0.5f);
        GetAllPlayers();
    }

    public override void OnLeftRoom()
    {

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
    {
        Debug.LogFormat("{0} joined the room", other.NickName);
        Debug.LogFormat("Players in room : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

        /*GameObject otherPlayer = GameManager.Instance.GetOtherPlayer(localPlayer);
        if (otherPlayer == null)
        {
            GetAllPlayers();
            return;
        }

        GameManager.Instance.playersList.Add(otherPlayer);*/
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