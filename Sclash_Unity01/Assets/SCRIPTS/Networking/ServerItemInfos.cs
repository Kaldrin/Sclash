using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun;





// Script for the room item for the rooms list
public class ServerItemInfos : MonoBehaviour
{
    GameObject roomInfosPanel;
    //Transform[] roomInfosElements;
    public RoomInfo room;
    public bool selected;
    [HideInInspector] public ServerListManager serverListManager = null;
    [HideInInspector] public MenuBrowser serverListBrowser = null;
    [HideInInspector] public MenuBrowser multiplayerButtonsBrowser = null;



    [Header("ROOM SETTINGS")]
    public string roomName = "Room";
    public int roomPlayerCount = 0;
    public int roomMaxPlayerCount = 2;
    public int roundCount = 5;
    public string roomGameMode;
    public string currentMap;




    [Header("DISPLAY ELEMENTS")]
    [SerializeField] public TextMeshProUGUI roomNameDisplay = null;
    [SerializeField] public Image bgImage = null;
    [SerializeField] public Image selectedImage = null;



    // DISPLAY ROOM SPECS REFERENCES
    [HideInInspector] public TextMeshProUGUI roomInfosDisplayName = null;
    [HideInInspector] public TextMeshProUGUI roomInfosDisplayCurrentPlayers = null;
    [HideInInspector] public TextMeshProUGUI roomInfosDisplayMaxPlayers = null;
    [HideInInspector] public TextMeshProUGUI roomInfosDisplayRounds = null;




    // AUDIO
    [HideInInspector] public PlayRandomSoundInList clickFXAudioSourceRef = null;











    #region FUNCTIONS
    private void Awake()                                                                                    // AWAKE
    {
        // Get room display components
        if (roomNameDisplay == null)
            if (transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>())
                roomNameDisplay = transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        if (bgImage == null)
            if (transform.GetChild(0).gameObject.GetComponent<Image>())
                bgImage = transform.GetChild(0).gameObject.GetComponent<Image>();

        /*
        roomInfosPanel = GameObject.Find("RoomInfo_Panel");


        if (roomInfosPanel != null)
            roomInfosElements = roomInfosPanel.GetComponentsInChildren<Transform>();
        else
            Debug.LogError("Room info panel not found");
            */
    }


    


    // DISPLAY ROOM SPECS ON THE SIDE
    public void DisplayInfos()
    {
        /*
        for (int i = 0; i < roomInfosElements.Length; i++)
            switch (i)
            {
                case 1:
                    roomInfosElements[i].GetComponent<TextMeshProUGUI>().text = roomName;
                    break;

                case 2:
                    roomInfosElements[i].GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/" + roomMaxPlayerCount;
                    break;

                case 3:
                    roomInfosElements[i].GetComponent<TextMeshProUGUI>().text = roundCount.ToString(); ;
                    break;

                default:
                    break;
            }
            */

        // NAME
        if (roomInfosDisplayName != null)
            roomInfosDisplayName.text = roomName;
        else
        {
            Debug.Log("Error, can't find the room infos display name Text Mesh Pro UGUI component, ignoring");
            roomInfosDisplayName.text = serverListManager.namePlaceholder;
        }


        // CURRENT PLAYERS
        if (roomInfosDisplayCurrentPlayers != null)
            roomInfosDisplayCurrentPlayers.text = room.PlayerCount.ToString();
        else
        {
            Debug.Log("Error, can't find the room infos display current players Text Mesh Pro UGUI component, ignoring");
            roomInfosDisplayCurrentPlayers.text = serverListManager.currentPlayersPlaceholder;
        }

        
        // MAX PLAYERS
        if (roomInfosDisplayMaxPlayers != null)
            roomInfosDisplayMaxPlayers.text = roomMaxPlayerCount.ToString();
        else
        {
            Debug.Log("Error, can't find the room infos display max players Text Mesh Pro UGUI component, ignoring");
            roomInfosDisplayMaxPlayers.text = serverListManager.maxPlayersPlaceholder;
        }


        // ROUNDS
        if (roomInfosDisplayRounds != null)
            roomInfosDisplayRounds.text = roundCount.ToString();
        else
        {
            Debug.Log("Error, can't find the room infos display rounds Text Mesh Pro UGUI component, ignoring");
            roomInfosDisplayRounds.text = serverListManager.roundsPlacerholder;
        }
    }





    #region SELECTION
    // Because it's a prefab I can't pass references to scene objects in the event trigger, I have to use variables references and a function triggered by the event trigger
    public void Hover()
    {
        if (multiplayerButtonsBrowser != null)
            multiplayerButtonsBrowser.enabled = false;
        else
            Debug.Log("Error, can't find multiplayer buttons browser, ignoring, navigation entraved");


        if (serverListBrowser != null)
            serverListBrowser.enabled = true;
        else
            Debug.Log("Error, can't find server list browser, ignoring, navigation entraved");



        serverListBrowser.SelectHoveredElement(gameObject);
    }


    public void SelectServer()
    {
        /*
        ServerItemInfos[] serverItemList = FindObjectsOfType<ServerItemInfos>();


        foreach (ServerItemInfos item in serverItemList)
            item.selected = false;
            */


        for (int i = 0; i < serverListManager.serverItemsList.Count; i++)
            serverListManager.serverItemsList[i].UnSelect();

        serverListManager.DisplayJoinRoomButton(true);

        selected = true;


        // AUDIO
        if (clickFXAudioSourceRef != null)
            clickFXAudioSourceRef.Play();
        else
            Debug.Log("Error, can't find click SFX Play Random Sound In List ref, ignoring");
    }


    public void UnSelect()
    {
        selected = false;


        // DIPSLAY
        if (selectedImage != null)
            selectedImage.gameObject.SetActive(false);
        else
            Debug.Log("Can't find selected image for this server item, ignoring");
    }
    #endregion











    // EDITOR
    private void OnDrawGizmosSelected()
    {
        // Get room display components
        if (roomNameDisplay == null)
            if (transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>())
                roomNameDisplay = transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        if (bgImage == null)
            if (transform.GetChild(0).gameObject.GetComponent<Image>())
                bgImage = transform.GetChild(0).gameObject.GetComponent<Image>();
    }
    #endregion
}
