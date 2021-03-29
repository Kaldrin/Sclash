using System.Collections;
using System.Collections.Generic;
using UnityEngine;





// OPTIMIZED
// For Sclash

// REQUIREMENTS
// DestructibleProfile scriptable object

/// <summary>
/// This script is for a destructible element, placed on an element with a 2D collider on a stage
/// </summary>

// Unity 2019.4.14
public class Destructible : MonoBehaviour
{
    [Header("COMPONENTS")]
    [SerializeField] SpriteRenderer destroyableElementSpriteRenderer = null;
    [SerializeField] MeshRenderer meshRenderer = null;


    [Header("DESTROYED EVENT")]
    [SerializeField] Sprite destroyedSprite = null;
    [SerializeField] Material destroyedMaterial = null;
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




    #region FUNCTIONS
    private void Start()                                                                                                                                                    // START
    {
        if (fallingPart && fallingPart.activeInHierarchy)
            fallingPart.SetActive(false);
    }


    private void FixedUpdate()                                                                                                                                                    // FIXED UPDATE
    {
        // Limit falling part velocity
        if (enabled && isActiveAndEnabled && destroyed && !destroyedFallingPart && fallingPart != null)
            if (fallingPart.GetComponent<Rigidbody>() && fallingPart.GetComponent<Rigidbody>().velocity.y < destructibleProfile.fallingPartMaxSpeed)
                fallingPart.GetComponent<Rigidbody>().velocity = new Vector3(fallingPart.GetComponent<Rigidbody>().velocity.x, destructibleProfile.fallingPartMaxSpeed, fallingPart.GetComponent<Rigidbody>().velocity.z);
    }



    public void Destroy()                                                                                                                                                                       // DESTROY
    {
        if (enabled && isActiveAndEnabled && !destroyed)
        {
            // Set right sorting layer for renderers
            SetLayer();

            destroyed = true;


            // FX
            if (destroyedFX1 != null)
                destroyedFX1.Play();

            if (destroyedSFX1 != null)
                destroyedSFX1.Play();

            if (objectToDisable != null && objectToDisable.activeInHierarchy)
                objectToDisable.SetActive(false);


            // CHANGE VISUAL TO DESTROYED
            if (destroyableElementSpriteRenderer != null && destroyedSprite != null)
                destroyableElementSpriteRenderer.sprite = destroyedSprite;
            if (destroyedMaterial != null)
                meshRenderer.material = destroyedMaterial;


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

    void DestroyFallingPart()                                                                                                                                                   // DESTROY FALLING PART
    {
        if (fallingPart != null)
        {
            Destroy(fallingPart);
            destroyedFallingPart = true;
        }
    }




    void SetLayer()
    {
        // Get layer
        int sortingLayerID = destroyableElementSpriteRenderer.sortingLayerID;
        string sortingLayerName = destroyableElementSpriteRenderer.sortingLayerName;
        int sortingOrder = destroyableElementSpriteRenderer.sortingOrder;


        // Set layer
        if (fallingPart && fallingPart.GetComponent<SpriteRenderer>())
        {
            SpriteRenderer spriteRenderer = fallingPart.GetComponent<SpriteRenderer>();
            if (spriteRenderer.sortingLayerName != sortingLayerName)
                spriteRenderer.sortingLayerName = sortingLayerName;
            if (spriteRenderer.sortingOrder != sortingOrder)
                spriteRenderer.sortingOrder = sortingOrder;
        }

        if (destroyedFX1)
        {
            if (destroyedFX1.GetComponent<ParticleSystemRenderer>().sortingOrder != sortingOrder + 1)
                destroyedFX1.GetComponent<ParticleSystemRenderer>().sortingOrder = sortingOrder + 1;
            if (destroyedFX1.GetComponent<ParticleSystemRenderer>().sortingLayerName != sortingLayerName)
                destroyedFX1.GetComponent<ParticleSystemRenderer>().sortingLayerName = sortingLayerName;

            ParticleSystem[] particleSystems = destroyedFX1.transform.GetComponentsInChildren<ParticleSystem>();

            for (int i = 0; i < particleSystems.Length; i++)
            {
                if (particleSystems[i].GetComponent<ParticleSystemRenderer>().sortingOrder != sortingOrder + 1)
                    particleSystems[i].GetComponent<ParticleSystemRenderer>().sortingOrder = sortingOrder + 1;
                if (particleSystems[i].GetComponent<ParticleSystemRenderer>().sortingLayerName != sortingLayerName)
                    particleSystems[i].GetComponent<ParticleSystemRenderer>().sortingLayerName = sortingLayerName;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        SetLayer();
    }
    #endregion
}
