using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapMenuLoader : MonoBehaviour, IPointerClickHandler
{
    // MAP LOADER
    [Header("MAP LOADER")]
    [SerializeField] string mapLoaderName = "GlobalManager";
    MapLoader mapLoader = null;



    // MENU ELEMENTS
    [Header("MENU ELEMENTS")]
    [SerializeField] GameObject mapMenuObjectsParent = null;
    [SerializeField] GameObject
        mapMenuObject = null,
        backButton = null,
        randomMapElement = null;

    List<GameObject> menuBrowserButtonsList = new List<GameObject>();

    [SerializeField] MapsDataBase mapsDatabase01 = null;
    [SerializeField] MenuBrowser mapsMenuBrowser = null;











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
            Debug.Log(i);
        }


        mapMenuObject.SetActive(false);
        mapsMenuBrowser.elements = menuBrowserButtonsList.ToArray();
        mapsMenuBrowser.backElement = backButton;
    }

    // Update is called once per graphic frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }





    void LoadMapWithMapLoader(int mapIndex)
    {
        mapLoader.LoadNewMapInGame(mapIndex);
    }
}
