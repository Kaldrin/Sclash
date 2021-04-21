using System.Collections;
using System.Collections.Generic;
using UnityEngine;




// Reusable asset
// Bastien BERNAND

/// <summary>
/// This script provides functions to open links to web pages from the game
/// </summary>

// UNITY 2020.3
public class OpenLinkFromBuild : MonoBehaviour
{
    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }
}
