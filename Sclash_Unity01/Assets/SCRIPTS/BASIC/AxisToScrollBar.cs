using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class AxisToScrollBar : MonoBehaviour
{
    [SerializeField] string axisToUse = "Scrollwheel";
    [SerializeField] Scrollbar scrollbar = null;
    [SerializeField] float multiplier = 0.2f;

    EventSystemControl eControl;

    // Start is called before the first frame update
    void Start()
    {
        eControl = new EventSystemControl();
        eControl.Enable();

        eControl.UI.ScrollWheel.performed += ctx =>
        {
            if (ctx.ReadValue<Vector2>().y != 0f)
                scrollbar.value += Mathf.Sign(ctx.ReadValue<Vector2>().y) * multiplier;
        };
    }
}
