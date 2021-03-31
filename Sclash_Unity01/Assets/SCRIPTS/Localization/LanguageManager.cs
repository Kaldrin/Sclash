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
    [Tooltip("Currently selected text language")]
    [SerializeField] public string language = "ENGLISH";
    [Tooltip("Currently selected voices language")]
    [SerializeField] public string voicesLanguage = "ENGLISH";
    [Tooltip("Path to the json file starting from the STreamingAssets folder")]
    [SerializeField] public string filePath = "igtexts.json";
    [Tooltip("Write here the languages you want to be available to the players. Please make sure the strings are exactly the same as in the GetDialog function")]
    [SerializeField] public List<string> availableLanguages = new List<string>() { "ENGLISH", "FRANCAIS" };
    [SerializeField] public List<string> availableVoicesLanguages = new List<string>() { "ENGLISH", "FRANCAIS" };


    [Header("DATA")]
    [Tooltip("Reference to the DiacriticsReplacementSettings object you want to use, the TextApparition component can be set to replace diacritics, and it will use these settings you can edit to do so")]
    [SerializeField] public DiacriticsReplacementSettings diacriticsReplacementSettings = null;
    [Tooltip("List of voice acting entries")]
    [SerializeField] VoiceClipsDataBase voiceClipsDataBase = null;
    [Tooltip("List of entries for the text in game")]
    [HideInInspector] public TextData textData;
    [Serializable] public class Entry
    {
        public string key;
        public string fr;
        public string en;
        public string ger;
        public string braz;
        public string port;
        public string spa;
        public string ita;
        public string jap;
        public string chi;
        public string kor;
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
                    // FRENCH
                    if (language == "FRANCAIS" && textData.mytexts[i].fr != null && textData.mytexts[i].fr != "")
                        return textData.mytexts[i].fr;
                    // GERMAN
                    else if (language == "DEUTSCHE" && textData.mytexts[i].ger != null && textData.mytexts[i].ger != "")
                        return textData.mytexts[i].ger;
                    // BRAZILIAN PORTUGUESE
                    else if (language == "BRASILEIRO" && textData.mytexts[i].braz != null && textData.mytexts[i].braz != "")
                        return textData.mytexts[i].braz;
                    // PORTUGUESE
                    else if (language == "PORTUGUES" && textData.mytexts[i].port != null && textData.mytexts[i].port != "")
                        return textData.mytexts[i].port;
                    // SPANISH
                    else if (language == "ESPANOL" && textData.mytexts[i].spa != null && textData.mytexts[i].spa != "")
                        return textData.mytexts[i].spa;
                    // ITALIAN
                    else if (language == "ITALIANO" && textData.mytexts[i].ita != null && textData.mytexts[i].ita != "")
                        return textData.mytexts[i].ita;
                    // JAPANESE
                    else if (language == "日本語" && textData.mytexts[i].jap != null && textData.mytexts[i].jap != "")
                        return textData.mytexts[i].jap;
                    // CHINESE
                    else if (language == "中国人" && textData.mytexts[i].chi != null && textData.mytexts[i].chi != "")
                        return textData.mytexts[i].chi;
                    // KOREAN
                    else if (language == "한국어" && textData.mytexts[i].kor != null && textData.mytexts[i].kor != "")
                        return textData.mytexts[i].kor;
                    // TURKISH
                    else if (language == "TURK" && textData.mytexts[i].turk != null && textData.mytexts[i].turk != "")
                        return textData.mytexts[i].turk;
                    // RUSSIAN
                    else if (language == "РУССКИЙ" && textData.mytexts[i].rus != null && textData.mytexts[i].rus != "")
                        return textData.mytexts[i].rus;
                    // ENGLISH, default
                    else
                        return textData.mytexts[i].en;
                }


        return null;
    }


    public AudioClip GetAudioClip(string key)
    {
        if (voiceClipsDataBase != null)
            for (int i = 0; i < voiceClipsDataBase.voiceClips.Count; i++)
                if (voiceClipsDataBase.voiceClips[i].key == key)
                {
                    // FRENCH
                    if (voicesLanguage == "FRANCAIS" && voiceClipsDataBase.voiceClips[i].fr != null)
                        return voiceClipsDataBase.voiceClips[i].fr;
                    // GERMAN
                    else if (voicesLanguage == "DEUTSCHE" && voiceClipsDataBase.voiceClips[i].ger != null)
                        return voiceClipsDataBase.voiceClips[i].ger;
                    // BRAZILIAN PORTUGUESE
                    else if (voicesLanguage == "BRASILEIRO" && voiceClipsDataBase.voiceClips[i].braz != null)
                        return voiceClipsDataBase.voiceClips[i].braz;
                    // PORTUGUESE
                    else if (voicesLanguage == "PORTUGUES" && voiceClipsDataBase.voiceClips[i].port != null)
                        return voiceClipsDataBase.voiceClips[i].port;
                    // SPANISH
                    else if (voicesLanguage == "ESPANOL" && voiceClipsDataBase.voiceClips[i].spa != null)
                        return voiceClipsDataBase.voiceClips[i].spa;
                    // ITALIAN
                    else if (voicesLanguage == "ITALIANO" && voiceClipsDataBase.voiceClips[i].ita != null)
                        return voiceClipsDataBase.voiceClips[i].ita;
                    // JAPANESE
                    else if (voicesLanguage == "日本語" && voiceClipsDataBase.voiceClips[i].jap != null)
                        return voiceClipsDataBase.voiceClips[i].jap;
                    // CHINESE
                    else if (voicesLanguage == "中国人" && voiceClipsDataBase.voiceClips[i].chi != null)
                        return voiceClipsDataBase.voiceClips[i].chi;
                    // KOREAN
                    else if (voicesLanguage == "한국어" && voiceClipsDataBase.voiceClips[i].kor != null)
                        return voiceClipsDataBase.voiceClips[i].kor;
                    // TURKISH
                    else if (voicesLanguage == "TURK" && voiceClipsDataBase.voiceClips[i].turk != null)
                        return voiceClipsDataBase.voiceClips[i].turk;
                    // RUSSIAN
                    else if (voicesLanguage == "РУССКИЙ" && voiceClipsDataBase.voiceClips[i].rus != null)
                        return voiceClipsDataBase.voiceClips[i].rus;
                    // ENGLISH, default
                    else
                        return voiceClipsDataBase.voiceClips[i].en;
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
            if (MenuManager.Instance.menuParametersSaveScriptableObject.voicesLanguage != voicesLanguage)
                voicesLanguage = MenuManager.Instance.menuParametersSaveScriptableObject.voicesLanguage;
        }
    }
    #endregion
}
