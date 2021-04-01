using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using TMPro;





// HEADER
// For Sclash
// Campaign only

// REQUIREMENTS
// Goes with the NarrationEngine script (Single instance)
// TextMeshPro package

/// <summary>
/// When the player enters it, sends a narration event to the NarrationEngine to trigger the display and play of the sentences
/// </summary>

// VERSION
// Unity 2019.4.14
public class NarrationTrigger : MonoBehaviour
{
    [Header("SETTINGS YOU SHOULD EDIT")]
    [SerializeField] NarrationEngine.NarrationEventData narrationEventData = new NarrationEngine.NarrationEventData();
    bool triggered = false;

    [Header("DATA")]
    [Tooltip("Yeah we need the list of tags of the game to know which one is the player, who knows who you name your tags and if you want to rename them at least it's centralized")]
    [SerializeField] TagsReferences tagsReferences = null;

    [Header("EDITOR")]
    bool wiredVolume = false;
    [SerializeField] GameObject infosDisplay = null;
    [SerializeField] TextMeshPro firstKeyDisplayText = null;
    [SerializeField] TextMeshPro numberOfSentencesDisplayText = null;
    [SerializeField] GameObject warning = null;








    // DETECTS PLAYER
    private void OnTriggerEnter2D(Collider2D collision)                                                                                                                           // ON TRIGGER ENTER 2D
    {
        if (!triggered)
            if (collision.gameObject.CompareTag(tagsReferences.playerTag) || collision.transform.parent.gameObject.CompareTag(tagsReferences.playerTag))
                TriggerNarrationEvent();
    }




    void TriggerNarrationEvent()                                                                                                                                                    // TRIGGER NARRATION EVENT
    {
        wiredVolume = true;
        triggered = true;
        if (NarrationEngine.Instance)
            NarrationEngine.Instance.NarrationEvent(narrationEventData);
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
        // Display warning if no NarrationCanvas
        else
        {
            if (warning && !warning.activeInHierarchy)
                warning.SetActive(true);

            if (infosDisplay && infosDisplay.activeInHierarchy)
                infosDisplay.SetActive(false);
        }




        Gizmos.color = Color.magenta;


        // Display the volume in editor
        if (wiredVolume)
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        else
            Gizmos.DrawCube(transform.position, transform.localScale);

        if (infosDisplay && infosDisplay.activeInHierarchy)
        {
            // Display key
            if (narrationEventData.sentences != null && narrationEventData.sentences.Count > 0 && narrationEventData.sentences[0].textKey != "")
                if (firstKeyDisplayText)
                {
                    if (!firstKeyDisplayText.gameObject.activeInHierarchy)
                        firstKeyDisplayText.gameObject.SetActive(true);

                    firstKeyDisplayText.text = narrationEventData.sentences[0].textKey;
                }
                else if (firstKeyDisplayText.gameObject.activeInHierarchy)
                    firstKeyDisplayText.gameObject.SetActive(false);


            // Display number of sentences
            if (narrationEventData.sentences != null && narrationEventData.sentences.Count > 0)
                if (numberOfSentencesDisplayText)
                {
                    if (!numberOfSentencesDisplayText.gameObject.activeInHierarchy)
                        numberOfSentencesDisplayText.gameObject.SetActive(true);

                    numberOfSentencesDisplayText.text = narrationEventData.sentences.Count.ToString();
                }
                else if (numberOfSentencesDisplayText.gameObject.activeInHierarchy)
                    numberOfSentencesDisplayText.gameObject.SetActive(false);
        }


#if UNITY_EDITOR
        HandleUtility.Repaint();
#endif
    }
}
