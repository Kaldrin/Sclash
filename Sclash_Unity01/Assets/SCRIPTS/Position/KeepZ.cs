using System.Collections;
using System.Collections.Generic;
using UnityEngine;






// OPTIMIZED
// Reusable asset
// Bastien BERNAND

/// <summary>
/// This script is for the object to stay at a specific Z position no matter what, so it's easier to move around without making mistakes
/// </summary>

// UNITY 2019
public class KeepZ : MonoBehaviour
{
    [SerializeField] float zToKeep = 0;
    [SerializeField] bool checkOnStart = true;
    [SerializeField] bool checkInEditor = true;







    private void Start()                                                                                                               // START
    {
        if (enabled)
            if (checkOnStart)
                Reposition();
    }



    public void Reposition()                                                                                                                // REPOSITION
    {
        if (transform.position.z != zToKeep)
            transform.position = new Vector3(transform.position.x, transform.position.y, zToKeep);
    }



    private void OnDrawGizmos()                                                                                                                     // ON DRAW GIzmos
    {
        if (enabled)
            if (checkInEditor)
                Reposition();
    }
}
