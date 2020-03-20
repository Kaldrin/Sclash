using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAScript : MonoBehaviour
{
    Player attachedPlayer;

    float rand;

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

    void Update()
    {
        rand = Random.Range(0f, 1f);

        Debug.Log(GameManager.Instance.gameState);

        if (GameManager.Instance.gameState == GameManager.GAMESTATE.game)
        {
            Debug.Log("Game Started !");
            Debug.Log("Player is :" + attachedPlayer.playerState);
            if (attachedPlayer.playerState == Player.STATE.sneathed)
            {
                StartCoroutine(WaitABit("Called"));
            }
        }
    }

    void Called()
    {
        Debug.Log("Hey, who called me !");
        attachedPlayer.TriggerDraw();
    }

    IEnumerator WaitABit(string calledFunc = null)
    {
        Debug.Log("Wait " + rand + "sec");
        yield return new WaitForSecondsRealtime(rand);

        if (calledFunc != null)
        {
            Invoke(calledFunc, 0);
        }
    }

}
