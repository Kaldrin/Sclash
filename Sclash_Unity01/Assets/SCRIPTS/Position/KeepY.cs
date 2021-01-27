using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EDITOR ONLY
// This script is for the object to stay at a specific Z position no matter what, so it's easier to move around without making mistakes
// OPTIMIZED
public class KeepY : MonoBehaviour
{
    [SerializeField] float yToKeep = 0;
    [Tooltip("If false, only sets the position while in editor, if true, only does it at runtime")]
    [SerializeField] bool activeAtRunTime = false;
    [Tooltip("Only works for runtime")]
    [SerializeField] bool delayBeforeActivation = false;
    [SerializeField] float delay = 0.5f;
    [Tooltip("Only works for runtime")]
    [SerializeField] bool takeCurrentYOnActivation = false;
    float delayStartTime = 0f;
    bool canBeActivated = true;









    private void Start()
    {
        if (delayBeforeActivation)
        {
            delayStartTime = Time.time;
            canBeActivated = false;
        }
        else if (takeCurrentYOnActivation)
            yToKeep = transform.position.y;
    }


    private void Update()
    {
        // CHECK DELAY FINISHED
        if (!canBeActivated && delayBeforeActivation && Time.time - delayStartTime >= delay)
        {
            canBeActivated = true;


            if (takeCurrentYOnActivation)
                yToKeep = transform.position.y;
        }


        if (isActiveAndEnabled && enabled && activeAtRunTime && canBeActivated)
            if (transform.position.y != yToKeep)
                transform.position = new Vector3(transform.position.x, yToKeep, transform.position.z);
    }


    private void OnDrawGizmos()
    {
        if (isActiveAndEnabled && enabled && !activeAtRunTime)
            if (transform.position.y != yToKeep)
                transform.position = new Vector3(transform.position.x, yToKeep, transform.position.z);
    }
}
