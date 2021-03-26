using UnityEngine;
using UnityEngine.UI;
using TMPro;





// HEADER
// For Sclash

// REQUIREMENTS
// Requires the TextMeshPro package to work

/// <summary>
/// Script present on a stage button object in the stage selection screen
/// </summary>

// VERSION
// Originally made for Unity 2019.1.1f1
public class MapMenuObject : MonoBehaviour
{
    [Header("MANAGERS")]
    [SerializeField] MapLoader mapLoader = null;


    [Header("MAP OBJECT ELEMENTS")]
    [SerializeField] public GameObject mapButtonObject = null;
    [SerializeField] public Image mapImage = null;
    [SerializeField] public TextMeshProUGUI mapText = null;
    [SerializeField] public TextApparition mapNameTextApparitionComponent = null;
    [SerializeField] public int stageIndex = 0;
    [SerializeField] public GameObject customListCheckBox = null;







    public void SwitchCustomList()                                                              // SWITCH CUSTOM LIST
    {
        Debug.Log("SwitchCustomList()");
        Map newMap = mapLoader.mapsData.stagesLists[stageIndex];

        if (mapLoader.mapsData.stagesLists[stageIndex].inCustomList)
            newMap.inCustomList = false;
        else
            newMap.inCustomList = true;

        mapLoader.mapsData.stagesLists[stageIndex] = newMap;
    }


    public void UpdateCustomListCheckBox()                                                          // UPDATE CUSTOM LIST CHECK BOX
    {
        if (mapLoader.mapsData.stagesLists[stageIndex].inCustomList)
            customListCheckBox.SetActive(true);
    }


    public void ChangeMap()                                                                 // CHANGE MAP
    {
        mapLoader.LoadNewMapInGame(stageIndex);
    }
}
