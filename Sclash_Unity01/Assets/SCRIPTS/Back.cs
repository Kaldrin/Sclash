using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back : MonoBehaviour
{
    public void SendBackToAllActiveMenuBrowsers()
    {
        MenuBrowser[] activeMenuBrowsers = GameObject.FindObjectsOfType<MenuBrowser>();
        bool hasBacked = false;

        for (int i = 0; i < activeMenuBrowsers.Length; i++)
        {
            if (activeMenuBrowsers[i].gameObject.activeInHierarchy && activeMenuBrowsers[i].isActiveAndEnabled && !hasBacked)
            {
                hasBacked = true;
                activeMenuBrowsers[i].Back();
            }
        }
    }
}
