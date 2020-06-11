using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MapLoader : MonoBehaviour
{

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
    # endregion




    # region MAP LOADING
    [Header("MAP LOADING")]
    bool canLoadNewMap = true;
    [SerializeField] bool loadMapOnStart = false;
    #endregion


    [Header("OTHER")]
    [SerializeField] PostProcessVolume cameraPostProcessVolume = null;
    #endregion










    #region FUNCTIONS
    #region BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        mapMenuLoader.LoadParameters();
        mapMenuLoader.SetUpMenu();


        // Load map
        if (loadMapOnStart)
        {
            for (int i = 0; i < mapContainer.transform.childCount; i++)
            {
                Destroy(mapContainer.transform.GetChild(i).gameObject);
            }


            int nextStageIndex = Random.Range(0, mapsData.stagesLists.Count);


            if (gameManager.gameParameters.keepLastLoadedStage)
                SetMap(gameManager.gameParameters.lastLoadedStageIndex);
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

                SetMap(nextStageIndex);
            }
            else
            {
                SetMap(Random.Range(0, mapsData.stagesLists.Count));
            }
        }
        else
        {
            for (int i = 0; i < mapContainer.transform.childCount; i++)
            {
                if (mapContainer.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    currentMap = mapContainer.transform.GetChild(i).gameObject;
                }
            }
        }
    }
    # endregion





    # region MAP LOADING
    // Immediatly changes the map
    public void SetMap(int mapIndex)
    {
        if (currentMap != null)
            Destroy(currentMap);


        currentMap = Instantiate(mapsData.stagesLists[mapIndex].mapObject, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), mapContainer.transform);
        currentMapIndex = mapIndex;

        // POST PROCESS
        cameraPostProcessVolume.profile = mapsData.stagesLists[mapIndex].postProcessProfile;


        // PARTICLES
        Debug.Log("Particles");
        bool state = false;

        for (int i = 0; i < gameManager.playersList.Count; i++)
        {
            for (int y = 0; y < gameManager.playersList[i].GetComponent<Player>().particlesSets.Count; y++)
            {
                if (y == (mapsData.stagesLists[mapIndex].particleSet))
                    state = true;
                else
                    state = false;


                for (int o = 0; o < gameManager.playersList[i].GetComponent<Player>().particlesSets[y].particleSystems.Count; o++)
                    gameManager.playersList[i].GetComponent<Player>().particlesSets[y].particleSystems[o].SetActive(state);
            }
        }



        gameManager.gameParameters.lastLoadedStageIndex = currentMapIndex;
        JsonSave save = SaveGameManager.GetCurrentSave();
        save.lastLoadedStageIndex = gameManager.gameParameters.lastLoadedStageIndex;
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
            SetMap(index);


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
