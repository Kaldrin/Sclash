using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Unity.RemoteConfig;
using System.IO;





// HEADER
// For Sclash
// OPTIMIZED

// REQUIREMENTS
// Requires Remote Config package
// Requires Post Processing Stack package
// Requires GameManager script (Single instance)
// Requires Audio Manager script (Single instance)
// Requires MapMenuLoader script (Single instance)

/// <summary>
/// This script controls stage loading and most stage related stuff. Strongly tied to the MapMenuLoader and MapMenu stuff classes
/// </summary>


public class MapLoader : MonoBehaviour
{
    #region VARIABLES
    // Singleton
    public static MapLoader Instance;



    [Header("MANAGERS")]
    [Tooltip("The reference for the unique game manager script of the scene")]
    [SerializeField] AudioManager audioManager = null;
    [SerializeField] MapMenuLoader mapMenuLoader = null;




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






    [Header("STAGE LOADING")]
    [SerializeField] bool loadMapOnStart = false;
    [HideInInspector] public bool halloween = false;
    [HideInInspector] public bool christmas = false;
    bool canLoadNewMap = true;
    int season = 0;
    int postProcessVolumeBlendState = 0;




    [Header("OTHER")]
    [SerializeField] PostProcessVolume cameraPostProcessVolume = null;





    // REMOTE CONFIG
    [HideInInspector] public struct userAttributes { }
    [HideInInspector] public struct appAttributes { }
    #endregion



    const int BridgeDay = 0;
    const int BridgeNight = 1;
    const int VillageDay = 2;
    const int VillageNight = 3;
    const int BattlefieldDay = 4;
    const int BattlefieldNight = 5;
    const int SnowTempleDay = 6;
    const int SnowTempleNight = 7;
    const int TempleDay = 8;
    const int TempleNight = 9;
    List<AssetBundle> loadedMapBundles = new List<AssetBundle>();














    #region FUNCTIONS
    #region BASE FUNCTIONS
    void Awake()                                                                                                                                                                    // AWAKE
    {
        Instance = this;


        // REMOTE CONFIG
        if (GameManager.Instance != null && GameManager.Instance.demo)
        {
            ConfigManager.FetchCompleted += SetRemoteVariables;
            ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
        }
        else
            LoadMapOnStart();
    }


    void Update()                                                                                                                                                                      // UPDATE
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
    void Start()                                                                                                                                                                    // START
    {
        // STAGES MENU
        if (GameManager.Instance != null && GameManager.Instance.demo)
            mapMenuLoader.LoadDemoParameters();
        else
            mapMenuLoader.LoadParameters();


        mapMenuLoader.SetUpMenu();
    }

    void OnDestroy()                                                                                                                                                           // ON DESTROY
    {
        // REMOTE CONFIG STUFF FOR DEMO
        ConfigManager.FetchCompleted -= SetRemoteVariables;
    }
    # endregion









    void SetRemoteVariables(ConfigResponse response) // Get remote config variables                                                                                     // SET REMOTE VARIABLES
    {
        season = ConfigManager.appConfig.GetInt("season");
        halloween = ConfigManager.appConfig.GetBool("halloween");
        christmas = ConfigManager.appConfig.GetBool("christmas");

        if (GameManager.Instance != null)
        {
            if (halloween) // HALLOWEEN SPRITES
                for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
                {
                    GameManager.Instance.playersList[i].GetComponent<CharacterChanger>().mask.sprite = GameManager.Instance.playersList[i].GetComponent<CharacterChanger>().masksDatabase.masksList[6].sprite;
                    GameManager.Instance.playersList[i].GetComponent<CharacterChanger>().weapon.sprite = GameManager.Instance.playersList[i].GetComponent<CharacterChanger>().weaponsDatabase.weaponsList[1].sprite;
                }
            else if (christmas)
            {
                GameManager.Instance.charactersData = GameManager.Instance.christmasCharactersData;
                Debug.Log("Christmas");

                for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
                {
                    GameManager.Instance.playersList[i].GetComponent<CharacterChanger>().charactersDatabase = GameManager.Instance.christmasCharactersData;
                    GameManager.Instance.playersList[i].GetComponent<CharacterChanger>().masksDatabase = GameManager.Instance.christmasMasksDatabase;
                    GameManager.Instance.playersList[i].GetComponent<CharacterChanger>().weaponsDatabase = GameManager.Instance.christmasWeaponsDatabase;
                }
            }
        }


        LoadMapOnStart();
    }







    # region STAGE LOADING
    void LoadMapOnStart()                                                                                                                                               // LOAD MAP ON START
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
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.demo)
                {
                    if (halloween)
                        SetMap(0, true); // HALLOWEEN STAGE REMOTE CONFIG
                    else if (christmas)
                        SetMap(1, true); // CHRISTMAS STAGE REMOTE CONFIG
                    else
                        SetMap(season * 2, false); // SEASON DEPENDANT STAGE REMOTE CONFIG
                } // ELSE IF NOT DEMO
                else if (GameManager.Instance.gameParameters.keepLastLoadedStage)
                    SetMap(GameManager.Instance.gameParameters.lastLoadedStageIndex, false);
                else if (GameManager.Instance.gameParameters.useCustomListForRandomStartStage)
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
        }
        else
            for (int i = 0; i < mapContainer.transform.childCount; i++)
                if (mapContainer.transform.GetChild(i).gameObject.activeInHierarchy)
                    currentMap = mapContainer.transform.GetChild(i).gameObject;
    }



    IEnumerator LoadMapPrefab(AssetBundle bundle, string assetName)
    {
        AssetBundleRequest request = bundle.LoadAssetAsync<GameObject>(assetName);
        yield return request;

        currentMap = Instantiate((GameObject)request.asset, Vector3.zero, Quaternion.identity, mapContainer.transform);
        Debug.Log("Map Loaded from bundle");

        yield return new WaitForEndOfFrame();

        bundle.Unload(false);
    }

    /// <summary>
    /// Immediatly changes the stage, affects corresponding music and fx, and saves its index
    /// </summary>
    /// <param name="mapIndex"></param>
    /// <param name="special"></param>
    public void SetMap(int mapIndex, bool special, AssetBundle bundle = null)                                                                                                                      // SET MAP
    {
        // IF THERE IS ALREADY A STAGE, DESTROY IT
        if (currentMap != null)
            Destroy(currentMap);
        if (mapContainer.transform.childCount > 0)
            for (int i = 0; i < mapContainer.transform.childCount; i++)
                Destroy(mapContainer.transform.GetChild(i).gameObject);


        // STAGE LOAD
        if (bundle == null)
        {
            if (special) // IF SPECIAL MAP LIST (Halloween & stuff)
                currentMap = Instantiate(specialMapsData.stagesLists[mapIndex].mapObject, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), mapContainer.transform);
            else
                currentMap = Instantiate(mapsData.stagesLists[mapIndex].mapObject, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), mapContainer.transform);
        }
        else
        {
            StartCoroutine(LoadMapPrefab(bundle, mapsData.stagesLists[mapIndex].prefabName));
        }
        currentMapIndex = mapIndex;



        // POST PROCESS
        if (special) // IF SPECIAL MAP LIST (Halloween & stuff)
            cameraPostProcessVolume.profile = specialMapsData.stagesLists[mapIndex].postProcessProfile;
        else
            // Starts post process volumes blend
            postProcessVolumeBlendState = 1;



        // CHOOSE PLAYER'S STAGE PARTICLES & FOOT STEP TO ACTIVATE
        StartCoroutine(SetPlayerParticlesSet(special));
        StartCoroutine(SetPlayersWalkSFXSet(special));
        //Invoke("SetPlayerParticlesSet", 0.3f);
        //Invoke("SetPlayersWalkSFXSet", 0.3f);



        // AUDIO
        if (special)
            audioManager.ChangeSelectedMusicIndex(specialMapsData.stagesLists[currentMapIndex].musicIndex);
        else
            audioManager.ChangeSelectedMusicIndex(mapsData.stagesLists[currentMapIndex].musicIndex);




        // SAVES
        if (GameManager.Instance != null)
        {
            GameManager.Instance.gameParameters.lastLoadedStageIndex = currentMapIndex; // Writes last loaded stage index variable in scriptable object
            JsonSave save = SaveGameManager.GetCurrentSave(); // Gets save file
            save.lastLoadedStageIndex = GameManager.Instance.gameParameters.lastLoadedStageIndex; // Writes last loaded stage index variable from scriptable object to save file
        }
    }



    IEnumerator SetPlayerParticlesSet(bool special)
    {
        yield return new WaitForSecondsRealtime(0.3f);
        bool state = false;

        if (GameManager.Instance != null)
            for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
                for (int y = 0; y < GameManager.Instance.playersList[i].GetComponent<Player>().particlesSets.Count; y++)
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


                    for (int o = 0; o < GameManager.Instance.playersList[i].GetComponent<Player>().particlesSets[y].particleSystems.Count; o++)
                        GameManager.Instance.playersList[i].GetComponent<Player>().particlesSets[y].particleSystems[o].SetActive(state);
                }
    }
    IEnumerator SetPlayersWalkSFXSet(bool special = false)
    {
        yield return new WaitForSecondsRealtime(0.3f);

        if (GameManager.Instance != null && GameManager.Instance.playersList != null && GameManager.Instance.playersList.Count > 0)
            for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
                if (GameManager.Instance.playersList[i] != null && GameManager.Instance.playersList[i].GetComponent<Player>())
                {
                    if (special)
                        GameManager.Instance.playersList[i].GetComponent<Player>().SetWalkSFXSet(specialMapsData.stagesLists[currentMapIndex].walkStepSFXSet);
                    else
                        GameManager.Instance.playersList[i].GetComponent<Player>().SetWalkSFXSet(mapsData.stagesLists[currentMapIndex].walkStepSFXSet);
                }
    }



    // Starts the LoadNewMap coroutine, launched by the play in the stages menu
    public void LoadNewMapInGame(int newMapIndex)
    {
        StartCoroutine(LoadNewMapInGameCoroutine(newMapIndex, false));
    }

    IEnumerator LoadMapbundle(int bundleIndex)
    {
        string bundle = null;
        switch (bundleIndex)
        {
            case BridgeDay:
            case BridgeNight:
                bundle = "bridgeday";
                Debug.Log("Loading Bridge bundle");
                break;

            case VillageDay:
            case VillageNight:

                bundle = "village";
                Debug.Log("Loading Village bundle");
                break;

            case BattlefieldDay:
            case BattlefieldNight:
                bundle = "battlefield";
                Debug.Log("Loading Battlefield bundle");
                break;

            case SnowTempleDay:
            case SnowTempleNight:
                bundle = "snowtemple";
                Debug.Log("Loading Snow temple bundle");
                break;

            case TempleDay:
            case TempleNight:
                bundle = "interior";
                Debug.Log("Loading Temple bundle");
                break;

            default:
                break;
        }

        if (bundle == null)
        {
            Debug.LogWarning("Bundle doesn't exist, leaving");
            yield break;
        }

        string path = Path.Combine(Application.streamingAssetsPath, "AssetBundles", bundle);
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
        if (request.assetBundle != null)
        {
            loadedMapBundles.Add(request.assetBundle);
        }
        else
        {
            Debug.LogWarning("Bundle not found");
            yield break;
        }

        Debug.Log(request.assetBundle == null ? "Asset not loaded" : "Asset successfully loaded");
        yield return request;
    }

    // Loads a new stage with the transition FX
    IEnumerator LoadNewMapInGameCoroutine(int newMapIndex, bool randomIndex)
    {
        if (canLoadNewMap)
        {
            int index = 0;
            index = newMapIndex;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.roundTransitionLeavesFX.gameObject.SetActive(false);
                GameManager.Instance.roundTransitionLeavesFX.gameObject.SetActive(true);
                GameManager.Instance.roundTransitionLeavesFX.Play();
            }
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
