using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapMenuLoader : MonoBehaviour
{
    # region MANAGERS
    [Header("MANAGERS")]
    [SerializeField] MapLoader mapLoader = null;
    #endregion



    #region DATA
    [Header("DATA")]
    [Tooltip("The reference to the scriptable object data containing all the maps")]
    [SerializeField] MapsDataBase mapsDatabase01;
    [SerializeField] MenuParameters parametersData = null;
    #endregion




    #region STAGE MODE
    [Header("STAGE MODE")]
    [SerializeField] List<string> stagesModes = new List<string>() { "all", "day", "night", "custom" };
    [SerializeField] int currentStageMode = 0;
    [SerializeField] List<GameObject> stageModeDisplayObjects = null;
    #endregion




    #region INPUT
    [Header("INPUT")]
    [SerializeField] string stageModeSwitchAxis = "MenuTriggers";
    [SerializeField] float stageModeSwitchAxisDeadzone = 0.3f;
    bool canInputModeChange = true;
    #endregion




    #region STAGES LIST
    [Header("STAGE LIST")]
    [Tooltip("The reference to the game object in which all the menu elements of the maps to choose it will be instantiated")]
    [SerializeField] Transform stagesListParent = null;
    [SerializeField]
    GameObject
        stageButtonObject = null;
    [HideInInspector] public List<int> currentlyDisplayedStagesList = new List<int>();
    #endregion




    #region BROWSING
    [SerializeField] MenuBrowser2D stagesListBrowser2D = null;
    [SerializeField] int numberOfElementsPerLine = 4;
    #endregion




    #region STAGE PARAMETERS
    [SerializeField] GameObject dayNightCycleCheck = null;
    [SerializeField] GameObject randomStageAfterGameCheck = null;
    [SerializeField] GameObject customListForRandomStageAfterGameCheck = null;
    [SerializeField] GameObject keepLastLoadedStageCheck = null;
    [SerializeField] GameObject customListForStartRandomStageCheck = null;
    #endregion




    #region AUDIO
    [Header("AUDIO")]
    [SerializeField] PlayRandomSoundInList clickSoundSource = null;
    #endregion











    #region BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        //LoadParameters();
        //SetUpMenu();
        ChangeStageMode(0);
    }

    private void OnEnable()
    {
        LoadParameters();
        SetUpMenu();
    }

    void Update()
    {
        ManageStageModeChange();
    }
    # endregion








    #region STAGE MODE
    void ManageStageModeChange()
    {
        if (canInputModeChange && Mathf.Abs(Input.GetAxis(stageModeSwitchAxis)) > stageModeSwitchAxisDeadzone)
        {
            canInputModeChange = false;


            if (Input.GetAxis(stageModeSwitchAxis) < -stageModeSwitchAxisDeadzone)
            {
                ChangeStageMode(-1);
            }
            else if (Input.GetAxis(stageModeSwitchAxis) > -stageModeSwitchAxisDeadzone)
            {
                ChangeStageMode(1);
            }
        }
        else
        if (Mathf.Abs(Input.GetAxis(stageModeSwitchAxis)) < stageModeSwitchAxisDeadzone)
        {
            canInputModeChange = true;
        }
    }

    public void ChangeStageMode(int indexIncrementation)
    {
        // AUDIO
        if (clickSoundSource)
            clickSoundSource.Play();


        currentStageMode += indexIncrementation;


        if (currentStageMode < 0)
            currentStageMode = stagesModes.Count - 1;
        else if (currentStageMode > stagesModes.Count - 1)
            currentStageMode = 0;


        UpdateDisplayedMapListTypeIndication();
        StartCoroutine(LoadStagesList());
    }

    void UpdateDisplayedMapListTypeIndication()
    {
        for (int i = 0; i < stagesModes.Count; i++)
        {
            if (i == currentStageMode)
                stageModeDisplayObjects[i].SetActive(true);
            else
                stageModeDisplayObjects[i].SetActive(false);
        }
    }
    #endregion








    #region STAGES LIST
    void ClearStagesList()
    {
        stageButtonObject.SetActive(false);
        currentlyDisplayedStagesList.Clear();
        stagesListBrowser2D.elements2D = new MenuBrowser2D.ElementsLine[0];


        for (int i = 0; i < stagesListParent.childCount; i++)
        {
            if (stagesListParent.GetChild(i).gameObject.activeInHierarchy)
                Destroy(stagesListParent.GetChild(i).gameObject);
        }
    }

    IEnumerator LoadStagesList()
    {
        // CLEAR LIST
        StopCoroutine(LoadStagesList());
        ClearStagesList();




        // BROWSING
        int numberOfElementsOnThisLine = 0;
        int numberOfLinesForBrowsing = 0;


        for (int i = 0; i < mapsDatabase01.stagesLists.Count; i++)
        {
            if ((mapsDatabase01.stagesLists[i].type.ToString() == stagesModes[currentStageMode]) || stagesModes[currentStageMode] == "all" || (mapsDatabase01.stagesLists[i].inCustomList && (stagesModes[currentStageMode] == stagesModes[3])))
            {
                numberOfElementsOnThisLine++;


                if (numberOfElementsOnThisLine == numberOfElementsPerLine)
                {
                    numberOfLinesForBrowsing++;
                    stagesListBrowser2D.elements2D = new MenuBrowser2D.ElementsLine[numberOfLinesForBrowsing];

                    for (int y = 0; y < stagesListBrowser2D.elements2D.Length - 1; y++)
                    {
                        stagesListBrowser2D.elements2D[y] = new MenuBrowser2D.ElementsLine();
                        stagesListBrowser2D.elements2D[y].line = new GameObject[numberOfElementsPerLine];
                    }
                    stagesListBrowser2D.elements2D[numberOfLinesForBrowsing - 1] = new MenuBrowser2D.ElementsLine();
                    stagesListBrowser2D.elements2D[numberOfLinesForBrowsing - 1].line = new GameObject[numberOfElementsOnThisLine];

                    numberOfElementsOnThisLine = 0;
                }
            }
        }


        if (numberOfElementsOnThisLine > 0)
        {
            numberOfLinesForBrowsing++;
            stagesListBrowser2D.elements2D = new MenuBrowser2D.ElementsLine[numberOfLinesForBrowsing];
            for (int y = 0; y < stagesListBrowser2D.elements2D.Length - 1; y++)
            {
                stagesListBrowser2D.elements2D[y] = new MenuBrowser2D.ElementsLine();
                stagesListBrowser2D.elements2D[y].line = new GameObject[numberOfElementsPerLine];
            }
            stagesListBrowser2D.elements2D[numberOfLinesForBrowsing - 1] = new MenuBrowser2D.ElementsLine();
            stagesListBrowser2D.elements2D[numberOfLinesForBrowsing - 1].line = new GameObject[numberOfElementsOnThisLine];
            numberOfElementsOnThisLine = 0;
        }


        numberOfElementsOnThisLine = 0;
        numberOfLinesForBrowsing = 0;


        // FILL LIST
        for (int i = 0; i < mapsDatabase01.stagesLists.Count; i++)
        {
            if ((mapsDatabase01.stagesLists[i].type.ToString() == stagesModes[currentStageMode]) || stagesModes[currentStageMode] == "all" || (mapsDatabase01.stagesLists[i].inCustomList && (stagesModes[currentStageMode] == stagesModes[3])))
            {
                GameObject newMapMenuObject = null;
                MapMenuObject newMapMenuObjectScript = null;


                newMapMenuObject = Instantiate(stageButtonObject, stagesListParent);
                newMapMenuObject.SetActive(true);
                newMapMenuObjectScript = newMapMenuObject.GetComponent<MapMenuObject>();


                newMapMenuObjectScript.mapImage.sprite = mapsDatabase01.stagesLists[i].mapImage;
                newMapMenuObjectScript.mapText.text = mapsDatabase01.stagesLists[i].stageName;
                newMapMenuObjectScript.stageIndex = i;
                newMapMenuObjectScript.UpdateCustomListCheckBox();
                currentlyDisplayedStagesList.Add(i);

                // BROWSING
                stagesListBrowser2D.elements2D[numberOfLinesForBrowsing].line[numberOfElementsOnThisLine] = newMapMenuObjectScript.mapButtonObject;
                numberOfElementsOnThisLine++;
                if (numberOfElementsOnThisLine == numberOfElementsPerLine)
                {
                    numberOfElementsOnThisLine = 0;
                    numberOfLinesForBrowsing++;
                }


                yield return new WaitForSeconds(0.05f);
            }
        }


        stageButtonObject.SetActive(false);
    }
    #endregion









    # region SAVE / LOAD PARAMETERS
    // Sets up the menu from scriptable object
    public void SetUpMenu()
    {
        dayNightCycleCheck.SetActive(parametersData.dayNightCycle);
        randomStageAfterGameCheck.SetActive(parametersData.randomStage);
        customListForRandomStageAfterGameCheck.SetActive(parametersData.useCustomListForRandom);
        keepLastLoadedStageCheck.SetActive(parametersData.keepLastLoadedStage);
        customListForStartRandomStageCheck.SetActive(parametersData.useCustomListForRandomStartStage);
    }


    // Load menu parameters save in the scriptable object
    public void LoadParameters()
    {
        JsonSave save = SaveGameManager.GetCurrentSave();

        parametersData.dayNightCycle = save.dayNightCycle;
        parametersData.randomStage = save.randomStage;
        parametersData.useCustomListForRandom = save.useCustomListForRandom;
        parametersData.keepLastLoadedStage = save.keepLastLoadedStage;
        parametersData.useCustomListForRandomStartStage = save.useCustomListForRandomStartStage;
        parametersData.lastLoadedStageIndex = save.lastLoadedStageIndex;


        // Favourite stages list
        parametersData.customList = save.customList;

        if (mapsDatabase01 == null)
        {
            Debug.LogWarning("Map database is null");
            return;
        }

        if (parametersData.customList.Count < mapsDatabase01.stagesLists.Count)
        {
            parametersData.customList.Clear();


            for (int i = 0; i < mapsDatabase01.stagesLists.Count; i++)
                parametersData.customList.Add(mapsDatabase01.stagesLists[i].inCustomList);
        }



        for (int i = 0; i < mapsDatabase01.stagesLists.Count; i++)
        {
            Map newMap = mapLoader.mapsData.stagesLists[i];


            if (parametersData.customList[i])
                newMap.inCustomList = true;
            else
                newMap.inCustomList = false;


            mapLoader.mapsData.stagesLists[i] = newMap;
        }
    }


    // Saves the menu parameters in the scriptable object
    void SaveInScriptableObject()
    {
        parametersData.dayNightCycle = dayNightCycleCheck.activeInHierarchy;
        parametersData.randomStage = randomStageAfterGameCheck.activeInHierarchy;
        parametersData.useCustomListForRandom = customListForRandomStageAfterGameCheck.activeInHierarchy;
        parametersData.keepLastLoadedStage = keepLastLoadedStageCheck.activeInHierarchy;
        parametersData.useCustomListForRandomStartStage = customListForStartRandomStageCheck.activeInHierarchy;


        // Custom stages list
        if (parametersData.customList.Count < mapsDatabase01.stagesLists.Count)
        {
            parametersData.customList.Clear();


            for (int i = 0; i < mapsDatabase01.stagesLists.Count; i++)
                parametersData.customList.Add(mapsDatabase01.stagesLists[i].inCustomList);
        }

        for (int i = 0; i < mapsDatabase01.stagesLists.Count; i++)
        {
            if (mapsDatabase01.stagesLists[i].inCustomList)
                parametersData.customList[i] = true;
            else
                parametersData.customList[i] = false;
        }
    }

    // Save forever from scriptable object data
    public void SaveParameters()
    {
        SaveInScriptableObject();


        JsonSave save = SaveGameManager.GetCurrentSave();


        save.customList = parametersData.customList;
        save.dayNightCycle = parametersData.dayNightCycle;
        save.randomStage = parametersData.randomStage;
        save.useCustomListForRandom = parametersData.useCustomListForRandom;
        save.keepLastLoadedStage = parametersData.keepLastLoadedStage;
        save.useCustomListForRandomStartStage = parametersData.useCustomListForRandomStartStage;
        save.lastLoadedStageIndex = parametersData.lastLoadedStageIndex;


        SaveGameManager.Save();
    }
    #endregion
}
