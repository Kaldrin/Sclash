using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class IAScript : MonoBehaviour
{
    #region Variables
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
    bool canAddWeight = true;

    float rand;


    public float DistanceTolerance = 3;
    float distBetweenPlayers;
    float hitDistance = 2;

    bool isChoosing = false;
    // Update rate when far 
    [SerializeField]
    float normalRate = 0.25f;
    // Update rate when on close combat
    [SerializeField]
    float closeRate = 0.05f;

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
        new Actions("InterruptAttack",0)
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
        dashToward,
        dashAway,
        interruptAttack;

    #endregion

    #region Built-in methods
    void OnEnable()
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

    void Update()
    {

        if (GameManager.Instance.gameState == GameManager.GAMESTATE.game)
        {
            if (attachedPlayer.playerState != Player.STATE.charging)
            {
                actionsList[8].weight = 0;
            }

            ShowWeight();
            //AI DRAW
            if (attachedPlayer.playerState == Player.STATE.sneathed && !isWaiting)
            {
                isWaiting = true;
                StartCoroutine(WaitABit("TriggerDraw", rand, false));
            }

            if (canAddWeight)
                AddWeights();

            if (!isChoosing && attachedPlayer.playerState == Player.STATE.normal)
            {
                isChoosing = true;
                float timeToWait = isClose ? closeRate : normalRate;

                if (!isClose)
                {
                    IncreaseWeight("MoveToward", 5);
                }

                StartCoroutine(WaitABit(ChooseState(), timeToWait, true));
            }

            distBetweenPlayers = Mathf.Abs(attachedPlayer.transform.position.x - opponent.transform.position.x);
            isClose = distBetweenPlayers <= DistanceTolerance ? true : false;
        }
    }

    #endregion

    void ShowWeight()
    {
        wait = actionsList[0].weight;
        attack = actionsList[1].weight;
        parry = actionsList[2].weight;
        pommel = actionsList[3].weight;
        moveToward = actionsList[4].weight;
        moveAway = actionsList[5].weight;
        dashToward = actionsList[6].weight;
        dashAway = actionsList[7].weight;
        interruptAttack = actionsList[8].weight;
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

    void AddWeights()
    {
        if (opponent.playerState == Player.STATE.dead || attachedPlayer.playerState == Player.STATE.dead)
        {
            foreach (Actions a in actionsList)
            {
                ResetSelfWeight(a);
            }
            return;
        }

        canAddWeight = false;

        if (attachedPlayer.stamina <= 2 && isClose)
        {
            if (attachedPlayer.stamina <= 1)
            {
                IncreaseWeight("MoveAway", 2);
            }
            else
            {
                IncreaseWeight("DashAway", 1);
                IncreaseWeight("MoveAway", 1);
            }
        }

        if (!isClose)
        {
            IncreaseWeight("MoveToward", 1);
            canAddWeight = true;
        }
        else
        {
            switch (opponent.playerState)
            {
                case Player.STATE.charging:
                    IncreaseWeight("Parry", 3);
                    IncreaseWeight("Attack", 2);
                    IncreaseWeight("DashToward", 1);
                    break;

                case Player.STATE.attacking:
                    if (isClose)
                    {
                        if (attachedPlayer.playerState == Player.STATE.charging)
                        {
                            Debug.Log("<color=red>INTERUPT !!!</color>");
                            IncreaseWeight("InterruptAttack", 100);
                        }
                        IncreaseWeight("Parry", 5);
                    }
                    break;

                case Player.STATE.parrying:
                    if (distBetweenPlayers <= hitDistance)
                    {
                        IncreaseWeight("Pommel", 4);
                    }
                    IncreaseWeight("Attack", 2);
                    break;

                case Player.STATE.dashing:
                    if (distBetweenPlayers <= hitDistance)
                        IncreaseWeight("Attack", 5);
                    break;
            }

            StartCoroutine(WaitABit("EnableWeight", 0.15f));
        }
    }

    void EnableWeight()
    {
        canAddWeight = true;
    }



    #region Action function
    void Wait()
    {
        StartCoroutine(WaitAction(.1f));
    }

    void InterruptAttack()
    {
        Debug.Log("Interrupt");
        StopCoroutine("WaitABit");
        ReleaseAttack();
        isChoosing = false;
    }

    void Attack()
    {
        InputManager.Instance.playerInputs[attachedPlayer.playerNum].attack = true;

        float chargeTime = Random.Range(0f, 3f);
        StartCoroutine(WaitABit("ReleaseAttack", chargeTime));
        isChoosing = false;
        //StartCoroutine(WaitAction(chargeTime + .01f));
    }

    void ReleaseAttack()
    {
        if (distBetweenPlayers <= hitDistance - 1)
        {
            if (Random.Range(0f, 1f) > 0.5)
            {
                Debug.Log("Back attack !");
                InputManager.Instance.playerInputs[attachedPlayer.playerNum].horizontal = Mathf.Sign(transform.position.x - opponent.transform.position.x);
            }
        }
        else
        {
            Debug.Log("Forward attack !");
            InputManager.Instance.playerInputs[attachedPlayer.playerNum].horizontal = Mathf.Sign(opponent.transform.position.x - transform.position.x);
        }

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

        StartCoroutine(WaitABit("ResetHorizontal", 0.35f));
        isChoosing = false;
    }

    void MoveAway()
    {
        ManageMovementsInputs((int)Mathf.Sign(transform.position.x - opponent.transform.position.x));

        StartCoroutine(WaitABit("ResetHorizontal", 0.35f));
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

    void IncreaseWeight(string actionName, int amount = 1)
    {
        foreach (Actions act in actionsList)
        {
            if (act.name == actionName)
            {
                act.weight += amount;
                return;
            }
        }
        Debug.LogError("Action " + actionName + " not found !");
    }

    void ResetWeight(string actionName)
    {
        foreach (Actions act in actionsList)
        {
            if (act.name == actionName)
            {
                act.weight = 1;
                return;
            }
        }
    }

    void IncreaseOtherWeights()
    {
        foreach (Actions a in actionsList)
        {
            if (a.name != "InterruptAttack")
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
        //Debug.Log("Wait " + t + "sec and invoke " + calledFunc);
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