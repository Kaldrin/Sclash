using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Unity.RemoteConfig;

// This class controls stage loading and most stage related stuff. Strongly tied to the MapMenuLoader and MapMenu stuff classes
// OPTIMIZED
public class MapLoader : MonoBehaviour
{
    #region VARIABLES
    #region Singleton
    public static MapLoader Instance;
    #endregion


    #region MANAGERS
    [Header("MANAGERS")]
    [Tooltip("The reference for the unique game manager script of the scene")]
    [SerializeField] GameManager gameManager = null;
    [SerializeField] AudioManager audioManager = null;
    [SerializeField] MapMenuLoader mapMenuLoader = null;
    # endregion





    # region STAGES DATA
    [Header("STAGES DATA")]
    [Tooltip("Parent object of the stages")]
    [SerializeField] GameObject mapContainer = null;
    [HideInInspector] public GameObject currentMap = null;
    [HideInInspector] public int currentMapIndex = 0;

    [Tooltip("Scriptable object data reference containing the stages objects, their images and names")]
    [SerializeField] public MapsDataBase mapsData = null;
    [Tooltip("Scriptable object data reference containing the special stages objects, their images and names")]
    [SerializeField] public MapsDataBase specialMapsData = null;
    # endregion






    # region STAGE LOADING
    [Header("STAGE LOADING")]
    [SerializeField] bool loadMapOnStart = false;
    [HideInInspector] public bool halloween = false;
    [HideInInspector] public bool christmas = false;
    bool canLoadNewMap = true;
    int season = 0;
    int postProcessVolumeBlendState = 0;
    #endregion




    [Header("OTHER")]
    [SerializeField] PostProcessVolume cameraPostProcessVolume = null;





    // REMOTE CONFIG
    [HideInInspector] public struct userAttributes { }
    [HideInInspector] public struct appAttributes { }
    #endregion




















    #region FUNCTIONS
    #region BASE FUNCTIONS
    void Awake()                                                                                                        // AWAKE
    {
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
    
    // Update is called once per graphic frame
    void Update()                                                                                                                           // UPDATE
    {
        // POST PROCESS
        // Blends last and current stages post process volumes profiles for smooth transition
        if (enabled && isActiveAndEnabled && cameraPostProcessVolume.enabled)
        {
            if (postProcessVolumeBlendState == 1)
            {
                cameraPostProcessVolume.weight = Mathf.Lerp(cameraPostProcessVolume.weight, 0, Time.deltaTime * 3);


                if (cameraPostProcessVolume.weight < 0.05f)
                {
                    cameraPostProcessVolume.profile = mapsData.stagesLists[currentMapIndex].postProcessProfile;
                    postProcessVolumeBlendState = 2;
                }
            }
            else if (postProcessVolumeBlendState == 2)
            {
                cameraPostProcessVolume.weight = Mathf.Lerp(cameraPostProcessVolume.weight, 1, Time.deltaTime * 3);


                if (cameraPostProcessVolume.weight > 0.95f)
                    postProcessVolumeBlendState = 0;
            }
        }
    }

    // Start is called before the first frame update
    void Start()                                                                                                                            // START
    {
        // STAGES MENU
        if (gameManager.demo)
            mapMenuLoader.LoadDemoParameters();
        else
            mapMenuLoader.LoadParameters();


        mapMenuLoader.SetUpMenu();
    }

    void OnDestroy()                                                                                            // ON DESTROY
    {
        // REMOTE CONFIG STUFF FOR DEMO
        ConfigManager.FetchCompleted -= SetRemoteVariables;
    }
    # endregion









    void SetRemoteVariables(ConfigResponse response) // Get remote config variables
    {
        season = ConfigManager.appConfig.GetInt("season");
        halloween = ConfigManager.appConfig.GetBool("halloween");
        christmas = ConfigManager.appConfig.GetBool("christmas");


        if (halloween) // HALLOWEEN SPRITES
            for (int i = 0; i < gameManager.playersList.Count; i++)
            {
                gameManager.playersList[i].GetComponent<CharacterChanger>().mask.sprite = gameManager.playersList[i].GetComponent<CharacterChanger>().masksDatabase.masksList[6].sprite;
                gameManager.playersList[i].GetComponent<CharacterChanger>().weapon.sprite = gameManager.playersList[i].GetComponent<CharacterChanger>().weaponsDatabase.weaponsList[1].sprite;
            }
        else if (christmas)
        {
            gameManager.charactersData = gameManager.christmasCharactersData;
            Debug.Log("Christmas");

            for (int i = 0; i < gameManager.playersList.Count; i++)
            {
                gameManager.playersList[i].GetComponent<CharacterChanger>().charactersDatabase = gameManager.christmasCharactersData;
                gameManager.playersList[i].GetComponent<CharacterChanger>().masksDatabase = gameManager.christmasMasksDatabase;
                gameManager.playersList[i].GetComponent<CharacterChanger>().weaponsDatabase = gameManager.christmasWeaponsDatabase;
            }
        }


        LoadMapOnStart();
    }







    # region STAGE LOADING
    void LoadMapOnStart()
    {
        // TRIES TO LOAD A STAGE WHEN THE GAME STARTS IF IT IS SET TO DO SO
        if (loadMapOnStart)
        {
            // CHECKS TO FIND THE STAGES PARENT OBJECT IF IT LOST REFERENCE
            if (mapContainer == null) // If reference to the map parent object is null, find it again with its name
                mapContainer = GameObject.Find("MAP / ESTHETICS");


            // DESTROYS CURRENT MAPS OBJECTS
            for (int i = 0; i < mapContainer.transform.childCount; i++)
                Destroy(mapContainer.transform.GetChild(i).gameObject);



            // CHOOSES INDEX FOR STAGE TO LOAD
            int nextStageIndex = Random.Range(0, mapsData.stagesLists.Count);




            // IF DEMO
            if (gameManager.demo)
            {
                if (halloween)
                    SetMap(0, true); // HALLOWEEN STAGE REMOTE CONFIG
                else if (christmas)
                    SetMap(1, true); // CHRISTMAS STAGE REMOTE CONFIG
                else
                    SetMap(season * 2, false); // SEASON DEPENDANT STAGE REMOTE CONFIG
            } // ELSE IF NOT DEMO
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
            for (int i = 0; i < mapContainer.transform.childCount; i++)
                if (mapContainer.transform.GetChild(i).gameObject.activeInHierarchy)
                    currentMap = mapContainer.transform.GetChild(i).gameObject;
    }


    // Immediatly changes the map
    public void SetMap(int mapIndex, bool special)
    {
        // IF THERE IS ALREADY A STAGE, DESTROY IT
        if (currentMap != null)
            Destroy(currentMap);
        if (mapContainer.transform.childCount > 0)
            for (int i = 0; i < mapContainer.transform.childCount; i++)
                Destroy(mapContainer.transform.GetChild(i).gameObject);


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
            // Starts post process volumes blend
            postProcessVolumeBlendState = 1;
        


        // CHOOSE PLAYER'S STAGE PARTICLES TO ACTIVATE
        StartCoroutine(SetPlayerParticlesSet(special));


        // AUDIO
        if (special)
            audioManager.ChangeSelectedMusicIndex(specialMapsData.stagesLists[currentMapIndex].musicIndex);
        else
            audioManager.ChangeSelectedMusicIndex(mapsData.stagesLists[currentMapIndex].musicIndex);




        // SAVES
        gameManager.gameParameters.lastLoadedStageIndex = currentMapIndex; // Writes last loaded stage index variable in scriptable object
        JsonSave save = SaveGameManager.GetCurrentSave(); // Gets save file
        save.lastLoadedStageIndex = gameManager.gameParameters.lastLoadedStageIndex; // Writes last loaded stage index variable from scriptable object to save file
        //mapMenuLoader.SaveParameters();
    }



    IEnumerator SetPlayerParticlesSet(bool special)
    {
        yield return new WaitForSecondsRealtime(0.3f);


        bool state = false;


        for (int i = 0; i < gameManager.playersList.Count; i++)
        {
            for (int y = 0; y < gameManager.playersList[i].GetComponent<Player>().particlesSets.Count; y++)
            {
                if (special) // SPECIAL STAGE PARTICLE SET
                {
                    if (y == (specialMapsData.stagesLists[currentMapIndex].particleSet))
                        state = true;
                    else
                        state = false;
                }
                else // NORMAL STAGE PARTICLE SET
                {
                    if (y == (mapsData.stagesLists[currentMapIndex].particleSet))
                        state = true;
                    else
                        state = false;
                }


                for (int o = 0; o < gameManager.playersList[i].GetComponent<Player>().particlesSets[y].particleSystems.Count; o++)
                    gameManager.playersList[i].GetComponent<Player>().particlesSets[y].particleSystems[o].SetActive(state);
            }
        }
    }



    // Starts the LoadNewMap coroutine, launched by the play in the stages menu
    public void LoadNewMapInGame(int newMapIndex)
    {
        StartCoroutine(LoadNewMapInGameCoroutine(newMapIndex, false));
    }



    // Loads a new stage with the transition FX
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

            // MUSIC
            audioManager.ChangeSelectedMusicIndex(mapsData.stagesLists[index].musicIndex);



            yield return new WaitForSeconds(1.5f);

            // LOAD STAGE
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
