using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMarker : MonoBehaviour
{
    [SerializeField] Transform levelMarkersParent = null;
    [SerializeField] GameObject levelMarkerObject = null;
    [SerializeField] public Image colorImage = null;





 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLevelMarkers(int actionLevel)
    {
        for (int i = 0; i < levelMarkersParent.childCount; i++)
        {
            if (levelMarkersParent.GetChild(i).gameObject != levelMarkerObject)
                Destroy(levelMarkersParent.GetChild(i));
        }


        for (int i = 0; i < actionLevel; i++)
        {
            Instantiate(levelMarkerObject, levelMarkersParent);
        }


        levelMarkerObject.SetActive(false);
    }
}
