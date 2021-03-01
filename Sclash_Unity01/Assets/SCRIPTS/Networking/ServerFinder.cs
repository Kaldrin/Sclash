using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;



// Script that handles the finding of servers for the online mode
public class ServerFinder : MonoBehaviourPunCallbacks
{
    private void Awake()                                                // AWAKE
    {
        //roomInfoList = new List<RoomInfo>();
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ServerListManager.Instance.roomInfosList = roomList;


        ServerListManager.Instance.DisplayServerList();
    }


    new void OnDisable()                                                            // On disable
    {
        if (PhotonNetwork.InRoom)
            return;

        ConnectManager.Instance.LeaveLobby();
    }
}
