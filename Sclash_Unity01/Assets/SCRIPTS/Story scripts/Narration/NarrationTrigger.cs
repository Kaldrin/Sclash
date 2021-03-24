using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using TMPro;





// HEADER
// For Sclash
// Campaign only

/// <summary>
/// When the player enters it, sends a narration event to the NarrationEngine to trigger the display and play of the sentences
/// </summary>

public class NarrationTrigger : MonoBehaviour
{
    [Header("SETTINGS YOU SHOULD EDIT")]
    [SerializeField] NarrationEngine.NarrationEventData narrationEventData = new NarrationEngine.NarrationEventData();
    bool triggered = false;

    [Header("DATA")]
    [SerializeField] TagsReferences tagsReferences = null;

    [Header("EDITOR")]
    [SerializeField] GameObject editorDisplayStuff = null;
    [SerializeField] bool wiredVolume = true;
    [SerializeField] TextMeshPro firstKeyDisplayText = null;
    [SerializeField] TextMeshPro numberOfSentencesDisplayText = null;







    private void Start()                                                                                                                                                            // START
    {
        if (editorDisplayStuff)
            editorDisplayStuff.SetActive(false);
    }

    // DETECTS PLAYER
    private void OnTriggerEnter2D(Collider2D collision)                                                                                                                           // ON TRIGGER ENTER 2D
    {
        if (!triggered)
            if (collision.gameObject.CompareTag(tagsReferences.playerTag) || collision.transform.parent.gameObject.CompareTag(tagsReferences.playerTag))
                TriggerNarrationEvent();
    }




    void TriggerNarrationEvent()                                                                                                                                                    // TRIGGER NARRATION EVENT
    {
        triggered = true;
        if (NarrationEngine.Instance)
            NarrationEngine.Instance.NarrationEvent(narrationEventData);
    }








    // EDITOR ONLY
    private void OnDrawGizmos()                                                                                                                                                           // ON DRAW GIZMOS
    {
        Gizmos.color = Color.magenta;


        // Display the volume in editor
        if (wiredVolume)
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        else
            Gizmos.DrawCube(transform.position, transform.localScale);


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






#if UNITY_EDITOR
        HandleUtility.Repaint();
#endif
    }
}
