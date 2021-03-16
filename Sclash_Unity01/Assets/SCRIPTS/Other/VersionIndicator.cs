using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;




// HEADER
// Reusable script
// For Sclash
// OPTIMIZED

// REQUIREMENTS
// Requires Text Mesh Pro package

/// <summary>
/// Script that allows to indicate the current version of the game on a text component. Entirely foolproof, should work
/// </summary>

// VERSION
// Originally made for Unity 2019.14
public class VersionIndicator : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshProUGUIToDiplay = null;
    [SerializeField] TextMeshPro textMeshproToDisplay = null;
    [SerializeField] Text textToDisplay = null;
    [SerializeField] TextMesh textMeshToDisplay = null;





    #region FUNCTIONS
    private void Awake()                                                                            // AWAKE
    {
        GetComponents();
    }


    void Start()                                                                                        // START
    {
        DisplayVersion();
    }


    private void OnEnable()                                                                                         // ON ENABLE
    {
        DisplayVersion();
    }




    public void DisplayVersion()                                                                                       // DISPLAY VERSION
    {
        // If components are missing, try to find them
        GetComponents();


        // Gets the version
        string version = Application.version;

        // Displays the version of the game in the text component(s)
        if (textMeshProUGUIToDiplay != null)
            textMeshProUGUIToDiplay.text = version;
        if (textMeshproToDisplay != null)
            textMeshproToDisplay.text = version;
        if (textToDisplay != null)
            textToDisplay.text = version;
        if (textMeshToDisplay != null)
            textMeshToDisplay.text = version;
    }







    // SECONDARY
    // Check if it's possible to find the components automatically if they are missing
    void GetComponents()
    {
        if (textMeshProUGUIToDiplay == null && GetComponent<TextMeshProUGUI>())
            textMeshProUGUIToDiplay = GetComponent<TextMeshProUGUI>();
        if (textMeshproToDisplay == null && GetComponent<TextMeshPro>())
            textMeshproToDisplay = GetComponent<TextMeshPro>();
        if (textToDisplay == null && GetComponent<Text>())
            textToDisplay = GetComponent<Text>();
        if (textMeshToDisplay == null && GetComponent<TextMesh>())
            textMeshToDisplay = GetComponent<TextMesh>();
    }







    // EDITOR
    private void OnDrawGizmosSelected()                                                                                     // ON DRAW GIZMOS SELECTED
    {
        // Ergonomy stuff
        // Tries to find the components as soon as it's in the scene before the user has ton drag'n drop them in the Serialized Fields
        GetComponents();
    }
    #endregion
}
