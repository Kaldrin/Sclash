using System.Collections;
using System.Collections.Generic;
using UnityEngine;




// For Sclash
// OPTIMIZED

/// <summary>
/// This script is just to enable / disable the right menus on start because I'm too lazy to always disable the one I'm working on and enable the main menu back. To put on the MENUS game object
/// </summary>
 
// UNITY 2019.1.14
public class LazyMenuSetUp : MonoBehaviour
{
    [Tooltip("The menu that will be active on start")]
    [SerializeField] GameObject menuToActivateOnStart = null;
    [SerializeField] GameObject blurPanel = null;







    void Awake()                                                                                                                                                                // AWAKE
    {
        GetMissingComponents();

        // Disable all menus
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).gameObject != null)
            {
                GameObject menuScreen = transform.GetChild(i).gameObject;


                if (menuScreen.activeInHierarchy != false)
                    menuScreen.SetActive(false);
            }
            else
                Debug.Log("Problem with disabling the menu index " + " i");

        // Activate required menu
        if (menuToActivateOnStart != null)
            if (menuToActivateOnStart.activeInHierarchy != true)
                menuToActivateOnStart.SetActive(true);


        if (blurPanel && !blurPanel.activeInHierarchy)
            blurPanel.SetActive(true);
    }






    void GetMissingComponents()
    {
        if (menuToActivateOnStart == null)
            menuToActivateOnStart = transform.GetChild(0).gameObject;
    }


    // EDITOR
    private void OnDrawGizmosSelected()                                                                                                                                     // ON DRAW GIZMOS SELECTED
    {
        GetMissingComponents();
    }
}
