using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;




// Bastien BERNAND
// For Sclash

// REQUIREMENTS
// PitchModulator script
// CampaignDoor script

/// <summary>
/// Script for the training dummies
/// </summary>

// UNITY 2020.3
public class DummyMain : MonoBehaviour
{
    #region VARIABLES
    [Header("COMPONENTS")]
    [SerializeField] Animator dummyAnimator = null;
    [SerializeField] List<Collider2D> colliders = new List<Collider2D>();
    [SerializeField] SpriteRenderer dummySpriteRenderer = null;
    [SerializeField] CampaignDoor doorLink = null;


    [Header("STATE")]
    [SerializeField] DummyType dummyType = DummyType.attack;
    int animatorType = 0;
    enum DummyType
    {
        attack,
        pommel,
        parry,
        dodge,
    }
    [SerializeField] DummyState dummyState = DummyState.idle;
    DummyState oldState = DummyState.idle;
    public enum DummyState
    {
        idle,
        kicked,
        destroyed,
        attacking,
    }
    [SerializeField] public bool attackFrame = false;



    // PARAMETERS
    protected List<GameObject> targetsHit = new List<GameObject>();
    float attackRange = 2f;
    float attackDelayStartTime = 0f;
    float attackDelay = 2f;
    bool attackDelayFinished = true;
    


    // ANIMATIONS
    string attackedAnimatorParameterName = "Attacked";
    string kickedAnimatorParameterName = "Kicked";
    string attackAnimatorParameterName = "Attack";


    [Header("WEAPONS")]
    [SerializeField] GameObject bokkenObject = null;
    [SerializeField] Rigidbody2D physicBokkenRigidbody2D = null;
    [SerializeField] Collider2D physicBokkenCollider2D = null;
    [SerializeField] GameObject shieldObject = null;
    [SerializeField] Rigidbody2D physicShieldRigidbody2D = null;
    [SerializeField] Collider2D physicShieldCollider2D = null;



    [Header("FX")]
    [SerializeField] ParticleSystem kickedFX01 = null;
    [SerializeField] ParticleSystem kickedBigFX01 = null;
    [SerializeField] ParticleSystem destroyedFX01 = null;
    [SerializeField] ParticleSystem sliceFX01 = null;
    [SerializeField] ParticleSystem attackDustFX01 = null;
    [SerializeField] ParticleSystem parryAttackCrickDustFX01 = null;



    [Header("AUDIO")]
    [SerializeField] PitchModulator kickedSFX = null;
    [SerializeField] PitchModulator kickedBigSFX = null;
    [SerializeField] PitchModulator destroyedSFX = null;
    [SerializeField] PitchModulator parryAttackCrickSFX01 = null;



    [Header("EDITOR")]
    [SerializeField] Sprite attackSprite = null;
    [SerializeField] Sprite pommelSprite = null;
    [SerializeField] Sprite parrySprite = null;
    [SerializeField] Sprite dodgeSprite = null;
    [SerializeField] Transform groundPoint = null;
    #endregion










    #region FUNCTIONS
    private void Awake()                                                                                                                    // AWAKE
    {
        SetDummyType();
        dummyAnimator.SetInteger("Type", animatorType);
    }


    private void Update()                                                                                                               // UPDATE
    {
        if (enabled && isActiveAndEnabled)
        {
            // Destroyed if finished and not visible
            if (dummyState == DummyState.destroyed && !dummySpriteRenderer.isVisible)
                Destroy(gameObject);


            // PARRY ATTACK
            if (dummyType == DummyType.parry || dummyType == DummyType.dodge)
            {
                if (dummyState == DummyState.idle)
                {
                    if (attackDelayFinished)
                        DetectEnemy();
                    else if (!attackDelayFinished && Time.time > attackDelayStartTime + attackDelay)
                        attackDelayFinished = true;
                }
                else if (dummyState == DummyState.attacking && attackFrame)
                    ApplyAttackFrames();
            }

        }
    }







    public void SwitchState(DummyState newState = DummyState.idle)                                                                                            // SWITCH STATE
    {
        if (dummyState != newState)
        {
            oldState = dummyState;
            dummyState = newState;

            switch (newState)
            {
                case DummyState.kicked:
                    break;
                case DummyState.idle:
                    break;
                case DummyState.destroyed:
                    if (doorLink)
                        doorLink.DummyDestroyed();
                    break;
            }
        }
    }


    public void Dodged()
    {
        if (dummyType == DummyType.dodge)
        {
            // PHYSICS
            if (physicBokkenRigidbody2D)
            {
                physicBokkenRigidbody2D.gameObject.SetActive(true);
                physicBokkenRigidbody2D.simulated = true;
                physicBokkenRigidbody2D.AddForce(new Vector2(Random.Range(transform.localScale.x * 2, transform.localScale.x * 10f), Random.Range(0.5f, 5f)), ForceMode2D.Impulse);
                physicBokkenRigidbody2D.AddTorque(Random.Range(5, 20f));
            }
            if (physicBokkenCollider2D)
                physicBokkenCollider2D.enabled = true;
            if (physicShieldRigidbody2D)
            {
                physicShieldRigidbody2D.gameObject.SetActive(true);
                physicShieldRigidbody2D.simulated = true;
                physicShieldRigidbody2D.AddForce(new Vector2(Random.Range(transform.localScale.x * 2, transform.localScale.x * 10f), Random.Range(0.5f, 5f)), ForceMode2D.Impulse);
                physicShieldRigidbody2D.AddTorque(Random.Range(5, 20f));
            }
            if (physicShieldCollider2D)
                physicShieldCollider2D.enabled = true;


            // FX
            if (destroyedFX01)
                destroyedFX01.Play();
            if (attackDustFX01)
                attackDustFX01.Play();

            // AUDIO
            if (destroyedSFX)
                destroyedSFX.Play();


            // STATE
            SwitchState(DummyState.destroyed);


            // ANIMATION
            if (dummyAnimator)
                dummyAnimator.SetTrigger(attackedAnimatorParameterName);


            // COLLISIONS
            for (int i = 0; i < colliders.Count; i++)
                colliders[i].enabled = false;
        }
    }

    public void Parried()
    {
        if (dummyType == DummyType.parry)
        {
            // PHYSICS
            if (physicBokkenRigidbody2D)
            {
                physicBokkenRigidbody2D.gameObject.SetActive(true);
                physicBokkenRigidbody2D.simulated = true;
                physicBokkenRigidbody2D.AddForce(new Vector2(Random.Range(transform.localScale.x * 2, transform.localScale.x * 10f), Random.Range(0.5f, 5f)), ForceMode2D.Impulse);
                physicBokkenRigidbody2D.AddTorque(Random.Range(5, 20f));
            }
            if (physicBokkenCollider2D)
                physicBokkenCollider2D.enabled = true;

            // FX
            if (destroyedFX01)
                destroyedFX01.Play();
            if (attackDustFX01)
                attackDustFX01.Play();

            // AUDIO
            if (destroyedSFX)
                destroyedSFX.Play();


            // STATE
            SwitchState(DummyState.destroyed);


            // ANIMATION
            if (dummyAnimator)
                dummyAnimator.SetTrigger(attackedAnimatorParameterName);


            // COLLISIONS
            for (int i = 0; i < colliders.Count; i++)
                colliders[i].enabled = false;
        }
        else
        {
            // STATE
            SwitchState(DummyState.kicked);


            ResetAttackDelay();

            // FX
            if (kickedFX01)
                kickedFX01.Play();


            // ANIM
            if (dummyAnimator)
                dummyAnimator.SetTrigger(kickedAnimatorParameterName);


            // AUDIO
            if (kickedSFX)
                kickedSFX.Play();
        }
    }


    public void Hit(int side, bool maxCharge = false)                                                                                                                       // HIT
    {
        // ORIENTATION
        if (side > 0)
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);


        if (dummyType == DummyType.attack)
        {
            if (dummyState == DummyState.idle || dummyState == DummyState.kicked)
            {
                // STATE
                SwitchState(DummyState.destroyed);


                // ANIM
                if (dummyAnimator)
                    dummyAnimator.SetTrigger(attackedAnimatorParameterName);


                // FX
                if (destroyedFX01)
                    destroyedFX01.Play();
                if (sliceFX01)
                    sliceFX01.Play();
                if (attackDustFX01)
                    attackDustFX01.Play();

                // AUDIO
                if (destroyedSFX)
                    destroyedSFX.Play();

                // COLLISIONS
                if (colliders != null && colliders.Count > 0)
                    for (int i = 0; i < colliders.Count; i++)
                        colliders[i].enabled = false;
            }
        }
        else if (maxCharge)
        {
            // STATE
            SwitchState(DummyState.destroyed);


            // FX
            if (sliceFX01)
                sliceFX01.Play();


            // COLLISIONS
            if (colliders != null && colliders.Count > 0)
                for (int i = 0; i < colliders.Count; i++)
                    colliders[i].enabled = false;


            if (dummyType == DummyType.pommel)
            {
                // FX
                if (kickedBigFX01)
                    kickedBigFX01.Play();

                // ANIM
                if (dummyAnimator)
                    dummyAnimator.SetTrigger(kickedAnimatorParameterName);

                // AUDIO
                if (kickedBigSFX)
                    kickedBigSFX.Play();
            }
            else
            {
                // FX
                if (destroyedFX01)
                    destroyedFX01.Play();
                if (attackDustFX01)
                    attackDustFX01.Play();

                // AUDIO
                if (destroyedSFX)
                    destroyedSFX.Play();


                // ANIM
                if (dummyAnimator)
                    dummyAnimator.SetTrigger(attackedAnimatorParameterName);
            }
            if (dummyType == DummyType.parry || dummyType == DummyType.dodge)
            {
                // PHYSICS
                if (physicBokkenRigidbody2D)
                {
                    physicBokkenRigidbody2D.gameObject.SetActive(true);
                    physicBokkenRigidbody2D.simulated = true;
                    physicBokkenRigidbody2D.AddForce(new Vector2(Random.Range(transform.localScale.x * 2, transform.localScale.x * 10f), Random.Range(0.5f, 5f)), ForceMode2D.Impulse);
                    physicBokkenRigidbody2D.AddTorque(Random.Range(5, 20f));
                }
                if (physicBokkenCollider2D)
                    physicBokkenCollider2D.enabled = true;

                if (dummyType == DummyType.dodge)
                {
                    if (physicShieldRigidbody2D)
                    {
                        physicShieldRigidbody2D.gameObject.SetActive(true);
                        physicShieldRigidbody2D.simulated = true;
                        physicShieldRigidbody2D.AddForce(new Vector2(Random.Range(transform.localScale.x * 2, transform.localScale.x * 10f), Random.Range(0.5f, 5f)), ForceMode2D.Impulse);
                        physicShieldRigidbody2D.AddTorque(Random.Range(5, 20f));
                    }
                    if (physicShieldCollider2D)
                        physicShieldCollider2D.enabled = true;
                }
            }
            
        }
        else
        {
            if (dummyState == DummyState.idle || dummyState == DummyState.attacking)
            {
                // STATE
                SwitchState(DummyState.kicked);

                ResetAttackDelay();

                // ANIM
                if ((dummyType == DummyType.parry || dummyType == DummyType.dodge) && dummyAnimator)
                    dummyAnimator.SetTrigger(kickedAnimatorParameterName);
                else if (dummyAnimator)
                    dummyAnimator.SetTrigger(attackedAnimatorParameterName);


                // FX
                if (sliceFX01)
                    sliceFX01.Play();


                // AUDIO
                if (kickedSFX)
                    kickedSFX.Play();
            }
        }
    }
    

    public void Kicked(int side)                                                                                                                    // KICKED
    {
        if (dummyType == DummyType.pommel)
        {
            if (dummyState == DummyState.idle || dummyState == DummyState.kicked)
            {
                // ORIENTATION
                if (side > 0)
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                else
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);


                // STATE
                SwitchState(DummyState.destroyed);

                // ANIM
                if (dummyAnimator)
                    dummyAnimator.SetTrigger(kickedAnimatorParameterName);


                // COLLISIONS
                if (colliders != null && colliders.Count > 0)
                    for (int i = 0; i < colliders.Count; i++)
                        colliders[i].enabled = false;

                // FX
                if (kickedBigFX01)
                    kickedBigFX01.Play();


                // AUDIO
                if (kickedBigSFX)
                    kickedBigSFX.Play();
            }
        }
        else
        {
            if (dummyState == DummyState.idle || dummyState == DummyState.attacking)
            {
                // ORIENTATION
                if (side > 0)
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                else
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);


                // STATE
                SwitchState(DummyState.kicked);


                ResetAttackDelay();

                // FX
                if (kickedFX01)
                    kickedFX01.Play();


                // ANIM
                if (dummyAnimator)
                    dummyAnimator.SetTrigger(kickedAnimatorParameterName);


                // AUDIO
                if (kickedSFX)
                    kickedSFX.Play();
            }
        }
    }


    #region ATTACK
    void DetectEnemy()
    {
        Collider2D[] hitsCol = Physics2D.OverlapBoxAll(transform.position, new Vector2(attackRange * 2, 1), 0);
        List<GameObject> hits = new List<GameObject>();


        foreach (Collider2D c in hitsCol)
            if (c.CompareTag("Player"))
                ReleaseAttack(c.transform.parent.gameObject);
    }

    void ReleaseAttack(GameObject enemy)
    {
        // ORIENTATION
        if (transform.position.x - enemy.transform.position.x < 0)
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);


        targetsHit.Clear();
        ResetAttackDelay();


        // STATE
        SwitchState(DummyState.attacking);



        // ANIM
        if (dummyAnimator)
            dummyAnimator.SetTrigger(attackAnimatorParameterName);


        // FX
        if (parryAttackCrickDustFX01)
            parryAttackCrickDustFX01.Play();


        // AUDIO
        if (parryAttackCrickSFX01)
            parryAttackCrickSFX01.Play();
    }

    void ApplyAttackFrames()
    {
        Vector2 attackPos = new Vector2(transform.position.x + (attackRange / 2) * -transform.localScale.x, transform.position.y);
        Collider2D[] hitsCol = Physics2D.OverlapBoxAll(attackPos, new Vector2(attackRange, 1), 0);
        List<GameObject> hits = new List<GameObject>();


        foreach (Collider2D c in hitsCol)
            if (c.CompareTag("Player") && !hits.Contains(c.transform.parent.gameObject))
                hits.Add(c.transform.parent.gameObject);


        foreach (GameObject g in hits)
            if (g != gameObject && !targetsHit.Contains(g) && g.GetComponent<Player>())
            {
                targetsHit.Add(g);
                g.GetComponent<Player>().Pommeled(gameObject);

                //enemyDead = g.GetComponent<Player>().TakeDamage(gameObject, chargeLevel);

                /*
                // FX
                attackRangeFX.gameObject.SetActive(false);
                attackRangeFX.gameObject.SetActive(true);


                if (enemyDead)
                    SwitchState(STATE.enemyKilled);
                    */
            }
    }

    void ResetAttackDelay()
    {
        attackDelayFinished = false;
        attackDelayStartTime = Time.time;
    }
    #endregion



    void SetDummyType()
    {
        switch (dummyType)
        {
            case DummyType.attack:
                if (bokkenObject)
                    bokkenObject.SetActive(false);
                if (shieldObject)
                    shieldObject.SetActive(false);
                animatorType = 0;
                if (dummySpriteRenderer && attackSprite)
                    dummySpriteRenderer.sprite = attackSprite;
                break;
            case DummyType.pommel:
                if (bokkenObject)
                    bokkenObject.SetActive(false);
                if (shieldObject)
                    shieldObject.SetActive(false);
                animatorType = 1;
                if (dummySpriteRenderer && pommelSprite)
                    dummySpriteRenderer.sprite = pommelSprite;
                break;
            case DummyType.parry:
                if (bokkenObject)
                    bokkenObject.SetActive(true);
                if (shieldObject)
                    shieldObject.SetActive(false);
                animatorType = 2;
                if (dummySpriteRenderer && parrySprite)
                    dummySpriteRenderer.sprite = parrySprite;
                break;
            case DummyType.dodge:
                if (bokkenObject)
                    bokkenObject.SetActive(true);
                if (shieldObject)
                    shieldObject.SetActive(true);
                animatorType = 3;
                if (dummySpriteRenderer && dodgeSprite)
                    dummySpriteRenderer.sprite = dodgeSprite;
                break;
        }
    }






    // EDITOR
    private void OnDrawGizmosSelected()
    {
        if (enabled)
        {
            // DRAW DETECTION ZONE
            Gizmos.DrawWireCube(transform.position, new Vector3(attackRange * 2, 1, 0.1f));
            // DRAW ATTACK ZONE
            Gizmos.color = Color.red;
            Vector3 attackPos = new Vector3(transform.position.x + (attackRange / 2) * -transform.localScale.x, transform.position.y, transform.position.z);
            //Collider2D[] hitsCol = Physics2D.OverlapBoxAll(attackPos, new Vector2(attackRange, 1), 0);
            Gizmos.DrawWireCube(attackPos, new Vector3(attackRange, 1, 0.1f));


            SetDummyType();



            // PLACE DUMMY ON GROUND
            float distance = 5;

            RaycastHit2D raycastHit2D = Physics2D.Raycast((Vector3)transform.position + (Vector3.up * distance), Vector3.down, distance * 2, LayerMask.GetMask("Level"));
            if (raycastHit2D.collider != null && raycastHit2D.collider != GetComponent<Collider>())
            {
                Debug.DrawRay((Vector3)transform.position + (Vector3.up * distance), Vector3.down * distance * 2, Color.red);
                if (groundPoint)
                    transform.position = new Vector3(transform.position.x, raycastHit2D.point.y - groundPoint.localPosition.y, transform.position.z);
            }
            else
                Debug.DrawRay((Vector3)transform.position + (Vector3.up * distance), Vector3.down * distance * 2, Color.white);


#if UNITY_EDITOR
            HandleUtility.Repaint();
            #endif
        }
    }
    #endregion
}
