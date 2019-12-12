using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    // MANAGERS
    [Header("MANAGERS")]
    [Tooltip("The reference for the unique game manager script of the scene")]
    [SerializeField] GameManager gameManager = null;




    // MAPS MENU
    [Header("MAPS MENU")]
    [Tooltip("The MenuBrowser script that browses through the map elements")]
    [SerializeField] MenuBrowser mapsMenuBrowser = null;





    // MAPS DATA
    [Header("MAPS DATA")]
    [Tooltip("Parent object of the currently instantiated map")]
    [SerializeField] GameObject mapContainer = null;
    [Tooltip("Currently instantiated map, visible in game")]
    [HideInInspector] public GameObject currentMap = null;

    [Tooltip("Scriptable object data reference containing the maps objects, their image and names")]
    [SerializeField] MapsDataBase mapsData = null;




    /*
    // MAPS MENU
    [Header("MAPS MENU")]
    [Tooltip("Parent object of the instantiated selectable map menu objects")]
    [SerializeField] GameObject mapMenuObjectsParent = null;
    */






    // MAP LOADING
    bool canLoadNewMap = true;














    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        // Get the managers
        //gameManager = GameObject.Find(gameManagerName).GetComponent<GameManager>();


        // Deactivates all activated in editor maps before loading the game's one
        for (int i = 0; i < mapContainer.transform.childCount; i++)
        {
            mapContainer.transform.GetChild(i).gameObject.SetActive(false);
        }


        // Load map
        int randomIndex = Random.Range(0, mapsData.mapsList.Count);
        SetMap(randomIndex);
    }

    // Update is called once per graphic frame
    void Update()
    {
        
    }







    // MAP LOADING
    // Immediatly changes the map
    void SetMap(int mapIndex)
    {
        if (currentMap != null)
            Destroy(currentMap);


        currentMap = Instantiate(mapsData.mapsList[mapIndex].mapObject, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), mapContainer.transform);
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


            if (!randomIndex)
                index = mapsMenuBrowser.browseIndex - 1;
            else
                index = newMapIndex;



            gameManager.roundTransitionLeavesFX.gameObject.SetActive(false);
            gameManager.roundTransitionLeavesFX.gameObject.SetActive(true);
            gameManager.roundTransitionLeavesFX.Play();
            canLoadNewMap = false;
            //mapMenuObjectsParent.SetActive(false);


            yield return new WaitForSeconds(1.5f);


            SetMap(index);


            yield return new WaitForSeconds(2f);


            //mapMenuObjectsParent.SetActive(true);
            canLoadNewMap = true;
        }
    }

    public void LoadRandomMap()
    {
        int randomIndex = Random.Range(0, mapsData.mapsList.Count);
        StartCoroutine(LoadNewMapInGameCoroutine(randomIndex, true));
    }
}
