using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Reusable asset

/// <summary>
/// This script provides public methods to help use the Animation component from a button or sos
/// </summary>

// UNITY 2020.3
public class PlayAnimations : MonoBehaviour
{
    [SerializeField] Animation animationComponentToUse = null;






    public void PlayAnim(string animationToPlay)
    {
        if (animationToPlay != null && animationToPlay != "")
            animationComponentToUse.Play(animationToPlay, PlayMode.StopAll);
        else
            animationComponentToUse.Play();
    }








    /// <summary>
    /// Try and get the required components from the game object if they're missing
    /// </summary>
    void GetComponents()
    {
        if (animationComponentToUse == null)
            if (GetComponent<Animation>())
                animationComponentToUse = GetComponent<Animation>();
    }





    // EDITOR
    private void OnDrawGizmosSelected()                                                                                                             // ON DRAW GIZMOS
    {
        GetComponents();
    }
}
