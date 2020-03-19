using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAScript : MonoBehaviour
{
    Player attachedPlayer;

    void Awake()
    {
        attachedPlayer = GetComponent<Player>();
        if (attachedPlayer == null)
        {
            Debug.LogError("Player script not found on GameObject", gameObject);
            return;
        }

        attachedPlayer.playerIsAI = true;
    }

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("Hello, I'm a *FRIENDLY* AI");
    }
}
