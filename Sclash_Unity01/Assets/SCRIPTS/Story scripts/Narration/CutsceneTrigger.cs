﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using TMPro;





// For Sclash
// Campaign only

// REQUIREMENTS
// Goes with the NarrationEngine script (Single instance)
// TextMeshPro package

/// <summary>
/// When the player enters it, sends an event to the NarrationEngine to trigger the referenced cutscene
/// </summary>

// Unity 2019.4.14
public class CutsceneTrigger : MonoBehaviour
{
    [Header("SETTINGS YOU SHOULD EDIT")]
    [SerializeField] GameObject cutscenePrefab = null;
    bool triggered = false;

    [Header("DATA")]
    [Tooltip("Yeah we need the list of tags of the game to know which one is the player, who knows who you name your tags and if you want to rename them at least it's centralized")]
    [SerializeField] TagsReferences tagsReferences = null;

    [Header("EDITOR")]
    bool wiredVolume = false;
    [SerializeField] GameObject infosDisplay = null;
    [SerializeField] TextMeshPro cutsceneNameText = null;
    [SerializeField] GameObject shadow = null;
    [SerializeField] GameObject warning = null;








    // DETECTS PLAYER
    private void OnTriggerEnter2D(Collider2D collision)                                                                                                                           // ON TRIGGER ENTER 2D
    {
        if (NarrationEngine.Instance)
            if (!triggered)
                if (collision.gameObject.CompareTag(tagsReferences.playerTag) || collision.transform.parent.gameObject.CompareTag(tagsReferences.playerTag))
                    TriggerCutsceneEvent();
    }




    void TriggerCutsceneEvent()                                                                                                                                                    // TRIGGER NARRATION EVENT
    {
        wiredVolume = true;
        triggered = true;
        if (NarrationEngine.Instance)
            NarrationEngine.Instance.TriggerCutScene(cutscenePrefab);
    }











    // EDITOR ONLY
    private void OnDrawGizmosSelected()
    {
        float distance = 5;

        RaycastHit2D raycastHit2D = Physics2D.Raycast((Vector2)transform.position + (Vector2.up * distance), Vector2.down, distance * 2, LayerMask.GetMask("Level"));
        if (raycastHit2D.collider != null && raycastHit2D.collider != GetComponent<Collider>())
        {
            Debug.DrawRay((Vector2)transform.position + (Vector2.up * distance), Vector2.down * distance * 2, Color.red);
            transform.position = new Vector3(transform.position.x, raycastHit2D.point.y + transform.localScale.y / 2, transform.position.z);
        }
        else
            Debug.DrawRay((Vector2)transform.position + (Vector2.up * distance), Vector2.down * distance * 2, Color.white);
    }


    private void OnDrawGizmos()                                                                                                                                                           // ON DRAW GIZMOS
    {
        if (FindObjectOfType<NarrationEngine>())
        {
            // Remove warning
            if (warning && warning.activeInHierarchy)
                warning.SetActive(false);

            if (infosDisplay && !infosDisplay.activeInHierarchy)
                infosDisplay.SetActive(true);
        }
        // WARNING IF NO NARRATION CANVAS
        else
        {
            if (warning && !warning.activeInHierarchy)
                warning.SetActive(true);

            if (infosDisplay && infosDisplay.activeInHierarchy)
                infosDisplay.SetActive(false);
        }

        Gizmos.color = Color.blue;


        // Display the volume in editor
        if (wiredVolume)
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        else
            Gizmos.DrawCube(transform.position, transform.localScale);


        if (infosDisplay && infosDisplay.activeInHierarchy)
        {
            // Display name
            if (cutscenePrefab)
            {
                if (cutsceneNameText)
                {
                    if (!cutsceneNameText.gameObject.activeInHierarchy)
                        cutsceneNameText.gameObject.SetActive(true);

                    cutsceneNameText.text = cutscenePrefab.name;
                }
                if (shadow && !shadow.gameObject.activeInHierarchy)
                    shadow.gameObject.SetActive(true);
            }
            else
            {
                if (cutsceneNameText && cutsceneNameText.gameObject.activeInHierarchy)
                    cutsceneNameText.gameObject.SetActive(false);
                if (shadow && shadow.gameObject.activeInHierarchy)
                    shadow.gameObject.SetActive(false);
            }
        }


        // Forces repaint more often
#if UNITY_EDITOR
        HandleUtility.Repaint();
#endif
    }
}
