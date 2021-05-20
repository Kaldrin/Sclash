using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using TMPro;




// For Sclash story mode
// Bastien BERNAND

// REQUIREMENTS
// TutorialManager script (Single instance)
// TextMeshPro package

/// <summary>
/// This script manages the triggering of tutorial messages using the TutorialManager
/// </summary>

// UNITY 2020.3
public class TutorialTrigger : MonoBehaviour
{

    [SerializeField] GameObject tutorialObject = null;
    [SerializeField] float tutoMessageDuration = 10;
    // PARAMETERS
    bool triggered = false;

    [Header("DATA")]
    [Tooltip("Yeah we need the list of tags of the game to know which one is the player, who knows who you name your tags and if you want to rename them at least it's centralized")]
    [SerializeField] TagsReferences tagsReferences = null;

    [Header("EDITOR")]
    [SerializeField] GameObject infosDisplay = null;
    [SerializeField] TextMeshPro tutorialNameDisplay = null;
    string parentName = "TutorialTriggers";




    // DETECTS PLAYER
    private void OnTriggerEnter2D(Collider2D collision)                                                                                                                // ON TRIGGER ENTER 2D
    {
        if (!triggered)
            if (collision.gameObject.CompareTag(tagsReferences.playerTag) || collision.transform.parent.gameObject.CompareTag(tagsReferences.playerTag))
                TriggerTutorial();
    }




    void TriggerTutorial()                                                                                                                                         // TRIGGER NARRATION EVENT
    {
        triggered = true;
        if (TutorialManager.Instance && tutorialObject)
            TutorialManager.Instance.TriggerTutorial(tutorialObject, tutoMessageDuration);
    }











    // EDITOR ONLY
    private void OnDrawGizmosSelected()                                                                                                                             // ON DRAW GOZMOS SELECTED
    {
        float distance = 5;

        RaycastHit2D raycastHit2D = Physics2D.Raycast((Vector3)transform.position + (Vector3.up * distance), Vector3.down, distance * 2, LayerMask.GetMask("Level"));
        if (raycastHit2D.collider != null && raycastHit2D.collider != GetComponent<Collider>())
        {
            Debug.DrawRay((Vector3)transform.position + (Vector3.up * distance), Vector3.down * distance * 2, Color.red);
            transform.position = new Vector3(transform.position.x, raycastHit2D.point.y + transform.localScale.y / 2, transform.position.z);
        }
        else
            Debug.DrawRay((Vector3)transform.position + (Vector3.up * distance), Vector3.down * distance * 2, Color.white);
    }


    private void OnDrawGizmos()
    {
        // Display the volume in editor
        Gizmos.color = Color.yellow;
        if (triggered)
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        else
            Gizmos.DrawCube(transform.position, transform.localScale);



        // Display tuto name
        if (tutorialNameDisplay && tutorialObject && tutorialObject.name != "")
        {
            if (infosDisplay && !infosDisplay.activeInHierarchy)
                infosDisplay.SetActive(true);
            tutorialNameDisplay.text = tutorialObject.name;
        }
        else if (infosDisplay && infosDisplay.activeInHierarchy)
            infosDisplay.SetActive(false);





        
        #if UNITY_EDITOR
            // Set parent
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
                if ((!transform.parent || transform.parent.gameObject.name != parentName) && GameObject.Find(parentName))
                    transform.parent = GameObject.Find(parentName).transform;
            // For repaint in editor
            HandleUtility.Repaint();
        #endif
    }
}
