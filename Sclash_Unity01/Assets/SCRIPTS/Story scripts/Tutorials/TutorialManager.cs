using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;



// For Sclash
// Bastien BERNAND

// REQUIREMENTS
// PitchModulator script

/// <summary>
/// This script manages the display of tutorial messages in the story mode
/// </summary>

// UNITY 2020.3
public class TutorialManager : MonoBehaviour
{
    // SINGLETON
    public static TutorialManager Instance;

    [SerializeField] Animation tutorialAnimationComponent = null;
    [SerializeField] RectTransform tutorialParent = null;
    [SerializeField] PitchModulator notificationSFX = null;
    GameObject currentTutorial = null;
    bool tutorialInProgress = false;
    float tutorialDisappearDuration = 1f;







    private void Awake()                                                                                                                                        // AWAKE
    {   
        Instance = this;

        // Clear children
        if (tutorialParent && tutorialParent.childCount > 0)
            for (int i = 0; i < tutorialParent.childCount; i++)
                if (tutorialParent.GetChild(i))
                    Destroy(tutorialParent.GetChild(i).gameObject);
    }



    public void TriggerTutorial(GameObject tutorialObject, float duration)                                                                                       // TRIGGER TUTORIAL
    {
        if (tutorialInProgress)
        {
            // NEXT
            CancelInvoke("InvokeEndTutorial");
            EndTutorial(tutorialObject, duration);
        }
        else
        {
            // SPAWN
            currentTutorial = Instantiate(tutorialObject, tutorialParent);
            // ANIM
            if (tutorialAnimationComponent)
                tutorialAnimationComponent.Play("FadeIn", PlayMode.StopAll);
            // AUDIO
            if (notificationSFX)
                notificationSFX.Play();
            tutorialInProgress = true;
            Invoke("InvokeEndTutorial", duration);
        }
    }

    void InvokeEndTutorial()                                                                                                                        // INVOKE END TUTORIAL
    {
        EndTutorial();
    }

    public void EndTutorial(GameObject nextTutorial = null, float nextTutorialDuration = 2)                                                                              // END TUTORIAL   
    {
        Debug.Log("End");
        // ANIM
        if (tutorialAnimationComponent)
            tutorialAnimationComponent.Play("FadeOut", PlayMode.StopAll);
        // DESTROY
        if (currentTutorial)
            Destroy(currentTutorial, tutorialDisappearDuration);

        tutorialInProgress = false;

        // NEXT
        if (nextTutorial)
            StartCoroutine(WaitAndStartNextTutorial(nextTutorial, nextTutorialDuration));
    }

    IEnumerator WaitAndStartNextTutorial(GameObject nextTutorial, float nextTutoDuration)
    {
        yield return new WaitForSecondsRealtime(tutorialDisappearDuration);
        TriggerTutorial(nextTutorial, nextTutoDuration);
    }





    void GetMissingComponents()                                                                                                                                 // GET MISSING COMPONENTS   
    {
        if (tutorialAnimationComponent == null && GetComponent<Animation>())
            tutorialAnimationComponent = GetComponent<Animation>();
    }





    // EDITOR
    private void OnDrawGizmosSelected()                                                                                                                     // ON DRAW GIZMOS SELECTED
    {
        GetMissingComponents();


    #if UNITY_EDITOR
        HandleUtility.Repaint();
    #endif
    }
}
