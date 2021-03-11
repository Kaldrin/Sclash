using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;




// HEADER
// Script that manages the apparition of the right language text in a text box
// Originally made by Dylan BROUSSE, modified by BASTIEN BERNAND to fit Sclash needs
// Reusable script

// REQUIREMENTS
// Requires the LanguageManager script to work
// Requires TextMeshPro package to work
// Requires the scriptable object DiacriticsReplacementSettings (referenced in the LanguageManager)

/// <summary>
/// Is linked to a Text or TMPUGUI component and will set their text to the right language depending on the affected key, customizable to set OnStart / OnEnable or on function call
/// </summary>

// VERSION
// Originally made for Unity 2019.14
public class TextApparition : MonoBehaviour
{
    [Header("COMPONENTS")]
    [Tooltip("Reference to the target TMPUGUI component to set the text into")]
    [SerializeField] TextMeshProUGUI tmpToChange = null;
    [Tooltip("Reference to the target Text component to set the text into")]
    [SerializeField] Text textToChange = null;

    [Header("SETTINGS")]
    [Tooltip("Key of the text entry in the json file")]
    [SerializeField] public string textKey = "KEY";
    [Tooltip("Should remove accents and stuff? (Depending on font and languages you might want to do it) It uses the DiacriticsReplacementSettings scriptable object data")]
    [SerializeField] bool removeDiacritics = false;
    [Tooltip("Should update the text on start?")]
    [SerializeField] bool onStart = true;
    [Tooltip("Should update the text on enable?")]
    [SerializeField] bool onEnable = true;
    private string textToDisplay = "";

    [Tooltip("Text formatting mode")]
    [SerializeField] TextMode textMode = TextMode.normal;
    [System.Serializable] public enum TextMode
    {
        allCaps,
        allLowerCase,
        normal
    }
    





    #region FUNCTIONS
    private void Awake()                                                                                            // AWAKE
    {
        // Check if it's possible to get the missing components
        GetComponents();
    }


    void Start()                                                                                                            // START
    {
        if (onStart)
            TransfersTrad();
    }


    void OnEnable()                                                                                                         // ON ENABLE
    {
        if (onEnable)
            TransfersTrad();
    }







    // DISPLAY RIGHT TEXT
    /// <summary>
    /// Gets the corresponding language text using the Language Manager and the key and set it in the text components associated to this instance
    /// </summary>
    public void TransfersTrad()
    {
        // In case components are not referenced, fool proof
        GetComponents();


        // Get the right text
        if (LanguageManager.Instance != null)
            textToDisplay = LanguageManager.Instance.GetDialog(textKey);




        // TEXT FORMATTING
        if (textToDisplay != null && textToDisplay != "")
        {
            // Accents & stuff
            if (removeDiacritics)
                textToDisplay = RemoveDiacritics(textToDisplay);
            

            switch (textMode)
            {
                case TextMode.allCaps:
                    textToDisplay = textToDisplay.ToUpper();
                    break;
                case TextMode.allLowerCase:
                    textToDisplay = textToDisplay.ToLower();
                    break;
            }
        }




        // Displays the text
        if (tmpToChange != null && tmpToChange.text != textToDisplay)
            tmpToChange.text = textToDisplay;
        if (textToChange != null && textToChange.text != textToDisplay)
            textToChange.text = textToDisplay;
    }







    #region SECONDARY
    /// <summary>
    /// Checks if it's possible to get the non referenced components, ergonomy method, also called OnDrawGizmosSelected
    /// </summary> 
    void GetComponents()
    {
        if (textToChange == null)
            if (GetComponent<Text>())
                textToChange = GetComponent<Text>();
        if (tmpToChange == null)
            if (GetComponent<TextMeshProUGUI>())
                tmpToChange = GetComponent<TextMeshProUGUI>();
    }


    /// <summary>
    /// Removes the diacritics characters (Specified in the DiacriticsReplacementSettings scriptable object referenced in the LanguageManager) of the string
    /// </summary>
    string RemoveDiacritics(string textToModifify)
    {
        string correctedText = textToModifify;
        char[] correctedArray = textToModifify.ToCharArray();
        List<DiacriticsReplacementSettings.DiacriticReplacement> charactersToReplace = LanguageManager.Instance.diacriticsReplacementSettings.charactersToReplace;

        
        for (int i = 0; i < correctedText.Length; i++)
            for (int y = 0; y < charactersToReplace.Count; y++)
                if (correctedText[i] == charactersToReplace[y].characterToReplace)
                {
                    correctedText = correctedText.Remove(i, 1);
                    correctedText = correctedText.Insert(i, charactersToReplace[y].replacementCharacter.ToString());
                }
        

        return correctedText;
    }
    #endregion








    // EDITOR
    // To automate the settings of references, check if it's possible to get them before the user has to drag'n drop them in the Serialized Fields
    private void OnDrawGizmosSelected()
    {
        GetComponents();
    }
    #endregion
}
