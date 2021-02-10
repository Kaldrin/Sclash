using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// This script is for the object to stay at a specific Z position no matter what, so it's easier to move around without making mistakes
// OPTIMIZED
public class KeepZ : MonoBehaviour
{
    [SerializeField] float zToKeep = 0;
    [SerializeField] bool checkOnStart = true;
    [SerializeField] bool checkInEditor = true;





    private void Start()                                                                // START
    {
        if (checkOnStart)
            Reposition();
    }




    public void Reposition()
    {
        if (transform.position.z != zToKeep)
            transform.position = new Vector3(transform.position.x, transform.position.y, zToKeep);
    }



    private void OnDrawGizmos()
    {
        if (checkInEditor)
            Reposition();
    }
}
