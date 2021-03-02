using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class ServerFinder : MonoBehaviourPunCallbacks, IConnectionCallbacks
{
    private void Awake()
    {
        //roomInfoList = new List<RoomInfo>();
    }

    new void OnEnable()
    {
        ConnectManager.Instance.SetMultiplayer(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate() Called by PUN");
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
