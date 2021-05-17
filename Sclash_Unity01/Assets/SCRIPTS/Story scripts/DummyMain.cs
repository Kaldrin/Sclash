using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;




// Bastien BERNAND
// For Sclash

// REQUIREMENTS
// PitchModulator Script

/// <summary>
/// Main script for the training dummies
/// </summary>

// UNITY 2020.3
public class DummyMain : MonoBehaviour
{
    // COMPONENTS
    [SerializeField] Animator dummyAnimator = null;
    [SerializeField] List<Collider2D> colliders = new List<Collider2D>();
    [SerializeField] SpriteRenderer dummySpriteRenderer = null;


    // TYPE

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
    }


    // ANIMATIONS
    string attackedParameterName = "Attacked";
    string kickedParameterName = "Kicked";

    [Header("FX")]
    [SerializeField] ParticleSystem kickedFX01 = null;
    [SerializeField] ParticleSystem kickedBigFX01 = null;
    [SerializeField] ParticleSystem destroyedFX01 = null;
    [SerializeField] ParticleSystem sliceFX01 = null;


    [Header("AUDIO")]
    [SerializeField] PitchModulator kickedSFX = null;
    [SerializeField] PitchModulator kickedBigSFX = null;
    [SerializeField] PitchModulator destroyedSFX = null;


    [Header("EDITOR VISUAL")]
    [SerializeField] Sprite attackSprite = null;
    [SerializeField] Sprite pommelSprite = null;
    [SerializeField] Sprite parrySprite = null;
    [SerializeField] Sprite dodgeSprite = null;







    private void Awake()                                                                                                                    // AWAKE
    {
        SetDummyType();
        dummyAnimator.SetInteger("Type", animatorType);
    }

    private void Start()                                                                                                                // START
    {
        
    }

    public void SwitchState(DummyState newState)                                                                                            // SWITCH STATE
    {
        oldState = dummyState;
        dummyState = newState;

        switch (newState)
        {
            case DummyState.kicked:
                break;
            case DummyState.idle:
                break;
        }
    }


    public void Hit(int side)                                                                                                                       // HIT
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
                    dummyAnimator.SetTrigger(attackedParameterName);


                // FX
                if (destroyedFX01)
                    destroyedFX01.Play();
                if (sliceFX01)
                    sliceFX01.Play();

                // AUDIO
                if (destroyedSFX)
                    destroyedSFX.Play();

                // COLLISIONS
                if (colliders != null && colliders.Count > 0)
                    for (int i = 0; i < colliders.Count; i++)
                        colliders[i].enabled = false;
            }
        }
        else
        {
            if (dummyState == DummyState.idle)
            {
                // STATE
                SwitchState(DummyState.kicked);

                // ANIM
                if (dummyAnimator)
                    dummyAnimator.SetTrigger(attackedParameterName);


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
        // ORIENTATION
        if (side > 0)
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);

        if (dummyType == DummyType.pommel)
        {
            if (dummyState == DummyState.idle || dummyState == DummyState.kicked)
            {
                // STATE
                SwitchState(DummyState.destroyed);

                // ANIM
                if (dummyAnimator)
                    dummyAnimator.SetTrigger("Kicked");


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
            if (dummyState == DummyState.idle)
            {
                // STATE
                SwitchState(DummyState.kicked);


                // FX
                if (kickedFX01)
                    kickedFX01.Play();


                // ANIM
                if (dummyAnimator)
                    dummyAnimator.SetTrigger("Kicked");


                // AUDIO
                if (kickedSFX)
                    kickedSFX.Play();
            }
        }
    }



    void SetDummyType()
    {
        switch (dummyType)
        {
            case DummyType.attack:
                animatorType = 0;
                if (dummySpriteRenderer && attackSprite)
                    dummySpriteRenderer.sprite = attackSprite;
                break;
            case DummyType.pommel:
                animatorType = 1;
                if (dummySpriteRenderer && pommelSprite)
                    dummySpriteRenderer.sprite = pommelSprite;
                break;
            case DummyType.parry:
                animatorType = 2;
                if (dummySpriteRenderer && parrySprite)
                    dummySpriteRenderer.sprite = parrySprite;
                break;
            case DummyType.dodge:
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

            SetDummyType();


            #if UNITY_EDITOR
                HandleUtility.Repaint();
            #endif
        }
    }
}
