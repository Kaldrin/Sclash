using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    // MANAGERS
    [Header("MANAGERS")]
    [SerializeField] string gameManagerName = "GlobalManager";
    GameManager gameManager = null;





    // MAPS DATA
    [Header("MAPS DATA")]
    [SerializeField] GameObject mapContainer = null;
    [HideInInspector] public GameObject currentMap = null;

    [SerializeField] MapsDataBase mapsData = null;





    // MAP LOADING
    bool canLoadNewMap = true;














    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        // Get the managers
        gameManager = GameObject.Find(gameManagerName).GetComponent<GameManager>();


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
        StartCoroutine(LoadNewMapInGameCoroutine(newMapIndex));
    }

    // Loads a new map with the transition FX
    IEnumerator LoadNewMapInGameCoroutine(int newMapIndex)
    {
        if (canLoadNewMap)
        {
            gameManager.roundLeaves.gameObject.SetActive(false);
            gameManager.roundLeaves.gameObject.SetActive(true);
            gameManager.roundLeaves.Play();
            canLoadNewMap = false;

            yield return new WaitForSeconds(1.5f);


            SetMap(newMapIndex);
            Debug.Log(newMapIndex);

            yield return new WaitForSeconds(2f);
            canLoadNewMap = true;
        }
    }

    public void LoadRandomMap()
    {
        int randomIndex = Random.Range(0, mapsData.mapsList.Count);
        StartCoroutine(LoadNewMapInGameCoroutine(randomIndex));
    }
}
