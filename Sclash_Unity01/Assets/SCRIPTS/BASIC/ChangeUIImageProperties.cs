using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;




// HEADER
// Reusable script

// REQUIREMENTS
// Requires the TextMeshPro package

/// <summary>
/// Script that enables more function to change UI elements properties from inspector events like buttons
/// </summary>

// VERSION
// Originally made for Unity 2019.1.1f1
public class ChangeUIImageProperties : MonoBehaviour
{
    [SerializeField] Image ImageToModify = null;
    [SerializeField] TextMeshProUGUI textToModify = null;
    [SerializeField] List<Color> colorsToPick = new List<Color>();





    #region FUNCTIONS
    public void ChangeImageColor(int colorIndex)                                                                                  // CHANGE IMAGE COLOR
    {
        GetComponents();


        if (colorIndex <= colorsToPick.Count - 1)
        {
            if (ImageToModify != null)
                ImageToModify.color = colorsToPick[colorIndex];
            if (textToModify != null)
                textToModify.color = colorsToPick[colorIndex];
        }
    }









    // Checks if it's possible to get the missing components
    void GetComponents()                                                                                                                       // GET COMPONENTS
    {
        if (ImageToModify == null)
            if (GetComponent<Image>())
                ImageToModify = GetComponent<Image>();

        if (textToModify == null)
            if (GetComponent<TextMeshProUGUI>())
                textToModify = GetComponent<TextMeshProUGUI>();
    }




    // EDITOR
    // Ergonomy stuff, tries to automatically get the components before the user has to drag'n drop the references in the Serialized Fields
    private void OnDrawGizmosSelected()                                                                                                                     // ON DRAW GIZMOS SELECTED  
    {
        GetComponents();
    }
    #endregion
}
