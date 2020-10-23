using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class IAScript_Solo : IAScript
{
    void Start()
    {
        attachedPlayer.Invoke("TriggerDraw", 0f);
    }

    protected override void Update()
    {
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



}