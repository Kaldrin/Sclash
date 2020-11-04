using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class IAScript_Solo : IAScript
{
    private Player _opponent;
    public new Player opponent
    {
        get { return GetPlayer(); }
    }

    PlayerAnimations playerAnimations;

    private float
        orientationCooldownStartTime = 0,
        orientationCooldown = 0.1f;
    private bool
        orientationCooldownFinished = true,
        canOrientTowardsEnemy = true;

    void Awake()
    {
        playerAnimations = GetComponent<PlayerAnimations>();
    }

    void Start()
    {
        attachedPlayer.Invoke("TriggerDraw", 0f);
    }

    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Die();
        }

        ManageOrientation();
        UpdateWeightSum();
        timeToWait = CalculateDistance() ? closeRate * IAMultiplicator : normalRate * IAMultiplicator;
        if (distBetweenPlayers > 5)
        {
            DisableWeight();
            if (attachedPlayer.stamina >= 2)
            {
                ManageMovementsInputs((int)Mathf.Sign(opponent.transform.position.x - transform.position.x)); //Move Towards
            }
            else
            {
                EnableWeight();
            }
        }
        else if (distBetweenPlayers <= 2.5f)
        {
            ManageMovementsInputs((int)Mathf.Sign(transform.position.x - opponent.transform.position.x)); //Move Backwards
        }
        else
        {
            if (Mathf.Abs(0 - attachedPlayer.rb.velocity.x) > 0.1)
                Invoke("ResetMovementInput", Random.Range(.75f, 1f));
        }

        if (canAddWeight)
            AddWeights();

        SelectAction();
    }

    void ResetMovementInput()
    {
        base.ManageMovementsInputs();
    }

    private bool CalculateDistance()
    {
        distBetweenPlayers = Mathf.Abs(transform.position.x - opponent.transform.position.x);
        return distBetweenPlayers <= DistanceTolerance ? true : false;
    }

    private void ManageOrientation()
    {
        float sign = Mathf.Sign(transform.position.x - opponent.transform.position.x);
        if (orientationCooldownFinished)
            ApplyOrientation(sign);

        if (Time.time >= orientationCooldown + orientationCooldownStartTime)
            orientationCooldownFinished = true;
    }

    private void ApplyOrientation(float sign)
    {
        if (sign > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        orientationCooldownStartTime = Time.time;
        orientationCooldownFinished = false;
    }

    public Player GetPlayer()
    {
        StoryPlayer[] entities = FindObjectsOfType<StoryPlayer>();
        foreach (StoryPlayer s in entities)
        {
            if (!s.gameObject.GetComponent<IAScript>())
            {
                return s;
            }
        }
        return null;
    }

    public void Die()
    {


        AudioManager.Instance.TriggerSuccessfulAttackAudio();
        AudioManager.Instance.BattleEventIncreaseIntensity();

        playerAnimations.TriggerDeath();
        Invoke("FallAnimation", 1f);
        playerAnimations.DeathActivated(true);
    }

    private void FallAnimation()
    {
        playerAnimations.TriggerRealDeath();
    }
}