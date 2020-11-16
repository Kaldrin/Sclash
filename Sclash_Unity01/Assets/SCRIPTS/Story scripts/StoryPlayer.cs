using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class StoryPlayer : Player
{
    float soloOrientation = -1;

    IAScript iAScript;
    public delegate void OnDeathEvent();
    public event OnDeathEvent DeathEvent;

    void Awake()
    {
        if (inputManager == null)
            inputManager = InputManager_Story.Instance;
    }

    public override void Start()
    {
        SetUpStaminaBars();
        stamina = maxStamina;
        TriggerDraw();

        GameManager_Story.Instance.playersList.Add(gameObject);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (playerIsAI)
            if (iAScript == null)
                iAScript = GetComponent<IAScript>();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void ManageChargeInput()
    {
        if (playerIsAI)
        {
            //base.ManageChargeInput();
            return;
        }

        if (InputManager.Instance.playerInputs[0].attack && canCharge)
        {
            if (stamina >= staminaCostForMoves)
            {
                canCharge = false;
                SwitchState(STATE.charging);

                playerAnimations.CancelCharge(false);
                playerAnimations.TriggerCharge(true);

                chargeStartTime = Time.time;

                chargeFlareFX.Play();
            }
        }

        if (InputManager.Instance.playerInputs[0].attackDown && canCharge && stamina <= staminaCostForMoves)
            TriggerNotEnoughStaminaAnim(true);

        if (!InputManager.Instance.playerInputs[0].attack)
            canCharge = true;
    }

    public override void ManageDashInput()
    {
        if (playerIsAI)
        {
            base.ManageDashInput();
            return;
        }

        if (Mathf.Abs(InputManager.Instance.playerInputs[0].dash) < shortcutDashDeadZone && currentShortcutDashStep == DASHSTEP.invalidated)
            currentShortcutDashStep = DASHSTEP.rest;

        if (Mathf.Abs(InputManager.Instance.playerInputs[0].dash) > shortcutDashDeadZone && currentShortcutDashStep == DASHSTEP.rest)
        {
            dashDirection = Mathf.Sign(InputManager.Instance.playerInputs[0].dash);
            TriggerBasicDash();
        }

        if (currentDashStep == DASHSTEP.firstInput || currentDashStep == DASHSTEP.firstRelease)
            if (Time.time - dashInitializationStartTime > allowanceDurationForDoubleTapDash)
                currentDashStep = DASHSTEP.invalidated;

        if (Mathf.Abs(InputManager.Instance.playerInputs[0].horizontal) < dashDeadZone)
        {
            if (currentDashStep == DASHSTEP.firstInput)
                currentDashStep = DASHSTEP.firstRelease;
            else if (currentDashStep == DASHSTEP.invalidated)
                currentDashStep = DASHSTEP.rest;
        }

        if (Mathf.Abs(InputManager.Instance.playerInputs[0].horizontal) > dashDeadZone && Mathf.Sign(InputManager.Instance.playerInputs[0].horizontal) != temporaryDashDirectionForCalculation)
            if (currentDashStep == DASHSTEP.firstInput || currentDashStep == DASHSTEP.firstRelease)
                currentDashStep = DASHSTEP.invalidated;

        if (Mathf.Abs(InputManager.Instance.playerInputs[0].horizontal) > dashDeadZone)
        {
            temporaryDashDirectionForCalculation = Mathf.Sign(InputManager.Instance.playerInputs[0].horizontal);

            if (currentDashStep == DASHSTEP.rest)
            {
                currentDashStep = DASHSTEP.firstInput;
                dashDirection = temporaryDashDirectionForCalculation;
                dashInitializationStartTime = Time.time;
            }
            else if (currentDashStep == DASHSTEP.firstRelease && dashDirection == temporaryDashDirectionForCalculation)
            {
                currentDashStep = DASHSTEP.invalidated;
                TriggerBasicDash();
            }
        }
    }

    public override void ManagePommel()
    {
        if (playerIsAI)
        {
            base.ManagePommel();
            return;
        }

        if (!InputManager.Instance.playerInputs[0].kick)
            canPommel = true;

        if (InputManager.Instance.playerInputs[0].kick && canPommel)
        {
            canPommel = false;
            TriggerPommel();
        }
    }

    public override void ManageParryInput()
    {
        if (playerIsAI)
        {
            base.ManageParryInput();
            return;
        }

        if (canBriefParry)
        {
            if (InputManager.Instance.playerInputs[0].parryDown && stamina <= staminaCostForMoves && canParry)
                TriggerNotEnoughStaminaAnim(true);

            if (InputManager.Instance.playerInputs[0].parry && canParry)
            {
                canParry = false;
                if (stamina >= staminaCostForMoves)
                    TriggerParry();
            }

            if (!InputManager.Instance.playerInputs[0].parry)
                canParry = true;
        }
    }

    public override void ManageMovementsInputs()
    {
        if (playerIsAI)
        {
            base.ManageMovementsInputs();
            return;
        }

        rb.simulated = true;

        rb.velocity = new Vector2(InputManager.Instance.playerInputs[playerNum].horizontal * actualMovementsSpeed, rb.velocity.y);

        if (Mathf.Abs(rb.velocity.x) > minSpeedForWalkFX && GameManager.Instance.gameState == GameManager.GAMESTATE.game && playerState == Player.STATE.normal)
        {
            if ((rb.velocity.x * -transform.localScale.x) < 0)
            {
                walkFXFront.Stop();


                if (!walkFXBack.isPlaying)
                    walkFXBack.Play();
            }
            else
            {
                if (!walkFXFront.isPlaying)
                    walkFXFront.Play();


                walkFXBack.Stop();
            }
        }
        else
        {
            walkFXBack.Stop();
            walkFXFront.Stop();
        }

    }

    public override void ManageOrientation()
    {
        if (playerIsAI)
        {
            //Orient toward the Player
            float sign = Mathf.Sign(transform.position.x - iAScript.opponent.transform.position.x);
            ApplyOrientation(sign);

            //base.ManageOrientation();
            return;
        }

        if (InputManager.Instance.playerInputs[0].horizontal != 0)
            soloOrientation = -InputManager.Instance.playerInputs[0].horizontal;

        if (canOrientTowardsEnemy)
        {
            ApplyOrientation(Mathf.Sign(soloOrientation));
        }

        if (Time.time >= orientationCooldown + orientationCooldownStartTime)
            orientationCooldownFinished = true;
    }

    protected override void ApplyPommelHitbox()
    {
        Collider2D[] hitsCol = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (transform.localScale.x * (-actualAttackRange + actualBackAttackRangeDisjoint) / 2), transform.position.y), new Vector2(actualAttackRange + actualBackAttackRangeDisjoint, 1), 0);
        List<GameObject> hits = new List<GameObject>();

        foreach (Collider2D c in hitsCol)
        {
            if (c.CompareTag("Player"))
            {
                if (!hits.Contains(c.transform.parent.gameObject))
                {
                    hits.Add(c.transform.parent.gameObject);
                }
            }
        }

        foreach (GameObject g in hits)
        {
            if (g != gameObject)
            {
                otherPlayerNum = GetTargetNum(g);
                if (g.GetComponent<Player>().playerState != Player.STATE.clashed)
                {
                    g.GetComponent<Player>().Pommeled();
                }
            }
        }
    }

    protected override void ApplyAttackHitbox()
    {
        Collider2D[] hitsCol = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (transform.localScale.x * (-actualAttackRange + actualBackAttackRangeDisjoint) / 2), transform.position.y), new Vector2(actualAttackRange + actualBackAttackRangeDisjoint, 1), 0);
        List<GameObject> hits = new List<GameObject>();

        foreach (Collider2D c in hitsCol)
        {
            if (c.CompareTag("Player"))
            {
                if (!hits.Contains(c.transform.parent.gameObject))
                {
                    hits.Add(c.transform.parent.gameObject);
                }
            }
        }

        foreach (GameObject g in hits)
        {
            if (g != gameObject)
            {
                otherPlayerNum = GetTargetNum(g);

                g.GetComponent<StoryPlayer>().TakeDamage(gameObject, chargeLevel);

                attackRangeFX.gameObject.SetActive(false);
                attackRangeFX.gameObject.SetActive(true);
            }
        }
    }

    public override void Pommeled()
    {
        //otherPlayerNum = GetTargetNum();
        base.Pommeled();
    }

    public override bool TakeDamage(GameObject instigator, int hitStrength = 1)
    {
        bool hit = false;
        Debug.Log("Die");

        if (clashFrames)
        {
            instigator.GetComponent<StoryPlayer>().TriggerClash();
            TriggerClash();

            //INSTANTIATE CLASH PREFAB AT FX POS
            Vector3 fxPos = new Vector3((instigator.transform.position.x + transform.position.x) / 2, clashFX.transform.position.y, clashFX.transform.position.z);
            Instantiate(clashFXPrefabRef, fxPos, clashFX.transform.rotation, null).GetComponent<ParticleSystem>().Play();

            AudioManager.Instance.TriggerClashAudioCoroutine();
        }
        else if (parryFrame)
        {
            StartCoroutine(TriggerStaminaRecupAnim());
            instigator.GetComponent<StoryPlayer>().TriggerClash();

            playerAnimations.TriggerPerfectParry();

            clashFX.Play();

            AudioManager.Instance.TriggerParriedAudio();
        }
        else if (untouchableFrame)
        {
            GameManager.Instance.TriggerSlowMoCoroutine(GameManager.Instance.dodgeSlowMoDuration, GameManager.Instance.dodgeSlowMoTimeScale, GameManager.Instance.dodgeTimeScaleFadeSpeed);
            AudioManager.Instance.BattleEventIncreaseIntensity();
        }
        else
        {
            hit = true;
            TriggerHit();
            CheckDeath(instigator.GetComponent<StoryPlayer>().playerNum);
        }

        attackRangeFX.gameObject.SetActive(false);
        attackRangeFX.gameObject.SetActive(true);

        return hit;
    }

    public override void CheckDeath(int instigatorNum)
    {


        if (DeathEvent != null)
            DeathEvent();
        //base.CheckDeath(instigatorNum);
    }

    public int GetTargetNum(GameObject g)
    {
        if (g.GetComponent<Player>())
        {
            return g.GetComponent<Player>().playerNum;
        }

        return -1;
    }

}