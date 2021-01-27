using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EDITOR ONLY
// This script is for the object to stay at a specific Z position no matter what, so it's easier to move around without making mistakes
// OPTIMIZED
public class KeepZ : MonoBehaviour
{
    [SerializeField] float zToKeep = 0;




    private void OnDrawGizmos()
    {
        if (transform.position.z != zToKeep)
            transform.position = new Vector3(transform.position.x, transform.position.y, zToKeep);
    }
}
