using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class IAScript : MonoBehaviour
{
    public class Actions
    {
        public string name;
        public int weight;

        public Actions(string n, int w)
        {
            name = n;
            weight = w;
        }
    }

    [SerializeField]
    Player attachedPlayer;
    [SerializeField]
    Player opponent;

    bool isWaiting;
    bool isClose = true;

    float rand;

    public float DistanceTolerance;

    bool isChoosing = false;
    // Update rate when far 
    [SerializeField]
    float normalRate = 0.25f;
    // Update rate when on close combat
    [SerializeField]
    float closeRate = 0.1f;

    bool nextState;
    float stateTimer;

    [SerializeField]
    List<Actions> actionsList = new List<Actions>()
    {
        new Actions("Wait",1),
        new Actions("Attack",1),
        new Actions("Parry",1),
        new Actions("Pommel",1),
        new Actions("MoveToward",1),
        new Actions("MoveAway",1),
        new Actions("DashToward",1),
        new Actions("DashAway",1),
    };
    [SerializeField]
    int actionWeightSum;

    [Header("Actions weights")]
    public int wait;
    public int attack,
        parry,
        pommel,
        moveToward,
        moveAway,
        dashRight,
        dashLeft;

    void Awake()
    {
        attachedPlayer = GetComponent<Player>();

        FindOpponent();

        if (attachedPlayer == null)
        {
            Debug.LogError("Player script not found on GameObject", gameObject);
            return;
        }

        attachedPlayer.playerIsAI = true;
    }

    void ShowWeight()
    {
        wait = actionsList[0].weight;
        attack = actionsList[1].weight;
        parry = actionsList[2].weight;
        pommel = actionsList[3].weight;
        moveToward = actionsList[4].weight;
        moveAway = actionsList[5].weight;
        dashRight = actionsList[6].weight;
        dashLeft = actionsList[7].weight;
    }

    void FindOpponent()
    {
        foreach (Player p in FindObjectsOfType<Player>())
        {
            if (p != attachedPlayer)
            {
                opponent = p;
                break;
            }
        }

        if (opponent == null)
        {
            Debug.LogWarning("Couldn't find opponent !", gameObject);
            StartCoroutine(WaitABit("FindOpponent", 0.12f));
        }
    }

    void Start()
    {

        Debug.Log("Hello, I'm a *FRIENDLY* AI");
    }

    void Update()
    {

        if (GameManager.Instance.gameState == GameManager.GAMESTATE.game)
        {
            ShowWeight();
            //AI DRAW
            if (attachedPlayer.playerState == Player.STATE.sneathed && !isWaiting)
            {
                isWaiting = true;
                StartCoroutine(WaitABit("TriggerDraw", rand, false));
            }

            if (!isChoosing && attachedPlayer.playerState == Player.STATE.normal)
            {
                isChoosing = true;
                float timeToWait = isClose ? closeRate : normalRate;

                StartCoroutine(WaitABit(ChooseState(), timeToWait, true));
            }

            isClose = Mathf.Abs(attachedPlayer.transform.position.x - opponent.transform.position.x) <= DistanceTolerance ? true : false;
        }
    }

    #region Action function
    void Wait()
    {
        StartCoroutine(WaitAction(.1f));
    }

    void Attack()
    {
        InputManager.Instance.playerInputs[attachedPlayer.playerNum].attack = true;

        float chargeTime = Random.Range(0f, 3f);
        StartCoroutine(WaitABit("ReleaseAttack", chargeTime));
        StartCoroutine(WaitAction(chargeTime + .01f));
    }

    void ReleaseAttack()
    {
        InputManager.Instance.playerInputs[attachedPlayer.playerNum].attack = false;
    }

    void Parry()
    {
        Debug.Log("Parry");

        InputManager.Instance.playerInputs[attachedPlayer.playerNum].parry = true;

        StartCoroutine(WaitABit("ResetInput", 0.5f));
        isChoosing = false;
    }

    void Pommel()
    {
        Debug.Log("Pommel");

        InputManager.Instance.playerInputs[attachedPlayer.playerNum].kick = true;

        StartCoroutine(WaitABit("ResetInput", 0.5f));

        isChoosing = false;
    }

    void MoveToward()
    {
        ManageMovementsInputs((int)Mathf.Sign(opponent.transform.position.x - transform.position.x));

        StartCoroutine(WaitABit("ResetHorizontal", 0.25f));
        isChoosing = false;
    }

    void MoveAway()
    {
        ManageMovementsInputs((int)Mathf.Sign(transform.position.x - opponent.transform.position.x));

        StartCoroutine(WaitABit("ResetHorizontal", 0.25f));
        isChoosing = false;
    }

    void DashToward()
    {
        if (attachedPlayer.stamina >= attachedPlayer.staminaCostForMoves)
            InputManager.Instance.playerInputs[attachedPlayer.playerNum].dash = Mathf.Sign(opponent.transform.position.x - transform.position.x);

        StartCoroutine(WaitABit("ResetInput", 0.15f));
        isChoosing = false;
    }

    void DashAway()
    {
        Debug.Log("DashBackward");

        if (attachedPlayer.stamina >= attachedPlayer.staminaCostForMoves)
            InputManager.Instance.playerInputs[attachedPlayer.playerNum].dash = Mathf.Sign(transform.position.x - opponent.transform.position.x);

        StartCoroutine(WaitABit("ResetInput", 0.5f));
        isChoosing = false;
    }
    #endregion

    string ChooseState()
    {
        //Calculate weight sum
        actionWeightSum = 0;
        foreach (Actions a in actionsList)
        {
            actionWeightSum += a.weight;
        }

        //Select a random action by weight
        int randomAction = Random.Range(1, actionWeightSum);
        string rState = null;
        foreach (Actions item in actionsList)
        {
            randomAction -= item.weight;
            if (randomAction <= 0)
            {
                IncreaseOtherWeights();
                ResetSelfWeight(item);

                rState = item.name;
                break;
            }
        }

        if (rState == "" || rState == null)
        {
            Debug.LogError("No Action selected ! Log : Action sum:" + actionWeightSum + " Random num: " + randomAction, gameObject);
        }

        return rState;
    }

    void IncreaseOtherWeights()
    {
        foreach (Actions a in actionsList)
        {
            a.weight++;
        }
    }

    void ResetSelfWeight(Actions a)
    {
        a.weight = 1;
    }

    #region Coroutines
    /// <summary>
    /// Wait for an amount of time and then Invoke a function. 
    /// Need to tell the name of the function, the amount of time you wan't to wait and if the function is on this script
    /// </summary>
    IEnumerator WaitABit(string calledFunc = null, float t = 0, bool self = true)
    {
        Debug.Log("Wait " + t + "sec and invoke " + calledFunc);
        yield return new WaitForSecondsRealtime(t);
        isWaiting = false;

        if (calledFunc != null)
        {
            if (self)
                Invoke(calledFunc, 0);
            else
                attachedPlayer.Invoke(calledFunc, 0);
        }
    }

    IEnumerator WaitAction(float t)
    {
        yield return new WaitForSecondsRealtime(t);
        isChoosing = false;
    }
    #endregion

    #region Input management
    void ResetHorizontal()
    {
        ManageMovementsInputs(0);
    }

    void ResetInput()
    {
        InputManager.Instance.playerInputs[attachedPlayer.playerNum].pauseUp = false;
        InputManager.Instance.playerInputs[attachedPlayer.playerNum].anyKey = false;
        InputManager.Instance.playerInputs[attachedPlayer.playerNum].dash = 0;
        InputManager.Instance.playerInputs[attachedPlayer.playerNum].attack = false;
        InputManager.Instance.playerInputs[attachedPlayer.playerNum].attackDown = false;
        InputManager.Instance.playerInputs[attachedPlayer.playerNum].kick = false;
        InputManager.Instance.playerInputs[attachedPlayer.playerNum].parry = false;
        InputManager.Instance.playerInputs[attachedPlayer.playerNum].parryDown = false;
        InputManager.Instance.playerInputs[attachedPlayer.playerNum].jump = false;
    }

    void ManageMovementsInputs(int direction = 0)
    {
        attachedPlayer.rb.velocity = new Vector2(direction * attachedPlayer.actualMovementsSpeed, attachedPlayer.rb.velocity.y);
    }
    #endregion
}