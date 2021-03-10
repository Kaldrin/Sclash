using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;



// Original script by Dylan BROUSSE
// Modified to fit Sclash by Bastien BERNAND
// Originally for Unity 2019.14
public class LanguageManager : MonoBehaviour
{
    // SINGLETON
    [HideInInspector] public static LanguageManager Instance;



    [Header("SETTINGS")]
    [SerializeField] public float dialogSpeed = 0.04f;
    [SerializeField] public string language = "english";
    [SerializeField] public string filePath;
    [SerializeField] public List<string> availableLanguages = new List<string>() { "English", "French" };


    [Header("DATA")]
    [SerializeField] public DiacriticsReplacementSettings diacriticsReplacementSettings = null;


    // TEXT DATA
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

    [HideInInspector] public TextData textData;












    void Awake()                                                                                            // AWAKE
    {
        Instance = this;
    }


    void Start()                                                                                                    // START
    {
        InitLanguage();


        string dataAsJson = null;
        if (File.Exists(Application.streamingAssetsPath + "/" + filePath))
            dataAsJson = File.ReadAllText(Application.streamingAssetsPath + "/" + filePath);
        textData = JsonUtility.FromJson<TextData>("{\"mytexts\":" + dataAsJson + "}");


        RefreshTexts();
    }






    // Finds all the texts and reapplies the right text to them
    public void RefreshTexts()
    {
        TextApparition[] textsToRefresh = GameObject.FindObjectsOfType<TextApparition>();

        
        foreach (TextApparition objet in textsToRefresh)
            objet.TransfersTrad();
    }


    public string GetDialog(string key)
    {
        if (textData != null)
            for (int i = 0; i < textData.mytexts.Length; i++)
                if (textData.mytexts[i].key == key)
                {
                    if (language == "русский" && textData.mytexts[i].rus != null && textData.mytexts[i].rus != "")
                        return textData.mytexts[i].rus;
                    else if (language == "FRANCAIS" && textData.mytexts[i].fr != null && textData.mytexts[i].fr != "")
                        return textData.mytexts[i].fr;
                    else
                        return textData.mytexts[i].en;
                }


        return null;
    }

    /*
    public List<string> GetListOfCharacters(int position, int length)
    {
        if (datas != null)
        {
            List<string> characterNames = new List<string>();
            string[] referenceKey = datas.mytexts[position].Key.Split("_"[0]);


            for (int i = position; i < position + length; i++)
                if (!characterNames.Contains(datas.mytexts[i].FRName))
                    characterNames.Add(datas.mytexts[i].FRName);


            return characterNames;
        }


        return null;
    }
    */
    

    public string GetDialogKeyfromIndex(int startIndex)
    {
        //save.language = "french";
        if (textData != null)
            return textData.mytexts[startIndex].key;
        return null;
    }

    /*
    public string GetNameOfTheSpeaker(string key)
    {
        if (datas != null)
        {
            for (int i = 0; i < datas.mytexts.Length; i++)
                if (datas.mytexts[i].Key == key)
                {
                    if (language == "french")
                        return datas.mytexts[i].FRName;
                    else
                        return datas.mytexts[i].ENName;
                }


            return null;
        }


        return null;
    }
    */


    public Sprite GetCharacterSprite(string key)
    {
        /*
        if (datas != null)
        {
            for (int i = 0; i < datas.mytexts.Length; i++)
                if (datas.mytexts[i].Key == key)
                    return CharactersBank.CharBank.GetSprite(datas.mytexts[i].FRName);
            return null;
        }
        */

        return null;
    }


    public int GetDialogPosition(string key)
    {
        if (textData != null)
        {
            string[] actualKeyPart = key.Split("_"[0]);


            for (int i = 0; i < textData.mytexts.Length; i++)
            {
                string[] keyParts = textData.mytexts[i].key.Split("_"[0]);


                if (keyParts.Length > 1)
                    if (keyParts[0] + "_" + keyParts[1] == key)
                        return i;
            }


            return 0;
        }


        return 0;
    }


    public int DialogQuoteLength(int startIndex)
    {
        if (textData != null)
        {
            int index = 0;
            string[] referenceKey = textData.mytexts[startIndex].key.Split("_"[0]);


            for (int i = startIndex; i < textData.mytexts.Length; i++)
            {
                string[] actualKey = textData.mytexts[i].key.Split("_"[0]);
                index = i;


                if (actualKey[0] + "_" + actualKey[1] != referenceKey[0] + "_" + referenceKey[1])
                    return i - startIndex;
            }


            return (index + 1) - startIndex;
        }


        return 0;
    }


    public List<string> GetAWholeDialog(string key)
    {
        string[] actualKeyPart = key.Split("_"[0]);
        List<string> list2trucs = new List<string>();


        for (int i = 0; i < textData.mytexts.Length; i++)
        {
            string[] keyParts = textData.mytexts[i].key.Split("_"[0]);


            if (keyParts[0] == actualKeyPart[0])
                for (int x = i; x < textData.mytexts.Length; x++)
                {
                    if (keyParts[1] == actualKeyPart[1])
                        list2trucs.Add(textData.mytexts[x].key);
                    else
                        return list2trucs;
                }
        }


        return null;
    }



    public void InitLanguage()
    {
        if (MenuManager.Instance != null)
        {
            //SettingsData settingsSave = GameSaveSystem.LoadSettingsData();
            MenuManager.Instance.LoadErgonomySaveInScriptableObject();


            if (MenuManager.Instance.menuParametersSaveScriptableObject.language != language)
                language = MenuManager.Instance.menuParametersSaveScriptableObject.language;
        }
    }
}
