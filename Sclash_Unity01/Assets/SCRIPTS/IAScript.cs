using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class IAScript : MonoBehaviour
{
    const float EASY_DIFFICULTY = 1.125f;
    const float MEDIUM_DIFFICULTY = 1f;
    const float HARD_DIFFICULTY = 0.25f;

    public Difficulty IADifficulty;
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    float IAMultiplicator;

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
    [SerializeField] float distBetweenPlayers;
    float hitDistance = 2f;

    bool isChoosing = false;
    // Update rate when far 
    [SerializeField]
    float normalRate = 0.25f;
    // Update rate when on close combat
    [SerializeField]
    float closeRate = 0.05f;

    bool nextState;
    float stateTimer;

    bool ready = false;

    public float timeToWait;

    [SerializeField]
    List<Actions> actionsList = new List<Actions>()
    {
        new Actions("Wait",1),
        new Actions("Attack",1),
        new Actions("Parry",1),
        new Actions("Pommel",1),
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

    void OnDisable()
    {
        attachedPlayer.playerIsAI = false;
    }

    public void EnemyReady()
    {
        Debug.Log("Ready");
        ready = true;
    }

    void Update()
    {
        if (GameManager.Instance.gameState == GameManager.GAMESTATE.game && ready)
        {
            //RESET WEIGHTS ON DEATH
            if (opponent.playerState == Player.STATE.enemyKilled || attachedPlayer.playerState == Player.STATE.enemyKilled)
            {
                ManageMovementsInputs(0);
                foreach (Actions a in actionsList)
                {
                    ResetSelfWeight(a);
                }
                return;
            }

            UpdateWeightSum();

            //MANAGE DISTANCE
            distBetweenPlayers = Mathf.Abs(attachedPlayer.transform.position.x - opponent.transform.position.x);
            isClose = distBetweenPlayers <= DistanceTolerance ? true : false;
            timeToWait = isClose ? closeRate * IAMultiplicator : normalRate * IAMultiplicator;

            //RESET INTERRUPTION
            if (attachedPlayer.playerState != Player.STATE.charging)
            {
                actionsList[actionsList.Count - 1].weight = 0;
            }

            ShowWeight();

            //AI DRAW
            if (attachedPlayer.playerState == Player.STATE.sneathed && !isWaiting)
            {
                isWaiting = true;
                StartCoroutine(WaitABit("TriggerDraw", rand, false));
                return;
            }

            //WAIT IF THE PLAYER IS DRAWING
            if (attachedPlayer.playerState == Player.STATE.drawing)
                return;

            if (opponent.playerState == Player.STATE.frozen)
                return;

            if (distBetweenPlayers > 5)
            {
                DisableWeight();
                if (attachedPlayer.stamina >= 2)
                {
                    ManageMovementsInputs((int)Mathf.Sign(opponent.transform.position.x - transform.position.x)); //MOVE TOWARD
                }
                return;
            }
            else
            {
                EnableWeight();
            }

            if (attachedPlayer.playerState == Player.STATE.charging && Mathf.Sign(attachedPlayer.transform.localScale.x) == Mathf.Sign(opponent.transform.localScale.x))
            {
                Debug.Log("<color=red>INTERUPT !!! He is behind you !</color>");
                InterruptAttack();
            }


            //WHILE THE PLAYER IS FAR, GET CLOSER
            if (attachedPlayer.stamina <= 2)
                ManageMovementsInputs((int)Mathf.Sign(transform.position.x - opponent.transform.position.x)); //MOVE AWAY
            else
                ManageMovementsInputs((int)Mathf.Sign(opponent.transform.position.x - transform.position.x)); //MOVE TOWARD





            //ADD WEIGHT TO ACTIONS
            if (canAddWeight)
                AddWeights();

            if (!isChoosing && attachedPlayer.playerState == Player.STATE.normal)
            {
                isChoosing = true;

                string calledAct = ChooseState();

                if (IADifficulty == Difficulty.Hard && calledAct == "Pommel" && hitDistance > 1)
                {
                    calledAct = "Wait";
                }


                StartCoroutine(WaitABit(calledAct, timeToWait, true));
            }
        }
    }

    #endregion

    public void SetDifficulty(Difficulty targetDifficulty)
    {
        IADifficulty = targetDifficulty;
        switch (IADifficulty)
        {
            case Difficulty.Easy:
                Debug.Log("Difficulty set to :" + IADifficulty.ToString());
                IAMultiplicator = EASY_DIFFICULTY;
                break;

            case Difficulty.Medium:
                Debug.Log("Difficulty set to :" + IADifficulty.ToString());
                IAMultiplicator = MEDIUM_DIFFICULTY;
                break;

            case Difficulty.Hard:
                Debug.Log("Difficulty set to :" + IADifficulty.ToString());
                IAMultiplicator = HARD_DIFFICULTY;
                break;
        }
    }

    void ShowWeight()
    {
        wait = actionsList[0].weight;
        attack = actionsList[1].weight;
        parry = actionsList[2].weight;
        pommel = actionsList[3].weight;
        dashToward = actionsList[4].weight;
        dashAway = actionsList[5].weight;
        interruptAttack = actionsList[6].weight;
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
            return;
        }

        Debug.Log("Opponent found !");
        opponent.DrawnEvent += EnemyReady;
    }

    void AddWeights()
    {
        if (distBetweenPlayers > 5 && attachedPlayer.stamina > 2)
            return;

        DisableWeight();

        if (attachedPlayer.playerState == Player.STATE.clashed)
            IncreaseWeight("Parry", 2);

        if (opponent.stamina <= 1)
        {
            switch (IADifficulty)
            {
                case Difficulty.Medium:
                    IncreaseWeight("Attack", 5);
                    break;

                case Difficulty.Hard:
                    if (distBetweenPlayers > hitDistance && attachedPlayer.stamina >= 2)
                    {
                        DashToward();
                        IncreaseWeight("Attack", 10);
                    }
                    else
                    {
                        Attack();
                    }
                    break;
            }
        }

        switch (opponent.playerState)
        {
            case Player.STATE.charging:
                if (distBetweenPlayers <= hitDistance)
                {
                    IncreaseWeight("Parry", 5);
                    IncreaseWeight("Attack", 2);
                }
                else if (attachedPlayer.stamina >= 2)
                {
                    IncreaseWeight("DashToward", 5);
                }
                break;

            case Player.STATE.attacking:
                if (isClose)
                {
                    if (attachedPlayer.playerState == Player.STATE.charging)
                    {
                        Debug.Log("<color=red>INTERUPT !!!</color>");

                        switch (IADifficulty)
                        {
                            case Difficulty.Easy:
                                IncreaseWeight("InterruptAttack", 1000);
                                break;

                            case Difficulty.Medium:
                                IncreaseWeight("InterruptAttack", 2000);
                                break;

                            case Difficulty.Hard:
                                InterruptAttack();
                                break;
                        }
                    }
                }
                break;

            case Player.STATE.parrying:
                if (distBetweenPlayers <= 1)
                {
                    switch (IADifficulty)
                    {
                        case Difficulty.Easy:
                            IncreaseWeight("Pommel", 2);
                            break;

                        case Difficulty.Medium:
                            IncreaseWeight("Pommel", 8);
                            break;

                        case Difficulty.Hard:
                            Pommel();
                            break;
                    }
                }

                if (attachedPlayer.stamina >= 2)
                {
                    switch (IADifficulty)
                    {
                        case Difficulty.Easy:
                            IncreaseWeight("DashToward", 2);
                            break;

                        case Difficulty.Medium:
                            IncreaseWeight("DashToward", 8);
                            break;

                        case Difficulty.Hard:
                            DashToward();
                            IncreaseWeight("Attack", 25);
                            break;
                    }
                }

                IncreaseWeight("Attack", 2);
                break;

            case Player.STATE.dashing:
                if (distBetweenPlayers <= hitDistance)
                    IncreaseWeight("Attack", 5);
                break;

            case Player.STATE.normal:
                if (attachedPlayer.stamina >= 2)
                {
                    IncreaseWeight("Attack", 1);
                }
                else
                {
                    ManageMovementsInputs((int)Mathf.Sign(transform.position.x - opponent.transform.position.x));
                    IncreaseWeight("Wait", 3);
                }
                break;

            case Player.STATE.clashed:
                IncreaseWeight("Attack", 10);
                IncreaseWeight("DashAway", 10);
                break;

        }

        StartCoroutine(WaitABit("EnableWeight", 0.15f));
    }

    void EnableWeight()
    {
        canAddWeight = true;
    }

    void DisableWeight()
    {
        canAddWeight = false;
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

        float randCancel = Random.Range(0f, 1f);
        if (randCancel >= 0.75f)
        {
            ReleaseAttack();
        }
        else
        {
            DashAway();
        }

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

    /*void MoveToward()
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
    }*/

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

    void UpdateWeightSum()
    {
        //Calculate weight sum
        actionWeightSum = 0;
        foreach (Actions a in actionsList)
        {
            actionWeightSum += a.weight;
        }
    }

    string ChooseState()
    {
        //Select a random action by weight
        int randomAction = Random.Range(1, actionWeightSum);
        string rState = null;
        foreach (Actions item in actionsList)
        {
            randomAction -= item.weight;
            if (randomAction <= 0)
            {
                //IncreaseOtherWeights();
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