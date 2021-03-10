using UnityEngine;
using UnityEngine.UI;
using TMPro;

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







    public void SwitchCustomList()
    {
        Map newMap = mapLoader.mapsData.stagesLists[stageIndex];

        if (mapLoader.mapsData.stagesLists[stageIndex].inCustomList)
            newMap.inCustomList = false;
        else
            newMap.inCustomList = true;

        mapLoader.mapsData.stagesLists[stageIndex] = newMap;
    }


    public void UpdateCustomListCheckBox()
    {
        if (mapLoader.mapsData.stagesLists[stageIndex].inCustomList)
            customListCheckBox.SetActive(true);
    }


    public void ChangeMap()
    {
        mapLoader.LoadNewMapInGame(stageIndex);
    }
}
