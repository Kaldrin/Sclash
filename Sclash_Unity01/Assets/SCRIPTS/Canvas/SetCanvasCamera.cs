using UnityEngine;

// This script assigns a reference camera to the attached world space canvas for optimization purpose
// OPTIMIZED
[RequireComponent(typeof(Canvas))]
public class SetCanvasCamera : MonoBehaviour
{
    Canvas attachedWorldSpaceCanvasToSetCameraOff = null;


    private void OnEnable()
    {
        if (attachedWorldSpaceCanvasToSetCameraOff == null)
            attachedWorldSpaceCanvasToSetCameraOff = GetComponent<Canvas>();

        if (attachedWorldSpaceCanvasToSetCameraOff.worldCamera == null)
            attachedWorldSpaceCanvasToSetCameraOff.worldCamera = Camera.main;
    }
}
