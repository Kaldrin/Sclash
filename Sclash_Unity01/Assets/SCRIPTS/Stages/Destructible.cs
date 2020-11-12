using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is for a destructible element, placed on an element with a 2D collider on a stage
// OPTIMIZED
public class Destructible : MonoBehaviour
{
    [SerializeField] SpriteRenderer destroyableElementSpriteRenderer = null;
    [SerializeField] Sprite destroyedSprite = null;
    [SerializeField] GameObject fallingPart = null;
    [Tooltip("Usually a light")]
    [SerializeField] GameObject objectToDisable = null;
    bool destroyed = false;
    bool destroyedFallingPart = false;


    [Header("SETTINGS")]
    [SerializeField] DestructibleProfile destructibleProfile = null;


    [Header("FX")]
    [SerializeField] ParticleSystem destroyedFX1 = null;
    [SerializeField] AudioSource destroyedSFX1 = null;




    private void Start()
    {
        if (fallingPart.activeInHierarchy)
            fallingPart.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (enabled && isActiveAndEnabled && destroyed && !destroyedFallingPart && fallingPart != null)
            if (fallingPart.GetComponent<Rigidbody>() && fallingPart.GetComponent<Rigidbody>().velocity.y < destructibleProfile.fallingPartMaxSpeed)
                fallingPart.GetComponent<Rigidbody>().velocity = new Vector3(fallingPart.GetComponent<Rigidbody>().velocity.x, destructibleProfile.fallingPartMaxSpeed, fallingPart.GetComponent<Rigidbody>().velocity.z);
    }

    public void Destroy()
    {
        if (enabled && isActiveAndEnabled && !destroyed)
        {
            destroyed = true;


            // FX
            if (destroyedFX1 != null)
                destroyedFX1.Play();

            if (destroyedSFX1 != null)
                destroyedSFX1.Play();

            if (objectToDisable != null && objectToDisable.activeInHierarchy)
                objectToDisable.SetActive(false);


            // CHANGE SPRITE
            if (destroyableElementSpriteRenderer != null && destroyedSprite != null)
                destroyableElementSpriteRenderer.sprite = destroyedSprite;

            // FALLING PART
            if (fallingPart != null)
            {
                fallingPart.SetActive(true);


                if (fallingPart.GetComponent<Rigidbody>())
                {
                    float horizontalSpeed = Random.Range(destructibleProfile.fallingPartHorizontalRandomSpeedRange.x, destructibleProfile.fallingPartHorizontalRandomSpeedRange.y);
                    float randomDirection = Mathf.Sign(Random.Range(-1, 1) + 0.1f);
                    horizontalSpeed = horizontalSpeed * randomDirection;

                    fallingPart.GetComponent<Rigidbody>().velocity = new Vector3(horizontalSpeed, Random.Range(destructibleProfile.fallingPartVerticalRandomSpeedRange.x, destructibleProfile.fallingPartVerticalRandomSpeedRange.y), Random.Range(-2f, 2f));
                    fallingPart.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, Random.Range(destructibleProfile.fallingPartAngularRandomSpeedRange.y, destructibleProfile.fallingPartAngularRandomSpeedRange.x));
                }


                Invoke("DestroyFallingPart", 2f);
            }
        }
    }

    void DestroyFallingPart()
    {
        if (fallingPart != null)
        {
            Destroy(fallingPart);
            destroyedFallingPart = true;
        }
    }

    
}
