using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternTrial : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Lantern;

    private Vector2 xBounds = new Vector2(-2, 2);
    private Vector2 yBounds = new Vector2(2, 5);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DropLantern();
        }
    }

    private void DropLantern()
    {
        GameObject l_lantern = Instantiate(m_Lantern, RandomizeStartingPosition(), transform.rotation);
        if (!l_lantern.GetComponent<Rigidbody2D>())
            l_lantern.AddComponent<Rigidbody2D>();
    }

    private Vector3 RandomizeStartingPosition()
    {
        Vector3 value = Vector3.zero;
        value.x += Random.Range(xBounds.x, xBounds.y);
        value.y += Random.Range(yBounds.x, yBounds.y);
        value.z = 1;

        //SI = Vector3.zero -> Error
        return value;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 l_size = new Vector3(xBounds.x + xBounds.y, yBounds.x + yBounds.y, 1);
        //Gizmos.DrawCube();
    }

}
