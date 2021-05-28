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
    protected float IAMultiplicator;

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
    protected Player attachedPlayer;
    [SerializeField]
    public Player opponent
    {
        get { return _opponent; }
        set { _opponent = value; }
    }
    private Player _opponent;



    bool isWaiting;
    protected bool isClose = true;
    protected bool canAddWeight = true;

    float rand = 0;


    public float DistanceTolerance = 3;
    [SerializeField] protected float distBetweenPlayers;
    float hitDistance = 2f;

    bool isChoosing = false;
    // Update rate when far 
    [SerializeField]
    protected float normalRate = 0.25f;
    // Update rate when on close combat
    [SerializeField]
    protected float closeRate = 0.05f;

    bool nextState;
    float stateTimer;

    bool ready = false;

    public float timeToWait;

    [SerializeField]
    public List<Actions> actionsList = new List<Actions>()
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


    #endregion

    #region Built-in methods
    public Player GetPlayer()
    {
        Player[] entities = FindObjectsOfType<Player>();
        foreach (Player s in entities)
        {
            if (s.GetType() == typeof(StoryPlayer))
            {

                if (!s.gameObject.GetComponent<IAScript>())
                    return s;
            }
            else if (!s.gameObject.GetComponent<IAScript>().enabled)
            {
                Debug.Log("Found player");
                return s;
            }
        }
        return null;
    }

    protected virtual void Awake()                                                                                                                                                        // AWAKE
    {
        if (GameManager.Instance)
            GameManager.Instance.ResetGameEvent += OnDisable;

        if (attachedPlayer.playerNum == 0)
        {
            if (this.enabled)
            {
                Debug.LogError("AI player num should never be 0");
                attachedPlayer.playerNum++;
            }
        }
    }

    protected void OnEnable()                                                                                                                                                     // ON ENABLE
    {
        ready = false;
        attachedPlayer = GetComponent<Player>();

        Debug.Log("Enable AI");
        // OPPONENT
        if (opponent == null)
            _opponent = GetPlayer();
        FindOpponent();


        if (attachedPlayer == null)
        {
            Debug.LogError("Player script not found on GameObject", gameObject);
            return;
        }

        attachedPlayer.playerIsAI = true;
    }

    protected void OnDisable()                                                                                                                                                    // ON DISABLE
    {
        if (attachedPlayer)
            attachedPlayer.playerIsAI = false;
    }


    protected virtual void Update()                                                                                                                                     // UPDATE
    {
        // OPPONENT
        if (opponent == null)
            _opponent = GetPlayer();


        if (GameManager.Instance.gameState == GameManager.GAMESTATE.game && ready)
        {
            UpdateWeightSum();

            //MANAGE DISTANCE
            distBetweenPlayers = Mathf.Abs(attachedPlayer.transform.position.x - opponent.transform.position.x);
            isClose = distBetweenPlayers <= DistanceTolerance ? true : false;
            timeToWait = isClose ? closeRate * IAMultiplicator : normalRate * IAMultiplicator;

            //RESET INTERRUPTION
            if (attachedPlayer.playerState != Player.STATE.charging)
            {
                //actionsList[actionsList.Count - 1].weight = 0;
                foreach (Actions a in actionsList)
                    if (a.name == "InterruptAttack")
                        a.weight = 0;
            }

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

            //WHILE THE PLAYER IS FAR, GET CLOSER
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
                //Debug.Log("<color=red>INTERUPT !!! He is behind you !</color>");
                InterruptAttack();
            }



            if (attachedPlayer.stamina <= 2)
                ManageMovementsInputs((int)Mathf.Sign(transform.position.x - opponent.transform.position.x)); //MOVE AWAY
            else
                ManageMovementsInputs((int)Mathf.Sign(opponent.transform.position.x - transform.position.x)); //MOVE TOWARD


            //ADD WEIGHT TO ACTIONS
            if (canAddWeight)
                AddWeights();

            SelectAction();
        }

        if (GameManager.Instance.gameState == GameManager.GAMESTATE.roundFinished)
        {
            //RESET WEIGHTS ON DEATH
            if (opponent.playerState == Player.STATE.dead)
            {
                ManageMovementsInputs(0);
                foreach (Actions a in actionsList)
                {
                    ResetSelfWeight(a);
                }
                return;
            }
        }
    }
    #endregion





    public void EnemyReady()
    {
        Debug.Log("Enemy is ready");
        ready = true;
    }


    protected void SelectAction()
    {
        Debug.Log("Select Action");
        if (!isChoosing && attachedPlayer.playerState == Player.STATE.normal)
        {
            isChoosing = true;

            string calledAct = ChooseState();

            if (IADifficulty == Difficulty.Hard && calledAct == "Pommel" && hitDistance > 1)
            {
                calledAct = "Wait";
            }

            Debug.Log("Calling " + calledAct);
            Invoke(calledAct, timeToWait);
        }
    }

    public void SetDifficulty(Difficulty targetDifficulty)
    {
        IADifficulty = targetDifficulty;
        switch (IADifficulty)
        {
            case Difficulty.Easy:
                //Debug.Log("Difficulty set to :" + IADifficulty.ToString());
                IAMultiplicator = EASY_DIFFICULTY;
                break;

            case Difficulty.Medium:
                //Debug.Log("Difficulty set to :" + IADifficulty.ToString());
                IAMultiplicator = MEDIUM_DIFFICULTY;
                break;

            case Difficulty.Hard:
                //Debug.Log("Difficulty set to :" + IADifficulty.ToString());
                IAMultiplicator = HARD_DIFFICULTY;
                break;
        }
    }

    void FindOpponent()
    {
        Debug.Log("Looking for opponent");
        if (opponent != null)
        {
            if (opponent.GetType() != typeof(StoryPlayer))
                opponent.DrawnEvent += EnemyReady;
            else
                EnemyReady();
        }
    }

    protected void AddWeights()
    {
        if (distBetweenPlayers > 5 && attachedPlayer.stamina > 2)
            return;

        DisableWeight();

        if (attachedPlayer.playerState == Player.STATE.clashed)
            IncreaseWeight("Parry", 2);

        if (opponent && opponent.stamina <= 1)
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
                        Attack();
                    break;
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
                    IncreaseWeight("DashToward", 5);
                break;

            case Player.STATE.attacking:
                if (isClose)
                    if (attachedPlayer.playerState == Player.STATE.charging)
                    {
                        //Debug.Log("<color=red>INTERUPT !!!</color>");

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
                break;

            case Player.STATE.parrying:
                if (distBetweenPlayers <= 1)
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

                if (attachedPlayer.stamina >= 2)
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

                IncreaseWeight("Attack", 2);
                break;

            case Player.STATE.dashing:
                if (distBetweenPlayers <= hitDistance)
                    IncreaseWeight("Attack", 5);
                break;

            case Player.STATE.normal:
                if (attachedPlayer.stamina >= 2)
                    IncreaseWeight("Attack", 1);
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

    protected void EnableWeight()
    {
        canAddWeight = true;
    }

    protected void DisableWeight()
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
        //Debug.Log("Interrupt");
        StopCoroutine("WaitABit");

        float randCancel = Random.Range(0f, 1f);
        if (randCancel >= 0.75f)
            ReleaseAttack();
        else
            DashAway();

        isChoosing = false;
    }

    void Attack()
    {
        InputManager.Instance.playerInputs[attachedPlayer.playerNum].attack = true;

        float chargeTime = Random.Range(0f, 3f);
        Invoke("ReleaseAttack", chargeTime);
        isChoosing = false;
    }

    void ReleaseAttack()
    {
        if (distBetweenPlayers <= hitDistance - 1)
        {
            if (Random.Range(0f, 1f) > 0.5)
                InputManager.Instance.playerInputs[attachedPlayer.playerNum].horizontal = Mathf.Sign(transform.position.x - opponent.transform.position.x);
        }
        else
            InputManager.Instance.playerInputs[attachedPlayer.playerNum].horizontal = Mathf.Sign(opponent.transform.position.x - transform.position.x);

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
        //Debug.Log("Pommel");

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
        Debug.Log("Dash Towards");
        if (attachedPlayer.stamina >= attachedPlayer.staminaCostForMoves)
            InputManager.Instance.playerInputs[attachedPlayer.playerNum].dash = Mathf.Sign(opponent.transform.position.x - transform.position.x);

        StartCoroutine(WaitABit("ResetInput", 0.15f));
        isChoosing = false;
    }


    void DashAway()
    {
        Debug.Log("Dash Away");

        if (attachedPlayer.stamina >= attachedPlayer.staminaCostForMoves)
            InputManager.Instance.playerInputs[attachedPlayer.playerNum].dash = Mathf.Sign(transform.position.x - opponent.transform.position.x);

        StartCoroutine(WaitABit("ResetInput", 0.5f));
        isChoosing = false;
    }
    #endregion


    protected void UpdateWeightSum()
    {
        //Calculate weight sum
        actionWeightSum = 0;
        foreach (Actions a in actionsList)
            actionWeightSum += a.weight;
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
            Debug.LogError("No Action selected ! Log : Action sum:" + actionWeightSum + " Random num: " + randomAction, gameObject);

        return rState;
    }


    void IncreaseWeight(string actionName, int amount = 1)
    {
        foreach (Actions act in actionsList)
            if (act.name == actionName)
            {
                act.weight += amount;
                return;
            }
        Debug.LogWarning("Action " + actionName + " not found !");
    }


    void ResetWeight(string actionName)
    {
        foreach (Actions act in actionsList)
            if (act.name == actionName)
            {
                act.weight = 1;
                return;
            }
    }


    void IncreaseOtherWeights()
    {
        foreach (Actions a in actionsList)
            if (a.name != "InterruptAttack")
                a.weight++;
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
    protected IEnumerator WaitABit(string calledFunc = null, float t = 0, bool self = true)
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

    protected void ManageMovementsInputs(int direction = 0)
    {
        Debug.Log("Move");
        GetComponent<Rigidbody2D>().velocity = new Vector2(direction * attachedPlayer.actualMovementsSpeed, GetComponent<Rigidbody2D>().velocity.y);
    }
    #endregion
}