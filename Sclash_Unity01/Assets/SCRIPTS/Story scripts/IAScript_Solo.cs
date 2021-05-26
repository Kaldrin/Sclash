using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

public class IAScript_Solo : IAScript
{

    StoryPlayer playerScript;

    PlayerAnimations playerAnimations;

    public event Action<IAScript_Solo> OnIADeath;

    private float
        orientationCooldownStartTime = 0,
        orientationCooldown = 0.1f;
    private bool
        orientationCooldownFinished = true,
        canOrientTowardsEnemy = true;

    bool isDead;

    new void Awake()
    {
        base.Awake();
        isDead = false;
        playerScript = GetComponent<StoryPlayer>();
        playerAnimations = GetComponent<PlayerAnimations>();
    }

    new void OnEnable()
    {
        base.OnEnable();
        playerScript.OnDeath += Die;
    }

    new void OnDisable()
    {
        base.OnDisable();
        playerScript.OnDeath -= Die;
    }

    void Start()
    {
        if (InputManager_Story.Instance_Solo && playerScript)
            InputManager_Story.Instance_Solo.AddInputs(playerScript.playerNum + 1);
        attachedPlayer.Invoke("TriggerDraw", 0f);
    }

    protected override void Update()
    {
        if (isDead)
        {
            playerScript.rb.velocity = Vector2.zero;
            return;
        }

        ManageOrientation();
        timeToWait = CalculateDistance() ? closeRate * IAMultiplicator : normalRate * IAMultiplicator;
        //ManageMovement();
        UpdateWeightSum();
        if (distBetweenPlayers > 5)
        {
            DisableWeight();
            if (attachedPlayer.stamina >= 2)
                ManageMovementsInputs((int)Mathf.Sign(opponent.transform.position.x - transform.position.x)); //Move Towards
            else
                EnableWeight();
        }
        else if (distBetweenPlayers <= 2.5f && opponent)
        {
            ManageMovementsInputs((int)Mathf.Sign(transform.position.x - opponent.transform.position.x)); //Move Backwards
        }
        else if (Mathf.Abs(0 - attachedPlayer.rb.velocity.x) > 0.1)
            Invoke("ResetMovementInput", UnityEngine.Random.Range(.75f, 1f));

        if (canAddWeight)
            AddWeights();

        SelectAction();
    }

    void ManageMovement()
    {
        // Temporary system !! Replace for build !!
        if (distBetweenPlayers > 5)
        {
            ManageMovementsInputs((int)Mathf.Sign(opponent.transform.position.x - transform.position.x)); //Move Towards
        }
        else if (distBetweenPlayers <= 2.5f)
        {
            ManageMovementsInputs((int)Mathf.Sign(transform.position.x - opponent.transform.position.x)); //Move Backwards
        }
        else
        {
            if (Mathf.Abs(0 - attachedPlayer.rb.velocity.x) > 0.1)
                Invoke("ResetMovementInput", UnityEngine.Random.Range(.75f, 1f));
        }
    }

    void ResetMovementInput()
    {
        base.ManageMovementsInputs();
    }

    private bool CalculateDistance()
    {
        if (opponent)
            distBetweenPlayers = Mathf.Abs(transform.position.x - opponent.transform.position.x);
        return distBetweenPlayers <= DistanceTolerance ? true : false;
    }

    private void ManageOrientation()
    {
        float sign = 1;
        if (opponent)
            sign = Mathf.Sign(transform.position.x - opponent.transform.position.x);
        if (orientationCooldownFinished)
            ApplyOrientation(sign);

        if (Time.time >= orientationCooldown + orientationCooldownStartTime)
            orientationCooldownFinished = true;
    }

    private void ApplyOrientation(float sign)
    {
        if (sign > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        orientationCooldownStartTime = Time.time;
        orientationCooldownFinished = false;
    }


    public void Die()
    {
        if (OnIADeath != null)
            OnIADeath(this);

        isDead = true;

        //playerScript.playerCollider.gameObject.SetActive(false);
        foreach (Collider2D col in playerScript.playerColliders)
            col.gameObject.SetActive(false);

        playerScript.rb.bodyType = RigidbodyType2D.Static;
        playerScript.enabled = false;

        GameManager_Story.StoryInstance.RemovePlayer(gameObject);

        AudioManager.Instance.BattleEventIncreaseIntensity();

        playerAnimations.TriggerDeath();
        Invoke("FallAnimation", 1f);
        playerAnimations.DeathActivated(true);
    }


    private void FallAnimation()
    {
        playerAnimations.TriggerRealDeath();
        this.enabled = false;
    }







    void RemoveWarnings()
    {
        canOrientTowardsEnemy = canOrientTowardsEnemy || canOrientTowardsEnemy;
    }
}