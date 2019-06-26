using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithMouse : MonoBehaviour
{
    [SerializeField] Camera cam = null;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = Input.mousePosition;
        Debug.Log(pos);
    }
}
