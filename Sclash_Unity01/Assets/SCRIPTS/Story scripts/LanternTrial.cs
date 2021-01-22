using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LanternTrial : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Lantern = null;

    [SerializeField]
    private Vector2 xBounds = new Vector2(-2, 2);
    [SerializeField]
    private Vector2 yBounds = new Vector2(2, 5);

    private void Awake()
    {
        if (m_Lantern == null)
            Debug.LogError("m_Lantern must be not null");
    }

    private void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
            DropLantern();

        //OLD_INPUT
        /*if (Input.GetKeyDown(KeyCode.T))
        {
            DropLantern();
        }*/
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
        Vector3 l_size = new Vector3(xBounds.y - xBounds.x, yBounds.y - yBounds.x, 1);
        Vector3 l_pos = new Vector3(xBounds.x + xBounds.y, yBounds.x + yBounds.y, 1) * 0.5f;
        Gizmos.DrawWireCube(l_pos, l_size);
    }
}
