﻿using System.Collections;
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
    [SerializeField] bool wiredVolume = true;
    [SerializeField] TextMeshPro particleIndexDisplay = null;











    private void Start()                                                                                                                                                // START
    {
        // Disable editor only stuff
        if (editorDisplayStuff && editorDisplayStuff.activeInHierarchy)
            editorDisplayStuff.SetActive(false);

    }

    private void Update()                                                                                                                                              // UPDATE
    {
        // Transition cooldown timer
        if (enabled && isActiveAndEnabled)
            if (transitoned && Time.time - cooldown > transitionStartTime)
                transitoned = false;
    }



    // DETECTS PLAYER
    private void OnTriggerEnter2D(Collider2D collision)                                                         // ON TRIGGER ENTER 2D
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
    void TriggerTransition(Player player)
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
    private void OnDrawGizmos()
    {
        if (volumeColor != null && Gizmos.color != volumeColor)
            Gizmos.color = volumeColor;


        // Display the volume in editor
        if (wiredVolume)
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        else
            Gizmos.DrawCube(transform.position, transform.localScale);


        // Link to environment to unload
        Gizmos.color = Color.red;
        if (environmentToUnload)
            Gizmos.DrawLine(transform.position, environmentToUnload.transform.position);

        // Link to environment to load
        Gizmos.color = Color.green;
        if (environmentToLoad)
            Gizmos.DrawLine(transform.position, environmentToLoad.transform.position);


        // Display particle set index
        if (particleIndexDisplay)
            particleIndexDisplay.text = newParticleSetIndex.ToString();
    }
}
