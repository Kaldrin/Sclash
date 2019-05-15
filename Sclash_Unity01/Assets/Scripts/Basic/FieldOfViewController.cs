using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewController : MonoBehaviour {

    public float
        zoomSpeed = 5,
        minFOV = 20,
        maxFOV = 100;
    public bool
        scrollZoom = true,
        instantZoom = false;
    public KeyCode
        instantZoomKey;
    public float
        instantZoomValue;
    float unZoomValue;
    bool
        zooming = false;

    void Start()
    {
        unZoomValue = Camera.main.fieldOfView;
    }

    void Update()
    {
        ScrollZoom();
        InstantZoom();
    }



    void ScrollZoom()
    {
        // assign zoom value to a variable
        if (scrollZoom)
        {
            float delta = -Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                // make sure the current FOV is within min/max values
                if ((Camera.main.fieldOfView + delta > minFOV) && (Camera.main.fieldOfView + delta < maxFOV))
                {
                    // apply the change to the main camera
                    Camera.main.fieldOfView = Camera.main.fieldOfView + delta;
                }
            }
        }
    }



    void InstantZoom()
    {
        if (instantZoom)
        {
            if (Input.GetKeyDown(instantZoomKey))
            {
                if (!zooming)
                {
                    zooming = true;
                    Camera.main.fieldOfView = instantZoomValue;
                }
                else if (zooming)
                {
                    zooming = false;
                    Camera.main.fieldOfView = unZoomValue;
                }
            }
        }
    }
}

