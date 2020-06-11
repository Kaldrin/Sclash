using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class ColliderSync : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    public GameObject colliderObject;
    [SerializeField]
    public Collider2D[] col;


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (col != null)
            {
                stream.SendNext(col[0].isTrigger);
                stream.SendNext(col[1].isTrigger);
                stream.SendNext(col[2].isTrigger);
            }
        }

        if (stream.IsReading)
        {
            if (col != null)
            {
                this.col[0].isTrigger = (bool)stream.ReceiveNext();
                this.col[1].isTrigger = (bool)stream.ReceiveNext();
                this.col[2].isTrigger = (bool)stream.ReceiveNext();
            }
        }
    }
}
