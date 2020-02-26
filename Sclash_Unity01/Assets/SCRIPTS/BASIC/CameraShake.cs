using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    # region COMPONENTS
    [Header("COMPONENTS")]
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    [SerializeField] Transform camTransform = null;
    # endregion


    [Header("IDENTIFICATION")]
    [SerializeField] string cameraShakeName = "Camera shake";




    #region SHAKE PARAMETERS
    [Header("SHAKE PARAMETERS")]
    // How long the object should shake for.
    [SerializeField] public float shakeDuration = 0f;
    // Amplitude of the shake. A larger value shakes the camera harder.
    [SerializeField] float
        shakeAmount = 0.5f,
        decreaseFactor = 1.5f;

    [SerializeField] Vector3 axisInfluence = new Vector3(1, 1, 0);

    bool
        hasResetPosition = false,
        hasBeganShaking = false;

    Vector3
        originalPos = new Vector3(0, 0, 0),
        beforeShakePos = new Vector3(0, 0, 0);
    #endregion







    # region BASE FUNCTIONS
    // BASE FUNCTIONS
    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    // OnEnable is called each time the object is set from inactive to active
    void OnEnable()
    {
        originalPos = camTransform.localPosition;
        beforeShakePos = originalPos;
    }

    // Update is called once per graphic frame
    void Update()
    {
        if (shakeDuration > 0)
        {
            if (!hasBeganShaking)
            {
                hasBeganShaking = true;
                beforeShakePos = camTransform.localPosition;
            }

            // Random shake calculation
            Vector3 randomShakeVector = Random.insideUnitSphere * shakeAmount;
            randomShakeVector = new Vector3(randomShakeVector.x * axisInfluence.x, randomShakeVector.y * axisInfluence.y, randomShakeVector.z * axisInfluence.z);


            // Which axis are influenced or not
            Vector3 baseShakePos = beforeShakePos;


            if (axisInfluence.x == 0)
            {
                baseShakePos.x = camTransform.localPosition.x;
            }
            if (axisInfluence.y == 0)
            {
                baseShakePos.y = camTransform.localPosition.y;
            }
            if (axisInfluence.z == 0)
            {
                baseShakePos.z = camTransform.localPosition.z;
            }




            camTransform.localPosition = baseShakePos + randomShakeVector;
            hasResetPosition = false;
            shakeDuration -= Time.fixedDeltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            hasBeganShaking = false;


            if (!hasResetPosition)
            {
                if (axisInfluence.x == 0)
                {
                    beforeShakePos.x = camTransform.localPosition.x;
                }
                if (axisInfluence.y == 0)
                {
                    beforeShakePos.y = camTransform.localPosition.y;
                }
                if (axisInfluence.z == 0)
                {
                    beforeShakePos.z = camTransform.localPosition.z;
                }


                camTransform.localPosition = beforeShakePos;
                hasResetPosition = true;
            }
        }
    }

    // FixedUpdate is called 50 times per frame
    void FixedUpdate()
    {
        
    }
    # endregion
}