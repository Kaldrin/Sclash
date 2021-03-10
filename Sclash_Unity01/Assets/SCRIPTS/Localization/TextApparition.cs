using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;





// Script that manages the apparition of the right language text in a text box
// Originally made by Dylan BROUSSE, modified by BASTIEN BERNAND to fit Sclash needs
// Originally made for Unity 2019.14
public class TextApparition : MonoBehaviour
{
    [Header("COMPONENTS")]
    [SerializeField] TextMeshProUGUI tmpToChange = null;
    [SerializeField] Text textToChange = null;

    [Header("SETTINGS")]
    [SerializeField] public string textKey = "KEY";
    [SerializeField] TextMode textMode = TextMode.normal;
    [SerializeField] bool removeDiacritics = false;
    [SerializeField] bool onStart = true;
    [SerializeField] bool onEnable = true;
    private string textToDisplay = "";


    [System.Serializable] public enum TextMode
    {
        allCaps,
        allLowerCase,
        normal
    }






    #region FUNCTIONS
    private void Awake()
    {
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


    void Update()                                                                                                           // UPDATE
    {
        /*
        if(realText != LanguageManager.Instance.GetDialog(text))
            TransfersTrad();
            */
    }



    // DISPLAY RIGHT TEXT
    public void TransfersTrad()
    {
        GetComponents();


        // Get the right text
        if (LanguageManager.Instance != null)
            textToDisplay = LanguageManager.Instance.GetDialog(textKey);

        /*
        if(textToDisplay != this.GetComponent<Text>().text)
            this.GetComponent<Text>().text = textToDisplay;
            */

        


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





    // SECONDARY
    // Get components if they're not referenced
    void GetComponents()
    {
        if (textToChange == null)
            if (GetComponent<Text>())
                textToChange = GetComponent<Text>();
        if (tmpToChange == null)
            if (GetComponent<TextMeshProUGUI>())
                tmpToChange = GetComponent<TextMeshProUGUI>();
    }

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
   








    // EDITOR
    private void OnDrawGizmosSelected()
    {
        GetComponents();
    }
    #endregion
}
