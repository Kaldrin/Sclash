using System.Collections;
using System.Collections.Generic;
using UnityEngine;




// Bastien BERNAND
// For Sclash

/// <summary>
/// Script for the doors and overall points in the story mode than can close / open to let the player pass or block them before they complete a certain thing
/// </summary>

// UNITY 2020.3
public class CampaignDoor : MonoBehaviour
{
    [SerializeField] Animation animationComponent = null;
    [SerializeField] ActivationMode activationMode = ActivationMode.other;
    enum ActivationMode
    {
        dummies,
        other
    }
    [SerializeField] int dummiesToActivate = 0;
    int dummiesDestroyed = 0;
    bool open = false;

    


    public void Close()
    {
        if (open)
        {
            if (animationComponent)
                animationComponent.Play("Close", PlayMode.StopAll);

            open = false;
        }
    }
    
    public void Open()
    {
        if (!open)
        {
            if (animationComponent)
                animationComponent.Play("Open", PlayMode.StopAll);
            open = true;
        }
    }

    public void DummyDestroyed()
    {
        if (activationMode == ActivationMode.dummies)
        {
            dummiesDestroyed++;
            if (dummiesDestroyed >= dummiesToActivate)
                Open();
        }
        else
            Debug.Log("This door is not set to be activated by dummies");
    }
}
