using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;



// Online script
//Created for Unity 2019.1.1f1
public class ServerListManager : MonoBehaviourPunCallbacks
{
    public static ServerListManager Instance = null;


    [SerializeField] GameObject serverItemPrefab = null;
    [SerializeField] TMP_InputField serverParameters = null;
    [SerializeField] TextMeshProUGUI placeholderText = null;


    public List<RoomInfo> roomInfosList = new List<RoomInfo>();
    public ServerFinder serverFinder = null;

    /*public string[] serverNames;
    public string[] serverIPs;*/









    #region BASE FUNCTIONS
    private void Awake()                                            // AWAKE
    {
        Instance = this;
    }


    private void FixedUpdate()                                                      // FIXED UPDATE
    {
        if (enabled && isActiveAndEnabled)
        {
            if (roomInfosList == null)
                return;


            if (roomInfosList.Count == 0)
            {
                if (placeholderText != null)
                    placeholderText.enabled = true;
                else
                    Debug.Log("Placeholder text not found, ignoring");
            }
            else
                placeholderText.enabled = false;
        }
    }


    public override void OnEnable()                                                 // ON ENABLE
    {
        DisplayServerList();
    }


    public override void OnDisable()                                                    // ON DISABLE
    {
        ClearServerList();
    }
    #endregion





    // CLEAR
    private void ClearServerList()
    {
        foreach (Transform c in transform)
            Destroy(c.gameObject);
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


        //Instantiate a server item for each server founds
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
        serverItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (string)roomInfosList[i].CustomProperties["rn"];
        serverItem.GetComponent<ServerItemInfos>().roomName = (string)roomInfosList[i].CustomProperties["rn"];
        int.TryParse((string)roomInfosList[i].CustomProperties["rc"], out serverItem.GetComponent<ServerItemInfos>().roundCount);


        //serverItem.GetComponent<ServerItemInfos>().roomMaxPlayerCount = int.Parse((string)roomInfosList[i].CustomProperties["pc"]);


        serverItem.GetComponent<ServerItemInfos>().room = roomInfosList[i];


        if (i % 2 == 1)
        {
            //Invert Rotation and color it slightly red
            serverItem.GetComponent<Image>().color = new Color32(231, 223, 223, 255);
            serverItem.transform.localScale = new Vector3(1, -1, 1);
            serverItem.transform.GetChild(0).localScale = new Vector3(1, -1, 1);
        }
    }
}
