using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    [SerializeField] GameObject mapContainer = null;
    [SerializeField] MapsDataBase mapsData = null;

    // Start is called before the first frame update
    void Start()
    {
        int randomIndex = Random.Range(0, mapsData.mapsList.Count);

        Instantiate(mapsData.mapsList[randomIndex].mapObject, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), mapContainer.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
