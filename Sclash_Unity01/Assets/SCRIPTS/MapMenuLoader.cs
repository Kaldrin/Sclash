using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapMenuLoader : MonoBehaviour, IPointerClickHandler
{
    # region MAP LOADER
    // MAP LOADER
    [Header("MAP LOADER")]
    [Tooltip("The name of the game object to find in scene where the MapLoader script is")]
    [SerializeField] string mapLoaderName = "GlobalManager";
    MapLoader mapLoader = null;
    # endregion





    #region MAP LOADER
    // MENU ELEMENTS
    [Header("MENU ELEMENTS")]
    [Tooltip("The reference to the game object in which all the menu elements of the maps to choose it will be instantiated")]
    [SerializeField] GameObject mapMenuObjectsParent = null;
    [SerializeField] GameObject
        mapMenuObject = null,
        backButton = null,
        randomMapElement = null;

    List<GameObject> menuBrowserButtonsList = new List<GameObject>();

    [Tooltip("The reference to the scriptable object data containing all the maps")]
    [SerializeField] MapsDataBase mapsDatabase01 = null;
    [Tooltip("The reference to the MenuBrowser script managing the map selection screen")]
    [SerializeField] MenuBrowser mapsMenuBrowser = null;
    #endregion












    # region BASE FUNCTIONS
    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        // Get the map loader
        mapLoader = GameObject.Find(mapLoaderName).GetComponent<MapLoader>();
        //menuBrowserButtonsList.Add(backButton);
        menuBrowserButtonsList.Add(randomMapElement);


        for (int i = 0; i < mapsDatabase01.mapsList.Count; i++)
        {
            GameObject newMapMenuObject = null;
            MapMenuObject newMapMenuObjectScript = null;

            newMapMenuObject = Instantiate(mapMenuObject, mapMenuObjectsParent.transform);
            newMapMenuObjectScript = newMapMenuObject.GetComponent<MapMenuObject>();

            newMapMenuObjectScript.mapImage.overrideSprite = mapsDatabase01.mapsList[i].mapImage;
            newMapMenuObjectScript.mapText.text = mapsDatabase01.mapsList[i].mapName;
            newMapMenuObject.name = mapsDatabase01.mapsList[i].mapName;

            menuBrowserButtonsList.Add(newMapMenuObjectScript.mapButtonObject);
            newMapMenuObjectScript.mapButtonObject.GetComponent<Button>().onClick.AddListener(delegate {mapLoader.LoadNewMapInGame(i);});
        }


        mapMenuObject.SetActive(false);
        mapsMenuBrowser.elements = menuBrowserButtonsList.ToArray();
        mapsMenuBrowser.backElement = backButton;
        mapsMenuBrowser.FixButtonColorUsageList();
    }

    // Update is called once per graphic frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
    # endregion





    # region MAP LOADING
    // MAP LOADING
    void LoadMapWithMapLoader(int mapIndex)
    {
        mapLoader.LoadNewMapInGame(mapIndex);
    }
    # endregion
}
