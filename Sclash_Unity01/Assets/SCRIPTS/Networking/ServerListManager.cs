using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;



// Online script
// Created for Unity 2019.1.1f1
public class ServerListManager : MonoBehaviourPunCallbacks
{
    // SINGLETON
    public static ServerListManager Instance = null;


    [Header("PREFABS")]
    [SerializeField] GameObject serverItemPrefab = null;



    [Header("BROWSER ELEMENTS")]
    [SerializeField] MenuBrowser serversListMenuBrowser = null;
    [SerializeField] MenuBrowser multiplayerButtonsBrowser = null;
    /// <summary>
    /// The reference to the input field the player can fill to search servers
    /// </summary>
    [SerializeField] TMP_InputField serverParameters = null;
    [SerializeField] GameObject placeholderText = null;



    // STUFF
    public List<RoomInfo> roomInfosList = new List<RoomInfo>();
    public ServerFinder serverFinder = null;
    [HideInInspector] public List<ServerItemInfos> serverItemsList = new List<ServerItemInfos>();



    [Header("ROOM SPECS DISPLAY REFERENCES")]
    [SerializeField] public TextMeshProUGUI roomInfosDisplayName = null;
    [SerializeField] public TextApparition roomInfosDisplayNameTextApparitionComponent = null;
    [SerializeField] public TextMeshProUGUI roomInfosDisplayCurrentPlayers = null;
    [SerializeField] public TextMeshProUGUI roomInfosDisplayMaxPlayers = null;
    [SerializeField] public TextMeshProUGUI roomInfosDisplayRounds = null;




    [Header("MENU")]
    [SerializeField] TextMeshProUGUI joinRoomText = null;
    [SerializeField] TextApparition joinRoomButtonTextApparitionCOmponent = null;
    [SerializeField] string matchmakingButtonName = "Matchmaking";
    [SerializeField] string joinServerButtonName = "Join room";
    [SerializeField] string matchmakingButtonNameKey = "OM_B_Matchmaking01";
    [SerializeField] string joinServerButtonNameKey = "OM_B_JoinRoom01";




    [Header("SERVER DISPLAY ERROR PLACEHOLDERS")]
    [SerializeField] public string namePlaceholder = "Error displaying info";
    [SerializeField] public string namePlaceholderKey = "OM_RoomName01";
    [SerializeField] public string currentPlayersPlaceholder = "?";
    [SerializeField] public string maxPlayersPlaceholder = "?";
    [SerializeField] public string roundsPlacerholder = "?";





    [Header("AUDIO")]
    [SerializeField] PlayRandomSoundInList clickFXAudioSourceRef = null;















    #region FUNCTIONS
    #region BASE FUNCTIONS
    private void Awake()                                            // AWAKE
    {
        Instance = this;
    }


    private void FixedUpdate()                                                      // FIXED UPDATE
    {
        if (enabled && isActiveAndEnabled)
        {
            // If there's no servers, just don't do anything
            if (roomInfosList == null)
            {
                Debug.Log("No server found");
                return;
            }



            if (roomInfosList.Count == 0)
            {
                if (placeholderText != null)
                {
                    if (!placeholderText.activeInHierarchy)
                        placeholderText.SetActive(true);
                }
                else
                    Debug.Log("Placeholder text not found, ignoring");
            }
            else if (placeholderText.activeInHierarchy)
                placeholderText.SetActive(false);
        }
    }


    public override void OnEnable()                                                 // ON ENABLE
    {
        DisplayServerList();
        FillPlaceholderInfos();

        // Set up join button
        DisplayJoinRoomButton(false);
    }


    public override void OnDisable()                                                    // ON DISABLE
    {
        ClearServerList();
    }
    #endregion











    // PLACEHOLDER
    public void FillPlaceholderInfos()
    {
        /*
        if (roomInfosDisplayName != null)
            roomInfosDisplayName.text = namePlaceholder;
            */
        if (roomInfosDisplayNameTextApparitionComponent != null)
        {
            roomInfosDisplayNameTextApparitionComponent.textKey = namePlaceholderKey;
            roomInfosDisplayNameTextApparitionComponent.TransfersTrad();
        }
        if (roomInfosDisplayCurrentPlayers != null)
            roomInfosDisplayCurrentPlayers.text = currentPlayersPlaceholder;
        if (roomInfosDisplayMaxPlayers != null)
            roomInfosDisplayMaxPlayers.text = maxPlayersPlaceholder;
        if (roomInfosDisplayRounds != null)
            roomInfosDisplayRounds.text = roundsPlacerholder;
    }



    // CLEAR
    private void ClearServerList()
    {
        // Remove elements from browsing
        for (int i = 0; i < serverItemsList.Count; i++)
            serversListMenuBrowser.RemoveElement(serverItemsList[i].gameObject);


        // DESTROY ELEMENTS
        for (int i = 0; i < serverItemsList.Count; i++)
            Destroy(serverItemsList[i].gameObject);

        foreach (Transform c in transform)
            Destroy(c.gameObject);


        // Clear list of elements
        serverItemsList.Clear();
    }






    // DISPLAY
    public void DisplayServerList()
    {
        //Remove all instanciated server items
        ClearServerList();


        if (roomInfosList == null)
        {
            Debug.Log("roomInfoList is null");
            return;
        }


        // Instantiate a server item for each server founds
        for (int i = 0; i < roomInfosList.Count; i++)
            if (serverParameters.text != "")
            {
                if (roomInfosList[i].Name.ToLower() == serverParameters.text.ToLower())
                    InstantiateServerItem(i);
            }
            else
                InstantiateServerItem(i);
    }


    private void InstantiateServerItem(int i)
    {
        Debug.Log("Instantiate Server item");
        // Instantiates the server item
        GameObject serverItem = Instantiate(serverItemPrefab, transform);
        ServerItemInfos serverItemScript = serverItem.GetComponent<ServerItemInfos>();



        // FILL INFOS
        serverItemScript.roomName = (string)roomInfosList[i].CustomProperties["rn"];
        serverItemScript.room = roomInfosList[i];


        // PASS REFERENCES
        serverItemScript.serverListBrowser = serversListMenuBrowser;
        serverItemScript.multiplayerButtonsBrowser = multiplayerButtonsBrowser;
        serverItemScript.clickFXAudioSourceRef = clickFXAudioSourceRef;
        serverItemScript.serverListManager = this;



        // DISPLAY NAME
        // Fill room display name
        //serverItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (string)roomInfosList[i].CustomProperties["rn"];
        if (serverItemScript.roomNameDisplay == null)
            if (serverItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>())
                serverItemScript.roomNameDisplay = serverItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        if (serverItemScript.roomNameDisplay != null)
            serverItemScript.roomNameDisplay.text = serverItemScript.roomName;
        else
            Debug.Log("Couldn't find server item display name component, ignoring");




        // PASS DISPLAY COMPONENTS REFERENCES
        if (roomInfosDisplayName != null)
            serverItemScript.roomInfosDisplayName = roomInfosDisplayName;
        if (roomInfosDisplayCurrentPlayers != null)
            serverItemScript.roomInfosDisplayCurrentPlayers = roomInfosDisplayCurrentPlayers;
        if (roomInfosDisplayMaxPlayers != null)
            serverItemScript.roomInfosDisplayMaxPlayers = roomInfosDisplayMaxPlayers;
        if (roomInfosDisplayRounds != null)
            serverItemScript.roomInfosDisplayRounds = roomInfosDisplayRounds;





        // ???
        int.TryParse((string)roomInfosList[i].CustomProperties["rc"], out serverItem.GetComponent<ServerItemInfos>().roundCount);
        //serverItem.GetComponent<ServerItemInfos>().roomMaxPlayerCount = int.Parse((string)roomInfosList[i].CustomProperties["pc"]);




        if (i % 2 == 1)
        {
            //Invert Rotation and color it slightly red

            // Image
            if (serverItemScript.bgImage == null)
                if (serverItem.transform.GetChild(0).gameObject.GetComponent<Image>())
                    serverItemScript.bgImage = serverItem.transform.GetChild(0).gameObject.GetComponent<Image>();
            if (serverItemScript.bgImage != null)
                serverItemScript.bgImage.color = new Color32(231, 223, 223, 255);
            else
                Debug.Log("Coudln't find server item background image, ignoring");



            serverItem.transform.localScale = new Vector3(1, -1, 1);
            serverItemScript.roomNameDisplay.transform.localScale = new Vector3(1, -1, 1);
        }



        // Add element to browsing list
        if (serversListMenuBrowser != null)
            serversListMenuBrowser.AddElement(serverItem);
        else
            Debug.Log("Can't find the server list menu browser, ignoring, navigation entraved");


        // Add element to list of items
        serverItemsList.Add(serverItemScript);
    }


    // Changes the text of the join room button depending on if a server is selected
    // Handles language through keys
    public void DisplayJoinRoomButton(bool on = false)
    {
        /*
        if (joinRoomText != null)
        {
            if (!on)
                joinRoomText.text = matchmakingButtonName;
            else
                joinRoomText.text = joinServerButtonName;
        }
        else
            Debug.Log("Can't find join room button, ignoring");
            */

        if (joinRoomButtonTextApparitionCOmponent != null)
        {
            if (on)
                joinRoomButtonTextApparitionCOmponent.textKey = joinServerButtonNameKey;
            else
                joinRoomButtonTextApparitionCOmponent.textKey = matchmakingButtonNameKey;

            joinRoomButtonTextApparitionCOmponent.TransfersTrad();
        }
    }









    // 'cause I'm stressed by these warnings but I don't want to remove these variables;
    void RemoveWarnings()
    {
        matchmakingButtonName += matchmakingButtonName;
        joinServerButtonName += joinServerButtonName;
        joinRoomText.text += joinRoomText.text;
    }
    #endregion
}
