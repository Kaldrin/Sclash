using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderCam : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.position = CameraManager.Instance.cameraComponent.transform.position;
    }

}
