using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

using Photon.Pun;
using Photon.Realtime;


public class Player_Online : Player, IPunObservable
{
    private bool releasedAttack = false;

    public override void Update()
    {
        if (!photonView.IsMine)
        {
            UpdateStaminaSlidersValue();
            UpdateStaminaColor();


            SetStaminaBarsOpacity(staminaBarsOpacity);

            return;
        }

        base.Update();
    }

    public override void FixedUpdate()
    {
        if (enabled && isActiveAndEnabled)
        {
            if (!photonView.IsMine)
            {
                Vector2 lagDistance = netTargetPos - rb.position;

                if (lagDistance.magnitude > 3f)
                {
                    rb.position = netTargetPos;
                    lagDistance = Vector2.zero;
                }

                if (lagDistance.magnitude < 0.11f)
                {
                    rb.velocity = Vector2.zero;
                }
                else
                {
                    if (playerState != STATE.dashing)
                        rb.velocity = new Vector2(lagDistance.normalized.x * baseMovementsSpeed, rb.velocity.y);
                    else
                        rb.velocity = new Vector2(lagDistance.normalized.x * 15, rb.velocity.y);
                }
                return;
            }
        }

        base.FixedUpdate();
    }

    public override bool TakeDamage(GameObject instigator, int hitStrength = 1)
    {
        bool hit = false;
        if (playerState != STATE.dead)
        {
            if (Mathf.Sign(instigator.transform.localScale.x) == Mathf.Sign(transform.localScale.x))
            {
                hit = true;
                photonView.RPC("TriggerHit", RpcTarget.AllViaServer);
            }
            else if (clashFrames)
            {
                foreach (GameObject p in GameManager.Instance.playersList)
                {
                    p.GetComponent<PhotonView>().RPC("TriggerClash", RpcTarget.AllViaServer);
                }

                Vector3 fxPos = new Vector3((GameManager.Instance.playersList[0].transform.position.x + GameManager.Instance.playersList[1].transform.position.x) / 2, clashFX.transform.position.y, clashFX.transform.position.z);
                Instantiate(clashFXPrefabRef, fxPos, clashFX.transform.rotation, null).GetComponent<ParticleSystem>().Play();

                AudioManager.Instance.TriggerClashAudioCoroutine();

                if (statsManager)
                {
                    statsManager.AddAction(ACTION.clash, playerNum, 0);
                    statsManager.AddAction(ACTION.clash, otherPlayerNum, 0);
                }
                else
                {
                    Debug.Log("Couldn't access statsManager to record action, ignoring");
                }
            }
            else if (parryFrame)
            {
                photonView.RPC("N_TriggerStaminaRecupAnim", RpcTarget.All);

                instigator.GetComponent<PhotonView>().RPC("TriggerClash", RpcTarget.AllViaServer);

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
                photonView.RPC("TriggerHit", RpcTarget.AllViaServer);
            }

            photonView.RPC("CheckDeath", RpcTarget.AllViaServer, instigator.GetComponent<Player>().playerNum);

        }

        attackRangeFX.gameObject.SetActive(false);
        attackRangeFX.gameObject.SetActive(true);

        return hit;
    }

    public override void ManageMovementsInputs()
    {
        rb.velocity = new Vector2(InputManager.Instance.playerInputs[0].horizontal * actualMovementsSpeed, rb.velocity.y);

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

    protected override void ManageDraw()
    {
        Debug.Log("Manage Draw");
        if (InputManager.Instance.playerInputs[0].anyKey)
            photonView.RPC("TriggerDraw", RpcTarget.AllBufferedViaServer);
    }

    protected override void ManageCharging()
    {
        if (!InputManager.Instance.playerInputs[0].attack && !releasedAttack)
        {
            photonView.RPC("ReleaseAttack", RpcTarget.AllViaServer);
            releasedAttack = true;
            return;
        }


        if (chargeLevel >= maxChargeLevel)
        {
            if (Time.time - maxChargeLevelStartTime >= maxHoldDurationAtMaxCharge)
            {
                photonView.RPC("ReleaseAttack", RpcTarget.AllViaServer);
                releasedAttack = true;
                return;
            }
        }
        else if (Time.time - chargeStartTime >= durationToNextChargeLevel)
        {
            chargeStartTime = Time.time;


            if (chargeLevel < maxChargeLevel)
            {
                chargeLevel++;
                chargeSlider.value = chargeSlider.maxValue - (chargeSlider.maxValue / maxChargeLevel) * chargeLevel;


                // FX
                chargeFX.Play();
            }


            if (chargeLevel >= maxChargeLevel)
            {
                chargeSlider.value = 0;
                chargeLevel = maxChargeLevel;
                maxChargeLevelStartTime = Time.time;


                // FX
                chargeFullFX.Play();
                chargeFlareFX.Stop();


                // ANIMATION
                playerAnimations.TriggerMaxCharge();
            }
        }
    }

    public override void ManageParryInput()
    {
        if (canBriefParry)
        {
            if (InputManager.Instance.playerInputs[0].parry && canParry)
            {
                currentParryFramesPressed++;
                canParry = false;
                if (stamina >= staminaCostForMoves)
                    photonView.RPC("TriggerParry", RpcTarget.AllViaServer);

                currentParryFramesPressed = 0;

                if (!InputManager.Instance.playerInputs[0].parry)
                    canParry = true;
            }
        }
    }

    public override void ManagePommel()
    {
        if (!InputManager.Instance.playerInputs[0].kick)
        {
            canPommel = true;
        }
        else if (canPommel)
        {
            canPommel = false;
            photonView.RPC("TriggerPommel", RpcTarget.All);
        }


    }

    protected override void ApplyPommelHitbox()
    {
        float pommelRange = characterChanger.charactersDatabase.charactersList[characterIndex].character.pommelRange;

        Collider2D[] hitsCol = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (transform.localScale.x * -pommelRange / 2), transform.position.y), new Vector2(pommelRange, 0.2f), 0);
        List<GameObject> hits = new List<GameObject>();


        foreach (Collider2D c in hitsCol)
            if (c.CompareTag("Player") && !hits.Contains(c.transform.parent.gameObject))
                hits.Add(c.transform.parent.gameObject);


        foreach (GameObject g in hits)
        {
            if (g != gameObject && !targetsHit.Contains(g))
            {
                targetsHit.Add(g);

                if (g.GetComponent<Player>().playerState != Player.STATE.clashed)
                    g.GetComponent<PhotonView>().RPC("Pommeled", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public override void Pommeled()
    {
        if (!kickFrame)
        {
            bool wasSneathed = false;


            // ASKS TO START MATCH IF SNEATHED
            if (playerState == STATE.sneathed || playerState == STATE.drawing)
                wasSneathed = true;


            // ANIMATIONs
            playerAnimations.CancelCharge(true);
            playerAnimations.ResetPommeledTrigger();
            playerAnimations.TriggerPommeled();


            // Stamina
            if (playerState == STATE.parrying || playerState == STATE.attacking)
            {
                photonView.RPC("InitStaminaBreak", RpcTarget.All);
            }


            //NE PAS SUPPRIMER
            //StopAllCoroutines();
            SwitchState(STATE.clashed);
            ApplyOrientation(-GameManager.Instance.playersList[otherPlayerNum].transform.localScale.x);


            // STARTS MATCH IF PLAYER WAS SNEATHED
            if (wasSneathed)
                GameManager.Instance.SaberDrawn(playerNum);


            canCharge = false;
            chargeLevel = 1;




            // If is behind opponent when parried / clashed adds additional distance to evade the position and not look weird like they're fused together
            if (((transform.position.x - GameManager.Instance.playersList[otherPlayerNum].transform.position.x) * Mathf.Sign(transform.localScale.x)) <= 0.7f)
                transform.position = new Vector3(GameManager.Instance.playersList[otherPlayerNum].transform.position.x + -Mathf.Sign(GameManager.Instance.playersList[otherPlayerNum].transform.localScale.x) * 0.7f, transform.position.y, transform.position.z);




            // Dash knockback
            dashDirection = transform.localScale.x;
            actualUsedDashDistance = kickKnockbackDistance;
            initPos = transform.position;
            targetPos = transform.position + new Vector3(actualUsedDashDistance * dashDirection, 0, 0);
            dashTime = 0;
            isDashing = true;


            // FX
            kickKanasFX.Play();
            kickedFX.Play();
            GameManager.Instance.pommelCameraShake.shakeDuration = GameManager.Instance.pommelCameraShakeDuration;



            // AUDIO
            //audioManager.TriggerClashAudioCoroutine();
            AudioManager.Instance.BattleEventIncreaseIntensity();


            // STATS
            if (characterType == CharacterType.duel)
            {
                if (statsManager)
                    statsManager.AddAction(ACTION.successfulPommel, otherPlayerNum, chargeLevel);
                else
                    Debug.Log("Couldn't access statsManager to record action, ignoring");
            }
        }
    }

    public override void ManageDashInput()
    {
        // Detects dash with basic input rather than double tap, shortcut
        if (Mathf.Abs(InputManager.Instance.playerInputs[0].dash) < shortcutDashDeadZone && currentShortcutDashStep == DASHSTEP.invalidated && stamina >= staminaCostForMoves)
            //inputManager.playerInputs[playerStats.playerNum - 1].horizontal;
            currentShortcutDashStep = DASHSTEP.rest;


        if (Mathf.Abs(InputManager.Instance.playerInputs[0].dash) > shortcutDashDeadZone && currentShortcutDashStep == DASHSTEP.rest)
        {
            dashDirection = Mathf.Sign(InputManager.Instance.playerInputs[0].dash);

            photonView.RPC("TriggerBasicDash", RpcTarget.All);
        }


        // Resets the dash input if too much time has passed
        if (currentDashStep == DASHSTEP.firstInput || currentDashStep == DASHSTEP.firstRelease)
            if (Time.time - dashInitializationStartTime > allowanceDurationForDoubleTapDash)
                currentDashStep = DASHSTEP.invalidated;


        // The player needs to let go the direction before pressing it again to dash
        if (Mathf.Abs(InputManager.Instance.playerInputs[0].horizontal) < dashDeadZone)
        {
            if (currentDashStep == DASHSTEP.firstInput)
                currentDashStep = DASHSTEP.firstRelease;
            // To make the first dash input he must have not been pressing it before, we need a double tap
            else if (currentDashStep == DASHSTEP.invalidated)
                currentDashStep = DASHSTEP.rest;
        }


        // When the player presses the direction
        // Presses the
        if (Mathf.Abs(InputManager.Instance.playerInputs[0].horizontal) > dashDeadZone)
        {
            temporaryDashDirectionForCalculation = Mathf.Sign(InputManager.Instance.playerInputs[0].horizontal);

            if (currentDashStep == DASHSTEP.rest)
            {
                currentDashStep = DASHSTEP.firstInput;
                dashDirection = temporaryDashDirectionForCalculation;
                dashInitializationStartTime = Time.time;

            }
            // Dash is validated, the player is gonna dash
            else if (currentDashStep == DASHSTEP.firstRelease && dashDirection == temporaryDashDirectionForCalculation)
            {
                currentDashStep = DASHSTEP.invalidated;
                photonView.RPC("TriggerBasicDash", RpcTarget.All);
            }
        }
    }

    #region RPC Methods
    [PunRPC]
    public override void ResetAllPlayerValuesForNextRound()
    {
        base.ResetAllPlayerValuesForNextRound();
    }

    [PunRPC]
    public override void CheckDeath(int i)
    {
        base.CheckDeath(i);
    }

    [PunRPC]
    protected override void TriggerHit()
    {
        base.TriggerHit();
    }

    [PunRPC]
    protected override void N_TriggerStaminaRecupAnim()
    {
        base.N_TriggerStaminaRecupAnim();
    }

    [PunRPC]
    protected override void InitStaminaBreak()
    {
        base.InitStaminaBreak();
    }

    [PunRPC]
    public override void TriggerDraw()
    {
        base.TriggerDraw();
    }

    [PunRPC]
    protected override void ReleaseAttack()
    {
        base.ReleaseAttack();
        releasedAttack = false;
    }

    [PunRPC]
    protected override void TriggerParry()
    {
        base.TriggerParry();
    }

    [PunRPC]
    protected override void TriggerPommel()
    {
        base.TriggerPommel();
    }

    [PunRPC]
    protected override void TriggerClash()
    {
        base.TriggerClash();
    }

    [PunRPC]
    public override void ResetPos()
    {
        base.ResetPos();
    }

    [PunRPC]
    protected override void TriggerBasicDash()
    {
        base.TriggerBasicDash();
    }
    #endregion



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.name);
            stream.SendNext(currentHealth);
            stream.SendNext(playerNum);
            stream.SendNext(stamina);
            stream.SendNext(transform.position);
            stream.SendNext(transform.localScale.x);
            stream.SendNext(enemyDead);
            stream.SendNext(staminaBarsOpacity);
            //stream.SendNext(rb.velocity);
            stream.SendNext(actualMovementsSpeed);
            stream.SendNext(playerState);
        }
        else if (stream.IsReading)
        {
            transform.name = (string)stream.ReceiveNext();
            currentHealth = (float)stream.ReceiveNext();
            playerNum = (int)stream.ReceiveNext();
            stamina = (float)stream.ReceiveNext();
            Vector3 DistantPos = (Vector3)stream.ReceiveNext();
            float xScale = (float)stream.ReceiveNext();
            enemyDead = (bool)stream.ReceiveNext();
            staminaBarsOpacity = (float)stream.ReceiveNext();
            //rb.velocity = (Vector2)stream.ReceiveNext();
            actualMovementsSpeed = (float)stream.ReceiveNext();
            SwitchState((STATE)stream.ReceiveNext());

            //Calculate target position based on lag
            netTargetPos = new Vector2(DistantPos.x, DistantPos.y);

            transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
        }
    }
}
