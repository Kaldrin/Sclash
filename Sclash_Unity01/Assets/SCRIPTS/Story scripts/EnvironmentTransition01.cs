using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using TMPro;






// HEADER
// For Sclash
// OPTIMIZED I THINK
// CAMPAIGN ONLY

// REQUIREMENTS
// Player script
// TextMeshPro package
// EnvironmentStart01 script
// WalkSoundsLists scriptable object

/// <summary>
/// Script used on environment transition objects for the campaign mode to load the next parts of the level and change the particles used by the player, when the player passes on it
/// </summary>

// VERSION
// Made for Unity 2019.4.14
public class EnvironmentTransition01 : MonoBehaviour
{
    [Header("DATA")]
    [SerializeField] TagsReferences tagsReferences = null;



    [Header("SETTINGS YOU SHOULD EDIT")]
    [Tooltip("The environment chunk game object to load")]
    [SerializeField] GameObject chunkToUnload = null;
    [Tooltip("The environment chunk game object to unload starting from this point")]
    [SerializeField] GameObject chunkToLoad = null;


    [SerializeField] bool changePlayerParticlesSet = false;
    [Range(0, 10)]
    [SerializeField] int newParticleSetIndex = 0;
    [SerializeField] bool changePlayerWalkSFXSet = false;
    [Range(0, 3)]
    [SerializeField] int newWalkSFXSetIndex = 0;
    [Tooltip("Duration before this transition objects can be used again (To prevent it triggered 50 times because the player has multiple colliders)")]
    [SerializeField] float cooldown = 10f;


    float transitionStartTime = 0f;
    bool transitoned = false;



    [Header("EDITOR")]
    [SerializeField] GameObject editorDisplayStuff = null;
    [SerializeField] bool wiredVolume = true;
    [SerializeField] GameObject FXIndexDisplayParent = null;
    [SerializeField] TextMeshPro particleIndexDisplay = null;
    [SerializeField] SpriteRenderer particleSetIconDisplay = null;
    [SerializeField] GameObject walkSFXIndexDisplayParent = null;
    [SerializeField] TextMeshPro walkSFXSetIndexDisplay = null;
    [SerializeField] SpriteRenderer walkSFXSetIconDisplay = null;
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


    private void OnEnable()                                                                                                                                                     // ON ENABLE
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
        if (chunkToUnload)
        {
            if (chunkToUnload.GetComponent<EnvironmentStart01>())
                chunkToUnload.GetComponent<EnvironmentStart01>().Disable();
            else
                chunkToUnload.SetActive(false);
        }
        if (chunkToLoad)
        {
            if (chunkToLoad.GetComponent<EnvironmentStart01>())
                chunkToLoad.GetComponent<EnvironmentStart01>().Enable();
            else
                chunkToLoad.SetActive(true);
        }



        // PARTICLES && WALK SFX
        if (player != null)
        {
            if (changePlayerParticlesSet)
                player.SetParticleSets(newParticleSetIndex);
            if (changePlayerWalkSFXSet)
                player.SetWalkSFXSet(newWalkSFXSetIndex);
        }

    }











    // EDITOR ONLY
    private void OnDrawGizmos()                                                                                                                                                           // ON DRAW GIZMOS
    {
        if (chunkToLoad != null || chunkToUnload != null && Color.red != null)
            Gizmos.color = Color.red;
        else if (!changePlayerParticlesSet && changePlayerWalkSFXSet && Gizmos.color != Color.blue)
            Gizmos.color = Color.blue;
        else if (Gizmos.color != Color.green)
            Gizmos.color = Color.green;


        // Display the volume in editor
        if (wiredVolume)
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        else
            Gizmos.DrawCube(transform.position, transform.localScale);






        // DISPLAYS ENVIRONMENTS TO LOAD / UNLOAD
        // Link to environment to unload
        if (Gizmos.color != Color.red)
            Gizmos.color = Color.red;
        if (chunkToUnload)
        {
            Gizmos.DrawLine(transform.position, chunkToUnload.transform.position);
            if (environmentToUnloadDisplay != null)
            {
                if (!environmentToUnloadDisplay.gameObject.activeInHierarchy)
                    environmentToUnloadDisplay.gameObject.SetActive(true);
                environmentToUnloadDisplay.text = chunkToUnload.gameObject.name;
            }
        }
        else if (environmentToUnloadDisplay && environmentToUnloadDisplay.gameObject.activeInHierarchy)
            environmentToUnloadDisplay.gameObject.SetActive(false);

        // Link to environment to load
        if (Gizmos.color != Color.green)
            Gizmos.color = Color.green;
        if (chunkToLoad)
        {
            Gizmos.DrawLine(transform.position, chunkToLoad.transform.position);
            if (environmentToLoadDisplay != null)
            {
                if (!environmentToLoadDisplay.gameObject.activeInHierarchy)
                    environmentToLoadDisplay.gameObject.SetActive(true);
                environmentToLoadDisplay.text = chunkToLoad.gameObject.name;
            }
        }
        else if (environmentToLoadDisplay && environmentToLoadDisplay.gameObject.activeInHierarchy)
            environmentToLoadDisplay.gameObject.SetActive(false);

        // If nothing to display remove the shadow
        if (backgroundShadow)
        {
            if (chunkToUnload == null && chunkToLoad == null && !changePlayerParticlesSet && !changePlayerWalkSFXSet)
            {
                if (backgroundShadow.activeInHierarchy)
                    backgroundShadow.SetActive(false);
            }
            else if (!backgroundShadow.activeInHierarchy)
                backgroundShadow.SetActive(true);
        }



        // Get player to get data in it
        if (player == null)
            player = FindObjectOfType<Player>();



        // Display particle set index
        if (changePlayerParticlesSet)
        {
            if (FXIndexDisplayParent && !FXIndexDisplayParent.activeInHierarchy)
                FXIndexDisplayParent.SetActive(true);
            if (particleIndexDisplay)
                particleIndexDisplay.text = newParticleSetIndex.ToString();
            if (particleSetIconDisplay && player && player.particlesSets != null && player.particlesSets.Count > newParticleSetIndex && newParticleSetIndex >= 0)
                particleSetIconDisplay.sprite = player.particlesSets[newParticleSetIndex].icon;
            else
                particleSetIconDisplay.sprite = null;
        }
        else
            if (FXIndexDisplayParent && FXIndexDisplayParent.activeInHierarchy)
                FXIndexDisplayParent.SetActive(false);


        // Display walk SFX set index
        if (changePlayerWalkSFXSet)
        {
            if (walkSFXIndexDisplayParent && !walkSFXIndexDisplayParent.activeInHierarchy)
                walkSFXIndexDisplayParent.SetActive(true);
            if (walkSFXSetIndexDisplay)
                walkSFXSetIndexDisplay.text = newWalkSFXSetIndex.ToString();
            if (walkSFXSetIconDisplay && player && player.walkSoundsList != null && player.walkSoundsList.audioClipsLists != null && player.walkSoundsList.audioClipsLists.Count > newWalkSFXSetIndex && newWalkSFXSetIndex >= 0)
                walkSFXSetIconDisplay.sprite = player.walkSoundsList.audioClipsLists[newWalkSFXSetIndex].icon;
            else
                walkSFXSetIconDisplay.sprite = null;
        }
        else
            if (walkSFXIndexDisplayParent && walkSFXIndexDisplayParent.activeInHierarchy)
                walkSFXIndexDisplayParent.SetActive(false);

        #if UNITY_EDITOR
            HandleUtility.Repaint();
        #endif
    }
}
