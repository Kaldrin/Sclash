using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;






// HEADER
// For Sclash
// OPTIMIZED I THINK
// CAMPAIGN ONLY

// REQUIREMENTS
// Player script
// TextMeshPro package
// EnvironmentStart01 script

/// <summary>
/// Script used on environment transition objects for the campaign mode to load the next parts of the level and change the particles used by the player, when the player passes on it
/// </summary>

// VERSION
// Made for Unity 2019.4.14
public class EnvironmentTransition01 : MonoBehaviour
{
    [Header("DATA")]
    [SerializeField] TagsReferences tagsReferences = null;



    [Header("SETTINGS")]
    [SerializeField] GameObject environmentToUnload = null;
    [SerializeField] GameObject environmentToLoad = null;
    [SerializeField] bool changePlayerParticlesSet = false;
    [SerializeField] int newParticleSetIndex = 0;
    [Tooltip("Duration before this transition objects can be used again (To prevent it triggered 50 times because the player has multiple colliders)")]
    [SerializeField] float cooldown = 10f;


    float transitionStartTime = 0f;
    bool transitoned = false;



    [Header("EDITOR")]
    [SerializeField] GameObject editorDisplayStuff = null;
    [SerializeField] Color volumeColor = Color.red;
    [SerializeField] Color volumeColorIfActivatesChunks = Color.red;
    [SerializeField] bool wiredVolume = true;
    [SerializeField] TextMeshPro particleIndexDisplay = null;
    [SerializeField] SpriteRenderer particleSetIconDisplay = null;
    [SerializeField] TextMeshPro environmentToLoadDisplay = null;
    [SerializeField] TextMeshPro environmentToUnloadDisplay = null;
    [SerializeField] GameObject backgroundShadow = null;
    public Player player;










    private void Start()                                                                                                                                                          // START
    {
        // Disable editor only stuff
        if (editorDisplayStuff && editorDisplayStuff.activeInHierarchy)
            editorDisplayStuff.SetActive(false);

    }


    private void Update()                                                                                                                                                        // UPDATE
    {
        // Transition cooldown timer
        if (enabled && isActiveAndEnabled)
            if (transitoned && Time.time - cooldown > transitionStartTime)
                transitoned = false;
    }


    // DETECTS PLAYER
    private void OnTriggerEnter2D(Collider2D collision)                                                                                                                           // ON TRIGGER ENTER 2D
    {
        if (!transitoned)
            if (collision.gameObject.CompareTag(tagsReferences.playerTag) || collision.transform.parent.gameObject.CompareTag(tagsReferences.playerTag))
            {
                Player player = null;


                if (collision.GetComponent<Player>())
                    player = collision.GetComponent<Player>();
                if (collision.transform.parent.GetComponent<Player>())
                    player = collision.transform.parent.GetComponent<Player>();


                TriggerTransition(player);
            }
    }








    // Loads next environment and unloads previous environment
    void TriggerTransition(Player player)                                                                                                                                       // TRIGGER TRANSITION
    {
        transitionStartTime = Time.time;
        transitoned = true;



        // LOAD / UNLOAD
        if (environmentToUnload)
        {
            if (environmentToUnload.GetComponent<EnvironmentStart01>())
                environmentToUnload.GetComponent<EnvironmentStart01>().Disable();
            else
                environmentToUnload.SetActive(false);
        }

        if (environmentToLoad)
        {
            if (environmentToLoad.GetComponent<EnvironmentStart01>())
                environmentToLoad.GetComponent<EnvironmentStart01>().Enable();
            else
                environmentToLoad.SetActive(true);
        }



        // PARTICLES
        if (player != null)
            if (changePlayerParticlesSet)
                player.SetParticleSets(newParticleSetIndex);
    }








    // EDITOR ONLY
    private void OnDrawGizmos()                                                                                                                                                           // ON DRAW GIZMOS
    {
        // Display all editor infos
        if (isActiveAndEnabled && enabled)
        {
            if (!editorDisplayStuff.activeInHierarchy)
                editorDisplayStuff.SetActive(true);


            if (environmentToLoad != null || environmentToUnload != null && volumeColorIfActivatesChunks != null && Gizmos.color != volumeColorIfActivatesChunks)
                Gizmos.color = volumeColorIfActivatesChunks;
            else if (volumeColor != null && Gizmos.color != volumeColor)
                Gizmos.color = volumeColor;


            // Display the volume in editor
            if (wiredVolume)
                Gizmos.DrawWireCube(transform.position, transform.localScale);
            else
                Gizmos.DrawCube(transform.position, transform.localScale);






            // DISPLAYS ENVIRONMENTS TO LOAD / UNLOAD
            // Link to environment to unload
            if (Gizmos.color != Color.red)
                Gizmos.color = Color.red;
            if (environmentToUnload)
            {
                Gizmos.DrawLine(transform.position, environmentToUnload.transform.position);
                if (environmentToUnloadDisplay != null)
                {
                    if (!environmentToUnloadDisplay.gameObject.activeInHierarchy)
                        environmentToUnloadDisplay.gameObject.SetActive(true);
                    environmentToUnloadDisplay.text = environmentToUnload.gameObject.name;
                }
            }
            else if (environmentToUnloadDisplay && environmentToUnloadDisplay.gameObject.activeInHierarchy)
                environmentToUnloadDisplay.gameObject.SetActive(false);

            // Link to environment to load
            if (Gizmos.color != Color.green)
                Gizmos.color = Color.green;
            if (environmentToLoad)
            {
                Gizmos.DrawLine(transform.position, environmentToLoad.transform.position);
                if (environmentToLoadDisplay != null)
                {
                    if (!environmentToLoadDisplay.gameObject.activeInHierarchy)
                        environmentToLoadDisplay.gameObject.SetActive(true);
                    environmentToLoadDisplay.text = environmentToLoad.gameObject.name;
                }
            }
            else if (environmentToLoadDisplay && environmentToLoadDisplay.gameObject.activeInHierarchy)
                environmentToLoadDisplay.gameObject.SetActive(false);

            // If nothing to display remove the shadow
            if (backgroundShadow)
            {
                if (environmentToUnload == null && environmentToLoad == null)
                {
                    if (backgroundShadow.activeInHierarchy)
                        backgroundShadow.SetActive(false);
                }
                else if (!backgroundShadow.activeInHierarchy)
                    backgroundShadow.SetActive(true);
            }



            if (player == null)
                player = FindObjectOfType<Player>();

            // Display particle set index
            if (particleIndexDisplay)
                particleIndexDisplay.text = newParticleSetIndex.ToString();
            if (particleSetIconDisplay)
                particleSetIconDisplay.sprite = player.particlesSets[newParticleSetIndex].icon;
        }
        else if (editorDisplayStuff.activeInHierarchy)
            editorDisplayStuff.SetActive(false);
    }
}
