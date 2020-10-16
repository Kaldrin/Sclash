using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Unity.RemoteConfig;

public class MapLoader : MonoBehaviour
{
    #region Singleton
    public static MapLoader Instance;
    #endregion


    #region VARIABLES
    #region MANAGERS
    [Header("MANAGERS")]
    [Tooltip("The reference for the unique game manager script of the scene")]
    [SerializeField] GameManager gameManager = null;
    [SerializeField] AudioManager audioManager = null;
    [SerializeField] MapMenuLoader mapMenuLoader = null;
    # endregion





    # region MAPS DATA
    [Header("MAPS DATA")]
    [Tooltip("Parent object of the currently instantiated map")]
    [SerializeField] GameObject mapContainer = null;
    [Tooltip("Currently instantiated map, visible in game")]
    [HideInInspector] public GameObject currentMap = null;
    [HideInInspector] public int currentMapIndex = 0;

    [Tooltip("Scriptable object data reference containing the maps objects, their image and names")]
    [SerializeField] public MapsDataBase mapsData = null;
    [SerializeField] public MapsDataBase specialMapsData = null;
    # endregion




    # region MAP LOADING
    [Header("MAP LOADING")]
    bool canLoadNewMap = true;
    [HideInInspector] public bool halloween = false;
    int season = 0;
    [SerializeField] bool loadMapOnStart = false;
    #endregion


    [Header("OTHER")]
    [SerializeField] PostProcessVolume cameraPostProcessVolume = null;

    // REMOTE CONFIG
    public struct userAttributes { }
    public struct appAttributes { }
    #endregion










    #region FUNCTIONS
    #region BASE FUNCTIONS

    void Awake(){
        Instance = this;


        // REMOTE CONFIG
        if (gameManager.demo)
        {
            ConfigManager.FetchCompleted += SetRemoteVariables;
            ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
        }
        else
            LoadMapOnStart();
    }

    void SetRemoteVariables(ConfigResponse response) // Get remote config variables
    {
        season = ConfigManager.appConfig.GetInt("season");
        halloween = ConfigManager.appConfig.GetBool("halloween");


        if (halloween) // HALLOWEEN SPRITES
            for (int i = 0; i < gameManager.playersList.Count; i++)
            {
                gameManager.playersList[i].GetComponent<CharacterChanger>().mask.sprite = gameManager.playersList[i].GetComponent<CharacterChanger>().masksDatabase.masksList[6].sprite;
                gameManager.playersList[i].GetComponent<CharacterChanger>().weapon.sprite = gameManager.playersList[i].GetComponent<CharacterChanger>().weaponsDatabase.weaponsList[1].sprite;
            }
            

        LoadMapOnStart();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager.demo)
            mapMenuLoader.LoadDemoParameters();
        else
            mapMenuLoader.LoadParameters();

        mapMenuLoader.SetUpMenu();
    }

    void OnDestroy()
    {
        // REMOTE CONFIG
        ConfigManager.FetchCompleted -= SetRemoteVariables;
    }
    # endregion





    # region MAP LOADING
    void LoadMapOnStart()
    {
        if (loadMapOnStart)
        {
            if (mapContainer == null) // If reference to the map parent object is null, find it again with its name
                mapContainer = GameObject.Find("MAP / ESTHETICS");


            for (int i = 0; i < mapContainer.transform.childCount; i++)
                Destroy(mapContainer.transform.GetChild(i).gameObject);


            int nextStageIndex = Random.Range(0, mapsData.stagesLists.Count);


            if (gameManager.demo)
            {
                if (halloween)
                    SetMap(0, true); // HALLOWEEN STAGE REMOTE CONFIG
                else
                    SetMap(season * 2, false); // SEASON DEPENDANT STAGE REMOTE CONFIG
            }
            else if (gameManager.gameParameters.keepLastLoadedStage)
                SetMap(gameManager.gameParameters.lastLoadedStageIndex, false);
            else if (gameManager.gameParameters.useCustomListForRandomStartStage)
            {
                int loopCount = 0;

                while (!mapsData.stagesLists[nextStageIndex].inCustomList)
                {
                    nextStageIndex = Random.Range(0, mapsData.stagesLists.Count);

                    loopCount++;
                    if (loopCount >= 100)
                    {
                        nextStageIndex = 0;
                        break;
                    }
                }


                SetMap(nextStageIndex, false);
            }
            else
                SetMap(Random.Range(0, mapsData.stagesLists.Count), false);
        }
        else
        {
            for (int i = 0; i < mapContainer.transform.childCount; i++)
            {
                if (mapContainer.transform.GetChild(i).gameObject.activeInHierarchy)
                    currentMap = mapContainer.transform.GetChild(i).gameObject;
            }
        }
    }

    // Immediatly changes the map
    public void SetMap(int mapIndex, bool special)
    {
        if (currentMap != null)
            Destroy(currentMap);


        // STAGE LOAD
        if (special) // IF SPECIAL MAP LIST (Halloween & stuff)
            currentMap = Instantiate(specialMapsData.stagesLists[mapIndex].mapObject, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), mapContainer.transform);
        else
            currentMap = Instantiate(mapsData.stagesLists[mapIndex].mapObject, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), mapContainer.transform);
        currentMapIndex = mapIndex;


        // POST PROCESS
        if (special) // IF SPECIAL MAP LIST (Halloween & stuff)
            cameraPostProcessVolume.profile = specialMapsData.stagesLists[mapIndex].postProcessProfile;
        else
            cameraPostProcessVolume.profile = mapsData.stagesLists[mapIndex].postProcessProfile;


        // PARTICLES
        bool state = false;


        for (int i = 0; i < gameManager.playersList.Count; i++)
        {
            for (int y = 0; y < gameManager.playersList[i].GetComponent<Player>().particlesSets.Count; y++)
            {
                if (special)
                {
                    if (y == (specialMapsData.stagesLists[mapIndex].particleSet))
                        state = true;
                    else
                        state = false;
                }
                else
                {
                    if (y == (mapsData.stagesLists[mapIndex].particleSet))
                        state = true;
                    else
                        state = false;
                }


                for (int o = 0; o < gameManager.playersList[i].GetComponent<Player>().particlesSets[y].particleSystems.Count; o++)
                    gameManager.playersList[i].GetComponent<Player>().particlesSets[y].particleSystems[o].SetActive(state);
            }
        }



        gameManager.gameParameters.lastLoadedStageIndex = currentMapIndex; // Writes last loaded stage index variable in scriptable object
        JsonSave save = SaveGameManager.GetCurrentSave(); // Gets save file
        save.lastLoadedStageIndex = gameManager.gameParameters.lastLoadedStageIndex; // Writes last loaded stage index variable from scriptable object to save file
        //mapMenuLoader.SaveParameters();
    }

    // Starts the LoadNewMap coroutine, launched by the play in the maps menu
    public void LoadNewMapInGame(int newMapIndex)
    {
        StartCoroutine(LoadNewMapInGameCoroutine(newMapIndex, false));
    }

    // Loads a new map with the transition FX
    IEnumerator LoadNewMapInGameCoroutine(int newMapIndex, bool randomIndex)
    {
        if (canLoadNewMap)
        {
            int index = 0;
            index = newMapIndex;


            gameManager.roundTransitionLeavesFX.gameObject.SetActive(false);
            gameManager.roundTransitionLeavesFX.gameObject.SetActive(true);
            gameManager.roundTransitionLeavesFX.Play();
            canLoadNewMap = false;


            yield return new WaitForSeconds(1.5f);

            Debug.Log("New music : " + mapsData.stagesLists[index].musicIndex);
            audioManager.ChangeSelectedMusicIndex(mapsData.stagesLists[index].musicIndex);
            SetMap(index, false);


            yield return new WaitForSeconds(2f);


            canLoadNewMap = true; 
        }
    }

    public void LoadRandomMap()
    {
        int randomIndex = Random.Range(0, mapMenuLoader.currentlyDisplayedStagesList.Count);
        int randomMapIndex = mapMenuLoader.currentlyDisplayedStagesList[randomIndex];


        StartCoroutine(LoadNewMapInGameCoroutine(randomMapIndex, true));
    }
    #endregion
    #endregion
}
