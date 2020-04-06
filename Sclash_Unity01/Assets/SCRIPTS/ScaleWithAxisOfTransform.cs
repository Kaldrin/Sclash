using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithAxisOfTransform : MonoBehaviour
{
    [SerializeField] bool useMainCamera = false;
    [SerializeField] bool findObjectByName = false;
    [SerializeField] string objectName = "Main Camera";
    [SerializeField] bool useReferencedObject = false;
    bool dontDoAnything = false;


    [SerializeField] Transform transformToObserve = null;


    [SerializeField] Vector2 axisLimits = new Vector2(-6.5f, -25);
    [SerializeField] Vector2 scaleMultiplicationLimits = new Vector2(0.5f, 2);
    Vector3 baseSelfScale = new Vector3(0, 0, 0);





    // Start is called before the first frame update
    void Start()
    {
        if (useMainCamera)
            transformToObserve = Camera.main.transform;
        else if (findObjectByName)
            transformToObserve = GameObject.Find(objectName).transform;
        else if (useReferencedObject)
        {

        }
        else
            dontDoAnything = true;


        if (!dontDoAnything)
            baseSelfScale = transform.localScale;
    }

    private void Update()
    {
        if (!dontDoAnything)
        {
            transform.localScale = baseSelfScale * (scaleMultiplicationLimits.x + (scaleMultiplicationLimits.y - scaleMultiplicationLimits.x) * (transformToObserve.localPosition.z - axisLimits.x) / (axisLimits.y - axisLimits.x));
        }
    }
}
