using UnityEngine;


// This script is supposed to make the object follow the mouse to achieve a custom cursor with a trail effect
// OPTIMIZED
public class MoveWithMouse : MonoBehaviour
{

    [SerializeField] Camera cam = null;




    void Start()
    {
        if (enabled)
        {
            Cursor.visible = true;
            if (cam == null)
                cam = Camera.main;
        }
    }

    void Update()
    {
        if (enabled)
        {
            Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Input.mousePosition;
            Debug.Log(pos);
        }
    }
}
