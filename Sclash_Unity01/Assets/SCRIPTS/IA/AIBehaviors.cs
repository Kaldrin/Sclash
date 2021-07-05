using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class AIBehaviors : MonoBehaviour
{
    public const float EASY = 1.125f;
    public const float MEDIUM = 1f;
    public const float HARD = .25f;
    public float closeUpDistance = 2.0f;
    public float staminaTrigger = 2.0f;

    public bool opponentReady = false;

    public Player playerComponent;
    public Player.STATE playerState
    { get { return playerComponent.playerState; } }
    public Player.STATE oldPlayerState
    { get { return playerComponent.oldState; } }

    [SerializeField]
    private Player opponent = null;

    [SerializeField]
    private string currentStateName;
    private INPCState currentState;

    public FrozenState frozenState = new FrozenState();
    public OffenseState offenseState = new OffenseState();
    public IdleState idleState = new IdleState();
    public DefenseState defenseState = new DefenseState();

    public float distance
    {
        get { return Mathf.Abs(0.0f - transform.position.x); }
    }

    [SerializeField]
    public float stamina
    {
        get { return playerComponent.stamina; }
    }


    private void OnEnable()
    {
        currentState = frozenState;

        //GET PLAYER AND SUBSCRIBE TO HIS EVENTS
        opponent = GameManager.Instance.playersList[0].GetComponent<Player>();
        playerComponent = GetComponent<Player>();
        if (opponent != null)
            opponent.DrawnEvent += EnemyReady;
        else
        {
            Debug.LogWarning("Opponent not found");
        }
    }

    private void OnDisable()
    {
        //UNSUBSCRIBE
        opponent.DrawnEvent -= EnemyReady;
    }

    void Update()
    {
        currentState = currentState.DoState(this);
        currentStateName = currentState.ToString();

        Spacing();
    }

    private void DrawAction()
    {
        playerComponent.Invoke("TriggerDraw", 0f);
    }

    private void EnemyReady()
    {
        DrawAction();
        opponentReady = true;
    }

    private void CalculateAdvantage()
    {

    }

    //PRESSURE DEPENDS ON STAMINA ADVANTAGE AND SPACING
    private void Presure()
    {

    }

    //SPACING SHOULD DEPEND ON STAMINA ADVANTAGE AND DISTANCE
    /*
        A good spacing is when you keep out of reach when your options are too limited
        and close the distance and increase presure when you have the advantage
    */
    private void Spacing()
    {
        float opDist = GetOptimalDistance();
        float distDiff = CalculateDistanceDifference(distance, opDist);

        if (distDiff < 1)
        {
            //Good
        }
        else if (distDiff >= 1 && distDiff < 2)
        {
            //Ok
        }
        else if (distDiff >= 2)
        {
            //Bad
        }
    }

    private float GetOptimalDistance()
    {
        float optimalDistance;
        if (StaminaAdvantage(true) > 0)
        {
            optimalDistance = closeUpDistance * (1 / StaminaAdvantage());
            Debug.Log("Op dist" + optimalDistance);
        }
        else
        {
            optimalDistance = closeUpDistance + StaminaAdvantage();
            Debug.Log("Op dist" + optimalDistance);
        }
        return optimalDistance;
    }

    private float StaminaAdvantage(bool getSign = false)
    {
        float staminaDiff = stamina - opponent.stamina;

        if (!getSign)
            staminaDiff = Mathf.Abs(staminaDiff);

        if (staminaDiff == 0f)
            staminaDiff++;

        return staminaDiff;
    }

    private int Int_StaminaAdvantage(bool getSign = false)
    {
        int staminaDiff = Mathf.RoundToInt(StaminaAdvantage(getSign));

        return staminaDiff;
    }

    private float CalculateDistanceDifference(float a, float b)
    {
        return Mathf.Abs(a - b);
    }
}