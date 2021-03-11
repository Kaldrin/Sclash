using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;


// HEADER
// Reusable script
// Original script by ???
// Customized by Bastien BERNAND to fit Sclash needs

/// <summary>
/// Script that manages saves of some variables on the local computer
/// </summary>

// VERSION
// Originally made for unity 2019.1.1f1
public static class SaveGameManager
{
    #region VARIABLES
    public const int NB_SAVES = 1;
    private static readonly string SAVE_FILE_PATH = Path.Combine(Application.persistentDataPath, "save");
    private static ObjectSaved currentSaves = new ObjectSaved();

    static SaveGameManager()
    {
        Load();
    }
    #endregion






    #region FUNCTIONS
    public static void Save()                                                                                           // SAVE
    {
        String stringSave = JsonUtility.ToJson(currentSaves);

        FileStream stream = new FileStream(SAVE_FILE_PATH, FileMode.Create, FileAccess.Write);

        byte[] savedBytes = Encoding.UTF8.GetBytes(stringSave);
        stream.Write(savedBytes, 0, savedBytes.Length);
        stream.Close();
        stream.Dispose();
            
        Debug.Log("Content file saved = " + stringSave.ToString());
    }


    public static void Load()                                                                                           // LOAD
    {
        try
        {
            if (File.Exists(SAVE_FILE_PATH))
            {
                FileStream stream = new FileStream(SAVE_FILE_PATH, FileMode.Open, FileAccess.Read);

                int streamLength = (int)stream.Length;
                byte[] bytesArray = new byte[streamLength];
                stream.Read(bytesArray, 0, streamLength);
                stream.Close();
                stream.Dispose();

                string contentFile = Encoding.UTF8.GetString(bytesArray);

                Debug.Log("Content file loaded = " + contentFile);
                    
                currentSaves = JsonUtility.FromJson<ObjectSaved>(contentFile);
            }
            else
            {
                Debug.Log("File " + SAVE_FILE_PATH + " doesn't exist");
                currentSaves = new ObjectSaved();
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Error opening file " + SAVE_FILE_PATH + " : " + e.Message);
            currentSaves = new ObjectSaved();
        }
    }


    public static JsonSave[] GetAllSaves()                                                                                      // GET ALL SAVES
    {
        return currentSaves.savesArray;
    }


    public static void DeleteAllSaves()                                                                                             // DELETE ALL SAVES
    {
        currentSaves = new ObjectSaved();
        Save();
            
        Debug.Log("All saves are successfully deleted");
    }


    public static JsonSave GetCurrentSave()                                                                                     // GET CURRENT SAVE
    {
        return currentSaves.savesArray[currentSaves.selectedSaveID];
    }


    public static void ResetCurrentSave()                                                                                       // RESET CURRENT SAVE
    {
        currentSaves.savesArray[currentSaves.selectedSaveID] = new JsonSave();
        Save();
            
        Debug.Log("Save id = " + currentSaves.selectedSaveID + " successfully reset");
    }


    public static int GetselectedSaveID()                                                                                       // GET SELECTED SAVE ID
    {
        return currentSaves.selectedSaveID;
    }


    public static JsonSave SelectSave(int id)                                                                                               // SELECTED SAVE
    {
        if(id == currentSaves.selectedSaveID)
            return GetCurrentSave();
        else if (id >= 0 && id < currentSaves.savesArray.Length)
        {
            currentSaves.savesArray[id].isEmpty = false;
            currentSaves.selectedSaveID = id;
            Save();
                
            Debug.Log("Select save id = " + id);
        }
        else
            Debug.LogError("Save id = " + id + " not found");

        return GetCurrentSave();
    }
    #endregion
}





#region CLASSES
[Serializable] public class ObjectSaved
{
    public JsonSave[] savesArray = new JsonSave[SaveGameManager.NB_SAVES];
    public int selectedSaveID = 0;

    public ObjectSaved()
    {
        for (int i = 0; i < savesArray.Length; i++)
            savesArray[i] = new JsonSave();
    }
}
[Serializable] public class stickerOnBoardAndPosition
{
    public int sticker;
    public Vector3 position;
}
[Serializable] public class JsonSave
{
    public bool isEmpty = true;


    [Header("AUDIO")]
    [SerializeField] public float masterVolume = 40;
    [SerializeField] public float menuMusicVolume = 40;
    [SerializeField] public float battleMusicVolume = 40;
    [SerializeField] public float menuFXVolume = 40;
    [SerializeField] public float fxVolume = 40;
    [SerializeField] public float voiceVolume = 40;


    [Header("GAME")]
    [SerializeField] public int roundsToWin = 5;
    [SerializeField] public bool displayHelp = true;


    [Header("ERGONOMY")]
    [SerializeField] public bool enableRumbles = true;
    [SerializeField] public string language = "en";


    [Header("STAGES")]
    [SerializeField] public List<bool> customList = new List<bool>();
    [SerializeField] public bool dayNightCycle = false;
    [SerializeField] public bool randomStage = false;
    [SerializeField] public bool useCustomListForRandom = false;
    [SerializeField] public bool keepLastLoadedStage = true;
    [SerializeField] public bool useCustomListForRandomStartStage = true;
    [SerializeField] public int lastLoadedStageIndex = 0;


    public Stats stats;
}
#endregion