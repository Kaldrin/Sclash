using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This script is just to enable / disable the right menus on start because I'm too lazy to always disable the one I'm working on and enable the main menu back
// To put on the MENUS game object
// OPTIMIZED
public class LazyMenuSetUp : MonoBehaviour
{
    [SerializeField] GameObject menuToActivateOnStart = null;








    // Start is called before the first frame update
    void Awake()
    {
        if (menuToActivateOnStart == null)
            menuToActivateOnStart = transform.GetChild(0).gameObject;


        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject != null)
            {
                GameObject menuScreen = transform.GetChild(i).gameObject;


                if (menuScreen.activeInHierarchy != false)
                    menuScreen.SetActive(false);
            }
            else
                Debug.Log("Problem with disabling the menu index " + " i");
        }


        if (menuToActivateOnStart != null)
            if (menuToActivateOnStart.activeInHierarchy != true)
                menuToActivateOnStart.SetActive(true);
    }









    // EDITOR
    private void OnDrawGizmosSelected()
    {
        if (menuToActivateOnStart == null)
            menuToActivateOnStart = transform.GetChild(0).gameObject;
    }
}
