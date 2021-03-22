using System.Collections;
using System.Collections.Generic;
using UnityEngine;






// HEADER
// Reusable script

// REQUIREMENTS
// None

/// <summary>
/// This script is to trigger an animator / animation after a certain time in the scene
/// </summary>

// VERSION
// Created for Unity 2019.1.1f1
public class TriggerAnimatorAfterDuration : MonoBehaviour
{
    [SerializeField] Animator animatorToTrigger = null;
    [SerializeField] Animation animationToTrigger = null;
    [SerializeField] string animationName = "LanternWind01";
    [SerializeField] Vector2 randomDelayBetween = new Vector2(0.1f, 1f);













    void Start()                                                                                                            // START
    {
        if (animatorToTrigger != null)
            animatorToTrigger.enabled = false;
        

        Invoke("TriggerAnimation", Random.Range(randomDelayBetween.x, randomDelayBetween.y));
    }




    void TriggerAnimation()                                                                                                  // TRIGGER ANIMATION
    {
        GetComponents();

        if (animationToTrigger != null && animationName != null && animationName != "" && animationToTrigger.GetClip(animationName) != null)
            animationToTrigger.Play(animationName, PlayMode.StopAll);
        if (animatorToTrigger != null)
            animatorToTrigger.enabled = true;
    }








    // See if it's possible to automatically get missing components
    void GetComponents()                                                                                                            // GET COMPONENTS
    {
        if (animationToTrigger == null && GetComponent<Animation>())
            animationToTrigger = GetComponent<Animation>();
        if (animatorToTrigger == null && GetComponent<Animator>())
            animatorToTrigger = GetComponent<Animator>();
    }






    // Automatically gets the components references before the user has to assign them
    private void OnDrawGizmosSelected()                                                                             // ON DRAW GIZMOS SELECTED
    {
        GetComponents();
    }
}
