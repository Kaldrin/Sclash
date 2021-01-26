using UnityEngine;
using System.Collections;
using EzySlice;
using UnityEngine.InputSystem;

public class Slicing : MonoBehaviour
{
    public GameObject plane;
    [SerializeField] GameObject[] cut;
    [SerializeField] Collider[] hits;

    private void Update()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            plane.transform.eulerAngles = new Vector3(0, 0, Random.Range(-30f, 30f));

            cut = Slice(gameObject, plane.transform.position, plane.transform.up);
        }
    }

    public GameObject[] Slice(GameObject obj, Vector3 planeWorldPos, Vector3 planeWorldDirection)
    {
        return obj.SliceInstantiate(planeWorldPos, planeWorldDirection);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(transform.position, new Vector3(5, .1f, 2.5f));
    }
}