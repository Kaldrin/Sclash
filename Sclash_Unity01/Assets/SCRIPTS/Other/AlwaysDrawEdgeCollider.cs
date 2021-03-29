using System.Collections;
using System.Collections.Generic;
using UnityEngine;






// HEADER
// By Bastien BERNAND

// REQUIREMENTS

/// <summary>
/// Put this component on an Edge Collider and it will always draw it in the scene view no matter if it's the focused object or not
/// </summary>

// VERSION
// UNITY 2019.4.14
[RequireComponent(typeof(EdgeCollider2D))]
public class AlwaysDrawEdgeCollider : MonoBehaviour
{
    [SerializeField] Color color = Color.green;
    EdgeCollider2D edgeCollider2D = null;




    // Just here to display the activation checkbox
    private void Start() {}

    // Draw on gizmos
    private void OnDrawGizmos()
    {
        if (isActiveAndEnabled && enabled)
        {
            // Get the component
            if (edgeCollider2D == null && GetComponent<EdgeCollider2D>())
                edgeCollider2D = GetComponent<EdgeCollider2D>();

            if (edgeCollider2D.enabled)
            {
                // COLOR
                if (Gizmos.color != color)
                    Gizmos.color = color;

                // DRAW LINE
                for (int i = 0; i < edgeCollider2D.pointCount - 1; i++)
                    Gizmos.DrawLine(transform.position + (Vector3)edgeCollider2D.offset + (Vector3)edgeCollider2D.points[i], transform.position + (Vector3)edgeCollider2D.offset + (Vector3)edgeCollider2D.points[i + 1]);
            }
        }
    }
}
