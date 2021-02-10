using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Script to assign colliders to the scarf, by finding them on the stage, so it behaves a right way
[RequireComponent(typeof(Cloth))]
public class Scarf02 : MonoBehaviour
{
    [SerializeField] Cloth clothToAssignCollidersTo = null;




    private void OnEnable()                                                             // ON ENABLE
    {
        if (clothToAssignCollidersTo == null)
            clothToAssignCollidersTo = GetComponent<Cloth>();


        FindColliders();
    }



    public void FindColliders()
    {
        if (clothToAssignCollidersTo != null)
            clothToAssignCollidersTo.capsuleColliders = FindObjectsOfType<CapsuleCollider>();
    }







    // EDITOR ONLY
    private void OnDrawGizmos()
    {
        if (clothToAssignCollidersTo == null)
            clothToAssignCollidersTo = GetComponent<Cloth>();
    }
}
