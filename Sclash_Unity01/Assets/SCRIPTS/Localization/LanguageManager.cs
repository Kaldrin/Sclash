using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;




// HEADER
// Original script by Dylan BROUSSE
// Modified to fit Sclash by Bastien BERNAND
// Reusable script

// REQUIREMENTS
// Needs the script TextApparition to work
// Requires the TextApparition script.
// Requires the TextMeshPro package (Referenced in the TextApparition script)
// Requires the MenuManager script to work, but it's just because it's where the load / save functions are, you can put your own script with such functions, just edit the code to fit it
// Requires the MenuParameters scriptable object (Referenced in the MenuManager) but it can be replaced with any game settings storage scriptable object of your own, just edit the code to fit it
// Requires the DiacriticsReplacementSettings scriptable object to work

/// <summary>
/// This script, as a unique instance, will manage language stuff in the game.
/// </summary>
// It gets the json with the translation data and builds it as a list of entries that can be found with keys to display the text in the right language.

// VERSION
// Originally made for Unity 2019.14
public class LanguageManager : MonoBehaviour
{
    // SINGLETON
    [HideInInspector] public static LanguageManager Instance;


    [Header("SETTINGS")]
    [Tooltip("Currently selected language")]
    [SerializeField] public string language = "english";
    [Tooltip("Path to the json file starting from the STreamingAssets folder")]
    [SerializeField] public string filePath;
    [Tooltip("Write here the languages you want to be available to the players. Please make sure the strings are exactly the same as in the GetDialog function")]
    [SerializeField] public List<string> availableLanguages = new List<string>() { "English", "French" };


    [Header("DATA")]
    [Tooltip("Reference to the DiacriticsReplacementSettings object you want to use, the TextApparition component can be set to replace diacritics, and it will use these settings you can edit to do so")]
    [SerializeField] public DiacriticsReplacementSettings diacriticsReplacementSettings = null;
    [Tooltip("List of entries for the text in game")]
    [HideInInspector] public TextData textData;
    [Serializable] public class Entry
    {
        public string key;
        public string fr;
        public string en;
        public string turk;
        public string rus;
    }
    [Serializable] public class TextData
    {
        public Entry[] mytexts;
    }













    #region FUNCTIONS
    void Awake()                                                                                                                  // AWAKE
    {
        Instance = this;
    }


    void Start()                                                                                                                        // START
    {
        InitLanguage();


        // Gets the json file, put it in a string and converts it to a TextData variable containing the list of entries
        string dataAsJson = null;
        if (File.Exists(Application.streamingAssetsPath + "/" + filePath))
            dataAsJson = File.ReadAllText(Application.streamingAssetsPath + "/" + filePath);
        textData = JsonUtility.FromJson<TextData>("{\"mytexts\":" + dataAsJson + "}");


        RefreshTexts();
    }








    /// <summary>
    /// Finds all the active TextApparition components in the scene and reapplies the right text to them using their already set up key, through calling their TransfersTrad method
    /// </summary>
    public void RefreshTexts()                                                                                                                // REFRESH TEXTS
    {
        TextApparition[] textsToRefresh = GameObject.FindObjectsOfType<TextApparition>();

        
        foreach (TextApparition objet in textsToRefresh)
            objet.TransfersTrad();
    }


    /// <summary>
    /// Returns the text of an entry through its key, in the currently selected language
    /// </summary>
    public string GetDialog(string key)                                                                                                 // GET DIALOG
    {
        if (textData != null)
            for (int i = 0; i < textData.mytexts.Length; i++)
                if (textData.mytexts[i].key == key)
                {
                    // RUSSIAN
                    if (language == "русский" && textData.mytexts[i].rus != null && textData.mytexts[i].rus != "")
                        return textData.mytexts[i].rus;
                    // FRENCH
                    else if (language == "FRANCAIS" && textData.mytexts[i].fr != null && textData.mytexts[i].fr != "")
                        return textData.mytexts[i].fr;
                    // ENGLISH, default
                    else
                        return textData.mytexts[i].en;
                }


        return null;
    }

    /// <summary>
    /// // Returns the key of an entry through its index
    /// </summary>
    public string GetDialogKeyfromIndex(int startIndex)                                                                                             // GET DIALOG KEY FROM INDEX
    {
        if (textData != null)
            return textData.mytexts[startIndex].key;
        return null;
    }


    // Set up language settings
    public void InitLanguage()                                                                                                          // INIT LANGUAGE
    {
        if (MenuManager.Instance != null)
        {
            // Load setting from save into scriptable object
            MenuManager.Instance.LoadErgonomySaveInScriptableObject();

            // Set up active language from scriptable object data
            if (MenuManager.Instance.menuParametersSaveScriptableObject.language != language)
                language = MenuManager.Instance.menuParametersSaveScriptableObject.language;
        }
    }
    #endregion
}
