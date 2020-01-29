using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class ServerItemInfos : MonoBehaviour
{

    GameObject roomInfosPanel;
    Transform[] roomInfosElements;

    public RoomInfo room;

    public bool selected;

    public string roomName;
    public int roomPlayerCount;
    public int roomMaxPlayerCount;
    public int roundCount;
    public string roomGameMode;
    public string currentMap;

    private void Awake()
    {
        roomInfosPanel = GameObject.Find("RoomInfo_Panel");

        if (roomInfosPanel == null)
        {
            Debug.LogError("Room info panel not found");
            return;
        }


        roomInfosElements = roomInfosPanel.GetComponentsInChildren<Transform>();
    }

    public void DisplayInfos()
    {
        

        for (int i = 0; i < roomInfosElements.Length; i++)
        {
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
        }
    }

    public void SelectServer()
    {
        ServerItemInfos[] serverItemList = FindObjectsOfType<ServerItemInfos>();
        foreach(ServerItemInfos item in serverItemList)
        {
            item.selected = false;
        }

        selected = true;
    }
}
