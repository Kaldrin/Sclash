using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script will scale the referenced element depending on another transform's parameters dynamically. Usefull for mdisplaying info with dynamic
// OPTIMIZED
public class ScaleWithAxisOfTransform : MonoBehaviour
{
    [SerializeField] bool useMainCamera = false;
    [SerializeField] bool findObjectByName = false;
    [Tooltip("Name of the object which transform should be observed and used, to find it in the scene on Start")]
    [SerializeField] string objectName = "Main Camera";
    [SerializeField] bool useReferencedObject = false;
    bool dontDoAnything = false;


    [SerializeField] Transform transformToObserve = null;


    [SerializeField] Vector2 axisLimits = new Vector2(-6.5f, -25);
    [SerializeField] Vector2 scaleMultiplicationLimits = new Vector2(0.5f, 2);
    Vector3 baseSelfScale = new Vector3(0, 0, 0);






    void Start()
    {
        // Observe main camera if set to do so
        if (useMainCamera)
            transformToObserve = Camera.main.transform;
        // Find transform by referenced name
        else if (findObjectByName)
            transformToObserve = GameObject.Find(objectName).transform;
        else if (useReferencedObject)
        {}
        else // If not set, don't do anything
            dontDoAnything = true;


        // NOTHING
        if (transformToObserve == null) // If nothing to observe, don't do anything
            dontDoAnything = true;


        if (!dontDoAnything) // Save its base scale
            baseSelfScale = transform.localScale;
    }

    private void Update()
    {
        if (enabled && isActiveAndEnabled)
        {
            if (!dontDoAnything) // Scale with observed transform
                transform.localScale = baseSelfScale * (scaleMultiplicationLimits.x + (scaleMultiplicationLimits.y - scaleMultiplicationLimits.x) * (transformToObserve.localPosition.z - axisLimits.x) / (axisLimits.y - axisLimits.x));
        }
    }
}
